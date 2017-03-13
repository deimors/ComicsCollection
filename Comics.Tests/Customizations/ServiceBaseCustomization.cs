using Comics.API.Services;
using Comics.Domain.Values;
using Core.CQS;
using Core.Entities;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;

namespace Comics.Tests.Customizations
{
	class ServiceBaseCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
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
}
