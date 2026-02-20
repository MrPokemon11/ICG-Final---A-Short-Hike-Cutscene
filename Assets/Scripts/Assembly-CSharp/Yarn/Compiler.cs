using System;
using System.Collections.Generic;

namespace Yarn
{
	internal class Compiler
	{
		private struct CompileFlags
		{
			public bool DisableShuffleOptionsAfterNextSet;
		}

		private CompileFlags flags;

		private int labelCount;

		internal Program program { get; private set; }

		internal Compiler(string programName)
		{
			program = new Program();
		}

		internal void CompileNode(Parser.Node node)
		{
			if (program.nodes.ContainsKey(node.name))
			{
				throw new ArgumentException("Duplicate node name " + node.name);
			}
			Node node2 = new Node();
			node2.name = node.name;
			node2.tags = node.nodeTags;
			if (node.source != null)
			{
				node2.sourceTextStringID = program.RegisterString(node.source, node.name, "line:" + node.name, 0, localisable: true);
			}
			else
			{
				string operandA = RegisterLabel();
				Emit(node2, ByteCode.Label, operandA);
				foreach (Parser.Statement statement in node.statements)
				{
					GenerateCode(node2, statement);
				}
				bool flag = false;
				foreach (Instruction instruction in node2.instructions)
				{
					if (instruction.operation == ByteCode.AddOption)
					{
						flag = true;
					}
					if (instruction.operation == ByteCode.ShowOptions)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					Emit(node2, ByteCode.Stop);
				}
				else
				{
					Emit(node2, ByteCode.ShowOptions);
					if (flags.DisableShuffleOptionsAfterNextSet)
					{
						Emit(node2, ByteCode.PushBool, false);
						Emit(node2, ByteCode.StoreVariable, "$Yarn.ShuffleOptions");
						Emit(node2, ByteCode.Pop);
						flags.DisableShuffleOptionsAfterNextSet = false;
					}
					Emit(node2, ByteCode.RunNode);
				}
			}
			program.nodes[node2.name] = node2;
		}

		private string RegisterLabel(string commentary = null)
		{
			return "L" + labelCount++ + commentary;
		}

		private void Emit(Node node, ByteCode code, object operandA = null, object operandB = null)
		{
			Instruction item = new Instruction
			{
				operation = code,
				operandA = operandA,
				operandB = operandB
			};
			node.instructions.Add(item);
			if (code == ByteCode.Label)
			{
				node.labels.Add((string)item.operandA, node.instructions.Count - 1);
			}
		}

