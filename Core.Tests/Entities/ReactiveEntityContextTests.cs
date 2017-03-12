using Core.CQS;
using Core.Entities;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace Core.Tests.Entities
{
	public class TestCommand : ICommand { }

	public class TestQuery : IQuery { }

	public class TestResult { }

	public class TestEvent : IEvent
	{
		public class Inv : IEvent { public IEvent Inverse => new TestEvent(); }
		public IEvent Inverse => new Inv();
	}

	public class MockCQSContext : ICQSContext
	{
		public MockCQSContext(IFixture fixture)
		{
			EventsOut.Subscribe(fixture.Freeze<IObserver<IEvent>>());
			fixture.Inject<IObserver<IObservableCommand>>(Commands);
			fixture.Inject<IObserver<IObservableQuery>>(Queries);
		}

		public ISubject<IObservableCommand> Commands { get; } = new Subject<IObservableCommand>();
		IObservable<IObservableCommand> ICQSContext.Commands => Commands;

		public ISubject<IEvent> EventsIn { get; } = new Subject<IEvent>();
		IObservable<IEvent> ICQSContext.EventsIn => EventsIn;

		public ISubject<IEvent> EventsOut { get; } = new Subject<IEvent>();
		IObserver<IEvent> ICQSContext.EventsOut => EventsOut;

		public ISubject<IObservableQuery> Queries { get; } = new Subject<IObservableQuery>();
		IObservable<IObservableQuery> ICQSContext.Queries => Queries;
	}

	public class BaseCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Customize(new AutoFakeItEasyCustomization());

			var context = fixture.Freeze<MockCQSContext>();
			fixture.Inject<ICQSContext>(context);

			var sut = fixture.Freeze<ReactiveEntityContext>();
			fixture.Inject<IEntityContext>(sut);
		}
	}

	public class CommandHandlerCustomization<TCommand, TResult> : ICustomization where TCommand : ICommand
	{
		private readonly Func<TCommand, IEnumerable<IEvent>> _handler;

		public CommandHandlerCustomization(Func<TCommand, IEnumerable<IEvent>> handler)
		{
			_handler = handler;
		}

		public void Customize(IFixture fixture)
		{
			var sut = fixture.Create<IEntityContext>();

			sut.HandleCommand<TCommand, TResult>(_handler);

			fixture.Inject(_handler);
		}
	}

	public class ApplyHandlerCustomization<TEvent> : ICustomization where TEvent : IEvent
	{
		private readonly Action<TEvent> _handler;

		public ApplyHandlerCustomization(Action<TEvent> handler)
		{
			_handler = handler;
		}

		public void Customize(IFixture fixture)
		{
			var sut = fixture.Create<IEntityContext>();

			sut.ApplyEvent(_handler);

			fixture.Inject(_handler);
		}
	}

	public class QueryHandlerCustomization<TQuery, TResult> : ICustomization where TQuery : IQuery
	{
		private readonly Func<TQuery, TResult> _handler;

		public QueryHandlerCustomization(Func<TQuery, TResult> handler)
		{
			_handler = handler;
		}

		public void Customize(IFixture fixture)
		{
			var sut = fixture.Create<IEntityContext>();

			sut.HandleQuery(_handler);

			fixture.Inject(_handler);
		}
	}

	public class ReactiveEntityContextTests
	{
		public static IEnumerable<IEvent> TestCommandHandler(TestCommand command)
		{
			yield return new TestEvent();
		}

		public static IEnumerable<IEvent> ErrorCommandHandler(TestCommand command)
		{
			yield return ThrowException();
		}

		public static IEnumerable<IEvent> TwiceCommandHandler(TestCommand command)
		{
			yield return new TestEvent();
			yield return new TestEvent();
		}

		public static IEnumerable<IEvent> EventThenErrorCommandHandler(TestCommand command)
		{
			yield return new TestEvent();
			yield return ThrowException();
			yield return new TestEvent();
		}

		public static IEvent ThrowException()
		{
			throw new Exception();
		}

		public static TestResult TestQueryHandler(TestQuery query)
		{
			return new TestResult();
		}

		public static TestResult ExceptionQueryHandler(TestQuery query)
		{
			throw new Exception();
		}

		public static void SendTestCommand(IObserver<IObservableCommand> commandsObserver)
			=> commandsObserver.OnNext(
				new ObservableCommand<TestCommand, Unit>(
					new TestCommand(), 
					e => e.Select(_ => Unit.Default)
				)
			);

		public static void SendTestQuery(IObserver<IObservableQuery> queriesObserver, IObserver<TestResult> resultObserver)
		{
			var query = new ObservableQuery<TestQuery, TestResult>(new TestQuery());

			query.Result.Subscribe(resultObserver);

			queriesObserver.OnNext(query);
		}

		public class WhenMockCommandHandlerRegistered
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new CommandHandlerCustomization<TestCommand, Unit>(A.Fake<Func<TestCommand, IEnumerable<IEvent>>>()))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenMockHandlerCalled(IObserver<IObservableCommand> commandsObserver, Func<TestCommand, IEnumerable<IEvent>> mockHandler)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => mockHandler.Invoke(A<TestCommand>._)).MustHaveHappened(Repeated.Exactly.Once);
			}
		}
		
		public class WhenTestCommandHandlerRegistered
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new CommandHandlerCustomization<TestCommand, Unit>(TestCommandHandler))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenTestEventObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnNext(A<TestEvent>._)).MustHaveHappened(Repeated.Exactly.Once);
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenErrorNotObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnError(A<Exception>._)).MustNotHaveHappened();
			}
		}

		public class WhenErrorCommandHandlerRegistered
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new CommandHandlerCustomization<TestCommand, Unit>(ErrorCommandHandler))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenTestEventNotObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnNext(A<TestEvent>._)).MustNotHaveHappened();
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenErrorObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnError(A<Exception>._)).MustHaveHappened(Repeated.Exactly.Once);
			}
		}

		public class WhenTwiceCommandHandlerRegistered
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new CommandHandlerCustomization<TestCommand, Unit>(TwiceCommandHandler))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenTestEventObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnNext(A<TestEvent>._)).MustHaveHappened(Repeated.Exactly.Twice);
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenErrorNotObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnError(A<Exception>._)).MustNotHaveHappened();
			}
		}

		public class WhenEventThenErrorCommandHandlerRegistered
		{

			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new CommandHandlerCustomization<TestCommand, Unit>(EventThenErrorCommandHandler))
						.Customize(new ApplyHandlerCustomization<TestEvent.Inv>(A.Fake<Action<TestEvent.Inv>>()))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenInverseTestEventAppliedThenErrorOut(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver, Action<TestEvent.Inv> mockApplyHandler)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => mockApplyHandler.Invoke(A<TestEvent.Inv>._)).MustHaveHappened(Repeated.Exactly.Once)
					.Then(A.CallTo(() => eventsOutObserver.OnError(A<Exception>._)).MustHaveHappened(Repeated.Exactly.Once));
			}

			[Theory, Arrange]
			public void SendTestCommand_ThenOutEventNotObserved(IObserver<IObservableCommand> commandsObserver, IObserver<IEvent> eventsOutObserver)
			{
				SendTestCommand(commandsObserver);

				A.CallTo(() => eventsOutObserver.OnNext(A<IEvent>._)).MustNotHaveHappened();
			}
		}

		public class WhenMockQueryHandlerRegistered
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new QueryHandlerCustomization<TestQuery, TestResult>(A.Fake<Func<TestQuery, TestResult>>()))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestQuery_ThenMockHandlerCalled(IObserver<IObservableQuery> queriesObserver, Func<TestQuery, TestResult> mockHandler, IObserver<TestResult> resultObserver)
			{
				SendTestQuery(queriesObserver, resultObserver);

				A.CallTo(() => mockHandler.Invoke(A<TestQuery>._)).MustHaveHappened(Repeated.Exactly.Once);
			}
		}

		public class WhenTestQueryHandlerRegistered
		{

			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new QueryHandlerCustomization<TestQuery, TestResult>(TestQueryHandler))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestQuery_ThenTestResultObserved(IObserver<IObservableQuery> queriesObserver, IObserver<IEvent> eventsOutObserver, IObserver<TestResult> resultObserver)
			{
				SendTestQuery(queriesObserver, resultObserver);

				A.CallTo(() => resultObserver.OnNext(A<TestResult>._)).MustHaveHappened(Repeated.Exactly.Once);
			}

			[Theory, Arrange]
			public void SendTestQuery_ThenErrorNotObserved(IObserver<IObservableQuery> queriesObserver, IObserver<IEvent> eventsOutObserver, IObserver<TestResult> resultObserver)
			{
				SendTestQuery(queriesObserver, resultObserver);

				A.CallTo(() => resultObserver.OnError(A<Exception>._)).MustNotHaveHappened();
			}
		}

		public class WhenExceptionQueryHandlerRegistered
		{

			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new QueryHandlerCustomization<TestQuery, TestResult>(ExceptionQueryHandler))
				)
				{ }
			}

			[Theory, Arrange]
			public void SendTestQuery_ThenTestResultNotObserved(IObserver<IObservableQuery> queriesObserver, IObserver<IEvent> eventsOutObserver, IObserver<TestResult> resultObserver)
			{
				SendTestQuery(queriesObserver, resultObserver);

				A.CallTo(() => resultObserver.OnNext(A<TestResult>._)).MustNotHaveHappened();
			}

			[Theory, Arrange]
			public void SendTestQuery_ThenErrorObserved(IObserver<IObservableQuery> queriesObserver, IObserver<IEvent> eventsOutObserver, IObserver<TestResult> resultObserver)
			{
				SendTestQuery(queriesObserver, resultObserver);

				A.CallTo(() => resultObserver.OnError(A<Exception>._)).MustHaveHappened(Repeated.Exactly.Once);
			}
		}
	}
}
