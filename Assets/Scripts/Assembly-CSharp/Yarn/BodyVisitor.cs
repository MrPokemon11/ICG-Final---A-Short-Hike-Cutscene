using System.Collections.Generic;
using System.Globalization;

namespace Yarn
{
	public class BodyVisitor : YarnSpinnerParserBaseVisitor<int>
	{
		internal AntlrCompiler compiler;

		private Dictionary<int, TokenType> tokens = new Dictionary<int, TokenType>();

		public BodyVisitor(AntlrCompiler compiler)
		{
			this.compiler = compiler;
			loadOperators();
		}

		public override int VisitLine_statement(YarnSpinnerParser.Line_statementContext context)
		{
			string theString = context.text().GetText().Trim('"');
			string lineID = compiler.GetLineID(context.hashtag_block());
			int line = context.Start.Line;
			string operandA = compiler.program.RegisterString(theString, compiler.currentNode.name, lineID, line, localisable: true);
			compiler.Emit(ByteCode.RunLine, operandA);
			return 0;
		}

		public override int VisitOption_statement(YarnSpinnerParser.Option_statementContext context)
		{
			if (context.OPTION_LINK() != null)
			{
				string text = context.OPTION_LINK().GetText();
				string text2 = context.OPTION_TEXT().GetText();
				int line = context.Start.Line;
				string lineID = compiler.GetLineID(context.hashtag_block());
				string operandA = compiler.program.RegisterString(text2, compiler.currentNode.name, lineID, line, localisable: true);
				compiler.Emit(ByteCode.AddOption, operandA, text);
			}
			else
			{
				string text3 = context.OPTION_TEXT().GetText();
				compiler.Emit(ByteCode.RunNode, text3);
			}
			return 0;
		}

		public override int VisitSet_statement(YarnSpinnerParser.Set_statementContext context)
		{
			if (context.variable() != null)
			{
				Visit(context.expression());
				string text = context.variable().GetText();
				compiler.Emit(ByteCode.StoreVariable, text);
				compiler.Emit(ByteCode.Pop);
			}
			else
			{
				YarnSpinnerParser.ExpressionContext expressionContext = context.expression();
				if (!(expressionContext is YarnSpinnerParser.ExpMultDivModEqualsContext) && !(expressionContext is YarnSpinnerParser.ExpPlusMinusEqualsContext))
				{
					throw ParseException.Make(context, "Invalid expression inside assignment statement");
				}
				Visit(expressionContext);
			}
			return 0;
		}

		public override int VisitAction_statement(YarnSpinnerParser.Action_statementContext context)
		{
			char[] trimChars = new char[2] { '<', '>' };
			string text = context.GetText().Trim(trimChars);
			if (!(text == "stop"))
			{
				if (text == "shuffleNextOptions")
				{
					compiler.Emit(ByteCode.PushBool, true);
					compiler.Emit(ByteCode.StoreVariable, "$Yarn.ShuffleOptions");
					compiler.Emit(ByteCode.Pop);
					compiler.flags.DisableShuffleOptionsAfterNextSet = true;
				}
				else
				{
					compiler.Emit(ByteCode.RunCommand, text);
				}
			}
			else
			{
				compiler.Emit(ByteCode.Stop);
			}
			return 0;
		}

		public override int VisitFunction_statement(YarnSpinnerParser.Function_statementContext context)
		{
			char[] trimChars = new char[1] { '<' };
			char[] trimChars2 = new char[1] { '(' };
			string text = context.GetChild(0).GetText().TrimStart(trimChars)
				.TrimEnd(trimChars2);
			if (!HandleFunction(text, context.expression()))
			{
				ParseException.Make(context, "Invalid number of parameters for " + text);
			}
			return 0;
		}

		private bool HandleFunction(string functionName, YarnSpinnerParser.ExpressionContext[] parameters)
		{
			FunctionInfo functionInfo = null;
			if (compiler.library != null)
			{
				functionInfo = compiler.library.GetFunction(functionName);
				if (functionInfo.paramCount != -1 && parameters.Length != functionInfo.paramCount)
				{
					return false;
				}
			}
			foreach (YarnSpinnerParser.ExpressionContext tree in parameters)
			{
				Visit(tree);
			}
			if (functionInfo != null && functionInfo.paramCount == -1)
			{
				compiler.Emit(ByteCode.PushNumber, parameters.Length);
			}
			compiler.Emit(ByteCode.CallFunc, functionName);
			return true;
		}

		public override int VisitFunction(YarnSpinnerParser.FunctionContext context)
		{
			string text = context.FUNC_ID().GetText();
			HandleFunction(text, context.expression());
			return 0;
		}

