using System;
using System.Collections.Generic;

namespace Yarn
{
	internal class VirtualMachine
	{
		internal class State
		{
			public string currentNodeName;

			public int programCounter;

			public List<KeyValuePair<string, string>> currentOptions = new List<KeyValuePair<string, string>>();

			private Stack<Value> stack = new Stack<Value>();

			public void PushValue(object o)
			{
				if (o is Value)
				{
					stack.Push(o as Value);
				}
				else
				{
					stack.Push(new Value(o));
				}
			}

			public Value PopValue()
			{
				return stack.Pop();
			}

			public Value PeekValue()
			{
				return stack.Peek();
			}

			public void ClearStack()
			{
				stack.Clear();
			}
		}

		internal static class SpecialVariables
		{
			public const string ShuffleOptions = "$Yarn.ShuffleOptions";
		}

		public delegate void LineHandler(Dialogue.LineResult line);

		public delegate void OptionsHandler(Dialogue.OptionSetResult options);

		public delegate void CommandHandler(Dialogue.CommandResult command);

		public delegate void NodeCompleteHandler(Dialogue.NodeCompleteResult complete);

		public enum ExecutionState
		{
			Stopped = 0,
			WaitingOnOptionSelection = 1,
			Running = 2
		}

		public LineHandler lineHandler;

		public OptionsHandler optionsHandler;

		public CommandHandler commandHandler;

		public NodeCompleteHandler nodeCompleteHandler;

		private Dialogue dialogue;

		private Program program;

		private State state = new State();

		private Random random = new Random();

		private ExecutionState _executionState;

		private Node currentNode;

		public string currentNodeName => state.currentNodeName;

		public ExecutionState executionState
		{
			get
			{
				return _executionState;
			}
			private set
			{
				_executionState = value;
				if (_executionState == ExecutionState.Stopped)
				{
					ResetState();
				}
			}
		}

		internal VirtualMachine(Dialogue d, Program p)
		{
			program = p;
			dialogue = d;
			state = new State();
		}

		private void ResetState()
		{
			state = new State();
		}

		public bool SetNode(string nodeName)
		{
			if (!program.nodes.ContainsKey(nodeName))
			{
				string message = "No node named " + nodeName;
				dialogue.LogErrorMessage(message);
				executionState = ExecutionState.Stopped;
				return false;
			}
			dialogue.LogDebugMessage("Running node " + nodeName);
			dialogue.continuity.SetValue("$Yarn.ShuffleOptions", new Value(false));
			currentNode = program.nodes[nodeName];
			ResetState();
			state.currentNodeName = nodeName;
			return true;
		}

		public void Stop()
		{
			executionState = ExecutionState.Stopped;
		}

		internal void RunNext()
		{
			if (executionState == ExecutionState.WaitingOnOptionSelection)
			{
				dialogue.LogErrorMessage("Cannot continue running dialogue. Still waiting on option selection.");
				executionState = ExecutionState.Stopped;
				return;
			}
			if (executionState == ExecutionState.Stopped)
			{
				executionState = ExecutionState.Running;
			}
			Instruction i = currentNode.instructions[state.programCounter];
			RunInstruction(i);
			state.programCounter++;
			if (state.programCounter >= currentNode.instructions.Count)
			{
				executionState = ExecutionState.Stopped;
				nodeCompleteHandler(new Dialogue.NodeCompleteResult(null));
				dialogue.LogDebugMessage("Run complete.");
			}
		}

		internal int FindInstructionPointForLabel(string labelName)
		{
			if (!currentNode.labels.ContainsKey(labelName))
			{
				throw new IndexOutOfRangeException("Unknown label " + labelName + " in node " + state.currentNodeName);
			}
			return currentNode.labels[labelName];
		}

