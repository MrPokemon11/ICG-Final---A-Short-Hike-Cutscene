using System;
using System.CodeDom.Compiler;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

[GeneratedCode("ANTLR", "4.7")]
[CLSCompliant(false)]
public class YarnSpinnerParser : Parser
{
	public class DialogueContext : ParserRuleContext
	{
		public override int RuleIndex => 0;

		public ITerminalNode Eof()
		{
			return GetToken(-1, 0);
		}

		public NodeContext[] node()
		{
			return GetRuleContexts<NodeContext>();
		}

		public NodeContext node(int i)
		{
			return GetRuleContext<NodeContext>(i);
		}

		public DialogueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterDialogue(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitDialogue(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitDialogue(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class NodeContext : ParserRuleContext
	{
		public override int RuleIndex => 1;

		public HeaderContext header()
		{
			return GetRuleContext<HeaderContext>(0);
		}

		public BodyContext body()
		{
			return GetRuleContext<BodyContext>(0);
		}

		public ITerminalNode[] NEWLINE()
		{
			return GetTokens(8);
		}

		public ITerminalNode NEWLINE(int i)
		{
			return GetToken(8, i);
		}

		public NodeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterNode(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitNode(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitNode(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class HeaderContext : ParserRuleContext
	{
		public override int RuleIndex => 2;

		public Header_titleContext header_title()
		{
			return GetRuleContext<Header_titleContext>(0);
		}

		public Header_tagContext[] header_tag()
		{
			return GetRuleContexts<Header_tagContext>();
		}

		public Header_tagContext header_tag(int i)
		{
			return GetRuleContext<Header_tagContext>(i);
		}

		public Header_lineContext[] header_line()
		{
			return GetRuleContexts<Header_lineContext>();
		}

		public Header_lineContext header_line(int i)
		{
			return GetRuleContext<Header_lineContext>(i);
		}

		public HeaderContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterHeader(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitHeader(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitHeader(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Header_titleContext : ParserRuleContext
	{
		public override int RuleIndex => 3;

		public ITerminalNode HEADER_TITLE()
		{
			return GetToken(2, 0);
		}

		public ITerminalNode TITLE_TEXT()
		{
			return GetToken(11, 0);
		}

		public ITerminalNode NEWLINE()
		{
			return GetToken(8, 0);
		}

		public Header_titleContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterHeader_title(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitHeader_title(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitHeader_title(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Header_tagContext : ParserRuleContext
	{
		public override int RuleIndex => 4;

		public ITerminalNode HEADER_TAGS()
		{
			return GetToken(3, 0);
		}

		public ITerminalNode TAG_TEXT()
		{
			return GetToken(12, 0);
		}

		public ITerminalNode NEWLINE()
		{
			return GetToken(8, 0);
		}

		public Header_tagContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterHeader_tag(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitHeader_tag(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitHeader_tag(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Header_lineContext : ParserRuleContext
	{
		public override int RuleIndex => 5;

		public ITerminalNode HEADER_NAME()
		{
			return GetToken(4, 0);
		}

		public ITerminalNode HEADER_TEXT()
		{
			return GetToken(14, 0);
		}

		public ITerminalNode NEWLINE()
		{
			return GetToken(8, 0);
		}

		public Header_lineContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterHeader_line(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitHeader_line(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitHeader_line(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class BodyContext : ParserRuleContext
	{
		public override int RuleIndex => 6;

		public ITerminalNode BODY_ENTER()
		{
			return GetToken(1, 0);
		}

		public ITerminalNode BODY_CLOSE()
		{
			return GetToken(17, 0);
		}

		public StatementContext[] statement()
		{
			return GetRuleContexts<StatementContext>();
		}

		public StatementContext statement(int i)
		{
			return GetRuleContext<StatementContext>(i);
		}

		public BodyContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterBody(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitBody(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitBody(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class StatementContext : ParserRuleContext
	{
		public override int RuleIndex => 7;

		public Shortcut_statementContext shortcut_statement()
		{
			return GetRuleContext<Shortcut_statementContext>(0);
		}

		public If_statementContext if_statement()
		{
			return GetRuleContext<If_statementContext>(0);
		}

		public Set_statementContext set_statement()
		{
			return GetRuleContext<Set_statementContext>(0);
		}

		public Option_statementContext option_statement()
		{
			return GetRuleContext<Option_statementContext>(0);
		}

		public Function_statementContext function_statement()
		{
			return GetRuleContext<Function_statementContext>(0);
		}

		public Action_statementContext action_statement()
		{
			return GetRuleContext<Action_statementContext>(0);
		}

		public Line_statementContext line_statement()
		{
			return GetRuleContext<Line_statementContext>(0);
		}

		public StatementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterStatement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitStatement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitStatement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Shortcut_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 8;

		public ShortcutContext[] shortcut()
		{
			return GetRuleContexts<ShortcutContext>();
		}

		public ShortcutContext shortcut(int i)
		{
			return GetRuleContext<ShortcutContext>(i);
		}

		public Shortcut_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterShortcut_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitShortcut_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitShortcut_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ShortcutContext : ParserRuleContext
	{
		public override int RuleIndex => 9;

		public ITerminalNode SHORTCUT_ENTER()
		{
			return GetToken(19, 0);
		}

		public Shortcut_textContext shortcut_text()
		{
			return GetRuleContext<Shortcut_textContext>(0);
		}

		public Shortcut_conditionalContext shortcut_conditional()
		{
			return GetRuleContext<Shortcut_conditionalContext>(0);
		}

		public Hashtag_blockContext hashtag_block()
		{
			return GetRuleContext<Hashtag_blockContext>(0);
		}

		public ITerminalNode INDENT()
		{
			return GetToken(20, 0);
		}

		public ITerminalNode DEDENT()
		{
			return GetToken(21, 0);
		}

		public StatementContext[] statement()
		{
			return GetRuleContexts<StatementContext>();
		}

		public StatementContext statement(int i)
		{
			return GetRuleContext<StatementContext>(i);
		}

		public ShortcutContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterShortcut(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitShortcut(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitShortcut(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Shortcut_conditionalContext : ParserRuleContext
	{
		public override int RuleIndex => 10;

		public ITerminalNode COMMAND_IF()
		{
			return GetToken(22, 0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ITerminalNode COMMAND_CLOSE()
		{
			return GetToken(34, 0);
		}

		public Shortcut_conditionalContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterShortcut_conditional(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitShortcut_conditional(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitShortcut_conditional(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Shortcut_textContext : ParserRuleContext
	{
		public override int RuleIndex => 11;

		public ITerminalNode SHORTCUT_TEXT()
		{
			return GetToken(32, 0);
		}

		public Shortcut_textContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterShortcut_text(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitShortcut_text(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitShortcut_text(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class If_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 12;

		public If_clauseContext if_clause()
		{
			return GetRuleContext<If_clauseContext>(0);
		}

		public ITerminalNode COMMAND_ENDIF()
		{
			return GetToken(25, 0);
		}

		public Else_if_clauseContext[] else_if_clause()
		{
			return GetRuleContexts<Else_if_clauseContext>();
		}

		public Else_if_clauseContext else_if_clause(int i)
		{
			return GetRuleContext<Else_if_clauseContext>(i);
		}

		public Else_clauseContext else_clause()
		{
			return GetRuleContext<Else_clauseContext>(0);
		}

		public Hashtag_blockContext hashtag_block()
		{
			return GetRuleContext<Hashtag_blockContext>(0);
		}

		public If_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterIf_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitIf_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitIf_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class If_clauseContext : ParserRuleContext
	{
		public override int RuleIndex => 13;

		public ITerminalNode COMMAND_IF()
		{
			return GetToken(22, 0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ITerminalNode COMMAND_CLOSE()
		{
			return GetToken(34, 0);
		}

		public StatementContext[] statement()
		{
			return GetRuleContexts<StatementContext>();
		}

		public StatementContext statement(int i)
		{
			return GetRuleContext<StatementContext>(i);
		}

		public If_clauseContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterIf_clause(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitIf_clause(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitIf_clause(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Else_if_clauseContext : ParserRuleContext
	{
		public override int RuleIndex => 14;

		public ITerminalNode COMMAND_ELSE_IF()
		{
			return GetToken(24, 0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ITerminalNode COMMAND_CLOSE()
		{
			return GetToken(34, 0);
		}

		public StatementContext[] statement()
		{
			return GetRuleContexts<StatementContext>();
		}

		public StatementContext statement(int i)
		{
			return GetRuleContext<StatementContext>(i);
		}

		public Else_if_clauseContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterElse_if_clause(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitElse_if_clause(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitElse_if_clause(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Else_clauseContext : ParserRuleContext
	{
		public override int RuleIndex => 15;

		public ITerminalNode COMMAND_ELSE()
		{
			return GetToken(23, 0);
		}

		public StatementContext[] statement()
		{
			return GetRuleContexts<StatementContext>();
		}

		public StatementContext statement(int i)
		{
			return GetRuleContext<StatementContext>(i);
		}

		public Else_clauseContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterElse_clause(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitElse_clause(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitElse_clause(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Set_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 16;

		public ITerminalNode COMMAND_SET()
		{
			return GetToken(26, 0);
		}

		public VariableContext variable()
		{
			return GetRuleContext<VariableContext>(0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ITerminalNode COMMAND_CLOSE()
		{
			return GetToken(34, 0);
		}

		public ITerminalNode[] KEYWORD_TO()
		{
			return GetTokens(43);
		}

		public ITerminalNode KEYWORD_TO(int i)
		{
			return GetToken(43, i);
		}

		public Set_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterSet_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitSet_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitSet_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Option_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 17;

		public ITerminalNode OPTION_TEXT()
		{
			return GetToken(73, 0);
		}

		public ITerminalNode OPTION_LINK()
		{
			return GetToken(75, 0);
		}

		public Hashtag_blockContext hashtag_block()
		{
			return GetRuleContext<Hashtag_blockContext>(0);
		}

		public Option_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterOption_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitOption_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitOption_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class FunctionContext : ParserRuleContext
	{
		public override int RuleIndex => 18;

		public ITerminalNode FUNC_ID()
		{
			return GetToken(69, 0);
		}

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ITerminalNode[] COMMA()
		{
			return GetTokens(66);
		}

		public ITerminalNode COMMA(int i)
		{
			return GetToken(66, i);
		}

		public FunctionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterFunction(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitFunction(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitFunction(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Function_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 19;

		public ITerminalNode COMMAND_FUNC()
		{
			return GetToken(27, 0);
		}

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ITerminalNode COMMAND_CLOSE()
		{
			return GetToken(34, 0);
		}

		public ITerminalNode[] COMMA()
		{
			return GetTokens(66);
		}

		public ITerminalNode COMMA(int i)
		{
			return GetToken(66, i);
		}

		public Function_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterFunction_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitFunction_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitFunction_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Action_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 20;

		public ITerminalNode ACTION()
		{
			return GetToken(71, 0);
		}

		public Action_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterAction_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitAction_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitAction_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class TextContext : ParserRuleContext
	{
		public override int RuleIndex => 21;

		public ITerminalNode TEXT()
		{
			return GetToken(31, 0);
		}

		public ITerminalNode TEXT_STRING()
		{
			return GetToken(18, 0);
		}

		public TextContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterText(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitText(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitText(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Line_statementContext : ParserRuleContext
	{
		public override int RuleIndex => 22;

		public TextContext text()
		{
			return GetRuleContext<TextContext>(0);
		}

		public Hashtag_blockContext hashtag_block()
		{
			return GetRuleContext<Hashtag_blockContext>(0);
		}

		public Line_statementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterLine_statement(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitLine_statement(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitLine_statement(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class Hashtag_blockContext : ParserRuleContext
	{
		public override int RuleIndex => 23;

		public HashtagContext[] hashtag()
		{
			return GetRuleContexts<HashtagContext>();
		}

		public HashtagContext hashtag(int i)
		{
			return GetRuleContext<HashtagContext>(i);
		}

		public Hashtag_blockContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterHashtag_block(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitHashtag_block(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitHashtag_block(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class HashtagContext : ParserRuleContext
	{
		public override int RuleIndex => 24;

		public ITerminalNode HASHTAG()
		{
			return GetToken(30, 0);
		}

		public HashtagContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterHashtag(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitHashtag(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitHashtag(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpressionContext : ParserRuleContext
	{
		public override int RuleIndex => 25;

		public ExpressionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public ExpressionContext()
		{
		}

		public virtual void CopyFrom(ExpressionContext context)
		{
			base.CopyFrom(context);
		}
	}

	public class ExpParensContext : ExpressionContext
	{
		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ExpParensContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpParens(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpParens(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpParens(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpMultDivModContext : ExpressionContext
	{
		public IToken op;

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ExpMultDivModContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpMultDivMod(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpMultDivMod(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpMultDivMod(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpMultDivModEqualsContext : ExpressionContext
	{
		public IToken op;

		public VariableContext variable()
		{
			return GetRuleContext<VariableContext>(0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ExpMultDivModEqualsContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpMultDivModEquals(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpMultDivModEquals(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpMultDivModEquals(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpComparisonContext : ExpressionContext
	{
		public IToken op;

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ITerminalNode OPERATOR_LOGICAL_LESS_THAN_EQUALS()
		{
			return GetToken(44, 0);
		}

		public ITerminalNode OPERATOR_LOGICAL_GREATER_THAN_EQUALS()
		{
			return GetToken(45, 0);
		}

		public ITerminalNode OPERATOR_LOGICAL_LESS()
		{
			return GetToken(47, 0);
		}

		public ITerminalNode OPERATOR_LOGICAL_GREATER()
		{
			return GetToken(48, 0);
		}

		public ExpComparisonContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpComparison(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpComparison(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpComparison(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpNegativeContext : ExpressionContext
	{
		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ExpNegativeContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpNegative(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpNegative(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpNegative(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpAndOrXorContext : ExpressionContext
	{
		public IToken op;

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ITerminalNode OPERATOR_LOGICAL_AND()
		{
			return GetToken(50, 0);
		}

		public ITerminalNode OPERATOR_LOGICAL_OR()
		{
			return GetToken(51, 0);
		}

		public ITerminalNode OPERATOR_LOGICAL_XOR()
		{
			return GetToken(52, 0);
		}

		public ExpAndOrXorContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpAndOrXor(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpAndOrXor(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpAndOrXor(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpPlusMinusEqualsContext : ExpressionContext
	{
		public IToken op;

		public VariableContext variable()
		{
			return GetRuleContext<VariableContext>(0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ExpPlusMinusEqualsContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpPlusMinusEquals(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpPlusMinusEquals(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpPlusMinusEquals(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpAddSubContext : ExpressionContext
	{
		public IToken op;

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ExpAddSubContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpAddSub(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpAddSub(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpAddSub(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpNotContext : ExpressionContext
	{
		public ITerminalNode OPERATOR_LOGICAL_NOT()
		{
			return GetToken(53, 0);
		}

		public ExpressionContext expression()
		{
			return GetRuleContext<ExpressionContext>(0);
		}

		public ExpNotContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpNot(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpNot(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpNot(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpValueContext : ExpressionContext
	{
		public ValueContext value()
		{
			return GetRuleContext<ValueContext>(0);
		}

		public ExpValueContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpValue(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpValue(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpValue(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ExpEqualityContext : ExpressionContext
	{
		public IToken op;

		public ExpressionContext[] expression()
		{
			return GetRuleContexts<ExpressionContext>();
		}

		public ExpressionContext expression(int i)
		{
			return GetRuleContext<ExpressionContext>(i);
		}

		public ITerminalNode OPERATOR_LOGICAL_EQUALS()
		{
			return GetToken(46, 0);
		}

		public ITerminalNode OPERATOR_LOGICAL_NOT_EQUALS()
		{
			return GetToken(49, 0);
		}

		public ExpEqualityContext(ExpressionContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterExpEquality(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitExpEquality(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitExpEquality(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueContext : ParserRuleContext
	{
		public override int RuleIndex => 26;

		public ValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public ValueContext()
		{
		}

		public virtual void CopyFrom(ValueContext context)
		{
			base.CopyFrom(context);
		}
	}

	public class ValueNullContext : ValueContext
	{
		public ITerminalNode KEYWORD_NULL()
		{
			return GetToken(42, 0);
		}

		public ValueNullContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueNull(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueNull(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueNull(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueNumberContext : ValueContext
	{
		public ITerminalNode BODY_NUMBER()
		{
			return GetToken(68, 0);
		}

		public ValueNumberContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueNumber(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueNumber(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueNumber(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueTrueContext : ValueContext
	{
		public ITerminalNode KEYWORD_TRUE()
		{
			return GetToken(40, 0);
		}

		public ValueTrueContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueTrue(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueTrue(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueTrue(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueFalseContext : ValueContext
	{
		public ITerminalNode KEYWORD_FALSE()
		{
			return GetToken(41, 0);
		}

		public ValueFalseContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueFalse(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueFalse(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueFalse(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueFuncContext : ValueContext
	{
		public FunctionContext function()
		{
			return GetRuleContext<FunctionContext>(0);
		}

		public ValueFuncContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueFunc(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueFunc(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueFunc(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueVarContext : ValueContext
	{
		public VariableContext variable()
		{
			return GetRuleContext<VariableContext>(0);
		}

		public ValueVarContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueVar(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueVar(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueVar(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class ValueStringContext : ValueContext
	{
		public ITerminalNode COMMAND_STRING()
		{
			return GetToken(35, 0);
		}

		public ValueStringContext(ValueContext context)
		{
			CopyFrom(context);
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterValueString(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitValueString(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitValueString(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	public class VariableContext : ParserRuleContext
	{
		public override int RuleIndex => 27;

		public ITerminalNode VAR_ID()
		{
			return GetToken(67, 0);
		}

		public VariableContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}

		public override void EnterRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.EnterVariable(this);
			}
		}

		public override void ExitRule(IParseTreeListener listener)
		{
			if (listener is IYarnSpinnerParserListener yarnSpinnerParserListener)
			{
				yarnSpinnerParserListener.ExitVariable(this);
			}
		}

		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
		{
			if (visitor is IYarnSpinnerParserVisitor<TResult> yarnSpinnerParserVisitor)
			{
				return yarnSpinnerParserVisitor.VisitVariable(this);
			}
			return visitor.VisitChildren(this);
		}
	}

	protected static DFA[] decisionToDFA;

	protected static PredictionContextCache sharedContextCache;

	public const int BODY_ENTER = 1;

	public const int HEADER_TITLE = 2;

	public const int HEADER_TAGS = 3;

	public const int HEADER_NAME = 4;

	public const int HEADER_SEPARATOR = 5;

	public const int STRING = 6;

	public const int ID = 7;

	public const int NEWLINE = 8;

	public const int UNKNOWN = 9;

	public const int TITLE_WS = 10;

	public const int TITLE_TEXT = 11;

	public const int TAG_TEXT = 12;

	public const int HEADER_WS = 13;

	public const int HEADER_TEXT = 14;

	public const int WS_IN_BODY = 15;

	public const int COMMENT = 16;

	public const int BODY_CLOSE = 17;

	public const int TEXT_STRING = 18;

	public const int SHORTCUT_ENTER = 19;

	public const int INDENT = 20;

	public const int DEDENT = 21;

	public const int COMMAND_IF = 22;

	public const int COMMAND_ELSE = 23;

	public const int COMMAND_ELSE_IF = 24;

	public const int COMMAND_ENDIF = 25;

	public const int COMMAND_SET = 26;

	public const int COMMAND_FUNC = 27;

	public const int COMMAND_OPEN = 28;

	public const int OPTION_ENTER = 29;

	public const int HASHTAG = 30;

	public const int TEXT = 31;

	public const int SHORTCUT_TEXT = 32;

	public const int COMMAND_WS = 33;

	public const int COMMAND_CLOSE = 34;

	public const int COMMAND_STRING = 35;

	public const int KEYWORD_IF = 36;

	public const int KEYWORD_ELSE = 37;

	public const int KEYWORD_ELSE_IF = 38;

	public const int KEYWORD_SET = 39;

	public const int KEYWORD_TRUE = 40;

	public const int KEYWORD_FALSE = 41;

	public const int KEYWORD_NULL = 42;

	public const int KEYWORD_TO = 43;

	public const int OPERATOR_LOGICAL_LESS_THAN_EQUALS = 44;

	public const int OPERATOR_LOGICAL_GREATER_THAN_EQUALS = 45;

	public const int OPERATOR_LOGICAL_EQUALS = 46;

	public const int OPERATOR_LOGICAL_LESS = 47;

	public const int OPERATOR_LOGICAL_GREATER = 48;

	public const int OPERATOR_LOGICAL_NOT_EQUALS = 49;

	public const int OPERATOR_LOGICAL_AND = 50;

	public const int OPERATOR_LOGICAL_OR = 51;

	public const int OPERATOR_LOGICAL_XOR = 52;

	public const int OPERATOR_LOGICAL_NOT = 53;

	public const int OPERATOR_MATHS_ADDITION_EQUALS = 54;

	public const int OPERATOR_MATHS_SUBTRACTION_EQUALS = 55;

	public const int OPERATOR_MATHS_MULTIPLICATION_EQUALS = 56;

	public const int OPERATOR_MATHS_MODULUS_EQUALS = 57;

	public const int OPERATOR_MATHS_DIVISION_EQUALS = 58;

	public const int OPERATOR_MATHS_ADDITION = 59;

	public const int OPERATOR_MATHS_SUBTRACTION = 60;

	public const int OPERATOR_MATHS_MULTIPLICATION = 61;

	public const int OPERATOR_MATHS_DIVISION = 62;

	public const int OPERATOR_MATHS_MODULUS = 63;

	public const int LPAREN = 64;

	public const int RPAREN = 65;

	public const int COMMA = 66;

	public const int VAR_ID = 67;

	public const int BODY_NUMBER = 68;

	public const int FUNC_ID = 69;

	public const int COMMAND_UNKNOWN = 70;

	public const int ACTION = 71;

	public const int OPTION_SEPARATOR = 72;

	public const int OPTION_TEXT = 73;

	public const int OPTION_CLOSE = 74;

	public const int OPTION_LINK = 75;

	public const int RULE_dialogue = 0;

	public const int RULE_node = 1;

	public const int RULE_header = 2;

	public const int RULE_header_title = 3;

	public const int RULE_header_tag = 4;

	public const int RULE_header_line = 5;

	public const int RULE_body = 6;

	public const int RULE_statement = 7;

	public const int RULE_shortcut_statement = 8;

	public const int RULE_shortcut = 9;

	public const int RULE_shortcut_conditional = 10;

	public const int RULE_shortcut_text = 11;

	public const int RULE_if_statement = 12;

	public const int RULE_if_clause = 13;

	public const int RULE_else_if_clause = 14;

	public const int RULE_else_clause = 15;

	public const int RULE_set_statement = 16;

	public const int RULE_option_statement = 17;

	public const int RULE_function = 18;

	public const int RULE_function_statement = 19;

	public const int RULE_action_statement = 20;

	public const int RULE_text = 21;

	public const int RULE_line_statement = 22;

	public const int RULE_hashtag_block = 23;

	public const int RULE_hashtag = 24;

	public const int RULE_expression = 25;

	public const int RULE_value = 26;

	public const int RULE_variable = 27;

	public static readonly string[] ruleNames;

	private static readonly string[] _LiteralNames;

	private static readonly string[] _SymbolicNames;

	public static readonly IVocabulary DefaultVocabulary;

	private static char[] _serializedATN;

	public static readonly ATN _ATN;

	[NotNull]
	public override IVocabulary Vocabulary => DefaultVocabulary;

	public override string GrammarFileName => "YarnSpinnerParser.g4";

	public override string[] RuleNames => ruleNames;

	public override string SerializedAtn => new string(_serializedATN);

	static YarnSpinnerParser()
	{
		sharedContextCache = new PredictionContextCache();
		ruleNames = new string[28]
		{
			"dialogue", "node", "header", "header_title", "header_tag", "header_line", "body", "statement", "shortcut_statement", "shortcut",
			"shortcut_conditional", "shortcut_text", "if_statement", "if_clause", "else_if_clause", "else_clause", "set_statement", "option_statement", "function", "function_statement",
			"action_statement", "text", "line_statement", "hashtag_block", "hashtag", "expression", "value", "variable"
		};
		_LiteralNames = new string[75]
		{
			null, "'---'", "'title:'", "'tags:'", null, "':'", null, null, null, null,
			null, null, null, null, null, null, null, "'==='", null, null,
			"'\a'", "'\v'", null, null, null, null, null, null, null, "'[['",
			null, null, null, null, null, null, null, null, null, null,
			null, null, null, null, null, null, null, null, null, null,
			null, null, null, null, "'+='", "'-='", "'*='", "'%='", "'/='", "'+'",
			"'-'", "'*'", "'/'", "'%'", "'('", "')'", "','", null, null, null,
			null, null, "'|'", null, "']]'"
		};
		_SymbolicNames = new string[76]
		{
			null, "BODY_ENTER", "HEADER_TITLE", "HEADER_TAGS", "HEADER_NAME", "HEADER_SEPARATOR", "STRING", "ID", "NEWLINE", "UNKNOWN",
			"TITLE_WS", "TITLE_TEXT", "TAG_TEXT", "HEADER_WS", "HEADER_TEXT", "WS_IN_BODY", "COMMENT", "BODY_CLOSE", "TEXT_STRING", "SHORTCUT_ENTER",
			"INDENT", "DEDENT", "COMMAND_IF", "COMMAND_ELSE", "COMMAND_ELSE_IF", "COMMAND_ENDIF", "COMMAND_SET", "COMMAND_FUNC", "COMMAND_OPEN", "OPTION_ENTER",
			"HASHTAG", "TEXT", "SHORTCUT_TEXT", "COMMAND_WS", "COMMAND_CLOSE", "COMMAND_STRING", "KEYWORD_IF", "KEYWORD_ELSE", "KEYWORD_ELSE_IF", "KEYWORD_SET",
			"KEYWORD_TRUE", "KEYWORD_FALSE", "KEYWORD_NULL", "KEYWORD_TO", "OPERATOR_LOGICAL_LESS_THAN_EQUALS", "OPERATOR_LOGICAL_GREATER_THAN_EQUALS", "OPERATOR_LOGICAL_EQUALS", "OPERATOR_LOGICAL_LESS", "OPERATOR_LOGICAL_GREATER", "OPERATOR_LOGICAL_NOT_EQUALS",
			"OPERATOR_LOGICAL_AND", "OPERATOR_LOGICAL_OR", "OPERATOR_LOGICAL_XOR", "OPERATOR_LOGICAL_NOT", "OPERATOR_MATHS_ADDITION_EQUALS", "OPERATOR_MATHS_SUBTRACTION_EQUALS", "OPERATOR_MATHS_MULTIPLICATION_EQUALS", "OPERATOR_MATHS_MODULUS_EQUALS", "OPERATOR_MATHS_DIVISION_EQUALS", "OPERATOR_MATHS_ADDITION",
			"OPERATOR_MATHS_SUBTRACTION", "OPERATOR_MATHS_MULTIPLICATION", "OPERATOR_MATHS_DIVISION", "OPERATOR_MATHS_MODULUS", "LPAREN", "RPAREN", "COMMA", "VAR_ID", "BODY_NUMBER", "FUNC_ID",
			"COMMAND_UNKNOWN", "ACTION", "OPTION_SEPARATOR", "OPTION_TEXT", "OPTION_CLOSE", "OPTION_LINK"
		};
		DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);
		_serializedATN = new char[2676]
		{
			'\u0003', '悋', 'Ꜫ', '脳', '맭', '䅼', '㯧', '瞆', '奤', '\u0003',
			'M', 'į', '\u0004', '\u0002', '\t', '\u0002', '\u0004', '\u0003', '\t', '\u0003',
			'\u0004', '\u0004', '\t', '\u0004', '\u0004', '\u0005', '\t', '\u0005', '\u0004', '\u0006',
			'\t', '\u0006', '\u0004', '\a', '\t', '\a', '\u0004', '\b', '\t', '\b',
			'\u0004', '\t', '\t', '\t', '\u0004', '\n', '\t', '\n', '\u0004', '\v',
			'\t', '\v', '\u0004', '\f', '\t', '\f', '\u0004', '\r', '\t', '\r',
			'\u0004', '\u000e', '\t', '\u000e', '\u0004', '\u000f', '\t', '\u000f', '\u0004', '\u0010',
			'\t', '\u0010', '\u0004', '\u0011', '\t', '\u0011', '\u0004', '\u0012', '\t', '\u0012',
			'\u0004', '\u0013', '\t', '\u0013', '\u0004', '\u0014', '\t', '\u0014', '\u0004', '\u0015',
			'\t', '\u0015', '\u0004', '\u0016', '\t', '\u0016', '\u0004', '\u0017', '\t', '\u0017',
			'\u0004', '\u0018', '\t', '\u0018', '\u0004', '\u0019', '\t', '\u0019', '\u0004', '\u001a',
			'\t', '\u001a', '\u0004', '\u001b', '\t', '\u001b', '\u0004', '\u001c', '\t', '\u001c',
			'\u0004', '\u001d', '\t', '\u001d', '\u0003', '\u0002', '\u0006', '\u0002', '<', '\n',
			'\u0002', '\r', '\u0002', '\u000e', '\u0002', '=', '\u0003', '\u0002', '\u0003', '\u0002',
			'\u0003', '\u0003', '\u0003', '\u0003', '\u0003', '\u0003', '\a', '\u0003', 'E', '\n',
			'\u0003', '\f', '\u0003', '\u000e', '\u0003', 'H', '\v', '\u0003', '\u0003', '\u0004',
			'\u0003', '\u0004', '\u0003', '\u0004', '\a', '\u0004', 'M', '\n', '\u0004', '\f',
			'\u0004', '\u000e', '\u0004', 'P', '\v', '\u0004', '\u0003', '\u0005', '\u0003', '\u0005',
			'\u0003', '\u0005', '\u0003', '\u0005', '\u0003', '\u0006', '\u0003', '\u0006', '\u0003', '\u0006',
			'\u0003', '\u0006', '\u0003', '\a', '\u0003', '\a', '\u0003', '\a', '\u0003', '\a',
			'\u0003', '\a', '\u0003', '\b', '\u0003', '\b', '\a', '\b', 'a', '\n',
			'\b', '\f', '\b', '\u000e', '\b', 'd', '\v', '\b', '\u0003', '\b',
			'\u0003', '\b', '\u0003', '\t', '\u0003', '\t', '\u0003', '\t', '\u0003', '\t',
			'\u0003', '\t', '\u0003', '\t', '\u0003', '\t', '\u0005', '\t', 'o', '\n',
			'\t', '\u0003', '\n', '\u0006', '\n', 'r', '\n', '\n', '\r', '\n',
			'\u000e', '\n', 's', '\u0003', '\v', '\u0003', '\v', '\u0003', '\v', '\u0005',
			'\v', 'y', '\n', '\v', '\u0003', '\v', '\u0005', '\v', '|', '\n',
			'\v', '\u0003', '\v', '\u0003', '\v', '\a', '\v', '\u0080', '\n', '\v',
			'\f', '\v', '\u000e', '\v', '\u0083', '\v', '\v', '\u0003', '\v', '\u0005',
			'\v', '\u0086', '\n', '\v', '\u0003', '\f', '\u0003', '\f', '\u0003', '\f',
			'\u0003', '\f', '\u0003', '\r', '\u0003', '\r', '\u0003', '\u000e', '\u0003', '\u000e',
			'\a', '\u000e', '\u0090', '\n', '\u000e', '\f', '\u000e', '\u000e', '\u000e', '\u0093',
			'\v', '\u000e', '\u0003', '\u000e', '\u0005', '\u000e', '\u0096', '\n', '\u000e', '\u0003',
			'\u000e', '\u0003', '\u000e', '\u0005', '\u000e', '\u009a', '\n', '\u000e', '\u0003', '\u000f',
			'\u0003', '\u000f', '\u0003', '\u000f', '\u0003', '\u000f', '\a', '\u000f', '\u00a0', '\n',
			'\u000f', '\f', '\u000f', '\u000e', '\u000f', '£', '\v', '\u000f', '\u0003', '\u0010',
			'\u0003', '\u0010', '\u0003', '\u0010', '\u0003', '\u0010', '\a', '\u0010', '©', '\n',
			'\u0010', '\f', '\u0010', '\u000e', '\u0010', '¬', '\v', '\u0010', '\u0003', '\u0011',
			'\u0003', '\u0011', '\a', '\u0011', '°', '\n', '\u0011', '\f', '\u0011', '\u000e',
			'\u0011', '³', '\v', '\u0011', '\u0003', '\u0012', '\u0003', '\u0012', '\u0003', '\u0012',
			'\a', '\u0012', '\u00b8', '\n', '\u0012', '\f', '\u0012', '\u000e', '\u0012', '»',
			'\v', '\u0012', '\u0003', '\u0012', '\u0003', '\u0012', '\u0003', '\u0012', '\u0003', '\u0012',
			'\u0003', '\u0012', '\u0003', '\u0012', '\u0003', '\u0012', '\u0005', '\u0012', 'Ä', '\n',
			'\u0012', '\u0003', '\u0013', '\u0003', '\u0013', '\u0003', '\u0013', '\u0003', '\u0013', '\u0003',
			'\u0013', '\u0003', '\u0013', '\u0003', '\u0013', '\u0003', '\u0013', '\u0005', '\u0013', 'Î',
			'\n', '\u0013', '\u0003', '\u0013', '\u0005', '\u0013', 'Ñ', '\n', '\u0013', '\u0003',
			'\u0014', '\u0003', '\u0014', '\u0003', '\u0014', '\u0005', '\u0014', 'Ö', '\n', '\u0014',
			'\u0003', '\u0014', '\u0003', '\u0014', '\a', '\u0014', 'Ú', '\n', '\u0014', '\f',
			'\u0014', '\u000e', '\u0014', 'Ý', '\v', '\u0014', '\u0003', '\u0014', '\u0003', '\u0014',
			'\u0003', '\u0015', '\u0003', '\u0015', '\u0003', '\u0015', '\u0003', '\u0015', '\a', '\u0015',
			'å', '\n', '\u0015', '\f', '\u0015', '\u000e', '\u0015', 'è', '\v', '\u0015',
			'\u0003', '\u0015', '\u0003', '\u0015', '\u0003', '\u0015', '\u0003', '\u0016', '\u0003', '\u0016',
			'\u0003', '\u0017', '\u0003', '\u0017', '\u0003', '\u0018', '\u0003', '\u0018', '\u0005', '\u0018',
			'ó', '\n', '\u0018', '\u0003', '\u0019', '\u0006', '\u0019', 'ö', '\n', '\u0019',
			'\r', '\u0019', '\u000e', '\u0019', '÷', '\u0003', '\u001a', '\u0003', '\u001a', '\u0003',
			'\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003',
			'\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003',
			'\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003',
			'\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0005', '\u001b', 'Ď', '\n', '\u001b',
			'\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b',
			'\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b',
			'\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b', '\u0003', '\u001b',
			'\a', '\u001b', 'ğ', '\n', '\u001b', '\f', '\u001b', '\u000e', '\u001b', 'Ģ',
			'\v', '\u001b', '\u0003', '\u001c', '\u0003', '\u001c', '\u0003', '\u001c', '\u0003', '\u001c',
			'\u0003', '\u001c', '\u0003', '\u001c', '\u0003', '\u001c', '\u0005', '\u001c', 'ī', '\n',
			'\u001c', '\u0003', '\u001d', '\u0003', '\u001d', '\u0003', '\u001d', '\u0002', '\u0003', '4',
			'\u001e', '\u0002', '\u0004', '\u0006', '\b', '\n', '\f', '\u000e', '\u0010', '\u0012',
			'\u0014', '\u0016', '\u0018', '\u001a', '\u001c', '\u001e', ' ', '"', '$', '&',
			'(', '*', ',', '.', '0', '2', '4', '6', '8', '\u0002',
			'\n', '\u0004', '\u0002', '\u0014', '\u0014', '!', '!', '\u0003', '\u0002', ':',
			'<', '\u0003', '\u0002', '8', '9', '\u0003', '\u0002', '?', 'A', '\u0003',
			'\u0002', '=', '>', '\u0004', '\u0002', '.', '/', '1', '2', '\u0004',
			'\u0002', '0', '0', '3', '3', '\u0003', '\u0002', '4', '6', '\u0002',
			'Ł', '\u0002', ';', '\u0003', '\u0002', '\u0002', '\u0002', '\u0004', 'A', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0006', 'I', '\u0003', '\u0002', '\u0002', '\u0002', '\b',
			'Q', '\u0003', '\u0002', '\u0002', '\u0002', '\n', 'U', '\u0003', '\u0002', '\u0002',
			'\u0002', '\f', 'Y', '\u0003', '\u0002', '\u0002', '\u0002', '\u000e', '^', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0010', 'n', '\u0003', '\u0002', '\u0002', '\u0002', '\u0012',
			'q', '\u0003', '\u0002', '\u0002', '\u0002', '\u0014', 'u', '\u0003', '\u0002', '\u0002',
			'\u0002', '\u0016', '\u0087', '\u0003', '\u0002', '\u0002', '\u0002', '\u0018', '\u008b', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u001a', '\u008d', '\u0003', '\u0002', '\u0002', '\u0002', '\u001c',
			'\u009b', '\u0003', '\u0002', '\u0002', '\u0002', '\u001e', '¤', '\u0003', '\u0002', '\u0002',
			'\u0002', ' ', '\u00ad', '\u0003', '\u0002', '\u0002', '\u0002', '"', 'Ã', '\u0003',
			'\u0002', '\u0002', '\u0002', '$', 'Í', '\u0003', '\u0002', '\u0002', '\u0002', '&',
			'Ò', '\u0003', '\u0002', '\u0002', '\u0002', '(', 'à', '\u0003', '\u0002', '\u0002',
			'\u0002', '*', 'ì', '\u0003', '\u0002', '\u0002', '\u0002', ',', 'î', '\u0003',
			'\u0002', '\u0002', '\u0002', '.', 'ð', '\u0003', '\u0002', '\u0002', '\u0002', '0',
			'õ', '\u0003', '\u0002', '\u0002', '\u0002', '2', 'ù', '\u0003', '\u0002', '\u0002',
			'\u0002', '4', 'č', '\u0003', '\u0002', '\u0002', '\u0002', '6', 'Ī', '\u0003',
			'\u0002', '\u0002', '\u0002', '8', 'Ĭ', '\u0003', '\u0002', '\u0002', '\u0002', ':',
			'<', '\u0005', '\u0004', '\u0003', '\u0002', ';', ':', '\u0003', '\u0002', '\u0002',
			'\u0002', '<', '=', '\u0003', '\u0002', '\u0002', '\u0002', '=', ';', '\u0003',
			'\u0002', '\u0002', '\u0002', '=', '>', '\u0003', '\u0002', '\u0002', '\u0002', '>',
			'?', '\u0003', '\u0002', '\u0002', '\u0002', '?', '@', '\a', '\u0002', '\u0002',
			'\u0003', '@', '\u0003', '\u0003', '\u0002', '\u0002', '\u0002', 'A', 'B', '\u0005',
			'\u0006', '\u0004', '\u0002', 'B', 'F', '\u0005', '\u000e', '\b', '\u0002', 'C',
			'E', '\a', '\n', '\u0002', '\u0002', 'D', 'C', '\u0003', '\u0002', '\u0002',
			'\u0002', 'E', 'H', '\u0003', '\u0002', '\u0002', '\u0002', 'F', 'D', '\u0003',
			'\u0002', '\u0002', '\u0002', 'F', 'G', '\u0003', '\u0002', '\u0002', '\u0002', 'G',
			'\u0005', '\u0003', '\u0002', '\u0002', '\u0002', 'H', 'F', '\u0003', '\u0002', '\u0002',
			'\u0002', 'I', 'N', '\u0005', '\b', '\u0005', '\u0002', 'J', 'M', '\u0005',
			'\n', '\u0006', '\u0002', 'K', 'M', '\u0005', '\f', '\a', '\u0002', 'L',
			'J', '\u0003', '\u0002', '\u0002', '\u0002', 'L', 'K', '\u0003', '\u0002', '\u0002',
			'\u0002', 'M', 'P', '\u0003', '\u0002', '\u0002', '\u0002', 'N', 'L', '\u0003',
			'\u0002', '\u0002', '\u0002', 'N', 'O', '\u0003', '\u0002', '\u0002', '\u0002', 'O',
			'\a', '\u0003', '\u0002', '\u0002', '\u0002', 'P', 'N', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Q', 'R', '\a', '\u0004', '\u0002', '\u0002', 'R', 'S', '\a',
			'\r', '\u0002', '\u0002', 'S', 'T', '\a', '\n', '\u0002', '\u0002', 'T',
			'\t', '\u0003', '\u0002', '\u0002', '\u0002', 'U', 'V', '\a', '\u0005', '\u0002',
			'\u0002', 'V', 'W', '\a', '\u000e', '\u0002', '\u0002', 'W', 'X', '\a',
			'\n', '\u0002', '\u0002', 'X', '\v', '\u0003', '\u0002', '\u0002', '\u0002', 'Y',
			'Z', '\a', '\u0006', '\u0002', '\u0002', 'Z', '[', '\a', '\a', '\u0002',
			'\u0002', '[', '\\', '\a', '\u0010', '\u0002', '\u0002', '\\', ']', '\a',
			'\n', '\u0002', '\u0002', ']', '\r', '\u0003', '\u0002', '\u0002', '\u0002', '^',
			'b', '\a', '\u0003', '\u0002', '\u0002', '_', 'a', '\u0005', '\u0010', '\t',
			'\u0002', '`', '_', '\u0003', '\u0002', '\u0002', '\u0002', 'a', 'd', '\u0003',
			'\u0002', '\u0002', '\u0002', 'b', '`', '\u0003', '\u0002', '\u0002', '\u0002', 'b',
			'c', '\u0003', '\u0002', '\u0002', '\u0002', 'c', 'e', '\u0003', '\u0002', '\u0002',
			'\u0002', 'd', 'b', '\u0003', '\u0002', '\u0002', '\u0002', 'e', 'f', '\a',
			'\u0013', '\u0002', '\u0002', 'f', '\u000f', '\u0003', '\u0002', '\u0002', '\u0002', 'g',
			'o', '\u0005', '\u0012', '\n', '\u0002', 'h', 'o', '\u0005', '\u001a', '\u000e',
			'\u0002', 'i', 'o', '\u0005', '"', '\u0012', '\u0002', 'j', 'o', '\u0005',
			'$', '\u0013', '\u0002', 'k', 'o', '\u0005', '(', '\u0015', '\u0002', 'l',
			'o', '\u0005', '*', '\u0016', '\u0002', 'm', 'o', '\u0005', '.', '\u0018',
			'\u0002', 'n', 'g', '\u0003', '\u0002', '\u0002', '\u0002', 'n', 'h', '\u0003',
			'\u0002', '\u0002', '\u0002', 'n', 'i', '\u0003', '\u0002', '\u0002', '\u0002', 'n',
			'j', '\u0003', '\u0002', '\u0002', '\u0002', 'n', 'k', '\u0003', '\u0002', '\u0002',
			'\u0002', 'n', 'l', '\u0003', '\u0002', '\u0002', '\u0002', 'n', 'm', '\u0003',
			'\u0002', '\u0002', '\u0002', 'o', '\u0011', '\u0003', '\u0002', '\u0002', '\u0002', 'p',
			'r', '\u0005', '\u0014', '\v', '\u0002', 'q', 'p', '\u0003', '\u0002', '\u0002',
			'\u0002', 'r', 's', '\u0003', '\u0002', '\u0002', '\u0002', 's', 'q', '\u0003',
			'\u0002', '\u0002', '\u0002', 's', 't', '\u0003', '\u0002', '\u0002', '\u0002', 't',
			'\u0013', '\u0003', '\u0002', '\u0002', '\u0002', 'u', 'v', '\a', '\u0015', '\u0002',
			'\u0002', 'v', 'x', '\u0005', '\u0018', '\r', '\u0002', 'w', 'y', '\u0005',
			'\u0016', '\f', '\u0002', 'x', 'w', '\u0003', '\u0002', '\u0002', '\u0002', 'x',
			'y', '\u0003', '\u0002', '\u0002', '\u0002', 'y', '{', '\u0003', '\u0002', '\u0002',
			'\u0002', 'z', '|', '\u0005', '0', '\u0019', '\u0002', '{', 'z', '\u0003',
			'\u0002', '\u0002', '\u0002', '{', '|', '\u0003', '\u0002', '\u0002', '\u0002', '|',
			'\u0085', '\u0003', '\u0002', '\u0002', '\u0002', '}', '\u0081', '\a', '\u0016', '\u0002',
			'\u0002', '~', '\u0080', '\u0005', '\u0010', '\t', '\u0002', '\u007f', '~', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0080', '\u0083', '\u0003', '\u0002', '\u0002', '\u0002', '\u0081',
			'\u007f', '\u0003', '\u0002', '\u0002', '\u0002', '\u0081', '\u0082', '\u0003', '\u0002', '\u0002',
			'\u0002', '\u0082', '\u0084', '\u0003', '\u0002', '\u0002', '\u0002', '\u0083', '\u0081', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0084', '\u0086', '\a', '\u0017', '\u0002', '\u0002', '\u0085',
			'}', '\u0003', '\u0002', '\u0002', '\u0002', '\u0085', '\u0086', '\u0003', '\u0002', '\u0002',
			'\u0002', '\u0086', '\u0015', '\u0003', '\u0002', '\u0002', '\u0002', '\u0087', '\u0088', '\a',
			'\u0018', '\u0002', '\u0002', '\u0088', '\u0089', '\u0005', '4', '\u001b', '\u0002', '\u0089',
			'\u008a', '\a', '$', '\u0002', '\u0002', '\u008a', '\u0017', '\u0003', '\u0002', '\u0002',
			'\u0002', '\u008b', '\u008c', '\a', '"', '\u0002', '\u0002', '\u008c', '\u0019', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u008d', '\u0091', '\u0005', '\u001c', '\u000f', '\u0002', '\u008e',
			'\u0090', '\u0005', '\u001e', '\u0010', '\u0002', '\u008f', '\u008e', '\u0003', '\u0002', '\u0002',
			'\u0002', '\u0090', '\u0093', '\u0003', '\u0002', '\u0002', '\u0002', '\u0091', '\u008f', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0091', '\u0092', '\u0003', '\u0002', '\u0002', '\u0002', '\u0092',
			'\u0095', '\u0003', '\u0002', '\u0002', '\u0002', '\u0093', '\u0091', '\u0003', '\u0002', '\u0002',
			'\u0002', '\u0094', '\u0096', '\u0005', ' ', '\u0011', '\u0002', '\u0095', '\u0094', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0095', '\u0096', '\u0003', '\u0002', '\u0002', '\u0002', '\u0096',
			'\u0097', '\u0003', '\u0002', '\u0002', '\u0002', '\u0097', '\u0099', '\a', '\u001b', '\u0002',
			'\u0002', '\u0098', '\u009a', '\u0005', '0', '\u0019', '\u0002', '\u0099', '\u0098', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u0099', '\u009a', '\u0003', '\u0002', '\u0002', '\u0002', '\u009a',
			'\u001b', '\u0003', '\u0002', '\u0002', '\u0002', '\u009b', '\u009c', '\a', '\u0018', '\u0002',
			'\u0002', '\u009c', '\u009d', '\u0005', '4', '\u001b', '\u0002', '\u009d', '¡', '\a',
			'$', '\u0002', '\u0002', '\u009e', '\u00a0', '\u0005', '\u0010', '\t', '\u0002', '\u009f',
			'\u009e', '\u0003', '\u0002', '\u0002', '\u0002', '\u00a0', '£', '\u0003', '\u0002', '\u0002',
			'\u0002', '¡', '\u009f', '\u0003', '\u0002', '\u0002', '\u0002', '¡', '¢', '\u0003',
			'\u0002', '\u0002', '\u0002', '¢', '\u001d', '\u0003', '\u0002', '\u0002', '\u0002', '£',
			'¡', '\u0003', '\u0002', '\u0002', '\u0002', '¤', '¥', '\a', '\u001a', '\u0002',
			'\u0002', '¥', '¦', '\u0005', '4', '\u001b', '\u0002', '¦', 'ª', '\a',
			'$', '\u0002', '\u0002', '§', '©', '\u0005', '\u0010', '\t', '\u0002', '\u00a8',
			'§', '\u0003', '\u0002', '\u0002', '\u0002', '©', '¬', '\u0003', '\u0002', '\u0002',
			'\u0002', 'ª', '\u00a8', '\u0003', '\u0002', '\u0002', '\u0002', 'ª', '«', '\u0003',
			'\u0002', '\u0002', '\u0002', '«', '\u001f', '\u0003', '\u0002', '\u0002', '\u0002', '¬',
			'ª', '\u0003', '\u0002', '\u0002', '\u0002', '\u00ad', '±', '\a', '\u0019', '\u0002',
			'\u0002', '®', '°', '\u0005', '\u0010', '\t', '\u0002', '\u00af', '®', '\u0003',
			'\u0002', '\u0002', '\u0002', '°', '³', '\u0003', '\u0002', '\u0002', '\u0002', '±',
			'\u00af', '\u0003', '\u0002', '\u0002', '\u0002', '±', '²', '\u0003', '\u0002', '\u0002',
			'\u0002', '²', '!', '\u0003', '\u0002', '\u0002', '\u0002', '³', '±', '\u0003',
			'\u0002', '\u0002', '\u0002', '\u00b4', 'µ', '\a', '\u001c', '\u0002', '\u0002', 'µ',
			'¹', '\u0005', '8', '\u001d', '\u0002', '¶', '\u00b8', '\a', '-', '\u0002',
			'\u0002', '·', '¶', '\u0003', '\u0002', '\u0002', '\u0002', '\u00b8', '»', '\u0003',
			'\u0002', '\u0002', '\u0002', '¹', '·', '\u0003', '\u0002', '\u0002', '\u0002', '¹',
			'º', '\u0003', '\u0002', '\u0002', '\u0002', 'º', '¼', '\u0003', '\u0002', '\u0002',
			'\u0002', '»', '¹', '\u0003', '\u0002', '\u0002', '\u0002', '¼', '½', '\u0005',
			'4', '\u001b', '\u0002', '½', '¾', '\a', '$', '\u0002', '\u0002', '¾',
			'Ä', '\u0003', '\u0002', '\u0002', '\u0002', '¿', 'À', '\a', '\u001c', '\u0002',
			'\u0002', 'À', 'Á', '\u0005', '4', '\u001b', '\u0002', 'Á', 'Â', '\a',
			'$', '\u0002', '\u0002', 'Â', 'Ä', '\u0003', '\u0002', '\u0002', '\u0002', 'Ã',
			'\u00b4', '\u0003', '\u0002', '\u0002', '\u0002', 'Ã', '¿', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ä', '#', '\u0003', '\u0002', '\u0002', '\u0002', 'Å', 'Æ', '\a',
			'\u001f', '\u0002', '\u0002', 'Æ', 'Ç', '\a', 'K', '\u0002', '\u0002', 'Ç',
			'È', '\a', 'J', '\u0002', '\u0002', 'È', 'É', '\a', 'M', '\u0002',
			'\u0002', 'É', 'Î', '\a', 'L', '\u0002', '\u0002', 'Ê', 'Ë', '\a',
			'\u001f', '\u0002', '\u0002', 'Ë', 'Ì', '\a', 'K', '\u0002', '\u0002', 'Ì',
			'Î', '\a', 'L', '\u0002', '\u0002', 'Í', 'Å', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Í', 'Ê', '\u0003', '\u0002', '\u0002', '\u0002', 'Î', 'Ð', '\u0003',
			'\u0002', '\u0002', '\u0002', 'Ï', 'Ñ', '\u0005', '0', '\u0019', '\u0002', 'Ð',
			'Ï', '\u0003', '\u0002', '\u0002', '\u0002', 'Ð', 'Ñ', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ñ', '%', '\u0003', '\u0002', '\u0002', '\u0002', 'Ò', 'Ó', '\a',
			'G', '\u0002', '\u0002', 'Ó', 'Õ', '\a', 'B', '\u0002', '\u0002', 'Ô',
			'Ö', '\u0005', '4', '\u001b', '\u0002', 'Õ', 'Ô', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Õ', 'Ö', '\u0003', '\u0002', '\u0002', '\u0002', 'Ö', 'Û', '\u0003',
			'\u0002', '\u0002', '\u0002', '×', 'Ø', '\a', 'D', '\u0002', '\u0002', 'Ø',
			'Ú', '\u0005', '4', '\u001b', '\u0002', 'Ù', '×', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ú', 'Ý', '\u0003', '\u0002', '\u0002', '\u0002', 'Û', 'Ù', '\u0003',
			'\u0002', '\u0002', '\u0002', 'Û', 'Ü', '\u0003', '\u0002', '\u0002', '\u0002', 'Ü',
			'Þ', '\u0003', '\u0002', '\u0002', '\u0002', 'Ý', 'Û', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Þ', 'ß', '\a', 'C', '\u0002', '\u0002', 'ß', '\'', '\u0003',
			'\u0002', '\u0002', '\u0002', 'à', 'á', '\a', '\u001d', '\u0002', '\u0002', 'á',
			'æ', '\u0005', '4', '\u001b', '\u0002', 'â', 'ã', '\a', 'D', '\u0002',
			'\u0002', 'ã', 'å', '\u0005', '4', '\u001b', '\u0002', 'ä', 'â', '\u0003',
			'\u0002', '\u0002', '\u0002', 'å', 'è', '\u0003', '\u0002', '\u0002', '\u0002', 'æ',
			'ä', '\u0003', '\u0002', '\u0002', '\u0002', 'æ', 'ç', '\u0003', '\u0002', '\u0002',
			'\u0002', 'ç', 'é', '\u0003', '\u0002', '\u0002', '\u0002', 'è', 'æ', '\u0003',
			'\u0002', '\u0002', '\u0002', 'é', 'ê', '\a', 'C', '\u0002', '\u0002', 'ê',
			'ë', '\a', '$', '\u0002', '\u0002', 'ë', ')', '\u0003', '\u0002', '\u0002',
			'\u0002', 'ì', 'í', '\a', 'I', '\u0002', '\u0002', 'í', '+', '\u0003',
			'\u0002', '\u0002', '\u0002', 'î', 'ï', '\t', '\u0002', '\u0002', '\u0002', 'ï',
			'-', '\u0003', '\u0002', '\u0002', '\u0002', 'ð', 'ò', '\u0005', ',', '\u0017',
			'\u0002', 'ñ', 'ó', '\u0005', '0', '\u0019', '\u0002', 'ò', 'ñ', '\u0003',
			'\u0002', '\u0002', '\u0002', 'ò', 'ó', '\u0003', '\u0002', '\u0002', '\u0002', 'ó',
			'/', '\u0003', '\u0002', '\u0002', '\u0002', 'ô', 'ö', '\u0005', '2', '\u001a',
			'\u0002', 'õ', 'ô', '\u0003', '\u0002', '\u0002', '\u0002', 'ö', '÷', '\u0003',
			'\u0002', '\u0002', '\u0002', '÷', 'õ', '\u0003', '\u0002', '\u0002', '\u0002', '÷',
			'ø', '\u0003', '\u0002', '\u0002', '\u0002', 'ø', '1', '\u0003', '\u0002', '\u0002',
			'\u0002', 'ù', 'ú', '\a', ' ', '\u0002', '\u0002', 'ú', '3', '\u0003',
			'\u0002', '\u0002', '\u0002', 'û', 'ü', '\b', '\u001b', '\u0001', '\u0002', 'ü',
			'ý', '\a', 'B', '\u0002', '\u0002', 'ý', 'þ', '\u0005', '4', '\u001b',
			'\u0002', 'þ', 'ÿ', '\a', 'C', '\u0002', '\u0002', 'ÿ', 'Ď', '\u0003',
			'\u0002', '\u0002', '\u0002', 'Ā', 'ā', '\a', '>', '\u0002', '\u0002', 'ā',
			'Ď', '\u0005', '4', '\u001b', '\f', 'Ă', 'ă', '\a', '7', '\u0002',
			'\u0002', 'ă', 'Ď', '\u0005', '4', '\u001b', '\v', 'Ą', 'ą', '\u0005',
			'8', '\u001d', '\u0002', 'ą', 'Ć', '\t', '\u0003', '\u0002', '\u0002', 'Ć',
			'ć', '\u0005', '4', '\u001b', '\u0006', 'ć', 'Ď', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ĉ', 'ĉ', '\u0005', '8', '\u001d', '\u0002', 'ĉ', 'Ċ', '\t',
			'\u0004', '\u0002', '\u0002', 'Ċ', 'ċ', '\u0005', '4', '\u001b', '\u0005', 'ċ',
			'Ď', '\u0003', '\u0002', '\u0002', '\u0002', 'Č', 'Ď', '\u0005', '6', '\u001c',
			'\u0002', 'č', 'û', '\u0003', '\u0002', '\u0002', '\u0002', 'č', 'Ā', '\u0003',
			'\u0002', '\u0002', '\u0002', 'č', 'Ă', '\u0003', '\u0002', '\u0002', '\u0002', 'č',
			'Ą', '\u0003', '\u0002', '\u0002', '\u0002', 'č', 'Ĉ', '\u0003', '\u0002', '\u0002',
			'\u0002', 'č', 'Č', '\u0003', '\u0002', '\u0002', '\u0002', 'Ď', 'Ġ', '\u0003',
			'\u0002', '\u0002', '\u0002', 'ď', 'Đ', '\f', '\n', '\u0002', '\u0002', 'Đ',
			'đ', '\t', '\u0005', '\u0002', '\u0002', 'đ', 'ğ', '\u0005', '4', '\u001b',
			'\v', 'Ē', 'ē', '\f', '\t', '\u0002', '\u0002', 'ē', 'Ĕ', '\t',
			'\u0006', '\u0002', '\u0002', 'Ĕ', 'ğ', '\u0005', '4', '\u001b', '\n', 'ĕ',
			'Ė', '\f', '\b', '\u0002', '\u0002', 'Ė', 'ė', '\t', '\a', '\u0002',
			'\u0002', 'ė', 'ğ', '\u0005', '4', '\u001b', '\t', 'Ę', 'ę', '\f',
			'\a', '\u0002', '\u0002', 'ę', 'Ě', '\t', '\b', '\u0002', '\u0002', 'Ě',
			'ğ', '\u0005', '4', '\u001b', '\b', 'ě', 'Ĝ', '\f', '\u0004', '\u0002',
			'\u0002', 'Ĝ', 'ĝ', '\t', '\t', '\u0002', '\u0002', 'ĝ', 'ğ', '\u0005',
			'4', '\u001b', '\u0005', 'Ğ', 'ď', '\u0003', '\u0002', '\u0002', '\u0002', 'Ğ',
			'Ē', '\u0003', '\u0002', '\u0002', '\u0002', 'Ğ', 'ĕ', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ğ', 'Ę', '\u0003', '\u0002', '\u0002', '\u0002', 'Ğ', 'ě', '\u0003',
			'\u0002', '\u0002', '\u0002', 'ğ', 'Ģ', '\u0003', '\u0002', '\u0002', '\u0002', 'Ġ',
			'Ğ', '\u0003', '\u0002', '\u0002', '\u0002', 'Ġ', 'ġ', '\u0003', '\u0002', '\u0002',
			'\u0002', 'ġ', '5', '\u0003', '\u0002', '\u0002', '\u0002', 'Ģ', 'Ġ', '\u0003',
			'\u0002', '\u0002', '\u0002', 'ģ', 'ī', '\a', 'F', '\u0002', '\u0002', 'Ĥ',
			'ī', '\a', '*', '\u0002', '\u0002', 'ĥ', 'ī', '\a', '+', '\u0002',
			'\u0002', 'Ħ', 'ī', '\u0005', '8', '\u001d', '\u0002', 'ħ', 'ī', '\a',
			'%', '\u0002', '\u0002', 'Ĩ', 'ī', '\u0005', '&', '\u0014', '\u0002', 'ĩ',
			'ī', '\a', ',', '\u0002', '\u0002', 'Ī', 'ģ', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ī', 'Ĥ', '\u0003', '\u0002', '\u0002', '\u0002', 'Ī', 'ĥ', '\u0003',
			'\u0002', '\u0002', '\u0002', 'Ī', 'Ħ', '\u0003', '\u0002', '\u0002', '\u0002', 'Ī',
			'ħ', '\u0003', '\u0002', '\u0002', '\u0002', 'Ī', 'Ĩ', '\u0003', '\u0002', '\u0002',
			'\u0002', 'Ī', 'ĩ', '\u0003', '\u0002', '\u0002', '\u0002', 'ī', '7', '\u0003',
			'\u0002', '\u0002', '\u0002', 'Ĭ', 'ĭ', '\a', 'E', '\u0002', '\u0002', 'ĭ',
			'9', '\u0003', '\u0002', '\u0002', '\u0002', ' ', '=', 'F', 'L', 'N',
			'b', 'n', 's', 'x', '{', '\u0081', '\u0085', '\u0091', '\u0095', '\u0099',
			'¡', 'ª', '±', '¹', 'Ã', 'Í', 'Ð', 'Õ', 'Û', 'æ',
			'ò', '÷', 'č', 'Ğ', 'Ġ', 'Ī'
		};
		_ATN = new ATNDeserializer().Deserialize(_serializedATN);
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++)
		{
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

	public YarnSpinnerParser(ITokenStream input)
		: this(input, Console.Out, Console.Error)
	{
	}

	public YarnSpinnerParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	[RuleVersion(0)]
	public DialogueContext dialogue()
	{
		DialogueContext dialogueContext = new DialogueContext(Context, base.State);
		EnterRule(dialogueContext, 0, 0);
		try
		{
			EnterOuterAlt(dialogueContext, 1);
			base.State = 57;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			do
			{
				base.State = 56;
				node();
				base.State = 59;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
			while (num == 2);
			base.State = 61;
			Match(-1);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (dialogueContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return dialogueContext;
	}

	[RuleVersion(0)]
	public NodeContext node()
	{
		NodeContext nodeContext = new NodeContext(Context, base.State);
		EnterRule(nodeContext, 2, 1);
		try
		{
			EnterOuterAlt(nodeContext, 1);
			base.State = 63;
			header();
			base.State = 64;
			body();
			base.State = 68;
			ErrorHandler.Sync(this);
			for (int num = base.TokenStream.LA(1); num == 8; num = base.TokenStream.LA(1))
			{
				base.State = 65;
				Match(8);
				base.State = 70;
				ErrorHandler.Sync(this);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (nodeContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return nodeContext;
	}

	[RuleVersion(0)]
	public HeaderContext header()
	{
		HeaderContext headerContext = new HeaderContext(Context, base.State);
		EnterRule(headerContext, 4, 2);
		try
		{
			EnterOuterAlt(headerContext, 1);
			base.State = 71;
			header_title();
			base.State = 76;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			while (num == 3 || num == 4)
			{
				base.State = 74;
				ErrorHandler.Sync(this);
				switch (base.TokenStream.LA(1))
				{
				case 3:
					base.State = 72;
					header_tag();
					break;
				case 4:
					base.State = 73;
					header_line();
					break;
				default:
					throw new NoViableAltException(this);
				}
				base.State = 78;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (headerContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return headerContext;
	}

	[RuleVersion(0)]
	public Header_titleContext header_title()
	{
		Header_titleContext header_titleContext = new Header_titleContext(Context, base.State);
		EnterRule(header_titleContext, 6, 3);
		try
		{
			EnterOuterAlt(header_titleContext, 1);
			base.State = 79;
			Match(2);
			base.State = 80;
			Match(11);
			base.State = 81;
			Match(8);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (header_titleContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return header_titleContext;
	}

	[RuleVersion(0)]
	public Header_tagContext header_tag()
	{
		Header_tagContext header_tagContext = new Header_tagContext(Context, base.State);
		EnterRule(header_tagContext, 8, 4);
		try
		{
			EnterOuterAlt(header_tagContext, 1);
			base.State = 83;
			Match(3);
			base.State = 84;
			Match(12);
			base.State = 85;
			Match(8);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (header_tagContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return header_tagContext;
	}

	[RuleVersion(0)]
	public Header_lineContext header_line()
	{
		Header_lineContext header_lineContext = new Header_lineContext(Context, base.State);
		EnterRule(header_lineContext, 10, 5);
		try
		{
			EnterOuterAlt(header_lineContext, 1);
			base.State = 87;
			Match(4);
			base.State = 88;
			Match(5);
			base.State = 89;
			Match(14);
			base.State = 90;
			Match(8);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (header_lineContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return header_lineContext;
	}

	[RuleVersion(0)]
	public BodyContext body()
	{
		BodyContext bodyContext = new BodyContext(Context, base.State);
		EnterRule(bodyContext, 12, 6);
		try
		{
			EnterOuterAlt(bodyContext, 1);
			base.State = 92;
			Match(1);
			base.State = 96;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			while (((num - 18) & -64) == 0 && ((1L << num - 18) & 0x20000000002B13L) != 0L)
			{
				base.State = 93;
				statement();
				base.State = 98;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
			base.State = 99;
			Match(17);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (bodyContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return bodyContext;
	}

	[RuleVersion(0)]
	public StatementContext statement()
	{
		StatementContext statementContext = new StatementContext(Context, base.State);
		EnterRule(statementContext, 14, 7);
		try
		{
			base.State = 108;
			ErrorHandler.Sync(this);
			switch (base.TokenStream.LA(1))
			{
			case 19:
				EnterOuterAlt(statementContext, 1);
				base.State = 101;
				shortcut_statement();
				break;
			case 22:
				EnterOuterAlt(statementContext, 2);
				base.State = 102;
				if_statement();
				break;
			case 26:
				EnterOuterAlt(statementContext, 3);
				base.State = 103;
				set_statement();
				break;
			case 29:
				EnterOuterAlt(statementContext, 4);
				base.State = 104;
				option_statement();
				break;
			case 27:
				EnterOuterAlt(statementContext, 5);
				base.State = 105;
				function_statement();
				break;
			case 71:
				EnterOuterAlt(statementContext, 6);
				base.State = 106;
				action_statement();
				break;
			case 18:
			case 31:
				EnterOuterAlt(statementContext, 7);
				base.State = 107;
				line_statement();
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return statementContext;
	}

	[RuleVersion(0)]
	public Shortcut_statementContext shortcut_statement()
	{
		Shortcut_statementContext shortcut_statementContext = new Shortcut_statementContext(Context, base.State);
		EnterRule(shortcut_statementContext, 16, 8);
		try
		{
			EnterOuterAlt(shortcut_statementContext, 1);
			base.State = 111;
			ErrorHandler.Sync(this);
			int num = 1;
			do
			{
				if (num == 1)
				{
					base.State = 110;
					shortcut();
					base.State = 113;
					ErrorHandler.Sync(this);
					num = Interpreter.AdaptivePredict(base.TokenStream, 6, Context);
					continue;
				}
				throw new NoViableAltException(this);
			}
			while (num != 2 && num != 0);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (shortcut_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return shortcut_statementContext;
	}

	[RuleVersion(0)]
	public ShortcutContext shortcut()
	{
		ShortcutContext shortcutContext = new ShortcutContext(Context, base.State);
		EnterRule(shortcutContext, 18, 9);
		try
		{
			EnterOuterAlt(shortcutContext, 1);
			base.State = 115;
			Match(19);
			base.State = 116;
			shortcut_text();
			base.State = 118;
			ErrorHandler.Sync(this);
			if (Interpreter.AdaptivePredict(base.TokenStream, 7, Context) == 1)
			{
				base.State = 117;
				shortcut_conditional();
			}
			base.State = 121;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			if (num == 30)
			{
				base.State = 120;
				hashtag_block();
			}
			base.State = 131;
			ErrorHandler.Sync(this);
			num = base.TokenStream.LA(1);
			if (num == 20)
			{
				base.State = 123;
				Match(20);
				base.State = 127;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
				while (((num - 18) & -64) == 0 && ((1L << num - 18) & 0x20000000002B13L) != 0L)
				{
					base.State = 124;
					statement();
					base.State = 129;
					ErrorHandler.Sync(this);
					num = base.TokenStream.LA(1);
				}
				base.State = 130;
				Match(21);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (shortcutContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return shortcutContext;
	}

	[RuleVersion(0)]
	public Shortcut_conditionalContext shortcut_conditional()
	{
		Shortcut_conditionalContext shortcut_conditionalContext = new Shortcut_conditionalContext(Context, base.State);
		EnterRule(shortcut_conditionalContext, 20, 10);
		try
		{
			EnterOuterAlt(shortcut_conditionalContext, 1);
			base.State = 133;
			Match(22);
			base.State = 134;
			expression(0);
			base.State = 135;
			Match(34);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (shortcut_conditionalContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return shortcut_conditionalContext;
	}

	[RuleVersion(0)]
	public Shortcut_textContext shortcut_text()
	{
		Shortcut_textContext shortcut_textContext = new Shortcut_textContext(Context, base.State);
		EnterRule(shortcut_textContext, 22, 11);
		try
		{
			EnterOuterAlt(shortcut_textContext, 1);
			base.State = 137;
			Match(32);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (shortcut_textContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return shortcut_textContext;
	}

	[RuleVersion(0)]
	public If_statementContext if_statement()
	{
		If_statementContext if_statementContext = new If_statementContext(Context, base.State);
		EnterRule(if_statementContext, 24, 12);
		try
		{
			EnterOuterAlt(if_statementContext, 1);
			base.State = 139;
			if_clause();
			base.State = 143;
			ErrorHandler.Sync(this);
			int num;
			for (num = base.TokenStream.LA(1); num == 24; num = base.TokenStream.LA(1))
			{
				base.State = 140;
				else_if_clause();
				base.State = 145;
				ErrorHandler.Sync(this);
			}
			base.State = 147;
			ErrorHandler.Sync(this);
			num = base.TokenStream.LA(1);
			if (num == 23)
			{
				base.State = 146;
				else_clause();
			}
			base.State = 149;
			Match(25);
			base.State = 151;
			ErrorHandler.Sync(this);
			num = base.TokenStream.LA(1);
			if (num == 30)
			{
				base.State = 150;
				hashtag_block();
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (if_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return if_statementContext;
	}

	[RuleVersion(0)]
	public If_clauseContext if_clause()
	{
		If_clauseContext if_clauseContext = new If_clauseContext(Context, base.State);
		EnterRule(if_clauseContext, 26, 13);
		try
		{
			EnterOuterAlt(if_clauseContext, 1);
			base.State = 153;
			Match(22);
			base.State = 154;
			expression(0);
			base.State = 155;
			Match(34);
			base.State = 159;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			while (((num - 18) & -64) == 0 && ((1L << num - 18) & 0x20000000002B13L) != 0L)
			{
				base.State = 156;
				statement();
				base.State = 161;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (if_clauseContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return if_clauseContext;
	}

	[RuleVersion(0)]
	public Else_if_clauseContext else_if_clause()
	{
		Else_if_clauseContext else_if_clauseContext = new Else_if_clauseContext(Context, base.State);
		EnterRule(else_if_clauseContext, 28, 14);
		try
		{
			EnterOuterAlt(else_if_clauseContext, 1);
			base.State = 162;
			Match(24);
			base.State = 163;
			expression(0);
			base.State = 164;
			Match(34);
			base.State = 168;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			while (((num - 18) & -64) == 0 && ((1L << num - 18) & 0x20000000002B13L) != 0L)
			{
				base.State = 165;
				statement();
				base.State = 170;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (else_if_clauseContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return else_if_clauseContext;
	}

	[RuleVersion(0)]
	public Else_clauseContext else_clause()
	{
		Else_clauseContext else_clauseContext = new Else_clauseContext(Context, base.State);
		EnterRule(else_clauseContext, 30, 15);
		try
		{
			EnterOuterAlt(else_clauseContext, 1);
			base.State = 171;
			Match(23);
			base.State = 175;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			while (((num - 18) & -64) == 0 && ((1L << num - 18) & 0x20000000002B13L) != 0L)
			{
				base.State = 172;
				statement();
				base.State = 177;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (else_clauseContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return else_clauseContext;
	}

	[RuleVersion(0)]
	public Set_statementContext set_statement()
	{
		Set_statementContext set_statementContext = new Set_statementContext(Context, base.State);
		EnterRule(set_statementContext, 32, 16);
		try
		{
			base.State = 193;
			ErrorHandler.Sync(this);
			switch (Interpreter.AdaptivePredict(base.TokenStream, 18, Context))
			{
			case 1:
			{
				EnterOuterAlt(set_statementContext, 1);
				base.State = 178;
				Match(26);
				base.State = 179;
				variable();
				base.State = 183;
				ErrorHandler.Sync(this);
				for (int num = base.TokenStream.LA(1); num == 43; num = base.TokenStream.LA(1))
				{
					base.State = 180;
					Match(43);
					base.State = 185;
					ErrorHandler.Sync(this);
				}
				base.State = 186;
				expression(0);
				base.State = 187;
				Match(34);
				break;
			}
			case 2:
				EnterOuterAlt(set_statementContext, 2);
				base.State = 189;
				Match(26);
				base.State = 190;
				expression(0);
				base.State = 191;
				Match(34);
				break;
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (set_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return set_statementContext;
	}

	[RuleVersion(0)]
	public Option_statementContext option_statement()
	{
		Option_statementContext option_statementContext = new Option_statementContext(Context, base.State);
		EnterRule(option_statementContext, 34, 17);
		try
		{
			EnterOuterAlt(option_statementContext, 1);
			base.State = 203;
			ErrorHandler.Sync(this);
			switch (Interpreter.AdaptivePredict(base.TokenStream, 19, Context))
			{
			case 1:
				base.State = 195;
				Match(29);
				base.State = 196;
				Match(73);
				base.State = 197;
				Match(72);
				base.State = 198;
				Match(75);
				base.State = 199;
				Match(74);
				break;
			case 2:
				base.State = 200;
				Match(29);
				base.State = 201;
				Match(73);
				base.State = 202;
				Match(74);
				break;
			}
			base.State = 206;
			ErrorHandler.Sync(this);
			if (base.TokenStream.LA(1) == 30)
			{
				base.State = 205;
				hashtag_block();
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (option_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return option_statementContext;
	}

	[RuleVersion(0)]
	public FunctionContext function()
	{
		FunctionContext functionContext = new FunctionContext(Context, base.State);
		EnterRule(functionContext, 36, 18);
		try
		{
			EnterOuterAlt(functionContext, 1);
			base.State = 208;
			Match(69);
			base.State = 209;
			Match(64);
			base.State = 211;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			if (((num - 35) & -64) == 0 && ((1L << num - 35) & 0x7220400E1L) != 0L)
			{
				base.State = 210;
				expression(0);
			}
			base.State = 217;
			ErrorHandler.Sync(this);
			for (num = base.TokenStream.LA(1); num == 66; num = base.TokenStream.LA(1))
			{
				base.State = 213;
				Match(66);
				base.State = 214;
				expression(0);
				base.State = 219;
				ErrorHandler.Sync(this);
			}
			base.State = 220;
			Match(65);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (functionContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return functionContext;
	}

	[RuleVersion(0)]
	public Function_statementContext function_statement()
	{
		Function_statementContext function_statementContext = new Function_statementContext(Context, base.State);
		EnterRule(function_statementContext, 38, 19);
		try
		{
			EnterOuterAlt(function_statementContext, 1);
			base.State = 222;
			Match(27);
			base.State = 223;
			expression(0);
			base.State = 228;
			ErrorHandler.Sync(this);
			for (int num = base.TokenStream.LA(1); num == 66; num = base.TokenStream.LA(1))
			{
				base.State = 224;
				Match(66);
				base.State = 225;
				expression(0);
				base.State = 230;
				ErrorHandler.Sync(this);
			}
			base.State = 231;
			Match(65);
			base.State = 232;
			Match(34);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (function_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return function_statementContext;
	}

	[RuleVersion(0)]
	public Action_statementContext action_statement()
	{
		Action_statementContext action_statementContext = new Action_statementContext(Context, base.State);
		EnterRule(action_statementContext, 40, 20);
		try
		{
			EnterOuterAlt(action_statementContext, 1);
			base.State = 234;
			Match(71);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (action_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return action_statementContext;
	}

	[RuleVersion(0)]
	public TextContext text()
	{
		TextContext textContext = new TextContext(Context, base.State);
		EnterRule(textContext, 42, 21);
		try
		{
			EnterOuterAlt(textContext, 1);
			base.State = 236;
			int num = base.TokenStream.LA(1);
			if (num != 18 && num != 31)
			{
				ErrorHandler.RecoverInline(this);
			}
			else
			{
				ErrorHandler.ReportMatch(this);
				Consume();
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (textContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return textContext;
	}

	[RuleVersion(0)]
	public Line_statementContext line_statement()
	{
		Line_statementContext line_statementContext = new Line_statementContext(Context, base.State);
		EnterRule(line_statementContext, 44, 22);
		try
		{
			EnterOuterAlt(line_statementContext, 1);
			base.State = 238;
			text();
			base.State = 240;
			ErrorHandler.Sync(this);
			if (base.TokenStream.LA(1) == 30)
			{
				base.State = 239;
				hashtag_block();
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (line_statementContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return line_statementContext;
	}

	[RuleVersion(0)]
	public Hashtag_blockContext hashtag_block()
	{
		Hashtag_blockContext hashtag_blockContext = new Hashtag_blockContext(Context, base.State);
		EnterRule(hashtag_blockContext, 46, 23);
		try
		{
			EnterOuterAlt(hashtag_blockContext, 1);
			base.State = 243;
			ErrorHandler.Sync(this);
			int num = base.TokenStream.LA(1);
			do
			{
				base.State = 242;
				hashtag();
				base.State = 245;
				ErrorHandler.Sync(this);
				num = base.TokenStream.LA(1);
			}
			while (num == 30);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (hashtag_blockContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return hashtag_blockContext;
	}

	[RuleVersion(0)]
	public HashtagContext hashtag()
	{
		HashtagContext hashtagContext = new HashtagContext(Context, base.State);
		EnterRule(hashtagContext, 48, 24);
		try
		{
			EnterOuterAlt(hashtagContext, 1);
			base.State = 247;
			Match(30);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (hashtagContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return hashtagContext;
	}

	[RuleVersion(0)]
	public ExpressionContext expression()
	{
		return expression(0);
	}

	private ExpressionContext expression(int _p)
	{
		ParserRuleContext context = Context;
		int state = base.State;
		ExpressionContext expressionContext = new ExpressionContext(Context, state);
		int state2 = 50;
		EnterRecursionRule(expressionContext, 50, 25, _p);
		try
		{
			EnterOuterAlt(expressionContext, 1);
			base.State = 267;
			ErrorHandler.Sync(this);
			switch (Interpreter.AdaptivePredict(base.TokenStream, 26, Context))
			{
			case 1:
				expressionContext = (ExpressionContext)(Context = new ExpParensContext(expressionContext));
				base.State = 250;
				Match(64);
				base.State = 251;
				expression(0);
				base.State = 252;
				Match(65);
				break;
			case 2:
				expressionContext = (ExpressionContext)(Context = new ExpNegativeContext(expressionContext));
				base.State = 254;
				Match(60);
				base.State = 255;
				expression(10);
				break;
			case 3:
				expressionContext = (ExpressionContext)(Context = new ExpNotContext(expressionContext));
				base.State = 256;
				Match(53);
				base.State = 257;
				expression(9);
				break;
			case 4:
			{
				expressionContext = (ExpressionContext)(Context = new ExpMultDivModEqualsContext(expressionContext));
				base.State = 258;
				variable();
				base.State = 259;
				((ExpMultDivModEqualsContext)expressionContext).op = base.TokenStream.LT(1);
				int num = base.TokenStream.LA(1);
				if ((num & -64) != 0 || ((1L << num) & 0x700000000000000L) == 0L)
				{
					((ExpMultDivModEqualsContext)expressionContext).op = ErrorHandler.RecoverInline(this);
				}
				else
				{
					ErrorHandler.ReportMatch(this);
					Consume();
				}
				base.State = 260;
				expression(4);
				break;
			}
			case 5:
			{
				expressionContext = (ExpressionContext)(Context = new ExpPlusMinusEqualsContext(expressionContext));
				base.State = 262;
				variable();
				base.State = 263;
				((ExpPlusMinusEqualsContext)expressionContext).op = base.TokenStream.LT(1);
				int num = base.TokenStream.LA(1);
				if (num != 54 && num != 55)
				{
					((ExpPlusMinusEqualsContext)expressionContext).op = ErrorHandler.RecoverInline(this);
				}
				else
				{
					ErrorHandler.ReportMatch(this);
					Consume();
				}
				base.State = 264;
				expression(3);
				break;
			}
			case 6:
				expressionContext = (ExpressionContext)(Context = new ExpValueContext(expressionContext));
				base.State = 266;
				value();
				break;
			}
			Context.Stop = base.TokenStream.LT(-1);
			base.State = 286;
			ErrorHandler.Sync(this);
			int num2 = Interpreter.AdaptivePredict(base.TokenStream, 28, Context);
			while (true)
			{
				switch (num2)
				{
				case 1:
					if (ParseListeners != null)
					{
						TriggerExitRuleEvent();
					}
					base.State = 284;
					ErrorHandler.Sync(this);
					switch (Interpreter.AdaptivePredict(base.TokenStream, 27, Context))
					{
					case 1:
					{
						expressionContext = new ExpMultDivModContext(new ExpressionContext(context, state));
						PushNewRecursionContext(expressionContext, state2, 25);
						base.State = 269;
						if (!Precpred(Context, 8))
						{
							throw new FailedPredicateException(this, "Precpred(Context, 8)");
						}
						base.State = 270;
						((ExpMultDivModContext)expressionContext).op = base.TokenStream.LT(1);
						int num = base.TokenStream.LA(1);
						if ((num & -64) != 0 || ((1L << num) & -2305843009213693952L) == 0L)
						{
							((ExpMultDivModContext)expressionContext).op = ErrorHandler.RecoverInline(this);
						}
						else
						{
							ErrorHandler.ReportMatch(this);
							Consume();
						}
						base.State = 271;
						expression(9);
						break;
					}
					case 2:
					{
						expressionContext = new ExpAddSubContext(new ExpressionContext(context, state));
						PushNewRecursionContext(expressionContext, state2, 25);
						base.State = 272;
						if (!Precpred(Context, 7))
						{
							throw new FailedPredicateException(this, "Precpred(Context, 7)");
						}
						base.State = 273;
						((ExpAddSubContext)expressionContext).op = base.TokenStream.LT(1);
						int num = base.TokenStream.LA(1);
						if (num != 59 && num != 60)
						{
							((ExpAddSubContext)expressionContext).op = ErrorHandler.RecoverInline(this);
						}
						else
						{
							ErrorHandler.ReportMatch(this);
							Consume();
						}
						base.State = 274;
						expression(8);
						break;
					}
					case 3:
					{
						expressionContext = new ExpComparisonContext(new ExpressionContext(context, state));
						PushNewRecursionContext(expressionContext, state2, 25);
						base.State = 275;
						if (!Precpred(Context, 6))
						{
							throw new FailedPredicateException(this, "Precpred(Context, 6)");
						}
						base.State = 276;
						((ExpComparisonContext)expressionContext).op = base.TokenStream.LT(1);
						int num = base.TokenStream.LA(1);
						if ((num & -64) != 0 || ((1L << num) & 0x1B00000000000L) == 0L)
						{
							((ExpComparisonContext)expressionContext).op = ErrorHandler.RecoverInline(this);
						}
						else
						{
							ErrorHandler.ReportMatch(this);
							Consume();
						}
						base.State = 277;
						expression(7);
						break;
					}
					case 4:
					{
						expressionContext = new ExpEqualityContext(new ExpressionContext(context, state));
						PushNewRecursionContext(expressionContext, state2, 25);
						base.State = 278;
						if (!Precpred(Context, 5))
						{
							throw new FailedPredicateException(this, "Precpred(Context, 5)");
						}
						base.State = 279;
						((ExpEqualityContext)expressionContext).op = base.TokenStream.LT(1);
						int num = base.TokenStream.LA(1);
						if (num != 46 && num != 49)
						{
							((ExpEqualityContext)expressionContext).op = ErrorHandler.RecoverInline(this);
						}
						else
						{
							ErrorHandler.ReportMatch(this);
							Consume();
						}
						base.State = 280;
						expression(6);
						break;
					}
					case 5:
					{
						expressionContext = new ExpAndOrXorContext(new ExpressionContext(context, state));
						PushNewRecursionContext(expressionContext, state2, 25);
						base.State = 281;
						if (!Precpred(Context, 2))
						{
							throw new FailedPredicateException(this, "Precpred(Context, 2)");
						}
						base.State = 282;
						((ExpAndOrXorContext)expressionContext).op = base.TokenStream.LT(1);
						int num = base.TokenStream.LA(1);
						if ((num & -64) != 0 || ((1L << num) & 0x1C000000000000L) == 0L)
						{
							((ExpAndOrXorContext)expressionContext).op = ErrorHandler.RecoverInline(this);
						}
						else
						{
							ErrorHandler.ReportMatch(this);
							Consume();
						}
						base.State = 283;
						expression(3);
						break;
					}
					}
					break;
				case 0:
				case 2:
					goto end_IL_077e;
				}
				base.State = 288;
				ErrorHandler.Sync(this);
				num2 = Interpreter.AdaptivePredict(base.TokenStream, 28, Context);
				continue;
				end_IL_077e:
				break;
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (expressionContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			UnrollRecursionContexts(context);
		}
		return expressionContext;
	}

	[RuleVersion(0)]
	public ValueContext value()
	{
		ValueContext valueContext = new ValueContext(Context, base.State);
		EnterRule(valueContext, 52, 26);
		try
		{
			base.State = 296;
			ErrorHandler.Sync(this);
			switch (base.TokenStream.LA(1))
			{
			case 68:
				valueContext = new ValueNumberContext(valueContext);
				EnterOuterAlt(valueContext, 1);
				base.State = 289;
				Match(68);
				break;
			case 40:
				valueContext = new ValueTrueContext(valueContext);
				EnterOuterAlt(valueContext, 2);
				base.State = 290;
				Match(40);
				break;
			case 41:
				valueContext = new ValueFalseContext(valueContext);
				EnterOuterAlt(valueContext, 3);
				base.State = 291;
				Match(41);
				break;
			case 67:
				valueContext = new ValueVarContext(valueContext);
				EnterOuterAlt(valueContext, 4);
				base.State = 292;
				variable();
				break;
			case 35:
				valueContext = new ValueStringContext(valueContext);
				EnterOuterAlt(valueContext, 5);
				base.State = 293;
				Match(35);
				break;
			case 69:
				valueContext = new ValueFuncContext(valueContext);
				EnterOuterAlt(valueContext, 6);
				base.State = 294;
				function();
				break;
			case 42:
				valueContext = new ValueNullContext(valueContext);
				EnterOuterAlt(valueContext, 7);
				base.State = 295;
				Match(42);
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (valueContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return valueContext;
	}

	[RuleVersion(0)]
	public VariableContext variable()
	{
		VariableContext variableContext = new VariableContext(Context, base.State);
		EnterRule(variableContext, 54, 27);
		try
		{
			EnterOuterAlt(variableContext, 1);
			base.State = 298;
			Match(67);
		}
		catch (RecognitionException exception)
		{
			RecognitionException e = (variableContext.exception = exception);
			ErrorHandler.ReportError(this, e);
			ErrorHandler.Recover(this, e);
		}
		finally
		{
			ExitRule();
		}
		return variableContext;
	}

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex)
	{
		if (ruleIndex == 25)
		{
			return expression_sempred((ExpressionContext)_localctx, predIndex);
		}
		return true;
	}

	private bool expression_sempred(ExpressionContext _localctx, int predIndex)
	{
		return predIndex switch
		{
			0 => Precpred(Context, 8), 
			1 => Precpred(Context, 7), 
			2 => Precpred(Context, 6), 
			3 => Precpred(Context, 5), 
			4 => Precpred(Context, 2), 
			_ => true, 
		};
	}
}