		public override int VisitIf_statement(YarnSpinnerParser.If_statementContext context)
		{
			string text = compiler.RegisterLabel("endif");
			YarnSpinnerParser.If_clauseContext if_clauseContext = context.if_clause();
			generateClause(text, if_clauseContext.statement(), if_clauseContext.expression());
			YarnSpinnerParser.Else_if_clauseContext[] array = context.else_if_clause();
			foreach (YarnSpinnerParser.Else_if_clauseContext else_if_clauseContext in array)
			{
				generateClause(text, else_if_clauseContext.statement(), else_if_clauseContext.expression());
			}
			YarnSpinnerParser.Else_clauseContext else_clauseContext = context.else_clause();
			if (else_clauseContext != null)
			{
				generateClause(text, else_clauseContext.statement(), null);
			}
			compiler.Emit(ByteCode.Label, text);
			return 0;
		}

		internal void generateClause(string jumpLabel, YarnSpinnerParser.StatementContext[] children, YarnSpinnerParser.ExpressionContext expression)
		{
			string operandA = compiler.RegisterLabel("skipclause");
			if (expression != null)
			{
				Visit(expression);
				compiler.Emit(ByteCode.JumpIfFalse, operandA);
			}
			foreach (YarnSpinnerParser.StatementContext tree in children)
			{
				Visit(tree);
			}
			compiler.Emit(ByteCode.JumpTo, jumpLabel);
			if (expression != null)
			{
				compiler.Emit(ByteCode.Label, operandA);
				compiler.Emit(ByteCode.Pop);
			}
		}

		private string ShortcutText(YarnSpinnerParser.Shortcut_textContext context)
		{
			return context.SHORTCUT_TEXT().GetText().Trim();
		}

		public override int VisitShortcut_statement(YarnSpinnerParser.Shortcut_statementContext context)
		{
			string operandA = compiler.RegisterLabel("group_end");
			List<string> list = new List<string>();
			int num = 0;
			YarnSpinnerParser.ShortcutContext[] array = context.shortcut();
			foreach (YarnSpinnerParser.ShortcutContext shortcutContext in array)
			{
				string text = compiler.RegisterLabel("option_" + (num + 1));
				list.Add(text);
				string operandA2 = null;
				if (shortcutContext.shortcut_conditional() != null)
				{
					operandA2 = compiler.RegisterLabel("conditional_" + num);
					Visit(shortcutContext.shortcut_conditional().expression());
					compiler.Emit(ByteCode.JumpIfFalse, operandA2);
				}
				string lineID = compiler.GetLineID(shortcutContext.hashtag_block());
				string theString = ShortcutText(shortcutContext.shortcut_text());
				string operandA3 = compiler.program.RegisterString(theString, compiler.currentNode.name, lineID, shortcutContext.Start.Line, localisable: true);
				compiler.Emit(ByteCode.AddOption, operandA3, text);
				if (shortcutContext.shortcut_conditional() != null)
				{
					compiler.Emit(ByteCode.Label, operandA2);
					compiler.Emit(ByteCode.Pop);
				}
				num++;
			}
			compiler.Emit(ByteCode.ShowOptions);
			if (compiler.flags.DisableShuffleOptionsAfterNextSet)
			{
				compiler.Emit(ByteCode.PushBool, false);
				compiler.Emit(ByteCode.StoreVariable, "$Yarn.ShuffleOptions");
				compiler.Emit(ByteCode.Pop);
				compiler.flags.DisableShuffleOptionsAfterNextSet = false;
			}
			compiler.Emit(ByteCode.Jump);
			num = 0;
			array = context.shortcut();
			foreach (YarnSpinnerParser.ShortcutContext obj in array)
			{
				compiler.Emit(ByteCode.Label, list[num]);
				YarnSpinnerParser.StatementContext[] array2 = obj.statement();
				foreach (YarnSpinnerParser.StatementContext tree in array2)
				{
					Visit(tree);
				}
				compiler.Emit(ByteCode.JumpTo, operandA);
				num++;
			}
			compiler.Emit(ByteCode.Label, operandA);
			compiler.Emit(ByteCode.Pop);
			return 0;
		}

		public override int VisitExpParens(YarnSpinnerParser.ExpParensContext context)
		{
			return Visit(context.expression());
		}

		public override int VisitExpNegative(YarnSpinnerParser.ExpNegativeContext context)
		{
			Visit(context.expression());
			compiler.Emit(ByteCode.CallFunc, TokenType.UnaryMinus.ToString());
			return 0;
		}

		public override int VisitExpNot(YarnSpinnerParser.ExpNotContext context)
		{
			Visit(context.expression());
			compiler.Emit(ByteCode.CallFunc, TokenType.Not.ToString());
			return 0;
		}

		public override int VisitExpValue(YarnSpinnerParser.ExpValueContext context)
		{
			return Visit(context.value());
		}

