using System;
using System.CodeDom.Compiler;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

[GeneratedCode("ANTLR", "4.7")]
[CLSCompliant(false)]
public interface IYarnSpinnerParserVisitor<Result> : IParseTreeVisitor<Result>
{
	Result VisitDialogue([NotNull] YarnSpinnerParser.DialogueContext context);

	Result VisitNode([NotNull] YarnSpinnerParser.NodeContext context);

	Result VisitHeader([NotNull] YarnSpinnerParser.HeaderContext context);

	Result VisitHeader_title([NotNull] YarnSpinnerParser.Header_titleContext context);

	Result VisitHeader_tag([NotNull] YarnSpinnerParser.Header_tagContext context);

	Result VisitHeader_line([NotNull] YarnSpinnerParser.Header_lineContext context);

	Result VisitBody([NotNull] YarnSpinnerParser.BodyContext context);

	Result VisitStatement([NotNull] YarnSpinnerParser.StatementContext context);

	Result VisitShortcut_statement([NotNull] YarnSpinnerParser.Shortcut_statementContext context);

	Result VisitShortcut([NotNull] YarnSpinnerParser.ShortcutContext context);

	Result VisitShortcut_conditional([NotNull] YarnSpinnerParser.Shortcut_conditionalContext context);

	Result VisitShortcut_text([NotNull] YarnSpinnerParser.Shortcut_textContext context);

	Result VisitIf_statement([NotNull] YarnSpinnerParser.If_statementContext context);

	Result VisitIf_clause([NotNull] YarnSpinnerParser.If_clauseContext context);

	Result VisitElse_if_clause([NotNull] YarnSpinnerParser.Else_if_clauseContext context);

	Result VisitElse_clause([NotNull] YarnSpinnerParser.Else_clauseContext context);

	Result VisitSet_statement([NotNull] YarnSpinnerParser.Set_statementContext context);

	Result VisitOption_statement([NotNull] YarnSpinnerParser.Option_statementContext context);

	Result VisitFunction([NotNull] YarnSpinnerParser.FunctionContext context);

	Result VisitFunction_statement([NotNull] YarnSpinnerParser.Function_statementContext context);

	Result VisitAction_statement([NotNull] YarnSpinnerParser.Action_statementContext context);

	Result VisitText([NotNull] YarnSpinnerParser.TextContext context);

	Result VisitLine_statement([NotNull] YarnSpinnerParser.Line_statementContext context);

	Result VisitHashtag_block([NotNull] YarnSpinnerParser.Hashtag_blockContext context);

	Result VisitHashtag([NotNull] YarnSpinnerParser.HashtagContext context);

	Result VisitExpParens([NotNull] YarnSpinnerParser.ExpParensContext context);

	Result VisitExpMultDivMod([NotNull] YarnSpinnerParser.ExpMultDivModContext context);

	Result VisitExpMultDivModEquals([NotNull] YarnSpinnerParser.ExpMultDivModEqualsContext context);

	Result VisitExpComparison([NotNull] YarnSpinnerParser.ExpComparisonContext context);

	Result VisitExpNegative([NotNull] YarnSpinnerParser.ExpNegativeContext context);

	Result VisitExpAndOrXor([NotNull] YarnSpinnerParser.ExpAndOrXorContext context);

	Result VisitExpPlusMinusEquals([NotNull] YarnSpinnerParser.ExpPlusMinusEqualsContext context);

	Result VisitExpAddSub([NotNull] YarnSpinnerParser.ExpAddSubContext context);

	Result VisitExpNot([NotNull] YarnSpinnerParser.ExpNotContext context);

	Result VisitExpValue([NotNull] YarnSpinnerParser.ExpValueContext context);

	Result VisitExpEquality([NotNull] YarnSpinnerParser.ExpEqualityContext context);

	Result VisitValueNumber([NotNull] YarnSpinnerParser.ValueNumberContext context);

	Result VisitValueTrue([NotNull] YarnSpinnerParser.ValueTrueContext context);

	Result VisitValueFalse([NotNull] YarnSpinnerParser.ValueFalseContext context);

	Result VisitValueVar([NotNull] YarnSpinnerParser.ValueVarContext context);

	Result VisitValueString([NotNull] YarnSpinnerParser.ValueStringContext context);

	Result VisitValueFunc([NotNull] YarnSpinnerParser.ValueFuncContext context);

	Result VisitValueNull([NotNull] YarnSpinnerParser.ValueNullContext context);

	Result VisitVariable([NotNull] YarnSpinnerParser.VariableContext context);
}
