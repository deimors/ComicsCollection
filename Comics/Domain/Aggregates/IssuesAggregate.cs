using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CQS;
using Comics.Domain.Values;
using Comics.Domain.Commands;
using Comics.Domain.Entities;
using Comics.Domain.Exceptions;
using Comics.Domain.Events;

namespace Comics.Domain.Aggregates
{
	public class IssuesAggregate
	{
		private readonly IDictionary<IssueIdentifier, Issue> _issues = new Dictionary<IssueIdentifier, Issue>();

		public IssuesAggregate(IEntityContext context)
		{
			context.HandleCommand<IssuesCommands.SetTitle>(HandleSetTitle);
			context.ApplyEvent<IssuesEvents.TitleSet>(ApplyTitleSet);

			context.HandleCommand<IssuesCommands.Create>(HandleCreate);
			context.ApplyEvent<IssuesEvents.Created>(ApplyCreate);
		}

		private IEnumerable<IEvent> HandleCreate(IssuesCommands.Create command)
		{
			var createdEvent = Create();

			yield return createdEvent;

			yield return SetTitle(createdEvent.IssueId, command.Title);
		}

		private void ApplyCreate(IssuesEvents.Created @event)
			=> Create(@event.IssueId);
		
		private IssuesEvents.Created Create(IssueIdentifier issueId = null)
		{
			var newIssue = new Issue(issueId);

			_issues.Add(newIssue.IssueId, newIssue);

			return new IssuesEvents.Created(newIssue.IssueId);
		}

		private IEnumerable<IEvent> HandleDestroy(IssuesCommands.Destroy command)
		{
			yield return Destroy(command.IssueId);
		}

		private void ApplyDestroy(IssuesEvents.Destroyed @event)
			=> Destroy(@event.IssueId);

		private IEvent Destroy(IssueIdentifier issueId)
		{
			if (issueId == null)
				throw new ArgumentNullException(nameof(issueId));

			if (!_issues.Remove(issueId))
				throw new IssuesExceptions.NotFound(issueId);

			return new IssuesEvents.Destroyed(issueId);
		}

		private IEnumerable<IEvent> HandleSetTitle(IssuesCommands.SetTitle command)
		{
			yield return SetTitle(command.IssueId, command.Title);
		}

		private void ApplyTitleSet(IssuesEvents.TitleSet @event)
		{
			SetTitle(@event.IssueId, @event.Title);
		}

		private IEvent SetTitle(IssueIdentifier issueId, IssueTitle title)
		{
			Issue issue;

			if (issueId == null)
				throw new ArgumentNullException(nameof(issueId));

			if (!_issues.TryGetValue(issueId, out issue))
			{
				throw new IssuesExceptions.NotFound(issueId);
			}

			var oldTitle = issue.Title;

			issue.Title = title;

			return new IssuesEvents.TitleSet(issue.IssueId, issue.Title, oldTitle);
		}
	}
}
