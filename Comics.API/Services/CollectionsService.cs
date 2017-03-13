using Comics.Commands;
using Comics.Domain.Events;
using Comics.Domain.Values;
using Core.CQS;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace Comics.API.Services
{
	public class CollectionsService
	{
		private ICQSDispatcher _dispatcher;

		public CollectionsService(ICQSDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public IObservable<CollectionIdentifier> Create(string name)
		=> _dispatcher.DispatchCommand(
			new CollectionsCommands.Create(name),
			events => events.OfType<CollectionsEvents.Created>().Select(e => e.CollectionId)
		);
	}
}
