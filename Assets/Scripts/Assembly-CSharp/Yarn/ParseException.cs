using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Yarn
{
	[Serializable]
	internal class ParseException : Exception
	{
		internal int lineNumber;

		internal static ParseException Make(Token foundToken, params TokenType[] expectedTypes)
		{
			int num = foundToken.lineNumber + 1;
			List<string> list = new List<string>();
			for (int i = 0; i < expectedTypes.Length; i++)
			{
				TokenType tokenType = expectedTypes[i];
				list.Add(tokenType.ToString());
			}
			string text = string.Join(",", list.ToArray());
			return new ParseException($"Line {num}:{foundToken.columnNumber}: Expected {text}, but found {foundToken.type.ToString()}")
			{
				lineNumber = num
			};
		}

		internal static ParseException Make(Token mostRecentToken, string message)
		{
			int num = mostRecentToken.lineNumber + 1;
			return new ParseException($"Line {num}:{mostRecentToken.columnNumber}: {message}")
			{
				lineNumber = num
			};
		}

		internal static ParseException Make(ParserRuleContext context, string message)
		{
			int line = context.Start.Line;
			int startIndex = context.Start.StartIndex;
			int stopIndex = context.Stop.StopIndex;
			string text = context.Start.InputStream.GetText(new Interval(startIndex, stopIndex));
			return new ParseException($"Error on line {line}\n{text}\n{message}")
			{
				lineNumber = line
			};
		}

		internal ParseException(string message)
			: base(message)
		{
		}
	}
}
