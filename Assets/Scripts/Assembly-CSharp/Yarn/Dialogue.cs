using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Yarn.Analysis;

namespace Yarn
{
	public class Dialogue : LoaderLogger
	{
		public abstract class RunnerResult
		{
		}

		public class LineResult : RunnerResult
		{
			public Line line;

			public LineResult(string text, string id)
			{
				line = new Line
				{
					text = text,
					id = id
				};
			}
		}

		public class CommandResult : RunnerResult
		{
			public Command command;

			public CommandResult(string text)
			{
				command = new Command
				{
					text = text
				};
			}
		}

		public class OptionSetResult : RunnerResult
		{
			public Options options;

			public OptionChooser setSelectedOptionDelegate;

			public OptionSetResult(IList<Line> optionStrings, OptionChooser setSelectedOption)
			{
				options = new Options
				{
					options = optionStrings
				};
				setSelectedOptionDelegate = setSelectedOption;
			}
		}

		public class NodeCompleteResult : RunnerResult
		{
			public string nextNode;

			public NodeCompleteResult(string nextNode)
			{
				this.nextNode = nextNode;
			}
		}

		public enum CompiledFormat
		{
			V1 = 0
		}

		public class StandardLibrary : Library
		{
			public StandardLibrary()
			{
				RegisterFunction(TokenType.Add.ToString(), 2, (Value[] parameters) => parameters[0] + parameters[1]);
				RegisterFunction(TokenType.Minus.ToString(), 2, (Value[] parameters) => parameters[0] - parameters[1]);
				RegisterFunction(TokenType.UnaryMinus.ToString(), 1, (Value[] parameters) => -parameters[0]);
				RegisterFunction(TokenType.Divide.ToString(), 2, (Value[] parameters) => parameters[0] / parameters[1]);
				RegisterFunction(TokenType.Multiply.ToString(), 2, (Value[] parameters) => parameters[0] * parameters[1]);
				RegisterFunction(TokenType.Modulo.ToString(), 2, (Value[] parameters) => parameters[0] % parameters[1]);
				RegisterFunction(TokenType.EqualTo.ToString(), 2, (Value[] parameters) => parameters[0].Equals(parameters[1]));
				RegisterFunction(TokenType.NotEqualTo.ToString(), 2, (Value[] parameters) => !GetFunction(TokenType.EqualTo.ToString()).Invoke(parameters).AsBool);
				RegisterFunction(TokenType.GreaterThan.ToString(), 2, (Value[] parameters) => parameters[0] > parameters[1]);
				RegisterFunction(TokenType.GreaterThanOrEqualTo.ToString(), 2, (Value[] parameters) => parameters[0] >= parameters[1]);
				RegisterFunction(TokenType.LessThan.ToString(), 2, (Value[] parameters) => parameters[0] < parameters[1]);
				RegisterFunction(TokenType.LessThanOrEqualTo.ToString(), 2, (Value[] parameters) => parameters[0] <= parameters[1]);
				RegisterFunction(TokenType.And.ToString(), 2, (Value[] parameters) => parameters[0].AsBool && parameters[1].AsBool);
				RegisterFunction(TokenType.Or.ToString(), 2, (Value[] parameters) => parameters[0].AsBool || parameters[1].AsBool);
				RegisterFunction(TokenType.Xor.ToString(), 2, (Value[] parameters) => parameters[0].AsBool ^ parameters[1].AsBool);
				RegisterFunction(TokenType.Not.ToString(), 1, (Value[] parameters) => !parameters[0].AsBool);
			}
		}

		internal VariableStorage continuity;

		public bool experimentalMode;

		public const string DEFAULT_START = "Start";

		internal Loader loader;

		private Program _program;

		private Dictionary<string, string> preloadedStringTable;

		public Library library;

		private VirtualMachine vm;

		public const CompiledFormat LATEST_FORMAT = CompiledFormat.V1;

		public Logger LogDebugMessage { get; set; }

		public Logger LogErrorMessage { get; set; }

		internal Program program
		{
			get
			{
				return _program;
			}
			set
			{
				bool num = _program == null && value != null;
				_program = value;
				if (num && preloadedStringTable != null)
				{
					AddStringTable(preloadedStringTable);
					preloadedStringTable = null;
				}
			}
		}

