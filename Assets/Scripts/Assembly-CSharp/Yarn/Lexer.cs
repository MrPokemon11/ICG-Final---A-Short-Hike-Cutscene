using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Yarn
{
	internal class Lexer
	{
		internal class LexerState
		{
			public string name;

			private Dictionary<TokenType, string> patterns;

			public List<TokenRule> tokenRules = new List<TokenRule>();

			public bool setTrackNextIndentation;

			public bool containsTextRule
			{
				get
				{
					foreach (TokenRule tokenRule in tokenRules)
					{
						if (tokenRule.isTextRule)
						{
							return true;
						}
					}
					return false;
				}
			}

			public LexerState(Dictionary<TokenType, string> patterns)
			{
				this.patterns = patterns;
			}

			public TokenRule AddTransition(TokenType type, string entersState = null, bool delimitsText = false)
			{
				string pattern = $"\\G{patterns[type]}";
				TokenRule tokenRule = new TokenRule(type, new Regex(pattern), entersState, delimitsText);
				tokenRules.Add(tokenRule);
				return tokenRule;
			}

			public TokenRule AddTextRule(TokenType type, string entersState = null)
			{
				if (containsTextRule)
				{
					throw new InvalidOperationException("State already contains a text rule");
				}
				List<string> list = new List<string>();
				foreach (TokenRule tokenRule2 in tokenRules)
				{
					if (tokenRule2.delimitsText)
					{
						list.Add($"({tokenRule2.regex.ToString().Substring(2)})");
					}
				}
				string pattern = string.Format("\\G((?!{0}).)*", string.Join("|", list.ToArray()));
				TokenRule tokenRule = AddTransition(type, entersState);
				tokenRule.regex = new Regex(pattern);
				tokenRule.isTextRule = true;
				return tokenRule;
			}
		}

		internal class TokenRule
		{
			public Regex regex;

			public string entersState;

			public TokenType type;

			public bool isTextRule;

			public bool delimitsText;

			public TokenRule(TokenType type, Regex regex, string entersState = null, bool delimitsText = false)
			{
				this.regex = regex;
				this.entersState = entersState;
				this.type = type;
				this.delimitsText = delimitsText;
			}

			public override string ToString()
			{
				return string.Format($"[TokenRule: {type} - {regex}]");
			}
		}

		private const string LINE_COMMENT = "//";

		private Dictionary<string, LexerState> states;

		private LexerState defaultState;

		private LexerState currentState;

		private Stack<KeyValuePair<int, bool>> indentationStack;

		private bool shouldTrackNextIndentation;

		public Lexer()
		{
			CreateStates();
		}

		private void CreateStates()
		{
			Dictionary<TokenType, string> dictionary = new Dictionary<TokenType, string>();
			dictionary[TokenType.Text] = ".*";
			dictionary[TokenType.Number] = "\\-?[0-9]+(\\.[0-9+])?";
			dictionary[TokenType.String] = "\"([^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\"";
			dictionary[TokenType.TagMarker] = "\\##";
			dictionary[TokenType.LeftParen] = "\\(";
			dictionary[TokenType.RightParen] = "\\)";
			dictionary[TokenType.EqualTo] = "(==|is(?!\\w)|eq(?!\\w))";
			dictionary[TokenType.EqualToOrAssign] = "(=|to(?!\\w))";
			dictionary[TokenType.NotEqualTo] = "(\\!=|neq(?!\\w))";
			dictionary[TokenType.GreaterThanOrEqualTo] = "(\\>=|gte(?!\\w))";
			dictionary[TokenType.GreaterThan] = "(\\>|gt(?!\\w))";
			dictionary[TokenType.LessThanOrEqualTo] = "(\\<=|lte(?!\\w))";
			dictionary[TokenType.LessThan] = "(\\<|lt(?!\\w))";
			dictionary[TokenType.AddAssign] = "\\+=";
			dictionary[TokenType.MinusAssign] = "\\-=";
			dictionary[TokenType.MultiplyAssign] = "\\*=";
			dictionary[TokenType.DivideAssign] = "\\/=";
			dictionary[TokenType.Add] = "\\+";
			dictionary[TokenType.Minus] = "\\-";
			dictionary[TokenType.Multiply] = "\\*";
			dictionary[TokenType.Divide] = "\\/";
			dictionary[TokenType.Modulo] = "\\%";
			dictionary[TokenType.And] = "(\\&\\&|and(?!\\w))";
			dictionary[TokenType.Or] = "(\\|\\||or(?!\\w))";
			dictionary[TokenType.Xor] = "(\\^|xor(?!\\w))";
			dictionary[TokenType.Not] = "(\\!|not(?!\\w))";
			dictionary[TokenType.Variable] = "\\$([A-Za-z0-9_\\.])+";
			dictionary[TokenType.Comma] = ",";
			dictionary[TokenType.True] = "true(?!\\w)";
			dictionary[TokenType.False] = "false(?!\\w)";
			dictionary[TokenType.Null] = "null(?!\\w)";
			dictionary[TokenType.BeginCommand] = "\\<\\<";
			dictionary[TokenType.EndCommand] = "\\>\\>";
			dictionary[TokenType.OptionStart] = "\\[\\[";
			dictionary[TokenType.OptionEnd] = "\\]\\]";
			dictionary[TokenType.OptionDelimit] = "\\|";
			dictionary[TokenType.Identifier] = "[a-zA-Z0-9_:\\.]+";
			dictionary[TokenType.If] = "if(?!\\w)";
			dictionary[TokenType.Else] = "else(?!\\w)";
			dictionary[TokenType.ElseIf] = "elseif(?!\\w)";
			dictionary[TokenType.EndIf] = "endif(?!\\w)";
			dictionary[TokenType.Set] = "set(?!\\w)";
			dictionary[TokenType.ShortcutOption] = "\\-\\>";
			states = new Dictionary<string, LexerState>();
			states["base"] = new LexerState(dictionary);
			states["base"].AddTransition(TokenType.BeginCommand, "command", delimitsText: true);
			states["base"].AddTransition(TokenType.OptionStart, "link", delimitsText: true);
			states["base"].AddTransition(TokenType.ShortcutOption, "shortcut-option");
			states["base"].AddTransition(TokenType.TagMarker, "tag", delimitsText: true);
			states["base"].AddTextRule(TokenType.Text);
			states["tag"] = new LexerState(dictionary);
			states["tag"].AddTransition(TokenType.Identifier, "base");
			states["shortcut-option"] = new LexerState(dictionary);
			states["shortcut-option"].setTrackNextIndentation = true;
			states["shortcut-option"].AddTransition(TokenType.BeginCommand, "expression", delimitsText: true);
			states["shortcut-option"].AddTransition(TokenType.TagMarker, "shortcut-option-tag", delimitsText: true);
			states["shortcut-option"].AddTextRule(TokenType.Text, "base");
			states["shortcut-option-tag"] = new LexerState(dictionary);
			states["shortcut-option-tag"].AddTransition(TokenType.Identifier, "shortcut-option");
			states["command"] = new LexerState(dictionary);
			states["command"].AddTransition(TokenType.If, "expression");
			states["command"].AddTransition(TokenType.Else);
			states["command"].AddTransition(TokenType.ElseIf, "expression");
			states["command"].AddTransition(TokenType.EndIf);
			states["command"].AddTransition(TokenType.Set, "assignment");
			states["command"].AddTransition(TokenType.EndCommand, "base", delimitsText: true);
			states["command"].AddTransition(TokenType.Identifier, "command-or-expression");
			states["command"].AddTextRule(TokenType.Text);
			states["command-or-expression"] = new LexerState(dictionary);
			states["command-or-expression"].AddTransition(TokenType.LeftParen, "expression");
			states["command-or-expression"].AddTransition(TokenType.EndCommand, "base", delimitsText: true);
			states["command-or-expression"].AddTextRule(TokenType.Text);
			states["assignment"] = new LexerState(dictionary);
			states["assignment"].AddTransition(TokenType.Variable);
			states["assignment"].AddTransition(TokenType.EqualToOrAssign, "expression");
			states["assignment"].AddTransition(TokenType.AddAssign, "expression");
			states["assignment"].AddTransition(TokenType.MinusAssign, "expression");
			states["assignment"].AddTransition(TokenType.MultiplyAssign, "expression");
			states["assignment"].AddTransition(TokenType.DivideAssign, "expression");
			states["expression"] = new LexerState(dictionary);
			states["expression"].AddTransition(TokenType.EndCommand, "base");
			states["expression"].AddTransition(TokenType.Number);
			states["expression"].AddTransition(TokenType.String);
			states["expression"].AddTransition(TokenType.LeftParen);
			states["expression"].AddTransition(TokenType.RightParen);
			states["expression"].AddTransition(TokenType.EqualTo);
			states["expression"].AddTransition(TokenType.EqualToOrAssign);
			states["expression"].AddTransition(TokenType.NotEqualTo);
			states["expression"].AddTransition(TokenType.GreaterThanOrEqualTo);
			states["expression"].AddTransition(TokenType.GreaterThan);
			states["expression"].AddTransition(TokenType.LessThanOrEqualTo);
			states["expression"].AddTransition(TokenType.LessThan);
			states["expression"].AddTransition(TokenType.Add);
			states["expression"].AddTransition(TokenType.Minus);
			states["expression"].AddTransition(TokenType.Multiply);
			states["expression"].AddTransition(TokenType.Divide);
			states["expression"].AddTransition(TokenType.Modulo);
			states["expression"].AddTransition(TokenType.And);
			states["expression"].AddTransition(TokenType.Or);
			states["expression"].AddTransition(TokenType.Xor);
			states["expression"].AddTransition(TokenType.Not);
			states["expression"].AddTransition(TokenType.Variable);
			states["expression"].AddTransition(TokenType.Comma);
			states["expression"].AddTransition(TokenType.True);
			states["expression"].AddTransition(TokenType.False);
			states["expression"].AddTransition(TokenType.Null);
			states["expression"].AddTransition(TokenType.Identifier);
			states["link"] = new LexerState(dictionary);
			states["link"].AddTransition(TokenType.OptionEnd, "base", delimitsText: true);
			states["link"].AddTransition(TokenType.OptionDelimit, "link-destination", delimitsText: true);
			states["link"].AddTextRule(TokenType.Text);
			states["link-destination"] = new LexerState(dictionary);
			states["link-destination"].AddTransition(TokenType.Identifier);
			states["link-destination"].AddTransition(TokenType.OptionEnd, "base");
			defaultState = states["base"];
			foreach (KeyValuePair<string, LexerState> state in states)
			{
				state.Value.name = state.Key;
			}
		}

		public TokenList Tokenise(string title, string text)
		{
			indentationStack = new Stack<KeyValuePair<int, bool>>();
			indentationStack.Push(new KeyValuePair<int, bool>(0, value: false));
			shouldTrackNextIndentation = false;
			TokenList tokenList = new TokenList();
			currentState = defaultState;
			List<string> obj = new List<string>(text.Split('\n')) { "" };
			int num = 1;
			foreach (string item2 in obj)
			{
				tokenList.AddRange(TokeniseLine(item2, num));
				num++;
			}
			Token item = new Token(TokenType.EndOfInput, currentState, num, 0);
			tokenList.Add(item);
			return tokenList;
		}

		private TokenList TokeniseLine(string line, int lineNumber)
		{
			Stack<Token> stack = new Stack<Token>();
			line = line.Replace("\t", "    ");
			line = line.Replace("\r", "");
			int num = LineIndentation(line);
			KeyValuePair<int, bool> keyValuePair = indentationStack.Peek();
			if (shouldTrackNextIndentation && num > keyValuePair.Key)
			{
				indentationStack.Push(new KeyValuePair<int, bool>(num, value: true));
				Token token = new Token(TokenType.Indent, currentState, lineNumber, keyValuePair.Key);
				token.value = "".PadLeft(num - keyValuePair.Key);
				shouldTrackNextIndentation = false;
				stack.Push(token);
			}
			else if (num < keyValuePair.Key)
			{
				while (num < indentationStack.Peek().Key)
				{
					if (indentationStack.Pop().Value)
					{
						Token item = new Token(TokenType.Dedent, currentState, lineNumber, 0);
						stack.Push(item);
					}
				}
			}
			int num2 = num;
			Regex regex = new Regex("\\s*");
			while (num2 < line.Length && !line.Substring(num2).StartsWith("//"))
			{
				bool flag = false;
				foreach (TokenRule tokenRule in currentState.tokenRules)
				{
					Match match = tokenRule.regex.Match(line, num2);
					if (!match.Success || match.Length == 0)
					{
						continue;
					}
					string text;
					if (tokenRule.type == TokenType.Text)
					{
						int num3 = num;
						if (stack.Count > 0)
						{
							while (stack.Peek().type == TokenType.Identifier)
							{
								stack.Pop();
							}
							Token token2 = stack.Peek();
							num3 = token2.columnNumber;
							if (token2.type == TokenType.Indent)
							{
								num3 += token2.value.Length;
							}
							if (token2.type == TokenType.Dedent)
							{
								num3 = num;
							}
						}
						num2 = num3;
						int num4 = match.Index + match.Length;
						text = line.Substring(num3, num4 - num3);
					}
					else
					{
						text = match.Value;
					}
					num2 += text.Length;
					if (tokenRule.type == TokenType.String)
					{
						text = text.Substring(1, text.Length - 2);
						text = text.Replace("\\\\", "\\");
						text = text.Replace("\\\"", "\"");
					}
					Token token3 = new Token(tokenRule.type, currentState, lineNumber, num2, text);
					token3.delimitsText = tokenRule.delimitsText;
					stack.Push(token3);
					if (tokenRule.entersState != null)
					{
						if (!states.ContainsKey(tokenRule.entersState))
						{
							throw new TokeniserException(lineNumber, num2, "Unknown tokeniser state " + tokenRule.entersState);
						}
						EnterState(states[tokenRule.entersState]);
						if (shouldTrackNextIndentation && indentationStack.Peek().Key < num)
						{
							indentationStack.Push(new KeyValuePair<int, bool>(num, value: false));
						}
					}
					flag = true;
					break;
				}
				if (!flag)
				{
					throw TokeniserException.ExpectedTokensFromState(lineNumber, num2, currentState);
				}
				Match match2 = regex.Match(line, num2);
				if (match2 != null)
				{
					num2 += match2.Length;
				}
			}
			TokenList tokenList = new TokenList(stack.ToArray());
			tokenList.Reverse();
			return tokenList;
		}

		private int LineIndentation(string line)
		{
			Match match = new Regex("^(\\s*)").Match(line);
			if (match == null || match.Groups[0] == null)
			{
				return 0;
			}
			return match.Groups[0].Length;
		}

		private void EnterState(LexerState state)
		{
			currentState = state;
			if (currentState.setTrackNextIndentation)
			{
				shouldTrackNextIndentation = true;
			}
		}
	}
}
