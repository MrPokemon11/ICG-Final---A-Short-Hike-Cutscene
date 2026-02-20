using System.Collections.Generic;

namespace Yarn
{
	internal class TokenList : List<Token>
	{
		public TokenList(params Token[] tokens)
		{
			AddRange(tokens);
		}
	}
}
