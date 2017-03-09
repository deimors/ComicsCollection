namespace Core.Functional
{
	public class Result
	{
		public readonly bool Success;
		public readonly string[] Errors;

		protected Result(bool success, params string[] errors)
		{
			Success = success;
			Errors = errors;
		}

		private static Result _success = new Result(true);

		public static Result Succeed() => _success;
		public static Result Fail(params string[] errors) => new Result(false, errors);
	}

	public class Result<T> : Result
	{
		public readonly T Value;

		protected Result(bool success, T value, params string[] errors)
			: base(success, errors)
		{
			Value = value;
		}

		public static Result Succeed(T value) => new Result<T>(true, value);
		new public static Result Fail(params string[] errors) => new Result<T>(false, default(T), errors);
	}
}