		public IEnumerable<string> allNodes => program.nodes.Keys;

		public string currentNode
		{
			get
			{
				if (vm == null)
				{
					return null;
				}
				return vm.currentNodeName;
			}
		}

		public int GetVisitCount(string nodeName)
		{
			return (int)continuity.GetValue("VISIT_" + nodeName).AsNumber;
		}

		public void SetVisitCount(string nodeName, int amount)
		{
			continuity.SetValue("VISIT_" + nodeName, new Value(amount));
		}

		private object YarnFunctionNodeVisitCount(Value[] parameters)
		{
			string text;
			if (parameters.Length == 0)
			{
				text = vm.currentNodeName;
			}
			else
			{
				if (parameters.Length != 1)
				{
					string message = $"Incorrect number of parameters to visitCount (expected 0 or 1, got {parameters.Length})";
					LogErrorMessage(message);
					return 0;
				}
				text = parameters[0].AsString;
				if (!NodeExists(text))
				{
					string message2 = $"The node {text} does not exist.";
					LogErrorMessage(message2);
					return 0;
				}
			}
			return GetVisitCount(text);
		}

		private object YarnFunctionIsNodeVisited(Value[] parameters)
		{
			return (int)YarnFunctionNodeVisitCount(parameters) > 0;
		}

		public Dialogue(VariableStorage continuity)
		{
			this.continuity = continuity;
			loader = new Loader(this);
			library = new Library();
			library.ImportLibrary(new StandardLibrary());
			library.RegisterFunction("visited", -1, (ReturningFunction)YarnFunctionIsNodeVisited);
			library.RegisterFunction("visitCount", -1, (ReturningFunction)YarnFunctionNodeVisitCount);
		}

		public void LoadFile(string fileName, bool showTokens = false, bool showParseTree = false, string onlyConsiderNode = null)
		{
			if (fileName.EndsWith(".yarn.bytes"))
			{
				byte[] bytes = File.ReadAllBytes(fileName);
				LoadCompiledProgram(bytes, fileName);
				return;
			}
			string text;
			using (StreamReader streamReader = new StreamReader(fileName))
			{
				text = streamReader.ReadToEnd();
			}
			LoadString(text, fileName, showTokens, showParseTree, onlyConsiderNode);
		}

		public void LoadCompiledProgram(byte[] bytes, string fileName, CompiledFormat format = CompiledFormat.V1)
		{
			if (LogDebugMessage == null)
			{
				throw new YarnException("LogDebugMessage must be set before loading");
			}
			if (LogErrorMessage == null)
			{
				throw new YarnException("LogErrorMessage must be set before loading");
			}
			if (format == CompiledFormat.V1)
			{
				LoadCompiledProgramV1(bytes);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}

		private void LoadCompiledProgramV1(byte[] bytes)
		{
			using MemoryStream stream = new MemoryStream(bytes);
			using BsonReader reader = new BsonReader(stream);
			JsonSerializer jsonSerializer = new JsonSerializer();
			try
			{
				Program otherProgram = jsonSerializer.Deserialize<Program>(reader);
				if (program != null)
				{
					program.Include(otherProgram);
				}
				else
				{
					program = otherProgram;
				}
			}
			catch (JsonReaderException ex)
			{
				LogErrorMessage($"Cannot load compiled program: {ex.Message}");
			}
		}

		public void LoadString(string text, string fileName = "<input>", bool showTokens = false, bool showParseTree = false, string onlyConsiderNode = null)
		{
			if (LogDebugMessage == null)
			{
				throw new YarnException("LogDebugMessage must be set before loading");
			}
			if (LogErrorMessage == null)
			{
				throw new YarnException("LogErrorMessage must be set before loading");
			}
			NodeFormat format = DetermineNodeFormat(text);
			program = loader.Load(text, library, fileName, program, showTokens, showParseTree, onlyConsiderNode, format, experimentalMode);
		}

		public static NodeFormat DetermineNodeFormat(string text)
		{
			if (text.StartsWith("[", StringComparison.Ordinal))
			{
				return NodeFormat.JSON;
			}
			if (text.Contains("---"))
			{
				return NodeFormat.Text;
			}
			return NodeFormat.SingleNodeText;
		}

		public IEnumerable<RunnerResult> Run(string startNode = "Start")
		{
			if (LogDebugMessage == null)
			{
				throw new YarnException("LogDebugMessage must be set before running");
			}
			if (LogErrorMessage == null)
			{
				throw new YarnException("LogErrorMessage must be set before running");
			}
			if (program == null)
			{
				LogErrorMessage("Dialogue.Run was called, but no program was loaded. Stopping.");
				yield break;
			}
			vm = new VirtualMachine(this, program);
			RunnerResult latestResult;
			vm.lineHandler = delegate(LineResult result)
			{
				latestResult = result;
			};
			vm.commandHandler = delegate(CommandResult result)
			{
				if (result != null && result.command.text == "stop")
				{
					vm.Stop();
				}
				latestResult = result;
			};
			vm.nodeCompleteHandler = delegate(NodeCompleteResult result)
			{
				int num = 0;
				num = GetVisitCount(vm.currentNodeName);
				SetVisitCount(vm.currentNodeName, num + 1);
				latestResult = result;
			};
			vm.optionsHandler = delegate(OptionSetResult result)
			{
				latestResult = result;
			};
			if (!vm.SetNode(startNode))
			{
				yield break;
			}
			do
			{
				latestResult = null;
				vm.RunNext();
				if (latestResult != null)
				{
					yield return latestResult;
				}
			}
			while (vm.executionState != VirtualMachine.ExecutionState.Stopped);
		}

		public void Stop()
		{
			if (vm != null)
			{
				vm.Stop();
			}
		}

		public Dictionary<string, string> GetTextForAllNodes()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, Node> node in program.nodes)
			{
				string textForNode = program.GetTextForNode(node.Key);
				if (textForNode != null)
				{
					dictionary[node.Key] = textForNode;
				}
			}
			return dictionary;
		}

