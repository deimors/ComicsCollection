using System;

namespace Comics.Domain.Values
{
	public class IssueIdentifier
	{
		public readonly Guid Id;

		private IssueIdentifier(Guid id)
		{
			Id = id;
		}

		public static IssueIdentifier New()
			=> new IssueIdentifier(Guid.NewGuid());

		public static IssueIdentifier From(Guid id)
			=> new IssueIdentifier(id);

		public bool Equals(IssueIdentifier other)
			=> other != null && Id == other.Id;

		public override bool Equals(object obj)
			=> Equals(obj as IssueIdentifier);

		public override int GetHashCode()
			=> Id.GetHashCode();

		public override string ToString()
			=> $"Issue-{Id}";

		public static bool operator ==(IssueIdentifier a, IssueIdentifier b)
			=> (ReferenceEquals(a, null) && ReferenceEquals(b, null))
			|| (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Equals(b));

		public static bool operator !=(IssueIdentifier a, IssueIdentifier b)
			=> !(a == b);
	}
}
