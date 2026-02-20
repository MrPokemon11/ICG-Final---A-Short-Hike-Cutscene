using Antlr4.Runtime.Tree;

namespace Yarn
{
	public class GraphListener : YarnSpinnerParserBaseListener
	{
		private string currentNode;

		public Graph graph = new Graph();

		private string yarnName = "G";

		public override void EnterHeader_title(YarnSpinnerParser.Header_titleContext context)
		{
			currentNode = context.HEADER_TITLE().GetText();
			graph.nodes.Add(currentNode);
		}

		public override void ExitOption_statement(YarnSpinnerParser.Option_statementContext context)
		{
			ITerminalNode terminalNode = context.OPTION_LINK();
			if (terminalNode != null)
			{
				graph.edge(currentNode, terminalNode.GetText());
			}
			else
			{
				graph.edge(currentNode, context.OPTION_TEXT().GetText());
			}
		}
	}
}
