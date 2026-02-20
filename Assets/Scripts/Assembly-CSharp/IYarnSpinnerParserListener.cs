using System;
using System.CodeDom.Compiler;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

[GeneratedCode("ANTLR", "4.7")]
[CLSCompliant(false)]
public interface IYarnSpinnerParserListener : IParseTreeListener
{
	void EnterDialogue([NotNull] YarnSpinnerParser.DialogueContext context);

	void ExitDialogue([NotNull] YarnSpinnerParser.DialogueContext context);

	void EnterNode([NotNull] YarnSpinnerParser.NodeContext context);

	void ExitNode([NotNull] YarnSpinnerParser.NodeContext context);

	void EnterHeader([NotNull] YarnSpinnerParser.HeaderContext context);

	void ExitHeader([NotNull] YarnSpinnerParser.HeaderContext context);

	void EnterHeader_title([NotNull] YarnSpinnerParser.Header_titleContext context);

	void ExitHeader_title([NotNull] YarnSpinnerParser.Header_titleContext context);

	void EnterHeader_tag([NotNull] YarnSpinnerParser.Header_tagContext context);

	void ExitHeader_tag([NotNull] YarnSpinnerParser.Header_tagContext context);

	void EnterHeader_line([NotNull] YarnSpinnerParser.Header_lineContext context);

	void ExitHeader_line([NotNull] YarnSpinnerParser.Header_lineContext context);

	void EnterBody([NotNull] YarnSpinnerParser.BodyContext context);

	void ExitBody([NotNull] YarnSpinnerParser.BodyContext context);

	void EnterStatement([NotNull] YarnSpinnerParser.StatementContext context);

	void ExitStatement([NotNull] YarnSpinnerParser.StatementContext context);

	void EnterShortcut_statement([NotNull] YarnSpinnerParser.Shortcut_statementContext context);

	void ExitShortcut_statement([NotNull] YarnSpinnerParser.Shortcut_statementContext context);

	void EnterShortcut([NotNull] YarnSpinnerParser.ShortcutContext context);

	void ExitShortcut([NotNull] YarnSpinnerParser.ShortcutContext context);

	void EnterShortcut_conditional([NotNull] YarnSpinnerParser.Shortcut_conditionalContext context);

	void ExitShortcut_conditional([NotNull] YarnSpinnerParser.Shortcut_conditionalContext context);

	void EnterShortcut_text([NotNull] YarnSpinnerParser.Shortcut_textContext context);

	void ExitShortcut_text([NotNull] YarnSpinnerParser.Shortcut_textContext context);

	void EnterIf_statement([NotNull] YarnSpinnerParser.If_statementContext context);

	void ExitIf_statement([NotNull] YarnSpinnerParser.If_statementContext context);

	void EnterIf_clause([NotNull] YarnSpinnerParser.If_clauseContext context);

	void ExitIf_clause([NotNull] YarnSpinnerParser.If_clauseContext context);

	void EnterElse_if_clause([NotNull] YarnSpinnerParser.Else_if_clauseContext context);

	void ExitElse_if_clause([NotNull] YarnSpinnerParser.Else_if_clauseContext context);

	void EnterElse_clause([NotNull] YarnSpinnerParser.Else_clauseContext context);

	void ExitElse_clause([NotNull] YarnSpinnerParser.Else_clauseContext context);

	void EnterSet_statement([NotNull] YarnSpinnerParser.Set_statementContext context);

	void ExitSet_statement([NotNull] YarnSpinnerParser.Set_statementContext context);

	void EnterOption_statement([NotNull] YarnSpinnerParser.Option_statementContext context);

	void ExitOption_statement([NotNull] YarnSpinnerParser.Option_statementContext context);

	void EnterFunction([NotNull] YarnSpinnerParser.FunctionContext context);

	void ExitFunction([NotNull] YarnSpinnerParser.FunctionContext context);

	void EnterFunction_statement([NotNull] YarnSpinnerParser.Function_statementContext context);

	void ExitFunction_statement([NotNull] YarnSpinnerParser.Function_statementContext context);

	void EnterAction_statement([NotNull] YarnSpinnerParser.Action_statementContext context);

	void ExitAction_statement([NotNull] YarnSpinnerParser.Action_statementContext context);

	void EnterText([NotNull] YarnSpinnerParser.TextContext context);

	void ExitText([NotNull] YarnSpinnerParser.TextContext context);

