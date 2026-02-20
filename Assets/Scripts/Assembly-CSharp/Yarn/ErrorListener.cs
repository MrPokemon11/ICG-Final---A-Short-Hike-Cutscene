using System.IO;
using System.Text;
using Antlr4.Runtime;

namespace Yarn
{
	public sealed class ErrorListener : BaseErrorListener
	{
		private static readonly ErrorListener instance = new ErrorListener();

		public static ErrorListener Instance => instance;

		private ErrorListener()
		{
		}

		public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			object[] args = new object[2]
			{
				line,
				charPositionInLine + 1
			};
			stringBuilder.AppendFormat("Error on line {0} at position {1}:\n", args);
			stringBuilder.AppendLine(msg);
			string value = offendingSymbol.TokenSource.InputStream.ToString().Split('\n')[line - 1];
			stringBuilder.AppendLine(value);
			int startIndex = offendingSymbol.StartIndex;
			int stopIndex = offendingSymbol.StopIndex;
			if (startIndex >= 0 && stopIndex >= 0)
			{
				int num = stopIndex - startIndex + charPositionInLine + 1;
				for (int i = 0; i < num; i++)
				{
					if (i >= charPositionInLine && i < num)
					{
						stringBuilder.Append("^");
					}
					else
					{
						stringBuilder.Append(" ");
					}
				}
			}
			throw new ParseException(stringBuilder.ToString());
		}
	}
}
