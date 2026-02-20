using System;

namespace Yarn
{
	public abstract class BaseVariableStorage : VariableStorage
	{
		[Obsolete]
		public void SetNumber(string variableName, float number)
		{
			SetValue(variableName, new Value(number));
		}

		[Obsolete]
		public float GetNumber(string variableName)
		{
			return GetValue(variableName).AsNumber;
		}

		public abstract void SetValue(string variableName, Value value);

		public abstract Value GetValue(string variableName);

		public abstract void Clear();
	}
}
