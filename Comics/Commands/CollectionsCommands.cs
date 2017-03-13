using Core.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comics.Commands
{
	public class CollectionsCommands
	{
		public class Create : ICommand
		{
			public readonly string Name;

			public Create(string name)
			{
				Name = name;
			}
		}
	}
}
