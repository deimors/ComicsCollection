using System;

namespace Comics.Domain.Values
{
	public class AuthorIdentifier
	{
		public readonly Guid Id;

		private AuthorIdentifier(Guid id)
		{
			Id = id;
		}

		public static AuthorIdentifier New()
			=> new AuthorIdentifier(Guid.NewGuid());

		public static AuthorIdentifier From(Guid id)
			=> new AuthorIdentifier(id);

		public bool Equals(AuthorIdentifier other)
			=> other != null && Id == other.Id;

		public override bool Equals(object obj)
			=> Equals(obj as AuthorIdentifier);

		public override int GetHashCode()
			=> Id.GetHashCode();

		public override string ToString()
			=> $"Author-{Id}";

		public static bool operator ==(AuthorIdentifier a, AuthorIdentifier b)
			=> (ReferenceEquals(a, null) && ReferenceEquals(b, null))
			|| (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Equals(b));

		public static bool operator !=(AuthorIdentifier a, AuthorIdentifier b)
			=> !(a == b);
	}
}
