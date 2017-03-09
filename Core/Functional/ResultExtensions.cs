using System;

namespace Core.Functional
{
	public static class ResultExtensions
	{
		public static void Match(this Result result, Action success, Action<string[]> fail)
		{
			if (result.Success)
				success();
			else
				fail(result.Errors);
		}

		public static void Match<T>(this Result<T> result, Action<T> success, Action<string[]> fail)
		{
			if (result.Success)
				success(result.Value);
			else
				fail(result.Errors);
		}
	}
}
