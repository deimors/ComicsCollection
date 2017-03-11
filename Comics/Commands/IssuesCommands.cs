using Comics.Domain.Values;
using Core.CQS;

namespace Comics.Domain.Commands
{
	public static class IssuesCommands
	{
		public class Create : ICommand
		{
			public readonly IssueTitle Title;

			public Create(IssueTitle title)
			{
				Title = title;
			}
		}

		public abstract class IssueCommand : ICommand
		{
			public readonly IssueIdentifier IssueId;

			protected IssueCommand(IssueIdentifier issueId)
			{
				IssueId = issueId;
			}
		}

		public class SetTitle : IssueCommand
		{
			public readonly IssueTitle Title;

			public SetTitle(IssueIdentifier issueId, IssueTitle title) : base(issueId)
			{
				Title = title;
			}
		}

		public class Destroy : IssueCommand
		{
			public Destroy(IssueIdentifier issueId) : base(issueId)
			{
			}
		}
	}
}
