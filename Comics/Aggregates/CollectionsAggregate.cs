using Comics.Domain.Entities;
using Comics.Domain.Events;
using Comics.Domain.Values;
using Comics.Exceptions;
using Core.Entities;
using System;
using System.Collections.Generic;

namespace Comics.Domain.Aggregates
{
	public class CollectionsAggregate
	{
		public IDictionary<CollectionIdentifier, Collection> _collections = new Dictionary<CollectionIdentifier, Collection>();

		public CollectionsAggregate(IEntityContext context)
		{
			
		}
		
		public CollectionsEvents.Created Create(CollectionIdentifier collectionId = null)
		{
			var newCollection = new Collection(collectionId);

			_collections.Add(newCollection.CollectionId, newCollection);

			return new CollectionsEvents.Created(newCollection.CollectionId);
		}

		public CollectionsEvents.NameSet SetName(CollectionIdentifier collectionId, string name)
		{
			Collection collection;

			if (collectionId == null)
				throw new ArgumentNullException(nameof(collectionId));

			if (!_collections.TryGetValue(collectionId, out collection))
			{
				throw new CollectionsExceptions.NotFound(collectionId);
			}

			var oldName = collection.Name;

			collection.Name = name;

			return new CollectionsEvents.NameSet(collection.CollectionId, collection.Name, oldName);
		}
	}
}
