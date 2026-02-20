using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Yarn
{
	internal class Parser
	{
		internal abstract class ParseNode
		{
			internal ParseNode parent;

			internal int lineNumber;

			internal string[] tags = new string[0];

			internal ParseNode(ParseNode parent, Parser p)
			{
				this.parent = parent;
				if (p.tokens.Count > 0)
				{
					lineNumber = p.tokens.Peek().lineNumber;
				}
				else
				{
					lineNumber = -1;
				}
			}

			internal abstract string PrintTree(int indentLevel);

			public string TagsToString(int indentLevel)
			{
				if (tags.Length != 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(Tab(indentLevel + 1, "Tags:"));
					string[] array = tags;
					foreach (string text in array)
					{
						stringBuilder.Append(Tab(indentLevel + 2, "#" + text));
					}
					return stringBuilder.ToString();
				}
				return "";
			}

			public override string ToString()
			{
				return GetType().Name;
			}

			internal Node NodeParent()
			{
				ParseNode parseNode = this;
				do
				{
					if (parseNode is Node)
					{
						return parseNode as Node;
					}
					parseNode = parseNode.parent;
				}
				while (parseNode != null);
				return null;
			}
		}

		internal class Node : ParseNode
		{
			private List<Statement> _statements = new List<Statement>();

			internal string name { get; set; }

			internal string source { get; set; }

			internal List<string> nodeTags { get; set; }

			internal IEnumerable<Statement> statements => _statements;

			internal Node(string name, ParseNode parent, Parser p)
				: base(parent, p)
			{
				this.name = name;
				while (p.tokens.Count > 0 && !p.NextSymbolIs(TokenType.Dedent, TokenType.EndOfInput))
				{
					_statements.Add(new Statement(this, p));
				}
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Tab(indentLevel, "Node " + name + " {"));
				foreach (Statement statement in _statements)
				{
					stringBuilder.Append(statement.PrintTree(indentLevel + 1));
				}
				stringBuilder.Append(Tab(indentLevel, "}"));
				return stringBuilder.ToString();
			}
		}

		internal class Statement : ParseNode
		{
			internal enum Type
			{
				CustomCommand = 0,
				ShortcutOptionGroup = 1,
				Block = 2,
				IfStatement = 3,
				OptionStatement = 4,
				AssignmentStatement = 5,
				Line = 6
			}

			internal Type type { get; private set; }

			internal Block block { get; private set; }

			internal IfStatement ifStatement { get; private set; }

			internal OptionStatement optionStatement { get; private set; }

			internal AssignmentStatement assignmentStatement { get; private set; }

			internal CustomCommand customCommand { get; private set; }

			internal string line { get; private set; }

			internal ShortcutOptionGroup shortcutOptionGroup { get; private set; }

			internal Statement(ParseNode parent, Parser p)
				: base(parent, p)
			{
				if (Block.CanParse(p))
				{
					type = Type.Block;
					block = new Block(this, p);
				}
				else if (IfStatement.CanParse(p))
				{
					type = Type.IfStatement;
					ifStatement = new IfStatement(this, p);
				}
				else if (OptionStatement.CanParse(p))
				{
					type = Type.OptionStatement;
					optionStatement = new OptionStatement(this, p);
				}
				else if (AssignmentStatement.CanParse(p))
				{
					type = Type.AssignmentStatement;
					assignmentStatement = new AssignmentStatement(this, p);
				}
				else if (ShortcutOptionGroup.CanParse(p))
				{
					type = Type.ShortcutOptionGroup;
					shortcutOptionGroup = new ShortcutOptionGroup(this, p);
				}
				else if (CustomCommand.CanParse(p))
				{
					type = Type.CustomCommand;
					customCommand = new CustomCommand(this, p);
				}
				else
				{
					if (!p.NextSymbolIs(TokenType.Text))
					{
						throw ParseException.Make(p.tokens.Peek(), "Expected a statement here but got " + p.tokens.Peek().ToString() + " instead (was there an unbalanced if statement earlier?)");
					}
					line = p.ExpectSymbol(TokenType.Text).value;
					type = Type.Line;
				}
				List<string> list = new List<string>();
				while (p.NextSymbolIs(TokenType.TagMarker))
				{
					p.ExpectSymbol(TokenType.TagMarker);
					string value = p.ExpectSymbol(TokenType.Identifier).value;
					list.Add(value);
				}
				if (list.Count > 0)
				{
					tags = list.ToArray();
				}
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				switch (type)
				{
				case Type.Block:
					stringBuilder.Append(block.PrintTree(indentLevel));
					break;
				case Type.IfStatement:
					stringBuilder.Append(ifStatement.PrintTree(indentLevel));
					break;
				case Type.OptionStatement:
					stringBuilder.Append(optionStatement.PrintTree(indentLevel));
					break;
				case Type.AssignmentStatement:
					stringBuilder.Append(assignmentStatement.PrintTree(indentLevel));
					break;
				case Type.ShortcutOptionGroup:
					stringBuilder.Append(shortcutOptionGroup.PrintTree(indentLevel));
					break;
				case Type.CustomCommand:
					stringBuilder.Append(customCommand.PrintTree(indentLevel));
					break;
				case Type.Line:
					stringBuilder.Append(Tab(indentLevel, "Line: " + line));
					break;
				default:
					throw new ArgumentNullException();
				}
				stringBuilder.Append(TagsToString(indentLevel));
				return stringBuilder.ToString();
			}
		}

		internal class CustomCommand : ParseNode
		{
			internal enum Type
			{
				Expression = 0,
				ClientCommand = 1
			}

			internal Type type;

			internal Expression expression { get; private set; }

			internal string clientCommand { get; private set; }

			internal static bool CanParse(Parser p)
			{
				if (!p.NextSymbolsAre(TokenType.BeginCommand, TokenType.Text))
				{
					return p.NextSymbolsAre(TokenType.BeginCommand, TokenType.Identifier);
				}
				return true;
			}

			internal CustomCommand(ParseNode parent, Parser p)
				: base(parent, p)
			{
				p.ExpectSymbol(TokenType.BeginCommand);
				List<Token> list = new List<Token>();
				do
				{
					list.Add(p.ExpectSymbol());
				}
				while (!p.NextSymbolIs(TokenType.EndCommand));
				p.ExpectSymbol(TokenType.EndCommand);
				if (list.Count > 1 && list[0].type == TokenType.Identifier && list[1].type == TokenType.LeftParen)
				{
					Parser p2 = new Parser(list, p.library);
					Expression expression = Expression.Parse(this, p2);
					type = Type.Expression;
					this.expression = expression;
				}
				else
				{
					type = Type.ClientCommand;
					clientCommand = list[0].value;
				}
			}

			internal override string PrintTree(int indentLevel)
			{
				return type switch
				{
					Type.Expression => Tab(indentLevel, "Expression: ") + expression.PrintTree(indentLevel + 1), 
					Type.ClientCommand => Tab(indentLevel, "Command: " + clientCommand), 
					_ => "", 
				};
			}
		}

		internal class ShortcutOptionGroup : ParseNode
		{
			private List<ShortcutOption> _options = new List<ShortcutOption>();

			internal IEnumerable<ShortcutOption> options => _options;

			internal static bool CanParse(Parser p)
			{
				return p.NextSymbolIs(TokenType.ShortcutOption);
			}

			internal ShortcutOptionGroup(ParseNode parent, Parser p)
				: base(parent, p)
			{
				int num = 1;
				do
				{
					_options.Add(new ShortcutOption(num++, this, p));
				}
				while (p.NextSymbolIs(TokenType.ShortcutOption));
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Tab(indentLevel, "Shortcut option group {"));
				foreach (ShortcutOption option in options)
				{
					stringBuilder.Append(option.PrintTree(indentLevel + 1));
				}
				stringBuilder.Append(Tab(indentLevel, "}"));
				return stringBuilder.ToString();
			}
		}

		internal class ShortcutOption : ParseNode
		{
			internal string label { get; private set; }

			internal Expression condition { get; private set; }

			internal Node optionNode { get; private set; }

			internal ShortcutOption(int optionIndex, ParseNode parent, Parser p)
				: base(parent, p)
			{
				p.ExpectSymbol(TokenType.ShortcutOption);
				label = p.ExpectSymbol(TokenType.Text).value;
				List<string> list = new List<string>();
				while (p.NextSymbolsAre(TokenType.BeginCommand, TokenType.If) || p.NextSymbolIs(TokenType.TagMarker))
				{
					if (p.NextSymbolsAre(TokenType.BeginCommand, TokenType.If))
					{
						p.ExpectSymbol(TokenType.BeginCommand);
						p.ExpectSymbol(TokenType.If);
						condition = Expression.Parse(this, p);
						p.ExpectSymbol(TokenType.EndCommand);
					}
					else if (p.NextSymbolIs(TokenType.TagMarker))
					{
						p.ExpectSymbol(TokenType.TagMarker);
						string value = p.ExpectSymbol(TokenType.Identifier).value;
						list.Add(value);
					}
				}
				tags = list.ToArray();
				if (p.NextSymbolIs(TokenType.Indent))
				{
					p.ExpectSymbol(TokenType.Indent);
					optionNode = new Node(NodeParent().name + "." + optionIndex, this, p);
					p.ExpectSymbol(TokenType.Dedent);
				}
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Tab(indentLevel, "Option \"" + label + "\""));
				if (condition != null)
				{
					stringBuilder.Append(Tab(indentLevel + 1, "(when:"));
					stringBuilder.Append(condition.PrintTree(indentLevel + 2));
					stringBuilder.Append(Tab(indentLevel + 1, "),"));
				}
				if (optionNode != null)
				{
					stringBuilder.Append(Tab(indentLevel, "{"));
					stringBuilder.Append(optionNode.PrintTree(indentLevel + 1));
					stringBuilder.Append(Tab(indentLevel, "}"));
				}
				stringBuilder.Append(TagsToString(indentLevel));
				return stringBuilder.ToString();
			}
		}

		internal class Block : ParseNode
		{
			private List<Statement> _statements = new List<Statement>();

			internal IEnumerable<Statement> statements => _statements;

			internal static bool CanParse(Parser p)
			{
				return p.NextSymbolIs(TokenType.Indent);
			}

			internal Block(ParseNode parent, Parser p)
				: base(parent, p)
			{
				p.ExpectSymbol(TokenType.Indent);
				while (!p.NextSymbolIs(TokenType.Dedent))
				{
					_statements.Add(new Statement(this, p));
				}
				p.ExpectSymbol(TokenType.Dedent);
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Tab(indentLevel, "Block {"));
				foreach (Statement statement in _statements)
				{
					stringBuilder.Append(statement.PrintTree(indentLevel + 1));
				}
				stringBuilder.Append(Tab(indentLevel, "}"));
				return stringBuilder.ToString();
			}
		}

		internal class OptionStatement : ParseNode
		{
			internal string destination { get; private set; }

			internal string label { get; private set; }

			internal static bool CanParse(Parser p)
			{
				return p.NextSymbolIs(TokenType.OptionStart);
			}

			internal OptionStatement(ParseNode parent, Parser p)
				: base(parent, p)
			{
				p.ExpectSymbol(TokenType.OptionStart);
				string value = p.ExpectSymbol(TokenType.Text).value;
				if (p.NextSymbolIs(TokenType.OptionDelimit))
				{
					p.ExpectSymbol(TokenType.OptionDelimit);
					string value2 = p.ExpectSymbol(TokenType.Text, TokenType.Identifier).value;
					label = value;
					destination = value2;
				}
				else
				{
					label = null;
					destination = value;
				}
				p.ExpectSymbol(TokenType.OptionEnd);
			}

			internal override string PrintTree(int indentLevel)
			{
				if (label != null)
				{
					return Tab(indentLevel, $"Option: \"{label}\" -> {destination}");
				}
				return Tab(indentLevel, $"Option: -> {destination}");
			}
		}

		internal class IfStatement : ParseNode
		{
			internal struct Clause
			{
				internal Expression expression;

				internal IEnumerable<Statement> statements;

				internal string PrintTree(int indentLevel)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (expression != null)
					{
						stringBuilder.Append(expression.PrintTree(indentLevel));
					}
					stringBuilder.Append(Tab(indentLevel, "{"));
					foreach (Statement statement in statements)
					{
						stringBuilder.Append(statement.PrintTree(indentLevel + 1));
					}
					stringBuilder.Append(Tab(indentLevel, "}"));
					return stringBuilder.ToString();
				}
			}

			internal List<Clause> clauses = new List<Clause>();

			internal static bool CanParse(Parser p)
			{
				return p.NextSymbolsAre(TokenType.BeginCommand, TokenType.If);
			}

			internal IfStatement(ParseNode parent, Parser p)
				: base(parent, p)
			{
				Clause item = default(Clause);
				p.ExpectSymbol(TokenType.BeginCommand);
				p.ExpectSymbol(TokenType.If);
				item.expression = Expression.Parse(this, p);
				p.ExpectSymbol(TokenType.EndCommand);
				List<Statement> list = new List<Statement>();
				while (!p.NextSymbolsAre(TokenType.BeginCommand, TokenType.EndIf) && !p.NextSymbolsAre(TokenType.BeginCommand, TokenType.Else) && !p.NextSymbolsAre(TokenType.BeginCommand, TokenType.ElseIf))
				{
					list.Add(new Statement(this, p));
					while (p.NextSymbolIs(TokenType.Dedent))
					{
						p.ExpectSymbol(TokenType.Dedent);
					}
				}
				item.statements = list;
				clauses.Add(item);
				while (p.NextSymbolsAre(TokenType.BeginCommand, TokenType.ElseIf))
				{
					Clause item2 = default(Clause);
					p.ExpectSymbol(TokenType.BeginCommand);
					p.ExpectSymbol(TokenType.ElseIf);
					item2.expression = Expression.Parse(this, p);
					p.ExpectSymbol(TokenType.EndCommand);
					List<Statement> list2 = new List<Statement>();
					while (!p.NextSymbolsAre(TokenType.BeginCommand, TokenType.EndIf) && !p.NextSymbolsAre(TokenType.BeginCommand, TokenType.Else) && !p.NextSymbolsAre(TokenType.BeginCommand, TokenType.ElseIf))
					{
						list2.Add(new Statement(this, p));
						while (p.NextSymbolIs(TokenType.Dedent))
						{
							p.ExpectSymbol(TokenType.Dedent);
						}
					}
					item2.statements = list2;
					clauses.Add(item2);
				}
				if (p.NextSymbolsAre(TokenType.BeginCommand, TokenType.Else, TokenType.EndCommand))
				{
					p.ExpectSymbol(TokenType.BeginCommand);
					p.ExpectSymbol(TokenType.Else);
					p.ExpectSymbol(TokenType.EndCommand);
					Clause item3 = default(Clause);
					List<Statement> list3 = new List<Statement>();
					while (!p.NextSymbolsAre(TokenType.BeginCommand, TokenType.EndIf))
					{
						list3.Add(new Statement(this, p));
					}
					item3.statements = list3;
					clauses.Add(item3);
					while (p.NextSymbolIs(TokenType.Dedent))
					{
						p.ExpectSymbol(TokenType.Dedent);
					}
				}
				p.ExpectSymbol(TokenType.BeginCommand);
				p.ExpectSymbol(TokenType.EndIf);
				p.ExpectSymbol(TokenType.EndCommand);
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (Clause clause in clauses)
				{
					if (flag)
					{
						stringBuilder.Append(Tab(indentLevel, "If:"));
						flag = false;
					}
					else if (clause.expression != null)
					{
						stringBuilder.Append(Tab(indentLevel, "Else If:"));
					}
					else
					{
						stringBuilder.Append(Tab(indentLevel, "Else:"));
					}
					stringBuilder.Append(clause.PrintTree(indentLevel + 1));
				}
				return stringBuilder.ToString();
			}
		}

		public class ValueNode : ParseNode
		{
			public Value value { get; private set; }

			private void UseToken(Token t)
			{
				switch (t.type)
				{
				case TokenType.Number:
					value = new Value(float.Parse(t.value, CultureInfo.InvariantCulture));
					break;
				case TokenType.String:
					value = new Value(t.value);
					break;
				case TokenType.False:
					value = new Value(false);
					break;
				case TokenType.True:
					value = new Value(true);
					break;
				case TokenType.Variable:
					value = new Value();
					value.type = Value.Type.Variable;
					value.variableName = t.value;
					break;
				case TokenType.Null:
					value = Value.NULL;
					break;
				default:
					throw ParseException.Make(t, "Invalid token type " + t.ToString());
				}
			}

			internal ValueNode(ParseNode parent, Token t, Parser p)
				: base(parent, p)
			{
				UseToken(t);
			}

			internal ValueNode(ParseNode parent, Parser p)
				: base(parent, p)
			{
				Token t = p.ExpectSymbol(TokenType.Number, TokenType.Variable, TokenType.String);
				UseToken(t);
			}

			internal override string PrintTree(int indentLevel)
			{
				return value.type switch
				{
					Value.Type.Number => Tab(indentLevel, value.numberValue.ToString()), 
					Value.Type.String => Tab(indentLevel, $"\"{value.stringValue}\""), 
					Value.Type.Bool => Tab(indentLevel, value.boolValue.ToString()), 
					Value.Type.Variable => Tab(indentLevel, value.variableName), 
					Value.Type.Null => Tab(indentLevel, "(null)"), 
					_ => throw new ArgumentException(), 
				};
			}
		}

		internal class Expression : ParseNode
		{
			internal enum Type
			{
				Value = 0,
				FunctionCall = 1
			}

			internal Type type;

			internal ValueNode value;

			internal FunctionInfo function;

			internal List<Expression> parameters;

			internal Expression(ParseNode parent, ValueNode value, Parser p)
				: base(parent, p)
			{
				type = Type.Value;
				this.value = value;
			}

			internal Expression(ParseNode parent, FunctionInfo function, List<Expression> parameters, Parser p)
				: base(parent, p)
			{
				type = Type.FunctionCall;
				this.function = function;
				this.parameters = parameters;
			}

			internal static Expression Parse(ParseNode parent, Parser p)
			{
				Queue<Token> queue = new Queue<Token>();
				Stack<Token> stack = new Stack<Token>();
				Stack<Token> stack2 = new Stack<Token>();
				List<TokenType> list = new List<TokenType>(Operator.OperatorTypes);
				list.Add(TokenType.Number);
				list.Add(TokenType.Variable);
				list.Add(TokenType.String);
				list.Add(TokenType.LeftParen);
				list.Add(TokenType.RightParen);
				list.Add(TokenType.Identifier);
				list.Add(TokenType.Comma);
				list.Add(TokenType.True);
				list.Add(TokenType.False);
				list.Add(TokenType.Null);
				Token token = null;
				while (p.tokens.Count > 0 && p.NextSymbolIs(list.ToArray()))
				{
					Token token2 = p.ExpectSymbol(list.ToArray());
					if (token2.type == TokenType.Number || token2.type == TokenType.Variable || token2.type == TokenType.String || token2.type == TokenType.True || token2.type == TokenType.False || token2.type == TokenType.Null)
					{
						queue.Enqueue(token2);
					}
					else if (token2.type == TokenType.Identifier)
					{
						stack.Push(token2);
						stack2.Push(token2);
						token2 = p.ExpectSymbol(TokenType.LeftParen);
						stack.Push(token2);
					}
					else if (token2.type == TokenType.Comma)
					{
						try
						{
							while (stack.Peek().type != TokenType.LeftParen)
							{
								queue.Enqueue(stack.Pop());
							}
						}
						catch (InvalidOperationException)
						{
							throw ParseException.Make(token2, "Error parsing expression: unbalanced parentheses");
						}
						if (stack.Peek().type != TokenType.LeftParen)
						{
							throw ParseException.Make(stack.Peek(), "Expression parser got confused dealing with a function");
						}
						if (p.NextSymbolIs(TokenType.RightParen, TokenType.Comma))
						{
							throw ParseException.Make(p.tokens.Peek(), "Expected expression");
						}
						stack2.Peek().parameterCount++;
					}
					else if (Operator.IsOperator(token2.type))
					{
						if (token2.type == TokenType.Minus && (token == null || token.type == TokenType.LeftParen || Operator.IsOperator(token.type)))
						{
							token2.type = TokenType.UnaryMinus;
						}
						if (token2.type == TokenType.EqualToOrAssign)
						{
							token2.type = TokenType.EqualTo;
						}
						while (ShouldApplyPrecedence(token2.type, stack))
						{
							Token item = stack.Pop();
							queue.Enqueue(item);
						}
						stack.Push(token2);
					}
					else if (token2.type == TokenType.LeftParen)
					{
						stack.Push(token2);
					}
					else if (token2.type == TokenType.RightParen)
					{
						try
						{
							while (stack.Peek().type != TokenType.LeftParen)
							{
								queue.Enqueue(stack.Pop());
							}
							stack.Pop();
						}
						catch (InvalidOperationException)
						{
							throw ParseException.Make(token2, "Error parsing expression: unbalanced parentheses");
						}
						if (stack.Peek().type == TokenType.Identifier)
						{
							if (token.type != TokenType.LeftParen)
							{
								stack2.Peek().parameterCount++;
							}
							queue.Enqueue(stack.Pop());
							stack2.Pop();
						}
					}
					token = token2;
				}
				while (stack.Count > 0)
				{
					queue.Enqueue(stack.Pop());
				}
				if (queue.Count == 0)
				{
					throw new ParseException("Error parsing expression: no expression found!");
				}
				Token mostRecentToken = queue.Peek();
				Stack<Expression> stack3 = new Stack<Expression>();
				while (queue.Count > 0)
				{
					Token token3 = queue.Dequeue();
					if (Operator.IsOperator(token3.type))
					{
						Operator.OperatorInfo operatorInfo = Operator.InfoForOperator(token3.type);
						if (stack3.Count < operatorInfo.arguments)
						{
							throw ParseException.Make(token3, "Error parsing expression: not enough arguments for operator " + token3.type);
						}
						List<Expression> list2 = new List<Expression>();
						for (int i = 0; i < operatorInfo.arguments; i++)
						{
							list2.Add(stack3.Pop());
						}
						list2.Reverse();
						FunctionInfo functionInfo = p.library.GetFunction(token3.type.ToString());
						Expression item2 = new Expression(parent, functionInfo, list2, p);
						stack3.Push(item2);
					}
					else if (token3.type == TokenType.Identifier)
					{
						FunctionInfo functionInfo2 = null;
						if (p.library != null)
						{
							functionInfo2 = p.library.GetFunction(token3.value);
							if (!functionInfo2.IsParameterCountCorrect(token3.parameterCount))
							{
								string message = $"Error parsing expression: Unsupported number of parameters for function {token3.value} (expected {functionInfo2.paramCount}, got {token3.parameterCount})";
								throw ParseException.Make(token3, message);
							}
						}
						else
						{
							functionInfo2 = new FunctionInfo(token3.value, token3.parameterCount, (Function)null);
						}
						List<Expression> list3 = new List<Expression>();
						for (int j = 0; j < token3.parameterCount; j++)
						{
							list3.Add(stack3.Pop());
						}
						list3.Reverse();
						Expression item3 = new Expression(parent, functionInfo2, list3, p);
						stack3.Push(item3);
					}
					else
					{
						ValueNode valueNode = new ValueNode(parent, token3, p);
						Expression item4 = new Expression(parent, valueNode, p);
						stack3.Push(item4);
					}
				}
				if (stack3.Count != 1)
				{
					throw ParseException.Make(mostRecentToken, "Error parsing expression (stack did not reduce correctly)");
				}
				return stack3.Pop();
			}

			private static bool ShouldApplyPrecedence(TokenType o1, Stack<Token> operatorStack)
			{
				if (operatorStack.Count == 0)
				{
					return false;
				}
				if (!Operator.IsOperator(o1))
				{
					throw new ParseException("Internal error parsing expression");
				}
				TokenType op = operatorStack.Peek().type;
				if (!Operator.IsOperator(op))
				{
					return false;
				}
				Operator.OperatorInfo operatorInfo = Operator.InfoForOperator(o1);
				Operator.OperatorInfo operatorInfo2 = Operator.InfoForOperator(op);
				if (operatorInfo.associativity == Operator.Associativity.Left && operatorInfo.precedence <= operatorInfo2.precedence)
				{
					return true;
				}
				if (operatorInfo.associativity == Operator.Associativity.Right && operatorInfo.precedence < operatorInfo2.precedence)
				{
					return true;
				}
				return false;
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				switch (type)
				{
				case Type.Value:
					return value.PrintTree(indentLevel);
				case Type.FunctionCall:
					if (parameters.Count == 0)
					{
						stringBuilder.Append(Tab(indentLevel, "Function call to " + function.name + " (no parameters)"));
					}
					else
					{
						stringBuilder.Append(Tab(indentLevel, "Function call to " + function.name + " (" + parameters.Count + " parameters) {"));
						foreach (Expression parameter in parameters)
						{
							stringBuilder.Append(parameter.PrintTree(indentLevel + 1));
						}
						stringBuilder.Append(Tab(indentLevel, "}"));
					}
					return stringBuilder.ToString();
				default:
					return Tab(indentLevel, "<error printing expression!>");
				}
			}
		}

		internal class AssignmentStatement : ParseNode
		{
			private static TokenType[] validOperators = new TokenType[5]
			{
				TokenType.EqualToOrAssign,
				TokenType.AddAssign,
				TokenType.MinusAssign,
				TokenType.DivideAssign,
				TokenType.MultiplyAssign
			};

			internal string destinationVariableName { get; private set; }

			internal Expression valueExpression { get; private set; }

			internal TokenType operation { get; private set; }

			internal static bool CanParse(Parser p)
			{
				return p.NextSymbolsAre(TokenType.BeginCommand, TokenType.Set);
			}

			internal AssignmentStatement(ParseNode parent, Parser p)
				: base(parent, p)
			{
				p.ExpectSymbol(TokenType.BeginCommand);
				p.ExpectSymbol(TokenType.Set);
				destinationVariableName = p.ExpectSymbol(TokenType.Variable).value;
				operation = p.ExpectSymbol(validOperators).type;
				valueExpression = Expression.Parse(this, p);
				p.ExpectSymbol(TokenType.EndCommand);
			}

			internal override string PrintTree(int indentLevel)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Tab(indentLevel, "Set:"));
				stringBuilder.Append(Tab(indentLevel + 1, destinationVariableName));
				stringBuilder.Append(Tab(indentLevel + 1, operation.ToString()));
				stringBuilder.Append(valueExpression.PrintTree(indentLevel + 1));
				return stringBuilder.ToString();
			}
		}

		internal class Operator : ParseNode
		{
			internal enum Associativity
			{
				Left = 0,
				Right = 1,
				None = 2
			}

			internal struct OperatorInfo
			{
				internal Associativity associativity;

				internal int precedence;

				internal int arguments;

				internal OperatorInfo(Associativity associativity, int precedence, int arguments)
				{
					this.associativity = associativity;
					this.precedence = precedence;
					this.arguments = arguments;
				}
			}

			internal TokenType operatorType { get; private set; }

			internal static TokenType[] OperatorTypes => new TokenType[17]
			{
				TokenType.Not,
				TokenType.UnaryMinus,
				TokenType.Add,
				TokenType.Minus,
				TokenType.Divide,
				TokenType.Multiply,
				TokenType.Modulo,
				TokenType.EqualToOrAssign,
				TokenType.EqualTo,
				TokenType.GreaterThan,
				TokenType.GreaterThanOrEqualTo,
				TokenType.LessThan,
				TokenType.LessThanOrEqualTo,
				TokenType.NotEqualTo,
				TokenType.And,
				TokenType.Or,
				TokenType.Xor
			};

			internal static OperatorInfo InfoForOperator(TokenType op)
			{
				if (Array.IndexOf(OperatorTypes, op) == -1)
				{
					throw new ParseException(op.ToString() + " is not a valid operator");
				}
				switch (op)
				{
				case TokenType.Not:
				case TokenType.UnaryMinus:
					return new OperatorInfo(Associativity.Right, 30, 1);
				case TokenType.Multiply:
				case TokenType.Divide:
				case TokenType.Modulo:
					return new OperatorInfo(Associativity.Left, 20, 2);
				case TokenType.Add:
				case TokenType.Minus:
					return new OperatorInfo(Associativity.Left, 15, 2);
				case TokenType.GreaterThan:
				case TokenType.GreaterThanOrEqualTo:
				case TokenType.LessThan:
				case TokenType.LessThanOrEqualTo:
					return new OperatorInfo(Associativity.Left, 10, 2);
				case TokenType.EqualTo:
				case TokenType.NotEqualTo:
				case TokenType.EqualToOrAssign:
					return new OperatorInfo(Associativity.Left, 5, 2);
				case TokenType.And:
					return new OperatorInfo(Associativity.Left, 4, 2);
				case TokenType.Or:
					return new OperatorInfo(Associativity.Left, 3, 2);
				case TokenType.Xor:
					return new OperatorInfo(Associativity.Left, 2, 2);
				default:
					throw new InvalidOperationException("Unknown operator " + op);
				}
			}

			internal static bool IsOperator(TokenType type)
			{
				return Array.IndexOf(OperatorTypes, type) != -1;
			}

			internal Operator(ParseNode parent, TokenType t, Parser p)
				: base(parent, p)
			{
				operatorType = t;
			}

			internal Operator(ParseNode parent, Parser p)
				: base(parent, p)
			{
				operatorType = p.ExpectSymbol(OperatorTypes).type;
			}

			internal override string PrintTree(int indentLevel)
			{
				return Tab(indentLevel, operatorType.ToString());
			}
		}

		private Queue<Token> tokens;

		private Library library;

		private static string Tab(int indentLevel, string input, bool newLine = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < indentLevel; i++)
			{
				stringBuilder.Append("| ");
			}
			stringBuilder.Append(input);
			if (newLine)
			{
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		internal Parser(ICollection<Token> tokens, Library library)
		{
			this.tokens = new Queue<Token>(tokens);
			this.library = library;
		}

		internal Node Parse()
		{
			return new Node("Start", null, this);
		}

		private bool NextSymbolIs(params TokenType[] validTypes)
		{
			TokenType type = tokens.Peek().type;
			foreach (TokenType tokenType in validTypes)
			{
				if (type == tokenType)
				{
					return true;
				}
			}
			return false;
		}

		private bool NextSymbolsAre(params TokenType[] validTypes)
		{
			Queue<Token> queue = new Queue<Token>(tokens);
			foreach (TokenType tokenType in validTypes)
			{
				if (queue.Dequeue().type != tokenType)
				{
					return false;
				}
			}
			return true;
		}

		private Token ExpectSymbol(TokenType type)
		{
			Token token = tokens.Dequeue();
			if (token.type != type)
			{
				throw ParseException.Make(token, type);
			}
			return token;
		}

		private Token ExpectSymbol()
		{
			Token token = tokens.Dequeue();
			if (token.type == TokenType.EndOfInput)
			{
				throw ParseException.Make(token, "Unexpected end of input");
			}
			return token;
		}

		private Token ExpectSymbol(params TokenType[] validTypes)
		{
			Token token = tokens.Dequeue();
			foreach (TokenType tokenType in validTypes)
			{
				if (token.type == tokenType)
				{
					return token;
				}
			}
			throw ParseException.Make(token, validTypes);
		}
	}
}
