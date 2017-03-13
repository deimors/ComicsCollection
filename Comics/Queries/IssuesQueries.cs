using Comics.Domain.Values;
using Core.CQS;

namespace Comics.Queries
{
	public class IssuesQueries
	{
		public class GetTitle : IQuery
		{
			public readonly IssueIdentifier IssueId;

			public GetTitle(IssueIdentifier issueId)
			{
				IssueId = issueId;
			}
		}
	}
}
