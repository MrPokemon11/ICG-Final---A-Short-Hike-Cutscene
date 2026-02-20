using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Yarn
{
	public class AntlrCompiler : YarnSpinnerParserBaseListener
	{
		internal struct CompileFlags
		{
			public bool DisableShuffleOptionsAfterNextSet;
		}

		internal CompileFlags flags;

		private int labelCount;

		internal Node currentNode;

		internal Library library;

		internal bool rawTextNode;

		internal Program program { get; private set; }

		internal AntlrCompiler(Library library)
		{
			program = new Program();
			this.library = library;
		}

		internal string RegisterLabel(string commentary = null)
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

		internal void Emit(ByteCode code, object operandA = null, object operandB = null)
		{
			Emit(currentNode, code, operandA, operandB);
		}

		internal string GetLineID(YarnSpinnerParser.Hashtag_blockContext context)
		{
			if (context != null)
			{
				YarnSpinnerParser.HashtagContext[] array = context.hashtag();
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].GetText().Trim('#');
					if (text.StartsWith("line:"))
					{
						return text;
					}
				}
			}
			return null;
		}

		internal void Compile(IParseTree tree)
		{
			new ParseTreeWalker().Walk(this, tree);
		}

		public override void EnterNode(YarnSpinnerParser.NodeContext context)
		{
			if (currentNode != null)
			{
				string arg = context.header().header_title().TITLE_TEXT()
					.GetText()
					.Trim();
				throw new ParseException($"Discovered a new node {arg} while {currentNode.name} is still being parsed");
			}
			currentNode = new Node();
			rawTextNode = false;
		}

		public override void ExitNode(YarnSpinnerParser.NodeContext context)
		{
			program.nodes[currentNode.name] = currentNode;
			currentNode = null;
			rawTextNode = false;
		}

		public override void EnterHeader(YarnSpinnerParser.HeaderContext context)
		{
			if (context.header_tag().Length > 1)
			{
				throw new ParseException($"Too many header tags defined inside {context.header_title().TITLE_TEXT().GetText().Trim()}");
			}
		}

		public override void EnterHeader_title(YarnSpinnerParser.Header_titleContext context)
		{
			currentNode.name = context.TITLE_TEXT().GetText().Trim();
		}

		public override void EnterHeader_tag(YarnSpinnerParser.Header_tagContext context)
		{
			List<string> list = new List<string>(context.TAG_TEXT().GetText().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
			if (list.Contains("rawText"))
			{
				rawTextNode = true;
			}
			currentNode.tags = list;
		}

		public override void ExitHeader(YarnSpinnerParser.HeaderContext context)
		{
			if (!rawTextNode)
			{
				Emit(currentNode, ByteCode.Label, RegisterLabel());
			}
		}

		public override void EnterBody(YarnSpinnerParser.BodyContext context)
		{
			if (!rawTextNode)
			{
				BodyVisitor bodyVisitor = new BodyVisitor(this);
				YarnSpinnerParser.StatementContext[] array = context.statement();
				foreach (YarnSpinnerParser.StatementContext tree in array)
				{
					bodyVisitor.Visit(tree);
				}
			}
			else
			{
				int a = context.Start.StartIndex + 4;
				int b = context.Stop.StopIndex - 4;
				string text = context.Start.InputStream.GetText(new Interval(a, b));
				currentNode.sourceTextStringID = program.RegisterString(text, currentNode.name, "line:" + currentNode.name, context.Start.Line, localisable: true);
			}
		}

		public override void ExitBody(YarnSpinnerParser.BodyContext context)
		{
			if (rawTextNode)
			{
				return;
			}
			bool flag = false;
			foreach (Instruction instruction in currentNode.instructions)
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
				Emit(currentNode, ByteCode.Stop);
				return;
			}
			Emit(currentNode, ByteCode.ShowOptions);
			if (flags.DisableShuffleOptionsAfterNextSet)
			{
				Emit(currentNode, ByteCode.PushBool, false);
				Emit(currentNode, ByteCode.StoreVariable, "$Yarn.ShuffleOptions");
				Emit(currentNode, ByteCode.Pop);
				flags.DisableShuffleOptionsAfterNextSet = false;
			}
			Emit(currentNode, ByteCode.RunNode);
		}
	}
}
