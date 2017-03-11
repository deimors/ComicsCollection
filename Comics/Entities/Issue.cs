using Comics.Domain.Exceptions;
using Comics.Domain.Values;

namespace Comics.Domain.Entities
{
	public class Issue
	{
		public readonly IssueIdentifier IssueId;

		private IssueTitle _title;
		public IssueTitle Title
		{
			get { return _title; }
			set
			{
				if (string.IsNullOrEmpty(value.Primary))
					throw new IssuesExceptions.EmptyTitle(IssueId);

				_title = value;
			}
		}

		public Issue(IssueIdentifier issueId)
		{
			IssueId = issueId ?? IssueIdentifier.New();
		}

		public Issue() : this(null) { }
	}
}
