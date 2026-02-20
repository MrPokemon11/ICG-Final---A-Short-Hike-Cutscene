using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;

namespace Yarn
{
	internal class Loader
	{
		private struct EmissionTuple
		{
			public int depth;

			public bool emitted;

			public EmissionTuple(int depth, bool emitted)
			{
				this.depth = depth;
				this.emitted = emitted;
			}
		}

		[JsonObject(MemberSerialization.OptOut)]
		public struct NodeInfo
		{
			public struct Position
			{
				public int x { get; set; }

				public int y { get; set; }
			}

			public string title { get; set; }

			public string body { get; set; }

			public string tags { get; set; }

			public int colorID { get; set; }

			public Position position { get; set; }

			[JsonIgnore]
			public List<string> tagsList
			{
				get
				{
					if (tags == null || tags.Length == 0)
					{
						return new List<string>();
					}
					return new List<string>(tags.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
				}
			}
		}

		private LoaderLogger dialogue;

		public Program program { get; private set; }

		private void PrintTokenList(IEnumerable<Token> tokenList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token token in tokenList)
			{
				stringBuilder.AppendLine($"{token.ToString()} ({token.context} line {token.lineNumber})");
			}
			dialogue.LogDebugMessage("Tokens:");
			dialogue.LogDebugMessage(stringBuilder.ToString());
		}

		private void PrintParseTree(Parser.ParseNode rootNode)
		{
			dialogue.LogDebugMessage("Parse Tree:");
			dialogue.LogDebugMessage(rootNode.PrintTree(0));
		}

		public Loader(LoaderLogger dialogue)
		{
			if (dialogue == null)
			{
				throw new ArgumentNullException("dialogue");
			}
			this.dialogue = dialogue;
		}

