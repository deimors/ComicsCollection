using Comics.Domain.Entities;
using Comics.Domain.Events;
using Comics.Domain.Exceptions;
using Comics.Domain.Values;
using System;
using System.Collections.Generic;

namespace Comics.Domain.Aggregates
{
	public class IssuesAggregate
	{
		private readonly IDictionary<IssueIdentifier, Issue> _issues = new Dictionary<IssueIdentifier, Issue>();
		
		public IssuesEvents.Created Create(IssueIdentifier issueId = null)
		{
			var newIssue = new Issue(issueId);

			_issues.Add(newIssue.IssueId, newIssue);

			return new IssuesEvents.Created(newIssue.IssueId);
		}

		public IssuesEvents.Destroyed Destroy(IssueIdentifier issueId)
		{
			if (issueId == null)
				throw new ArgumentNullException(nameof(issueId));

			if (!_issues.Remove(issueId))
				throw new IssuesExceptions.NotFound(issueId);

			return new IssuesEvents.Destroyed(issueId);
		}

		public IssuesEvents.TitleSet SetTitle(IssueIdentifier issueId, IssueTitle title)
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
