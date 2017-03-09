using System;
using System.Collections.Generic;

namespace Core.Extensions
{
	public static class EnumerableExtensions
	{
		public static void Apply<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (action == null) throw new ArgumentNullException(nameof(action));

			foreach (var item in source)
			{
				action(item);
			}
		}
	}
}
