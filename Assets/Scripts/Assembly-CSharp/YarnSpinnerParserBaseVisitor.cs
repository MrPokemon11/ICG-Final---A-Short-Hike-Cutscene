using System;
using System.CodeDom.Compiler;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

[GeneratedCode("ANTLR", "4.7")]
[CLSCompliant(false)]
public class YarnSpinnerParserBaseVisitor<Result> : AbstractParseTreeVisitor<Result>, IYarnSpinnerParserVisitor<Result>, IParseTreeVisitor<Result>
{
	public virtual Result VisitDialogue([NotNull] YarnSpinnerParser.DialogueContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitNode([NotNull] YarnSpinnerParser.NodeContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitHeader([NotNull] YarnSpinnerParser.HeaderContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitHeader_title([NotNull] YarnSpinnerParser.Header_titleContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitHeader_tag([NotNull] YarnSpinnerParser.Header_tagContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitHeader_line([NotNull] YarnSpinnerParser.Header_lineContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitBody([NotNull] YarnSpinnerParser.BodyContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitStatement([NotNull] YarnSpinnerParser.StatementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitShortcut_statement([NotNull] YarnSpinnerParser.Shortcut_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitShortcut([NotNull] YarnSpinnerParser.ShortcutContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitShortcut_conditional([NotNull] YarnSpinnerParser.Shortcut_conditionalContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitShortcut_text([NotNull] YarnSpinnerParser.Shortcut_textContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitIf_statement([NotNull] YarnSpinnerParser.If_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitIf_clause([NotNull] YarnSpinnerParser.If_clauseContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitElse_if_clause([NotNull] YarnSpinnerParser.Else_if_clauseContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitElse_clause([NotNull] YarnSpinnerParser.Else_clauseContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitSet_statement([NotNull] YarnSpinnerParser.Set_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitOption_statement([NotNull] YarnSpinnerParser.Option_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitFunction([NotNull] YarnSpinnerParser.FunctionContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitFunction_statement([NotNull] YarnSpinnerParser.Function_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitAction_statement([NotNull] YarnSpinnerParser.Action_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitText([NotNull] YarnSpinnerParser.TextContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitLine_statement([NotNull] YarnSpinnerParser.Line_statementContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitHashtag_block([NotNull] YarnSpinnerParser.Hashtag_blockContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitHashtag([NotNull] YarnSpinnerParser.HashtagContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpParens([NotNull] YarnSpinnerParser.ExpParensContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpMultDivMod([NotNull] YarnSpinnerParser.ExpMultDivModContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpMultDivModEquals([NotNull] YarnSpinnerParser.ExpMultDivModEqualsContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpComparison([NotNull] YarnSpinnerParser.ExpComparisonContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpNegative([NotNull] YarnSpinnerParser.ExpNegativeContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpAndOrXor([NotNull] YarnSpinnerParser.ExpAndOrXorContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpPlusMinusEquals([NotNull] YarnSpinnerParser.ExpPlusMinusEqualsContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpAddSub([NotNull] YarnSpinnerParser.ExpAddSubContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpNot([NotNull] YarnSpinnerParser.ExpNotContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpValue([NotNull] YarnSpinnerParser.ExpValueContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitExpEquality([NotNull] YarnSpinnerParser.ExpEqualityContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueNumber([NotNull] YarnSpinnerParser.ValueNumberContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueTrue([NotNull] YarnSpinnerParser.ValueTrueContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueFalse([NotNull] YarnSpinnerParser.ValueFalseContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueVar([NotNull] YarnSpinnerParser.ValueVarContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueString([NotNull] YarnSpinnerParser.ValueStringContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueFunc([NotNull] YarnSpinnerParser.ValueFuncContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitValueNull([NotNull] YarnSpinnerParser.ValueNullContext context)
	{
		return VisitChildren(context);
	}

	public virtual Result VisitVariable([NotNull] YarnSpinnerParser.VariableContext context)
	{
		return VisitChildren(context);
	}
}
