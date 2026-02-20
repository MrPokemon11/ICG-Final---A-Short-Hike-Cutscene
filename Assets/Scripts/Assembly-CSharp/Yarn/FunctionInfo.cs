using System;

namespace Yarn
{
	public class FunctionInfo
	{
		public string name { get; private set; }

		public int paramCount { get; private set; }

		public Function function { get; private set; }

		public ReturningFunction returningFunction { get; private set; }

		public bool returnsValue => returningFunction != null;

		public Value Invoke(params Value[] parameters)
		{
			return InvokeWithArray(parameters);
		}

		public Value InvokeWithArray(Value[] parameters)
		{
			int parameterCount = ((parameters != null) ? parameters.Length : 0);
			if (IsParameterCountCorrect(parameterCount))
			{
				if (returnsValue)
				{
					return new Value(returningFunction(parameters));
				}
				function(parameters);
				return Value.NULL;
			}
			throw new InvalidOperationException($"Incorrect number of parameters for function {name} (expected {paramCount}, got {parameters.Length}");
		}

		internal FunctionInfo(string name, int paramCount, Function implementation)
		{
			this.name = name;
			this.paramCount = paramCount;
			function = implementation;
			returningFunction = null;
		}

		internal FunctionInfo(string name, int paramCount, ReturningFunction implementation)
		{
			this.name = name;
			this.paramCount = paramCount;
			returningFunction = implementation;
			function = null;
		}

		internal bool IsParameterCountCorrect(int parameterCount)
		{
			if (paramCount != parameterCount)
			{
				return paramCount == -1;
			}
			return true;
		}
	}
}
