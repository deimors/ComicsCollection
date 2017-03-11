using Comics.API.AggregateWrappers;
using Comics.Domain.Aggregates;
using Comics.Domain.Commands;
using Comics.Domain.Events;
using Comics.Domain.Values;
using Core.CQS;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

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