		internal void genericExpVisitor(YarnSpinnerParser.ExpressionContext left, YarnSpinnerParser.ExpressionContext right, int op)
		{
			Visit(left);
			Visit(right);
			compiler.Emit(ByteCode.CallFunc, tokens[op].ToString());
		}

		public override int VisitExpMultDivMod(YarnSpinnerParser.ExpMultDivModContext context)
		{
			genericExpVisitor(context.expression(0), context.expression(1), context.op.Type);
			return 0;
		}

		public override int VisitExpAddSub(YarnSpinnerParser.ExpAddSubContext context)
		{
			genericExpVisitor(context.expression(0), context.expression(1), context.op.Type);
			return 0;
		}

		public override int VisitExpComparison(YarnSpinnerParser.ExpComparisonContext context)
		{
			genericExpVisitor(context.expression(0), context.expression(1), context.op.Type);
			return 0;
		}

		public override int VisitExpEquality(YarnSpinnerParser.ExpEqualityContext context)
		{
			genericExpVisitor(context.expression(0), context.expression(1), context.op.Type);
			return 0;
		}

		public override int VisitExpAndOrXor(YarnSpinnerParser.ExpAndOrXorContext context)
		{
			genericExpVisitor(context.expression(0), context.expression(1), context.op.Type);
			return 0;
		}

		internal void opEquals(string varName, YarnSpinnerParser.ExpressionContext expression, int op)
		{
			compiler.Emit(ByteCode.PushVariable, varName);
			Visit(expression);
			compiler.Emit(ByteCode.CallFunc, tokens[op].ToString());
			compiler.Emit(ByteCode.StoreVariable, varName);
			compiler.Emit(ByteCode.Pop);
		}

		public override int VisitExpMultDivModEquals(YarnSpinnerParser.ExpMultDivModEqualsContext context)
		{
			opEquals(context.variable().GetText(), context.expression(), context.op.Type);
			return 0;
		}

		public override int VisitExpPlusMinusEquals(YarnSpinnerParser.ExpPlusMinusEqualsContext context)
		{
			opEquals(context.variable().GetText(), context.expression(), context.op.Type);
			return 0;
		}

		public override int VisitValueVar(YarnSpinnerParser.ValueVarContext context)
		{
			return Visit(context.variable());
		}

		public override int VisitValueNumber(YarnSpinnerParser.ValueNumberContext context)
		{
			float num = float.Parse(context.BODY_NUMBER().GetText(), CultureInfo.InvariantCulture);
			compiler.Emit(ByteCode.PushNumber, num);
			return 0;
		}

		public override int VisitValueTrue(YarnSpinnerParser.ValueTrueContext context)
		{
			compiler.Emit(ByteCode.PushBool, true);
			return 0;
		}

		public override int VisitValueFalse(YarnSpinnerParser.ValueFalseContext context)
		{
			compiler.Emit(ByteCode.PushBool, false);
			return 0;
		}

		public override int VisitVariable(YarnSpinnerParser.VariableContext context)
		{
			string text = context.VAR_ID().GetText();
			compiler.Emit(ByteCode.PushVariable, text);
			return 0;
		}

		public override int VisitValueString(YarnSpinnerParser.ValueStringContext context)
		{
			string theString = context.COMMAND_STRING().GetText().Trim('"');
			int line = context.Start.Line;
			string operandA = compiler.program.RegisterString(theString, compiler.currentNode.name, null, line, localisable: false);
			compiler.Emit(ByteCode.PushString, operandA);
			return 0;
		}

		public override int VisitValueFunc(YarnSpinnerParser.ValueFuncContext context)
		{
			Visit(context.function());
			return 0;
		}

		public override int VisitValueNull(YarnSpinnerParser.ValueNullContext context)
		{
			compiler.Emit(ByteCode.PushNull);
			return 0;
		}

		private void loadOperators()
		{
			tokens[44] = TokenType.LessThanOrEqualTo;
			tokens[45] = TokenType.GreaterThanOrEqualTo;
			tokens[47] = TokenType.LessThan;
			tokens[48] = TokenType.GreaterThan;
			tokens[46] = TokenType.EqualTo;
			tokens[49] = TokenType.NotEqualTo;
			tokens[50] = TokenType.And;
			tokens[51] = TokenType.Or;
			tokens[52] = TokenType.Xor;
			tokens[59] = TokenType.Add;
			tokens[60] = TokenType.Minus;
			tokens[61] = TokenType.Multiply;
			tokens[62] = TokenType.Divide;
			tokens[63] = TokenType.Modulo;
			tokens[54] = TokenType.Add;
			tokens[55] = TokenType.Minus;
			tokens[56] = TokenType.Multiply;
			tokens[58] = TokenType.Divide;
			tokens[57] = TokenType.Modulo;
		}
	}
}