		private void GenerateCode(Node node, Parser.Statement statement)
		{
			switch (statement.type)
			{
			case Parser.Statement.Type.CustomCommand:
				GenerateCode(node, statement.customCommand);
				break;
			case Parser.Statement.Type.ShortcutOptionGroup:
				GenerateCode(node, statement.shortcutOptionGroup);
				break;
			case Parser.Statement.Type.Block:
			{
				foreach (Parser.Statement statement2 in statement.block.statements)
				{
					GenerateCode(node, statement2);
				}
				break;
			}
			case Parser.Statement.Type.IfStatement:
				GenerateCode(node, statement.ifStatement);
				break;
			case Parser.Statement.Type.OptionStatement:
				GenerateCode(node, statement.optionStatement);
				break;
			case Parser.Statement.Type.AssignmentStatement:
				GenerateCode(node, statement.assignmentStatement);
				break;
			case Parser.Statement.Type.Line:
				GenerateCode(node, statement, statement.line);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void GenerateCode(Node node, Parser.CustomCommand statement)
		{
			if (statement.expression != null)
			{
				GenerateCode(node, statement.expression);
				return;
			}
			string clientCommand = statement.clientCommand;
			if (!(clientCommand == "stop"))
			{
				if (clientCommand == "shuffleNextOptions")
				{
					Emit(node, ByteCode.PushBool, true);
					Emit(node, ByteCode.StoreVariable, "$Yarn.ShuffleOptions");
					Emit(node, ByteCode.Pop);
					flags.DisableShuffleOptionsAfterNextSet = true;
				}
				else
				{
					Emit(node, ByteCode.RunCommand, statement.clientCommand);
				}
			}
			else
			{
				Emit(node, ByteCode.Stop);
			}
		}

		private string GetLineIDFromNodeTags(Parser.ParseNode node)
		{
			string[] tags = node.tags;
			foreach (string text in tags)
			{
				if (text.StartsWith("line:"))
				{
					return text;
				}
			}
			return null;
		}

		private void GenerateCode(Node node, Parser.Statement parseNode, string line)
		{
			string lineIDFromNodeTags = GetLineIDFromNodeTags(parseNode);
			string operandA = program.RegisterString(line, node.name, lineIDFromNodeTags, parseNode.lineNumber, localisable: true);
			Emit(node, ByteCode.RunLine, operandA);
		}

		private void GenerateCode(Node node, Parser.ShortcutOptionGroup statement)
		{
			string operandA = RegisterLabel("group_end");
			List<string> list = new List<string>();
			int num = 0;
			foreach (Parser.ShortcutOption option in statement.options)
			{
				string text = RegisterLabel("option_" + (num + 1));
				list.Add(text);
				string operandA2 = null;
				if (option.condition != null)
				{
					operandA2 = RegisterLabel("conditional_" + num);
					GenerateCode(node, option.condition);
					Emit(node, ByteCode.JumpIfFalse, operandA2);
				}
				string lineIDFromNodeTags = GetLineIDFromNodeTags(option);
				string operandA3 = program.RegisterString(option.label, node.name, lineIDFromNodeTags, option.lineNumber, localisable: true);
				Emit(node, ByteCode.AddOption, operandA3, text);
				if (option.condition != null)
				{
					Emit(node, ByteCode.Label, operandA2);
					Emit(node, ByteCode.Pop);
				}
				num++;
			}
			Emit(node, ByteCode.ShowOptions);
			if (flags.DisableShuffleOptionsAfterNextSet)
			{
				Emit(node, ByteCode.PushBool, false);
				Emit(node, ByteCode.StoreVariable, "$Yarn.ShuffleOptions");
				Emit(node, ByteCode.Pop);
				flags.DisableShuffleOptionsAfterNextSet = false;
			}
			Emit(node, ByteCode.Jump);
			num = 0;
			foreach (Parser.ShortcutOption option2 in statement.options)
			{
				Emit(node, ByteCode.Label, list[num]);
				if (option2.optionNode != null)
				{
					GenerateCode(node, option2.optionNode.statements);
				}
				Emit(node, ByteCode.JumpTo, operandA);
				num++;
			}
			Emit(node, ByteCode.Label, operandA);
			Emit(node, ByteCode.Pop);
		}

		private void GenerateCode(Node node, IEnumerable<Parser.Statement> statementList)
		{
			if (statementList == null)
			{
				return;
			}
			foreach (Parser.Statement statement in statementList)
			{
				GenerateCode(node, statement);
			}
		}

		private void GenerateCode(Node node, Parser.IfStatement statement)
		{
			string operandA = RegisterLabel("endif");
			foreach (Parser.IfStatement.Clause clause in statement.clauses)
			{
				string operandA2 = RegisterLabel("skipclause");
				if (clause.expression != null)
				{
					GenerateCode(node, clause.expression);
					Emit(node, ByteCode.JumpIfFalse, operandA2);
				}
				GenerateCode(node, clause.statements);
				Emit(node, ByteCode.JumpTo, operandA);
				if (clause.expression != null)
				{
					Emit(node, ByteCode.Label, operandA2);
				}
				if (clause.expression != null)
				{
					Emit(node, ByteCode.Pop);
				}
			}
			Emit(node, ByteCode.Label, operandA);
		}

		private void GenerateCode(Node node, Parser.OptionStatement statement)
		{
			string destination = statement.destination;
			if (statement.label == null)
			{
				Emit(node, ByteCode.RunNode, destination);
				return;
			}
			string lineIDFromNodeTags = GetLineIDFromNodeTags(statement.parent);
			string operandA = program.RegisterString(statement.label, node.name, lineIDFromNodeTags, statement.lineNumber, localisable: true);
			Emit(node, ByteCode.AddOption, operandA, destination);
		}

		private void GenerateCode(Node node, Parser.AssignmentStatement statement)
		{
			if (statement.operation == TokenType.EqualToOrAssign)
			{
				GenerateCode(node, statement.valueExpression);
			}
			else
			{
				Emit(node, ByteCode.PushVariable, statement.destinationVariableName);
				GenerateCode(node, statement.valueExpression);
				switch (statement.operation)
				{
				case TokenType.AddAssign:
					Emit(node, ByteCode.CallFunc, TokenType.Add.ToString());
					break;
				case TokenType.MinusAssign:
					Emit(node, ByteCode.CallFunc, TokenType.Minus.ToString());
					break;
				case TokenType.MultiplyAssign:
					Emit(node, ByteCode.CallFunc, TokenType.Multiply.ToString());
					break;
				case TokenType.DivideAssign:
					Emit(node, ByteCode.CallFunc, TokenType.Divide.ToString());
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			Emit(node, ByteCode.StoreVariable, statement.destinationVariableName);
			Emit(node, ByteCode.Pop);
		}

		private void GenerateCode(Node node, Parser.Expression expression)
		{
			switch (expression.type)
			{
			case Parser.Expression.Type.Value:
				GenerateCode(node, expression.value);
				break;
			case Parser.Expression.Type.FunctionCall:
				foreach (Parser.Expression parameter in expression.parameters)
				{
					GenerateCode(node, parameter);
				}
				if (expression.function.paramCount == -1)
				{
					Emit(node, ByteCode.PushNumber, expression.parameters.Count);
				}
				Emit(node, ByteCode.CallFunc, expression.function.name);
				break;
			}
		}

		private void GenerateCode(Node node, Parser.ValueNode value)
		{
			switch (value.value.type)
			{
			case Value.Type.Number:
				Emit(node, ByteCode.PushNumber, value.value.numberValue);
				break;
			case Value.Type.String:
			{
				string operandA = program.RegisterString(value.value.stringValue, node.name, null, value.lineNumber, localisable: false);
				Emit(node, ByteCode.PushString, operandA);
				break;
			}
			case Value.Type.Bool:
				Emit(node, ByteCode.PushBool, value.value.boolValue);
				break;
			case Value.Type.Variable:
				Emit(node, ByteCode.PushVariable, value.value.variableName);
				break;
			case Value.Type.Null:
				Emit(node, ByteCode.PushNull);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
