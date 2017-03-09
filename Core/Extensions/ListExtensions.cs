using System.Collections.Generic;

namespace Core.Extensions
{
	public static class ListExtensions
	{
		public static void AddTo<T>(this T item, IList<T> list)
			=> list.Add(item);
	}
}
