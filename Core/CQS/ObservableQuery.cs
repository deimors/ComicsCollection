using System.Reactive.Subjects;

namespace Core.CQS
{
	public interface IObservableQuery
	{

	}

	public class ObservableQuery<TQuery, TResult> : IObservableQuery
	{
		public TQuery Query { get; }

		public ISubject<TResult> Result { get; } = new Subject<TResult>();
		
		public ObservableQuery(TQuery query)
		{
			Query = query;
		}
	}
}
