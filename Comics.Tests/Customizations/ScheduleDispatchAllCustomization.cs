using Core.CQS;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using System;
using System.Reactive.Concurrency;

namespace Comics.Tests.Customizations
{
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
}
