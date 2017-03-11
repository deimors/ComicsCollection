using Comics.Domain.Values;
using System;

namespace Comics.Domain.Exceptions
{
	public static class IssuesExceptions
	{
		public class NotFound : Exception
		{
			private readonly IssueIdentifier IssueId;

			public NotFound(IssueIdentifier issueId)
			{
				IssueId = issueId;
			}
		}

		public class EmptyTitle : Exception
		{
			public readonly IssueIdentifier IssueId;

			public EmptyTitle(IssueIdentifier issueId)
			{
				IssueId = issueId;
			}
		}
	}
}
