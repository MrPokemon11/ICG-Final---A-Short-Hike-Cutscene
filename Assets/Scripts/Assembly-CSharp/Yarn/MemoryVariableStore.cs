using System.Collections.Generic;

namespace Yarn
{
	public class MemoryVariableStore : BaseVariableStorage
	{
		private Dictionary<string, Value> variables = new Dictionary<string, Value>();

		public override void SetValue(string variableName, Value value)
		{
			variables[variableName] = value;
		}

		public override Value GetValue(string variableName)
		{
			Value result = Value.NULL;
			if (variables.ContainsKey(variableName))
			{
				result = variables[variableName];
			}
			return result;
		}

		public override void Clear()
		{
			variables.Clear();
		}
	}
}
