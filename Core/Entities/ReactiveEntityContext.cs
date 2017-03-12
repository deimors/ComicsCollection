using Core.CQS;
using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
	public class ReactiveEntityContext : IDisposable, IEntityContext
	{
		private readonly ICQSContext _context;

		private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

		private readonly ISubject<IEvent> _rollbackSubject = new Subject<IEvent>();

		public ReactiveEntityContext(ICQSContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			_context = context;
		}

		void IDisposable.Dispose()
			=> _subscriptions.Apply(s => s.Dispose());

		void IEntityContext.HandleCommand<TCommand, TResult>(Func<TCommand, IEnumerable<IEvent>> commandHandler)
			=> _context.Commands
				.OfType<ObservableCommand<TCommand, TResult>>()
				.Subscribe(command => HandleCommandTransaction(command, commandHandler))
				.AddTo(_subscriptions);
		
		void IEntityContext.HandleQuery<TQuery, TResult>(Func<TQuery, TResult> queryHandler)
			=> _context.Queries
				.OfType<ObservableQuery<TQuery, TResult>>()
				.Subscribe(query => HandleQuery(query, queryHandler))
				.AddTo(_subscriptions);

		void IEntityContext.ApplyEvent<TEvent>(Action<TEvent> eventHandler)
			=> _context.EventsIn
				.Merge(_rollbackSubject.Select(@event => @event.Inverse))
				.OfType<TEvent>()
				.Subscribe(@event => HandleExceptions(() => eventHandler(@event)))
				.AddTo(_subscriptions);

		private void HandleCommandTransaction<TCommand, TResult>(ObservableCommand<TCommand, TResult> command, Func<TCommand, IEnumerable<IEvent>> commandHandler)
			where TCommand : ICommand
		{
			var transacted = new List<IEvent>();

			try
			{
				commandHandler(command.Command).Apply(@event => transacted.Add(@event));
				transacted.Apply(command.Events.OnNext);
				command.Events.OnCompleted();
				transacted.Apply(_context.EventsOut.OnNext);
			}
			catch (Exception error)
			{
				transacted.AsEnumerable().Reverse().Apply(_rollbackSubject.OnNext);
				command.Events.OnError(error);
				_context.EventsOut.OnError(error);
			}
		}

		private void HandleQuery<TQuery, TResult>(ObservableQuery<TQuery, TResult> query, Func<TQuery, TResult> queryHandler)
		{
			try
			{
				query.Result.OnNext(queryHandler(query.Query));
				query.Result.OnCompleted();
			}
			catch (Exception error)
			{
				query.Result.OnError(error);
				_context.EventsOut.OnError(error);
			}
		}
		
		private void HandleExceptions(Action action)
		{
			try
			{
				action();
			}
			catch (Exception error)
			{
				_context.EventsOut.OnError(error);
			}
		}
	}
}
