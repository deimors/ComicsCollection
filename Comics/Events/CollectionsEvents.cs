using Comics.Domain.Values;
using Core.CQS;

namespace Comics.Domain.Events
{
	public class CollectionsEvents
	{
		public class Created : IEvent
		{
			public readonly CollectionIdentifier CollectionId;

			public Created(CollectionIdentifier collectionId)
			{
				CollectionId = collectionId;
			}

			public IEvent Inverse => new Destroyed(CollectionId);
		}

		public class Destroyed : IEvent
		{
			public readonly CollectionIdentifier CollectionId;

			public Destroyed(CollectionIdentifier collectionId)
			{
				CollectionId = collectionId;
			}

			public IEvent Inverse => new Created(CollectionId);
		}

		public class NameSet : IEvent
		{
			public readonly CollectionIdentifier IssueId;
			public readonly string Name;
			public readonly string OldName;

			public IEvent Inverse => new NameSet(IssueId, OldName, Name);

			public NameSet(CollectionIdentifier issueId, string name, string oldName)
			{
				IssueId = issueId;
				Name = name;
				OldName = oldName;
			}
		}
	}
}