		private string preprocessor(string nodeText)
		{
			string text = null;
			using StringReader stringReader = new StringReader(nodeText);
			List<string> list = new List<string>();
			Stack<EmissionTuple> stack = new Stack<EmissionTuple>();
			stack.Push(new EmissionTuple(0, emitted: false));
			bool flag = false;
			char c = '\a';
			char c2 = '\v';
			string value = "->";
			string text2;
			while ((text2 = stringReader.ReadLine()) != null)
			{
				string text3 = text2.Replace("\t", "    ");
				text3 = text3.TrimEnd('\r', '\n');
				int num = text3.TakeWhile(char.IsWhiteSpace).Count();
				bool flag2 = text3.TrimStart(' ').StartsWith(value);
				EmissionTuple emissionTuple = stack.Peek();
				if (flag && num > emissionTuple.depth)
				{
					stack.Push(new EmissionTuple(num, emitted: true));
					if (list.Count == 0)
					{
						text3 = c + text3;
					}
					else
					{
						list[list.Count - 1] = list[list.Count - 1] + c;
					}
					flag = false;
				}
				else if (num < emissionTuple.depth)
				{
					while (num < stack.Peek().depth)
					{
						if (stack.Pop().emitted)
						{
							if (list.Count == 0)
							{
								text3 = c2 + text3;
							}
							else
							{
								list[list.Count - 1] = list[list.Count - 1] + c2;
							}
						}
					}
				}
				else
				{
					flag = false;
				}
				if (flag2)
				{
					flag = true;
					if (stack.Peek().depth < num)
					{
						stack.Push(new EmissionTuple(num, emitted: false));
					}
				}
				list.Add(text3);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in list)
			{
				stringBuilder.Append(item);
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		public Program Load(string text, Library library, string fileName, Program includeProgram, bool showTokens, bool showParseTree, string onlyConsiderNode, NodeFormat format, bool experimentalMode = false)
		{
			if (format == NodeFormat.Unknown)
			{
				format = GetFormatFromFileName(fileName);
			}
			if (experimentalMode && (format == NodeFormat.Text || format == NodeFormat.SingleNodeText))
			{
				if (format == NodeFormat.SingleNodeText)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("title:Start\n");
					stringBuilder.Append("---\n");
					stringBuilder.Append(text);
					stringBuilder.Append("\n===\n");
					text = stringBuilder.ToString();
				}
				YarnSpinnerParser yarnSpinnerParser = new YarnSpinnerParser(new CommonTokenStream(new YarnSpinnerLexer(CharStreams.fromstring(preprocessor(text)))));
				yarnSpinnerParser.RemoveErrorListeners();
				yarnSpinnerParser.AddErrorListener(ErrorListener.Instance);
				IParseTree tree = yarnSpinnerParser.dialogue();
				AntlrCompiler antlrCompiler = new AntlrCompiler(library);
				antlrCompiler.Compile(tree);
				if (includeProgram != null)
				{
					antlrCompiler.program.Include(includeProgram);
				}
				return antlrCompiler.program;
			}
			Dictionary<string, Parser.Node> dictionary = new Dictionary<string, Parser.Node>();
			NodeInfo[] nodesFromText = GetNodesFromText(text, format);
			int num = 0;
			NodeInfo[] array = nodesFromText;
			for (int i = 0; i < array.Length; i++)
			{
				NodeInfo nodeInfo = array[i];
				if (onlyConsiderNode != null && nodeInfo.title != onlyConsiderNode)
				{
					continue;
				}
				try
				{
					if (dictionary.ContainsKey(nodeInfo.title))
					{
						throw new InvalidOperationException("Attempted to load a node called " + nodeInfo.title + ", but a node with that name has already been loaded!");
					}
					TokenList tokenList = new Lexer().Tokenise(nodeInfo.title, nodeInfo.body);
					if (showTokens)
					{
						PrintTokenList(tokenList);
					}
					Parser.Node node = new Parser(tokenList, library).Parse();
					if (!string.IsNullOrEmpty(nodeInfo.tags) && nodeInfo.tags.Contains("rawText"))
					{
						node.source = nodeInfo.body;
					}
					node.name = nodeInfo.title;
					node.nodeTags = nodeInfo.tagsList;
					if (showParseTree)
					{
						PrintParseTree(node);
					}
					dictionary[nodeInfo.title] = node;
					num++;
				}
				catch (TokeniserException ex)
				{
					throw new TokeniserException($"In file {fileName}: Error reading node {nodeInfo.title}: {ex.Message}");
				}
				catch (ParseException ex2)
				{
					throw new ParseException($"In file {fileName}: Error parsing node {nodeInfo.title}: {ex2.Message}");
				}
				catch (InvalidOperationException ex3)
				{
					throw new InvalidOperationException($"In file {fileName}: Error reading node {nodeInfo.title}: {ex3.Message}");
				}
			}
			Compiler compiler = new Compiler(fileName);
			foreach (KeyValuePair<string, Parser.Node> item in dictionary)
			{
				compiler.CompileNode(item.Value);
			}
			if (includeProgram != null)
			{
				compiler.program.Include(includeProgram);
			}
			return compiler.program;
		}

		internal static NodeFormat GetFormatFromFileName(string fileName)
		{
			if (fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				return NodeFormat.JSON;
			}
			if (fileName.EndsWith(".yarn.txt", StringComparison.OrdinalIgnoreCase))
			{
				return NodeFormat.Text;
			}
			if (fileName.EndsWith(".node", StringComparison.OrdinalIgnoreCase))
			{
				return NodeFormat.SingleNodeText;
			}
			throw new FormatException($"Unknown file format for file '{fileName}'");
		}

		internal NodeInfo[] GetNodesFromText(string text, NodeFormat format)
		{
			List<NodeInfo> list = new List<NodeInfo>();
			switch (format)
			{
			case NodeFormat.SingleNodeText:
				list.Add(new NodeInfo
				{
					title = "Start",
					body = text
				});
				break;
			case NodeFormat.JSON:
				try
				{
					list = JsonConvert.DeserializeObject<List<NodeInfo>>(text);
				}
				catch (JsonReaderException ex3)
				{
					dialogue.LogErrorMessage("Error parsing Yarn input: " + ex3.Message);
				}
				break;
			case NodeFormat.Text:
			{
				if (!Regex.IsMatch(text, "---.?\n"))
				{
					dialogue.LogErrorMessage("Error parsing input: text appears corrupt (no header sentinel)");
					break;
				}
				Regex regex = new Regex("(?<field>.*): *(?<value>.*)");
				PropertyInfo[] properties = typeof(NodeInfo).GetProperties();
				int num = 0;
				using (StringReader stringReader = new StringReader(text))
				{
					string text2;
					while ((text2 = stringReader.ReadLine()) != null)
					{
						NodeInfo nodeInfo = default(NodeInfo);
						do
						{
							num++;
							if (text2.Length == 0)
							{
								continue;
							}
							Match match = regex.Match(text2);
							if (match == null)
							{
								dialogue.LogErrorMessage($"Line {num}: Can't parse header '{text2}'");
								continue;
							}
							string value = match.Groups["field"].Value;
							string value2 = match.Groups["value"].Value;
							PropertyInfo[] array = properties;
							foreach (PropertyInfo propertyInfo in array)
							{
								if (propertyInfo.Name != value || !propertyInfo.CanWrite)
								{
									continue;
								}
								try
								{
									Type propertyType = propertyInfo.PropertyType;
									object value3;
									if (propertyType.IsAssignableFrom(typeof(string)))
									{
										value3 = value2;
									}
									else if (propertyType.IsAssignableFrom(typeof(int)))
									{
										value3 = int.Parse(value2, CultureInfo.InvariantCulture);
									}
									else
									{
										if (!propertyType.IsAssignableFrom(typeof(NodeInfo.Position)))
										{
											throw new NotSupportedException();
										}
										string[] array2 = value2.Split(',');
										if (array2.Length != 2)
										{
											throw new FormatException();
										}
										value3 = new NodeInfo.Position
										{
											x = int.Parse(array2[0], CultureInfo.InvariantCulture),
											y = int.Parse(array2[1], CultureInfo.InvariantCulture)
										};
									}
									object obj = nodeInfo;
									propertyInfo.SetValue(obj, value3, null);
									nodeInfo = (NodeInfo)obj;
								}
								catch (FormatException)
								{
									dialogue.LogErrorMessage($"{num}: Error setting '{value}': invalid value '{value2}'");
									continue;
								}
								catch (NotSupportedException)
								{
									dialogue.LogErrorMessage($"{num}: Error setting '{value}': This property cannot be set");
									continue;
								}
								break;
							}
						}
						while ((text2 = stringReader.ReadLine()) != "---");
						num++;
						List<string> list2 = new List<string>();
						while ((text2 = stringReader.ReadLine()) != "===" && text2 != null)
						{
							num++;
							list2.Add(text2);
						}
						nodeInfo.body = string.Join("\n", list2.ToArray());
						list.Add(nodeInfo);
					}
				}
				break;
			}
			default:
				throw new InvalidOperationException("Unknown format " + format);
			}
			return list.ToArray();
		}
	}
}
