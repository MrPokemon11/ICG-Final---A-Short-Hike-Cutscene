using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yarn
{
	[JsonObject(MemberSerialization.OptIn)]
	internal class Program
	{
		internal Dictionary<string, string> strings = new Dictionary<string, string>();

		internal Dictionary<string, LineInfo> lineInfo = new Dictionary<string, LineInfo>();

		[JsonProperty]
		internal Dictionary<string, Node> nodes = new Dictionary<string, Node>();

		private int stringCount;

		[JsonProperty("strings")]
		internal Dictionary<string, string> untaggedStrings
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> @string in strings)
				{
					if (!@string.Key.StartsWith("line:"))
					{
						dictionary.Add(@string.Key, @string.Value);
					}
				}
				return dictionary;
			}
		}

		public void LoadStrings(Dictionary<string, string> newStrings)
		{
			foreach (KeyValuePair<string, string> newString in newStrings)
			{
				strings[newString.Key] = newString.Value;
			}
		}

		public string RegisterString(string theString, string nodeName, string lineID, int lineNumber, bool localisable)
		{
			string text = ((lineID != null) ? lineID : $"{nodeName}-{stringCount++}");
			strings.Add(text, theString);
			if (localisable)
			{
				lineInfo.Add(text, new LineInfo(nodeName, lineNumber));
			}
			return text;
		}

		public string GetString(string key)
		{
			string value = null;
			strings.TryGetValue(key, out value);
			return value;
		}

		public string DumpCode(Library l)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, Node> node in nodes)
			{
				stringBuilder.AppendLine("Node " + node.Key + ":");
				int num = 0;
				foreach (Instruction instruction in node.Value.instructions)
				{
					string text = ((instruction.operation != ByteCode.Label) ? ("    " + instruction.ToString(this, l)) : instruction.ToString(this, l));
					string text2 = ((num % 5 != 0 && num != node.Value.instructions.Count - 1) ? string.Format("{0,6}   ", " ") : $"{num,6}   ");
					stringBuilder.AppendLine(text2 + text);
					num++;
				}
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine("String table:");
			foreach (KeyValuePair<string, string> @string in strings)
			{
				LineInfo lineInfo = this.lineInfo[@string.Key];
				stringBuilder.AppendLine($"{@string.Key}: {@string.Value} ({lineInfo.nodeName}:{lineInfo.lineNumber})");
			}
			return stringBuilder.ToString();
		}

		public string GetTextForNode(string nodeName)
		{
			return GetString(nodes[nodeName].sourceTextStringID);
		}

		public void Include(Program otherProgram)
		{
			foreach (KeyValuePair<string, Node> node in otherProgram.nodes)
			{
				if (nodes.ContainsKey(node.Key))
				{
					throw new InvalidOperationException($"This program already contains a node named {node.Key}");
				}
				nodes[node.Key] = node.Value;
			}
			foreach (KeyValuePair<string, string> @string in otherProgram.strings)
			{
				if (nodes.ContainsKey(@string.Key))
				{
					throw new InvalidOperationException($"This program already contains a string with key {@string.Key}");
				}
				strings[@string.Key] = @string.Value;
			}
		}
	}
}
