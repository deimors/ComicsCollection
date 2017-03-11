using System;

namespace Core.CQS
{
	public interface ICQSDispatcher
	{
		IObservable<TResult> DispatchCommand<TCommand, TResult>(TCommand command, ResultFilter<TResult> resultFilter) where TCommand : ICommand;
	}
}
