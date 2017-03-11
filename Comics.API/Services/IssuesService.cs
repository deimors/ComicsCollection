using Comics.API.AggregateWrappers;
using Comics.Domain.Aggregates;
using Comics.Domain.Commands;
using Comics.Domain.Values;
using Core.CQS;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
		{
			var command = new IssuesCommands.Create(title);

			_dispatcher.DispatchCommand(command);

			throw new NotImplementedException();
		}
	}
}
