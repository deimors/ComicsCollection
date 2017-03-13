using Comics.Domain.Values;
using Comics.Exceptions;
using System.Collections.Generic;

namespace Comics.Domain.Entities
{
	public class Collection
	{
		public readonly CollectionIdentifier CollectionId;

		private readonly IList<IssueIdentifier> _issues = new List<IssueIdentifier>();

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new CollectionsExceptions.EmptyName(CollectionId);

				_name = value;
			}
		}

		public Collection(CollectionIdentifier collectionId)
		{
			CollectionId = collectionId ?? CollectionIdentifier.New();
		}

		public Collection() : this(null) { }
	}
}
