using System;
using System.Linq;

namespace Comics.Domain.Values
{
	public class IssueTitle : IEquatable<IssueTitle>
	{
		public readonly string Primary;
		public readonly string[] Secondary;

		public IssueTitle(string primary, params string[] secondary)
		{
			Primary = primary;
			Secondary = secondary;
		}

		public bool SamePrimary(IssueTitle other)
			=> other != null
			&& Primary == other.Primary;

		public bool Equals(IssueTitle other)
			=> SamePrimary(other)
			&& Secondary.SequenceEqual(other.Secondary);

		public override bool Equals(object obj)
			=> Equals(obj as IssueTitle);

		public override int GetHashCode()
			=> Secondary.Aggregate(37 * Primary.GetHashCode(), (accum, next) => accum + (37 * next.GetHashCode()));

		public override string ToString()
			=> $"{Primary} : {string.Join(", ", Secondary)}";

		public static bool operator ==(IssueTitle a, IssueTitle b)
			=> (ReferenceEquals(a, null) && ReferenceEquals(b, null))
			|| (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Equals(b));

		public static bool operator !=(IssueTitle a, IssueTitle b)
			=> !(a == b);
	}
}
