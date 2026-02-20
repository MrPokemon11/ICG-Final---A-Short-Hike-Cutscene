using System;

namespace Yarn
{
	public interface VariableStorage
	{
		[Obsolete]
		void SetNumber(string variableName, float number);

		[Obsolete]
		float GetNumber(string variableName);

		void SetValue(string variableName, Value value);

		Value GetValue(string variableName);

		void Clear();
	}
}
