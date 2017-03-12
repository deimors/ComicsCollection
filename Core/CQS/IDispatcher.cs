namespace Core.CQS
{
	public interface IDispatcher
	{
		bool Dispatch();
		bool DispatchAll();
	}
}