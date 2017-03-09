using System;
using System.Linq;

namespace Comics.Domain.Values
{
	public class AuthorName : IEquatable<AuthorName>
	{
		public readonly string[] Names;

		public AuthorName(params string[] names)
		{
			Names = names;
		}

		public bool Equals(AuthorName other)
			=> other != null
			&& Names.SequenceEqual(other.Names);

		public override bool Equals(object obj)
			=> Equals(obj as AuthorName);

		public override int GetHashCode()
			=> Names.Aggregate(0, (accum, next) => accum + (163 * next.GetHashCode()));

		public override string ToString()
			=> string.Join(" ", Names);

		public static bool operator ==(AuthorName a, AuthorName b)
			=> (ReferenceEquals(a, null) && ReferenceEquals(b, null))
			|| (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Equals(b));

		public static bool operator !=(AuthorName a, AuthorName b)
			=> !(a == b);
	}
}
