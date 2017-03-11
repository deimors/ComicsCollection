using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CQS
{
	public interface IEventBus
	{
		IObserver<IEvent> In { get; }

		IObservable<IEvent> Out { get; }
	}
}
