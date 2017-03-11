using Comics.Domain.Aggregates;
using Comics.Domain.Commands;
using Comics.Domain.Events;
using Core.CQS;
using Core.Entities;
using System.Collections.Generic;

namespace Comics.API.AggregateWrappers
{
	public class IssuesAggregateWrapper
	{
		public IssuesAggregateWrapper(IEntityContext context, IssuesAggregate aggregate)
		{
			context.HandleCommand<IssuesCommands.SetTitle>(command => HandleSetTitle(aggregate, command));
			context.ApplyEvent<IssuesEvents.TitleSet>(command => ApplyTitleSet(aggregate, command));

			context.HandleCommand<IssuesCommands.Create>(command => HandleCreate(aggregate, command));
			context.ApplyEvent<IssuesEvents.Created>(command => ApplyCreate(aggregate, command));

			context.HandleCommand<IssuesCommands.Destroy>(command => HandleDestroy(aggregate, command));
			context.ApplyEvent<IssuesEvents.Destroyed>(command => ApplyDestroy(aggregate, command));
		}

		private IEnumerable<IEvent> HandleCreate(IssuesAggregate aggregate, IssuesCommands.Create command)
		{
			var createdEvent = aggregate.Create();

			yield return createdEvent;

			yield return aggregate.SetTitle(createdEvent.IssueId, command.Title);
		}

		private void ApplyCreate(IssuesAggregate aggregate, IssuesEvents.Created @event)
			=> aggregate.Create(@event.IssueId);

		private IEnumerable<IEvent> HandleDestroy(IssuesAggregate aggregate, IssuesCommands.Destroy command)
		{
			yield return aggregate.Destroy(command.IssueId);
		}

		private void ApplyDestroy(IssuesAggregate aggregate, IssuesEvents.Destroyed @event)
			=> aggregate.Destroy(@event.IssueId);

		private IEnumerable<IEvent> HandleSetTitle(IssuesAggregate aggregate, IssuesCommands.SetTitle command)
		{
			yield return aggregate.SetTitle(command.IssueId, command.Title);
		}

		private void ApplyTitleSet(IssuesAggregate aggregate, IssuesEvents.TitleSet @event)
			=> aggregate.SetTitle(@event.IssueId, @event.Title);
	}
}
