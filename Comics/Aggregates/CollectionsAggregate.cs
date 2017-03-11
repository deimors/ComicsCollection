using Comics.Domain.Entities;
using Comics.Domain.Values;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comics.Domain.Aggregates
{
	public class CollectionsAggregate
	{
		public IDictionary<CollectionIdentifier, Collection> _collections = new Dictionary<CollectionIdentifier, Collection>();

		public CollectionsAggregate(IEntityContext context)
		{
			
		}
	}
}
