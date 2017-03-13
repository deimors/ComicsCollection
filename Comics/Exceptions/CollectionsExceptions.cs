using Comics.Domain.Values;
using System;

namespace Comics.Exceptions
{
	public class CollectionsExceptions
	{
		public class EmptyName : Exception
		{
			public readonly CollectionIdentifier CollectionId;

			public EmptyName(CollectionIdentifier collectionId)
			{
				CollectionId = collectionId;
			}
		}

		public class NotFound : Exception
		{
			private readonly CollectionIdentifier CollectionId;

			public NotFound(CollectionIdentifier collectionId)
			{
				CollectionId = collectionId;
			}
		}
	}
}
