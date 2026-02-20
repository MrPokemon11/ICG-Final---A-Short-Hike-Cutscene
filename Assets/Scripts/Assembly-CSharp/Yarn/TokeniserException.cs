using System;
using System.Collections.Generic;

namespace Yarn
{
	internal class TokeniserException : InvalidOperationException
	{
		public int lineNumber;

		public int columnNumber;

		public TokeniserException(string message)
			: base(message)
		{
		}

		public TokeniserException(int lineNumber, int columnNumber, string message)
			: base($"{lineNumber}:{columnNumber}: {message}")
		{
			this.lineNumber = lineNumber;
			this.columnNumber = columnNumber;
		}

		public static TokeniserException ExpectedTokensFromState(int lineNumber, int columnNumber, Lexer.LexerState state)
		{
			List<string> list = new List<string>();
			foreach (Lexer.TokenRule tokenRule in state.tokenRules)
			{
				list.Add(tokenRule.type.ToString());
			}
			string text;
			if (list.Count > 1)
			{
				text = string.Join(", ", list.ToArray(), 0, list.Count - 1);
				text = text + ", or " + list[list.Count - 1];
			}
			else
			{
				text = list[0];
			}
			string message = $"Expected {text}";
			return new TokeniserException(lineNumber, columnNumber, message);
		}
	}
}
