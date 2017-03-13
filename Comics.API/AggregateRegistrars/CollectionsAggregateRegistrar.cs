using Comics.Commands;
using Comics.Domain.Aggregates;
using Comics.Domain.Events;
using Comics.Domain.Values;
using Core.CQS;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comics.API.AggregateRegistrars
{
	public class CollectionsAggregateRegistrar
	{
		public CollectionsAggregateRegistrar(IEntityContext context, CollectionsAggregate aggregate)
		{
			context.HandleCommand<CollectionsCommands.Create, CollectionIdentifier>(command => HandleCreate(aggregate, command));
			context.ApplyEvent<CollectionsEvents.Created>(command => ApplyCreate(aggregate, command));
		}

		private IEnumerable<IEvent> HandleCreate(CollectionsAggregate aggregate, CollectionsCommands.Create command)
		{
			var createdEvent = aggregate.Create();

			yield return createdEvent;

			yield return aggregate.SetName(createdEvent.CollectionId, command.Name);
		}

		private void ApplyCreate(CollectionsAggregate aggregate, CollectionsEvents.Created @event)
			=> aggregate.Create(@event.CollectionId);
	}
}
