using System.Text;
using Antlr4.Runtime.Misc;

namespace Yarn
{
	public class Graph
	{
		public ArrayList<string> nodes = new ArrayList<string>();

		public MultiMap<string, string> edges = new MultiMap<string, string>();

		public string graphName = "G";

		public void edge(string source, string target)
		{
			edges.Map(source, target);
		}

		public string toDot()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("digraph {0} ", graphName);
			stringBuilder.Append("{\n");
			stringBuilder.Append("  ");
			foreach (string node in nodes)
			{
				stringBuilder.Append(node);
				stringBuilder.Append("; ");
			}
			stringBuilder.Append("\n");
			foreach (string key in edges.Keys)
			{
				if (!edges.TryGetValue(key, out var value))
				{
					continue;
				}
				foreach (string item in value)
				{
					stringBuilder.Append("  ");
					stringBuilder.Append(key);
					stringBuilder.Append(" -> ");
					stringBuilder.Append(item);
					stringBuilder.Append(";\n");
				}
			}
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}
	}
}
