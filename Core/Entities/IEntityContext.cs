using System;
using System.Collections.Generic;
using Core.CQS;

namespace Core.Entities
{
	public interface IEntityContext
	{
		void ApplyEvent<TEvent>(Action<TEvent> eventHandler) where TEvent : IEvent;
		void HandleCommand<TCommand>(Func<TCommand, IEnumerable<IEvent>> commandHandler) where TCommand : ICommand;
		void HandleQuery<TQuery>(Action<TQuery> queryHandler) where TQuery : IQuery;
	}
}