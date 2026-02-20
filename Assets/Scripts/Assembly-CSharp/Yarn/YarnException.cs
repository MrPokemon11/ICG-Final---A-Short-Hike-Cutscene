using System;

namespace Yarn
{
	public class YarnException : Exception
	{
		public YarnException(string message)
			: base(message)
		{
		}
	}
}
