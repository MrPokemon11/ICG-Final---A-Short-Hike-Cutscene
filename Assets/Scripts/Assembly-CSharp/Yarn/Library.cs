using System;
using System.Collections.Generic;

namespace Yarn
{
	public class Library
	{
		private Dictionary<string, FunctionInfo> functions = new Dictionary<string, FunctionInfo>();

		public FunctionInfo GetFunction(string name)
		{
			try
			{
				return functions[name];
			}
			catch (KeyNotFoundException)
			{
				throw new InvalidOperationException(name + " is not a valid function");
			}
		}

		public void ImportLibrary(Library otherLibrary)
		{
			foreach (KeyValuePair<string, FunctionInfo> function in otherLibrary.functions)
			{
				functions[function.Key] = function.Value;
			}
		}

		public void RegisterFunction(FunctionInfo function)
		{
			functions[function.name] = function;
		}

		public void RegisterFunction(string name, int parameterCount, ReturningFunction implementation)
		{
			FunctionInfo function = new FunctionInfo(name, parameterCount, implementation);
			RegisterFunction(function);
		}

		public void RegisterFunction(string name, int parameterCount, Function implementation)
		{
			FunctionInfo function = new FunctionInfo(name, parameterCount, implementation);
			RegisterFunction(function);
		}

		public bool FunctionExists(string name)
		{
			return functions.ContainsKey(name);
		}

		public void DeregisterFunction(string name)
		{
			if (functions.ContainsKey(name))
			{
				functions.Remove(name);
			}
		}
	}
}
