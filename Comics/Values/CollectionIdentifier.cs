using System;

namespace Comics.Domain.Values
{
	public class CollectionIdentifier : IEquatable<CollectionIdentifier>
	{
		public readonly Guid Id;
		
		private CollectionIdentifier(Guid id)
		{
			Id = id;
		}

		public static CollectionIdentifier New()
			=> new CollectionIdentifier(Guid.NewGuid());

		public static CollectionIdentifier From(Guid id)
			=> new CollectionIdentifier(id);

		public bool Equals(CollectionIdentifier other)
			=> other != null && Id == other.Id;

		public override bool Equals(object obj)
			=> Equals(obj as CollectionIdentifier);
		
		public override int GetHashCode()
			=> Id.GetHashCode();

		public override string ToString()
			=> $"collection-{Id}";

		public static bool operator ==(CollectionIdentifier a, CollectionIdentifier b)
			=> (ReferenceEquals(a, null) && ReferenceEquals(b, null))
			|| (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Equals(b));

		public static bool operator !=(CollectionIdentifier a, CollectionIdentifier b)
			=> !(a == b);
	}
}
