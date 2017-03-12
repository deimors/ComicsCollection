using Comics.Domain.Commands;
using Comics.Domain.Events;
using Comics.Domain.Values;
using Core.CQS;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace Comics.API.Services
{
	public class IssuesService
	{
		private ICQSDispatcher _dispatcher;

		public IssuesService(ICQSDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public IObservable<IssueIdentifier> CreateIssue(IssueTitle title)
			=> _dispatcher.DispatchCommand(
				new IssuesCommands.Create(title),
				events => events.OfType<IssuesEvents.Created>().Select(e => e.IssueId)
			);
	}
}