		public string GetTextForNode(string nodeName)
		{
			if (program.nodes.Count == 0)
			{
				LogErrorMessage("No nodes are loaded!");
				return null;
			}
			if (program.nodes.ContainsKey(nodeName))
			{
				return program.GetTextForNode(nodeName);
			}
			LogErrorMessage("No node named " + nodeName);
			return null;
		}

		public void AddStringTable(Dictionary<string, string> stringTable)
		{
			if (program == null)
			{
				preloadedStringTable = stringTable;
			}
			else
			{
				program.LoadStrings(stringTable);
			}
		}

		public Dictionary<string, string> GetStringTable()
		{
			return program.strings;
		}

		internal Dictionary<string, LineInfo> GetStringInfoTable()
		{
			return program.lineInfo;
		}

		public byte[] GetCompiledProgram(CompiledFormat format = CompiledFormat.V1)
		{
			if (format == CompiledFormat.V1)
			{
				return GetCompiledProgramV1();
			}
			throw new ArgumentOutOfRangeException();
		}

		private byte[] GetCompiledProgramV1()
		{
			using MemoryStream memoryStream = new MemoryStream();
			using (BsonWriter jsonWriter = new BsonWriter(memoryStream))
			{
				new JsonSerializer().Serialize(jsonWriter, program);
			}
			return memoryStream.ToArray();
		}

		public void UnloadAll(bool clearVisitedNodes = true)
		{
			program = null;
		}

		[Obsolete("Calling Compile() is no longer necessary.")]
		public string Compile()
		{
			return program.DumpCode(library);
		}

		public string GetByteCode()
		{
			return program.DumpCode(library);
		}

		public bool NodeExists(string nodeName)
		{
			if (program == null)
			{
				if (program.nodes.Count > 0)
				{
					LogErrorMessage("Internal consistency error: Called NodeExists, and there are nodes loaded, but the program hasn't been compiled yet, somehow?");
					return false;
				}
				LogErrorMessage("Tried to call NodeExists, but no nodes have been compiled!");
				return false;
			}
			if (program.nodes == null || program.nodes.Count == 0)
			{
				LogDebugMessage("Called NodeExists, but there are zero nodes. This may be an error.");
				return false;
			}
			return program.nodes.ContainsKey(nodeName);
		}

		public void Analyse(Context context)
		{
			context.AddProgramToAnalysis(program);
		}
	}
}
