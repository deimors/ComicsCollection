using Comics.API.AggregateRegistrars;
using Comics.API.Services;
using Comics.Domain.Aggregates;
using Comics.Domain.Events;
using Comics.Domain.Exceptions;
using Comics.Domain.Values;
using Comics.Tests.Customizations;
using Core.CQS;
using Core.Entities;
using Core.Extensions;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Reactive.Concurrency;
using Xunit;

namespace Comics.Tests.API
{
	public class IssuesServiceTests : ReactiveTest
	{
		private static readonly long[] DispatchTicks = new long[] { 1, 2, 3, 4 };
		private static readonly long DisposeTick = 10;

		public class BaseCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Customize(new AutoFakeItEasyCustomization());

				fixture.Customize(new ServiceBaseCustomization());

				fixture.Customize(new ScheduleDispatchCustomization(DispatchTicks));

				fixture.Freeze<IssuesAggregate>();
				fixture.Freeze<IssuesAggregateRegistrar>();

				fixture.Freeze<IssuesService>();

				fixture.Register(() => IssueIdentifier.New());
			}
		}

		public class CreateIssueCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				var sut = fixture.Create<IssuesService>();
				
				var dispatcher = fixture.Create<IDispatcher>();

				var title = fixture.Create<IssueTitle>();

				sut.CreateIssue(title).Subscribe(issueId => fixture.Inject(issueId));

				dispatcher.DispatchAll();
			}
		}
		
		public static ITestableObserver<IssueIdentifier> CreateIssue(IssuesService sut, IssueTitle title, TestScheduler scheduler)
			=> scheduler.Start(
				() => sut.CreateIssue(title),
				created: 0,
				subscribed: 0,
				disposed: DisposeTick
			);

		public static ITestableObserver<IssueTitle> GetTitle(IssuesService sut, IssueIdentifier issueId, TestScheduler scheduler)
			=> scheduler.Start(
				() => sut.GetTitle(issueId),
				created: 0,
				subscribed: 0,
				disposed: DisposeTick
			);

		public class WhenNew
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
				)
				{ }
			}

			[Theory, Arrange]
			public void CreateIssue_ThenReceivedIssueIdAndCompleted(IssuesService sut, TestScheduler scheduler, IssueTitle title)
			{
				var result = CreateIssue(sut, title, scheduler);

				var expected = new[] { OnNext(DispatchTicks[1], (IssueIdentifier id) => true), OnCompleted<IssueIdentifier>(DispatchTicks[1]) };

				ReactiveAssert.AreElementsEqual(expected, result.Messages);
			}

			[Theory, Arrange]
			public void CreateIssue_ThenEventBusReceivedIssueCreated(IssuesService sut, TestScheduler scheduler, IssueTitle title, IEventBus eventBus)
			{
				var result = CreateIssue(sut, title, scheduler);

				A.CallTo(() => eventBus.In.OnNext(A<IEvent>.That.Matches(e => e is IssuesEvents.Created))).MustHaveHappened();
			}

			[Theory, Arrange]
			public void CreateIssue_ThenEventBusReceivedIssueTitleSet(IssuesService sut, TestScheduler scheduler, IssueTitle title, IEventBus eventBus)
			{
				var result = CreateIssue(sut, title, scheduler);

				A.CallTo(() => eventBus.In.OnNext(A<IEvent>.That.Matches(e => e is IssuesEvents.TitleSet))).MustHaveHappened();
			}

			[Theory, Arrange]
			public void CreateIssueWithEmptyTitle_ThenReceivedOnError(IssuesService sut, TestScheduler scheduler, IEventBus eventBus)
			{
				var result = CreateIssue(sut, new IssueTitle(string.Empty), scheduler);

				var expected = new[] { OnError<IssueIdentifier>(DispatchTicks[1], ex => ex is IssuesExceptions.EmptyTitle) };

				ReactiveAssert.AreElementsEqual(expected, result.Messages);
			}

			[Theory, Arrange]
			public void CreateIssueWithEmptyTitle_ThenEventBusReceivedEmptyTitleException(IssuesService sut, TestScheduler scheduler, IEventBus eventBus)
			{
				var result = CreateIssue(sut, new IssueTitle(string.Empty), scheduler);

				A.CallTo(() => eventBus.In.OnError(A<Exception>.That.Matches(e => e is IssuesExceptions.EmptyTitle))).MustHaveHappened();
			}

			[Theory, Arrange]
			public void GetTitle_ThenEventBusReceivedNotFoundException(IssuesService sut, TestScheduler scheduler, IEventBus eventBus, IssueIdentifier issueId)
			{
				var result = GetTitle(sut, issueId, scheduler);

				A.CallTo(() => eventBus.In.OnError(A<Exception>.That.Matches(e => e is IssuesExceptions.NotFound))).MustHaveHappened();
			}
		}

		public class WhenIssueCreated
		{
			public class ArrangeAttribute : AutoDataAttribute
			{
				public ArrangeAttribute() : base(
					new Fixture()
						.Customize(new BaseCustomization())
						.Customize(new CreateIssueCustomization())
				)
				{ }
			}

			[Theory, Arrange]
			public void GetTitle_ThenReceivedIssueTitle(IssuesService sut, TestScheduler scheduler, IssueIdentifier issueId)
			{
				var result = GetTitle(sut, issueId, scheduler);

				var expected = new[] { OnNext<IssueTitle>(DispatchTicks[1], title => true), OnCompleted<IssueTitle>(DispatchTicks[1]) };

				ReactiveAssert.AreElementsEqual(expected, result.Messages);
			}

			[Theory, Arrange]
			public void GetTitleForDifferentId_ThenReceivedNotFoundException(IssuesService sut, TestScheduler scheduler)
			{
				var result = GetTitle(sut, IssueIdentifier.New(), scheduler);

				var expected = new[] { OnError<IssueTitle>(DispatchTicks[1], ex => ex is IssuesExceptions.NotFound) };

				ReactiveAssert.AreElementsEqual(expected, result.Messages);
			}
		}
	}
}
