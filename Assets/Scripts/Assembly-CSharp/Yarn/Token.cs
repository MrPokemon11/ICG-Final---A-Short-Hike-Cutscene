namespace Yarn
{
	internal class Token
	{
		public TokenType type;

		public string value;

		public int lineNumber;

		public int columnNumber;

		public string context;

		public bool delimitsText;

		public int parameterCount;

		public string lexerState;

		public Token(TokenType type, Lexer.LexerState lexerState, int lineNumber = -1, int columnNumber = -1, string value = null)
		{
			this.type = type;
			this.value = value;
			this.lineNumber = lineNumber;
			this.columnNumber = columnNumber;
			this.lexerState = lexerState.name;
		}

		public override string ToString()
		{
			if (value != null)
			{
				return $"{type.ToString()} ({value.ToString()}) at {lineNumber}:{columnNumber} (state: {lexerState})";
			}
			return $"{type} at {lineNumber}:{columnNumber} (state: {lexerState})";
		}
	}
}
