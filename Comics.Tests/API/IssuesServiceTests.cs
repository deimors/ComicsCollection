using System;
using System.Text;
using System.Collections.Generic;
using Xunit;
using Microsoft.Reactive.Testing;
using FakeItEasy;
using Core.CQS;
using Comics.API.AggregateWrappers;
using Core.Entities;
using Comics.Domain.Aggregates;
using Comics.API.Services;
using Comics.Domain.Values;
using System.Reactive.Concurrency;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Comics.Domain.Events;

namespace Comics.Tests.API
{
	public class IssuesServiceTests : ReactiveTest
	{
		public class BaseCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Customize(new AutoFakeItEasyCustomization());

				var eventBus = fixture.Create<IEventBus>();
				fixture.Inject(eventBus);

				var dispatcher = fixture.Create<CQSDispatcher>();
				fixture.Inject<ICQSDispatcher>(dispatcher);
				fixture.Inject<ICQSContext>(dispatcher);
				fixture.Inject<IDispatcher>(dispatcher);

				var context = fixture.Create<ReactiveEntityContext>();
				fixture.Inject<IEntityContext>(context);

				fixture.Freeze<TestScheduler>();
			}
		}

		public class IssuesAggregateCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Freeze<IssuesAggregate>();
				fixture.Freeze<IssuesAggregateWrapper>();
			}
		}

		public class ScheduleDispatchAllCustomization : ICustomization
		{
			private readonly long _ticks;

			public ScheduleDispatchAllCustomization(long ticks = 1)
			{
				_ticks = ticks;
			}

			public void Customize(IFixture fixture)
			{
				var dispatcher = fixture.Create<IDispatcher>();
				var scheduler = fixture.Create<TestScheduler>();

				scheduler.Schedule(TimeSpan.FromTicks(_ticks), () => { dispatcher.DispatchAll(); });
			}
		}

		public class WhenNewWithIssueAggregate
		{
			private static readonly long DispatchTick = 5;
			private static readonly long DisposeTick = 10;

			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new IssuesAggregateCustomization())
						.Customize(new ScheduleDispatchAllCustomization(DispatchTick))
				)
				{ }
			}

			public static ITestableObserver<IssueIdentifier> CreateIssue(IssuesService sut, IssueTitle title, TestScheduler scheduler)
				=> scheduler.Start(
					() => sut.CreateIssue(title),
					created: 0,
					subscribed: 0,
					disposed: DisposeTick
				);

			[Theory, Arrange]
			public void CreateIssueAndDispatchAll_ThenReceivedIssueIdAndCompleted(IssuesService sut, TestScheduler scheduler, IDispatcher dispatcher, IssueTitle title)
			{
				var result = CreateIssue(sut, title, scheduler);

				var expected = new[] { OnNext(5, (IssueIdentifier id) => true), OnCompleted<IssueIdentifier>(5) };

				ReactiveAssert.AreElementsEqual(expected, result.Messages);
			}

			[Theory, Arrange]
			public void CreateIssueAndDispatchAll_ThenEventBusReceivedIssueCreated(IssuesService sut, TestScheduler scheduler, IDispatcher dispatcher, IssueTitle title, IEventBus eventBus)
			{
				var result = CreateIssue(sut, title, scheduler);

				A.CallTo(() => eventBus.In.OnNext(A<IEvent>.That.Matches(e => e is IssuesEvents.Created))).MustHaveHappened();
			}

			[Theory, Arrange]
			public void CreateIssueAndDispatchAll_ThenEventBusReceivedIssueTitleSet(IssuesService sut, TestScheduler scheduler, IDispatcher dispatcher, IssueTitle title, IEventBus eventBus)
			{
				var result = CreateIssue(sut, title, scheduler);

				A.CallTo(() => eventBus.In.OnNext(A<IEvent>.That.Matches(e => e is IssuesEvents.TitleSet))).MustHaveHappened();
			}
		}
	}
}
