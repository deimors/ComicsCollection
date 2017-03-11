using System;
using System.Reactive.Subjects;

namespace Core.CQS
{
	public delegate IObservable<TResult> ResultFilter<TResult>(IObservable<IEvent> events);

	public interface IObservableCommand
	{
		
	}

	public class ObservableCommand<TCommand, TResult> : IObservableCommand
		where TCommand : ICommand
	{
		public TCommand Command { get; }

		public IObservable<TResult> Result => _resultSubject;

		public IObserver<IEvent> Events => _eventSubject;

		private readonly Subject<TResult> _resultSubject = new Subject<TResult>();

		private readonly Subject<IEvent> _eventSubject = new Subject<IEvent>();

		public ObservableCommand(TCommand command, ResultFilter<TResult> resultFilter)
		{
			Command = command;

			resultFilter(_eventSubject).Subscribe(_resultSubject);
		}
	}
}
