using System.Collections.Generic;

namespace Yarn
{
	internal class Node
	{
		public List<Instruction> instructions = new List<Instruction>();

		public string name;

		public string sourceTextStringID;

		public Dictionary<string, int> labels = new Dictionary<string, int>();

		public List<string> tags;
	}
}