		internal void RunInstruction(Instruction i)
		{
			switch (i.operation)
			{
			case ByteCode.JumpTo:
				state.programCounter = FindInstructionPointForLabel((string)i.operandA);
				break;
			case ByteCode.RunLine:
			{
				string text2 = program.GetString((string)i.operandA);
				if (text2 == null)
				{
					dialogue.LogErrorMessage("No loaded string table includes line " + i.operandA);
				}
				else
				{
					lineHandler(new Dialogue.LineResult(text2, (string)i.operandA));
				}
				break;
			}
			case ByteCode.RunCommand:
				commandHandler(new Dialogue.CommandResult((string)i.operandA));
				break;
			case ByteCode.PushString:
				state.PushValue(program.GetString((string)i.operandA));
				break;
			case ByteCode.PushNumber:
				state.PushValue(Convert.ToSingle(i.operandA));
				break;
			case ByteCode.PushBool:
				state.PushValue(Convert.ToBoolean(i.operandA));
				break;
			case ByteCode.PushNull:
				state.PushValue(Value.NULL);
				break;
			case ByteCode.JumpIfFalse:
				if (!state.PeekValue().AsBool)
				{
					state.programCounter = FindInstructionPointForLabel((string)i.operandA);
				}
				break;
			case ByteCode.Jump:
			{
				string asString = state.PeekValue().AsString;
				state.programCounter = FindInstructionPointForLabel(asString);
				break;
			}
			case ByteCode.Pop:
				state.PopValue();
				break;
			case ByteCode.CallFunc:
			{
				string name = (string)i.operandA;
				FunctionInfo function = dialogue.library.GetFunction(name);
				int num = function.paramCount;
				if (num == -1)
				{
					num = (int)state.PopValue().AsNumber;
				}
				Value o;
				if (num == 0)
				{
					o = function.Invoke();
				}
				else
				{
					Value[] array = new Value[num];
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						array[num2] = state.PopValue();
					}
					o = function.InvokeWithArray(array);
				}
				if (function.returnsValue)
				{
					state.PushValue(o);
				}
				break;
			}
			case ByteCode.PushVariable:
			{
				string variableName = (string)i.operandA;
				Value value3 = dialogue.continuity.GetValue(variableName);
				state.PushValue(value3);
				break;
			}
			case ByteCode.StoreVariable:
			{
				Value value4 = state.PeekValue();
				string variableName2 = (string)i.operandA;
				dialogue.continuity.SetValue(variableName2, value4);
				break;
			}
			case ByteCode.Stop:
				nodeCompleteHandler(new Dialogue.NodeCompleteResult(null));
				executionState = ExecutionState.Stopped;
				break;
			case ByteCode.RunNode:
			{
				string text = ((!string.IsNullOrEmpty((string)i.operandA)) ? ((string)i.operandA) : state.PeekValue().AsString);
				nodeCompleteHandler(new Dialogue.NodeCompleteResult(text));
				SetNode(text);
				break;
			}
			case ByteCode.AddOption:
				state.currentOptions.Add(new KeyValuePair<string, string>((string)i.operandA, (string)i.operandB));
				break;
			case ByteCode.ShowOptions:
			{
				if (state.currentOptions.Count == 0)
				{
					nodeCompleteHandler(new Dialogue.NodeCompleteResult(null));
					executionState = ExecutionState.Stopped;
					break;
				}
				if (state.currentOptions.Count == 1 && state.currentOptions[0].Key == null)
				{
					string value = state.currentOptions[0].Value;
					state.PushValue(value);
					state.currentOptions.Clear();
					break;
				}
				if (dialogue.continuity.GetValue("$Yarn.ShuffleOptions").AsBool)
				{
					int count = state.currentOptions.Count;
					for (int j = 0; j < count; j++)
					{
						int index = j + (int)(random.NextDouble() * (double)(count - j));
						KeyValuePair<string, string> value2 = state.currentOptions[index];
						state.currentOptions[index] = state.currentOptions[j];
						state.currentOptions[j] = value2;
					}
				}
				List<Line> list = new List<Line>();
				foreach (KeyValuePair<string, string> currentOption in state.currentOptions)
				{
					list.Add(new Line
					{
						text = program.GetString(currentOption.Key),
						id = currentOption.Key
					});
				}
				executionState = ExecutionState.WaitingOnOptionSelection;
				optionsHandler(new Dialogue.OptionSetResult(list, delegate(int selectedOption)
				{
					string value5 = state.currentOptions[selectedOption].Value;
					state.PushValue(value5);
					state.currentOptions.Clear();
					executionState = ExecutionState.Running;
				}));
				break;
			}
			default:
				executionState = ExecutionState.Stopped;
				throw new ArgumentOutOfRangeException();
			case ByteCode.Label:
				break;
			}
		}
	}
}
