using Core.CQS;
using Core.Extensions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using System;
using System.Reactive.Concurrency;

namespace Comics.Tests.Customizations
{
	public class ScheduleDispatchCustomization : ICustomization
	{
		private readonly long[] _ticks;

		public ScheduleDispatchCustomization(params long[] ticks)
		{
			_ticks = ticks;
		}

		public void Customize(IFixture fixture)
		{
			var dispatcher = fixture.Create<IDispatcher>();
			var scheduler = fixture.Create<TestScheduler>();

			_ticks.Apply(
				tick => scheduler.Schedule(
					TimeSpan.FromTicks(tick),
					() => { dispatcher.Dispatch(); }
				)
			);
		}
	}
}
