using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Core.CQS
{
	public class CQSDispatcher : ICQSDispatcher, ICQSContext, IDispatcher
	{
		public IObservable<IObservableCommand> Commands => _commandsSubject;

		public IObservable<IEvent> EventsIn => _eventsInSubject;

		public IObserver<IEvent> EventsOut => _eventsOutSubject;

		public IObservable<IObservableQuery> Queries => _queriesSubject;

		private readonly Subject<IObservableCommand> _commandsSubject = new Subject<IObservableCommand>();

		private readonly Subject<IObservableQuery> _queriesSubject = new Subject<IObservableQuery>();

		private readonly Subject<IEvent> _eventsOutSubject = new Subject<IEvent>();

		private readonly Subject<IEvent> _eventsInSubject = new Subject<IEvent>();

		private readonly Queue<Action> _dispatchQueue = new Queue<Action>();

		private readonly IEventBus _eventBus;

		public CQSDispatcher(IEventBus eventBus)
		{
			_eventBus = eventBus;

			BindOnDispatcher(_eventsOutSubject, _eventBus.In);
			BindOnDispatcher(_eventBus.Out, _eventsInSubject);
		}

		private void BindOnDispatcher<T>(IObservable<T> observable, IObserver<T> observer)
		{
			observable.Subscribe(
				e => _dispatchQueue.Enqueue(() => observer.OnNext(e)),
				onError: ex => _dispatchQueue.Enqueue(() => observer.OnError(ex))
			);
		}

		public IObservable<TResult> DispatchCommand<TCommand, TResult>(TCommand command, ResultFilter<TResult> resultFilter) where TCommand : ICommand
		{
			var observableCommand = new ObservableCommand<TCommand, TResult>(command, resultFilter);

			_dispatchQueue.Enqueue(() => _commandsSubject.OnNext(observableCommand));

			return observableCommand.Result;
		}

		public bool Dispatch() 
			=> InvokeOnDispatchQueue(() => _dispatchQueue.Dequeue().Invoke());

		public bool DispatchAll() 
			=> InvokeOnDispatchQueue(() => { while (Dispatch()) { } });

		private bool InvokeOnDispatchQueue(Action action)
		{
			if (!_dispatchQueue.Any())
				return false;

			action();

			return true;
		}
	}
}
