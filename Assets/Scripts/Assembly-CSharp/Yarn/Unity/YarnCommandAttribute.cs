using System;

namespace Yarn.Unity
{
	public class YarnCommandAttribute : Attribute
	{
		public string commandString { get; private set; }

		public YarnCommandAttribute(string commandString)
		{
			this.commandString = commandString;
		}
	}
}
