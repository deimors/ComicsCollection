using System;

namespace Core.CQS
{
	public interface ICQSContext
	{
		IObservable<IEvent> EventsIn { get; }
		IObserver<IEvent> EventsOut { get; }
		IObservable<ICommand> Commands { get; }
		IObservable<IQuery> Queries { get; }
	}
}