	void EnterLine_statement([NotNull] YarnSpinnerParser.Line_statementContext context);

	void ExitLine_statement([NotNull] YarnSpinnerParser.Line_statementContext context);

	void EnterHashtag_block([NotNull] YarnSpinnerParser.Hashtag_blockContext context);

	void ExitHashtag_block([NotNull] YarnSpinnerParser.Hashtag_blockContext context);

	void EnterHashtag([NotNull] YarnSpinnerParser.HashtagContext context);

	void ExitHashtag([NotNull] YarnSpinnerParser.HashtagContext context);

	void EnterExpParens([NotNull] YarnSpinnerParser.ExpParensContext context);

	void ExitExpParens([NotNull] YarnSpinnerParser.ExpParensContext context);

	void EnterExpMultDivMod([NotNull] YarnSpinnerParser.ExpMultDivModContext context);

	void ExitExpMultDivMod([NotNull] YarnSpinnerParser.ExpMultDivModContext context);

	void EnterExpMultDivModEquals([NotNull] YarnSpinnerParser.ExpMultDivModEqualsContext context);

	void ExitExpMultDivModEquals([NotNull] YarnSpinnerParser.ExpMultDivModEqualsContext context);

	void EnterExpComparison([NotNull] YarnSpinnerParser.ExpComparisonContext context);

	void ExitExpComparison([NotNull] YarnSpinnerParser.ExpComparisonContext context);

	void EnterExpNegative([NotNull] YarnSpinnerParser.ExpNegativeContext context);

	void ExitExpNegative([NotNull] YarnSpinnerParser.ExpNegativeContext context);

	void EnterExpAndOrXor([NotNull] YarnSpinnerParser.ExpAndOrXorContext context);

	void ExitExpAndOrXor([NotNull] YarnSpinnerParser.ExpAndOrXorContext context);

	void EnterExpPlusMinusEquals([NotNull] YarnSpinnerParser.ExpPlusMinusEqualsContext context);

	void ExitExpPlusMinusEquals([NotNull] YarnSpinnerParser.ExpPlusMinusEqualsContext context);

	void EnterExpAddSub([NotNull] YarnSpinnerParser.ExpAddSubContext context);

	void ExitExpAddSub([NotNull] YarnSpinnerParser.ExpAddSubContext context);

	void EnterExpNot([NotNull] YarnSpinnerParser.ExpNotContext context);

	void ExitExpNot([NotNull] YarnSpinnerParser.ExpNotContext context);

	void EnterExpValue([NotNull] YarnSpinnerParser.ExpValueContext context);

	void ExitExpValue([NotNull] YarnSpinnerParser.ExpValueContext context);

	void EnterExpEquality([NotNull] YarnSpinnerParser.ExpEqualityContext context);

	void ExitExpEquality([NotNull] YarnSpinnerParser.ExpEqualityContext context);

	void EnterValueNumber([NotNull] YarnSpinnerParser.ValueNumberContext context);

	void ExitValueNumber([NotNull] YarnSpinnerParser.ValueNumberContext context);

	void EnterValueTrue([NotNull] YarnSpinnerParser.ValueTrueContext context);

	void ExitValueTrue([NotNull] YarnSpinnerParser.ValueTrueContext context);

	void EnterValueFalse([NotNull] YarnSpinnerParser.ValueFalseContext context);

	void ExitValueFalse([NotNull] YarnSpinnerParser.ValueFalseContext context);

	void EnterValueVar([NotNull] YarnSpinnerParser.ValueVarContext context);

	void ExitValueVar([NotNull] YarnSpinnerParser.ValueVarContext context);

	void EnterValueString([NotNull] YarnSpinnerParser.ValueStringContext context);

	void ExitValueString([NotNull] YarnSpinnerParser.ValueStringContext context);

	void EnterValueFunc([NotNull] YarnSpinnerParser.ValueFuncContext context);

	void ExitValueFunc([NotNull] YarnSpinnerParser.ValueFuncContext context);

	void EnterValueNull([NotNull] YarnSpinnerParser.ValueNullContext context);

	void ExitValueNull([NotNull] YarnSpinnerParser.ValueNullContext context);

	void EnterVariable([NotNull] YarnSpinnerParser.VariableContext context);

	void ExitVariable([NotNull] YarnSpinnerParser.VariableContext context);
}
