using Comics.Domain.Values;
using Core.CQS;
using System;

namespace Comics.Domain.Events
{
	public static class IssuesEvents
	{
		public class TitleSet : IEvent
		{
			public readonly IssueIdentifier IssueId;
			public readonly IssueTitle Title;
			public readonly IssueTitle OldTitle;

			public IEvent Inverse => new TitleSet(IssueId, OldTitle, Title);

			public TitleSet(IssueIdentifier issueId, IssueTitle title, IssueTitle oldTitle)
			{
				IssueId = issueId;
				Title = title;
				OldTitle = oldTitle;
			}
		}

		public class Created : IEvent
		{
			public readonly IssueIdentifier IssueId;

			public Created(IssueIdentifier issueId)
			{
				IssueId = issueId;
			}

			public IEvent Inverse => new Destroyed(IssueId);
		}

		public class Destroyed : IEvent
		{
			public readonly IssueIdentifier IssueId;

			public Destroyed(IssueIdentifier issueId)
			{
				IssueId = issueId;
			}

			public IEvent Inverse => new Created(IssueId);
		}
	}
}
