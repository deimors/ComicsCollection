using Comics.API.AggregateRegistrars;
using Comics.API.Services;
using Comics.Domain.Aggregates;
using Comics.Domain.Values;
using Comics.Tests.Customizations;
using Core.CQS;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using System;
using Xunit;

namespace Comics.Tests.API
{
	public class CollectionsServiceTests : ReactiveTest
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

				fixture.Freeze<CollectionsAggregate>();
				fixture.Freeze<CollectionsAggregateRegistrar>();

				fixture.Freeze<CollectionsService>();

				fixture.Register(() => CollectionIdentifier.New());
			}
		}

		public class CreateCollectionCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				var sut = fixture.Create<CollectionsService>();

				var dispatcher = fixture.Create<IDispatcher>();

				var title = fixture.Create<string>();

				sut.Create(title).Subscribe(collectionId => fixture.Inject(collectionId));

				dispatcher.DispatchAll();
			}
		}

		public static ITestableObserver<CollectionIdentifier> CreateCollection(CollectionsService sut, string name, TestScheduler scheduler)
			=> scheduler.Start(
				() => sut.Create(name),
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
			public void CreateCollection_ThenReceivedCollectionIdAndCompleted(CollectionsService sut, TestScheduler scheduler, string name)
			{
				var result = CreateCollection(sut, name, scheduler);

				var expected = new[] { OnNext(DispatchTicks[1], (CollectionIdentifier id) => true), OnCompleted<CollectionIdentifier>(DispatchTicks[1]) };

				ReactiveAssert.AreElementsEqual(expected, result.Messages);
			}
		}
	}
}
