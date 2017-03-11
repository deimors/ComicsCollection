using Comics.Domain.Values;

namespace Comics.Domain.Entities
{
	public class Collection
	{
		public readonly CollectionIdentifier CollectionId;

		public Collection(CollectionIdentifier collectionId)
		{
			CollectionId = collectionId ?? CollectionIdentifier.New();
		}

		public Collection() : this(null) { }
	}
}
