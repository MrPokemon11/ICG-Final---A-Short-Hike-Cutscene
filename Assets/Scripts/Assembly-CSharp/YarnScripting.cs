using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Yarn;

public static class YarnScripting<T>
{
	public class YarnFunction
	{
		public string name;

		public int parameters;

		public YarnFunctionHandler handler;

		public YarnFunction(string name, int parameters, YarnFunctionHandler handler)
		{
			this.name = name;
			this.parameters = parameters;
			this.handler = handler;
		}
	}

	public delegate IEnumerator YarnCommandHandler(T context, string[] parameters);

	public delegate object YarnFunctionHandler(T context, Value[] parameters);

	private class VerifyException : Exception
	{
		public VerifyException(string message)
			: base(message)
		{
		}

		public void Report(string methodName)
		{
			Debug.LogError("VerifyException for " + methodName + ": " + Message);
		}
	}

	private static bool initalized;

	private static Dictionary<string, YarnCommandHandler> yarnCommands;

	private static Dictionary<string, YarnFunction> yarnFunctions;

	public static void Initalize(Type yarnCommandsType, Type yarnFunctionsType)
	{
		if (yarnCommands == null || yarnFunctions == null)
		{
			yarnCommands = new Dictionary<string, YarnCommandHandler>();
			yarnFunctions = new Dictionary<string, YarnFunction>();
			MethodInfo[] methods = yarnCommandsType.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				RegisterCommand(methods[i]);
			}
			methods = yarnFunctionsType.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				RegisterFunction(methods[i]);
			}
			initalized = true;
		}
	}

	public static YarnCommandHandler GetCommand(string commandName)
	{
		if (!initalized)
		{
			Debug.LogError("YarnScripts must be initalized first!");
			return null;
		}
		return yarnCommands.GetValueOrDefault(commandName, null);
	}

	public static IEnumerable<YarnFunction> GetAllFunctions()
	{
		if (!initalized)
		{
			Debug.LogError("YarnScripts must be initalized first!");
			return null;
		}
		return yarnFunctions.Values;
	}

	private static void RegisterFunction(MethodInfo method)
	{
		if (method.GetCustomAttributes(typeof(YarnFunctionAttribute), inherit: true).Length == 0)
		{
			return;
		}
		try
		{
			ParameterInfo[] parameters = method.GetParameters();
			Verify(parameters.Length >= 1, "Needs at least one parameter.");
			Verify(parameters[0].ParameterType.IsAssignableFrom(typeof(T)), "First parameter must be context.");
			Verify(parameters.Skip(1).All((ParameterInfo p) => p.ParameterType.IsAssignableFrom(typeof(Value))), "Every parameter must be a Value.");
			Verify(method.ReturnType != typeof(void), "Return type must be an object.");
			int parameters2 = parameters.Length - 1;
			YarnFunctionHandler handler = (T context, Value[] arguments) => method.Invoke(null, ((object)context).Yield().Concat(arguments).ToArray());
			YarnFunction value = new YarnFunction(method.Name, parameters2, handler);
			yarnFunctions.Add(method.Name, value);
		}
		catch (VerifyException ex)
		{
			ex.Report(method.Name);
		}
	}

	private static void RegisterCommand(MethodInfo method)
	{
		if (!(method.GetCustomAttributes(typeof(YarnCommandAttribute), inherit: true).FirstOrDefault() is YarnCommandAttribute))
		{
			return;
		}
		try
		{
			ParameterInfo[] parameters = method.GetParameters();
			Verify(parameters.Length == 2, "Needs two parameters.");
			Verify(parameters[0].ParameterType.IsAssignableFrom(typeof(T)), "First parameter must be the context.");
			Verify(parameters[1].ParameterType.IsAssignableFrom(typeof(string[])), "Second parameter must be string[].");
			Verify(method.ReturnType.IsAssignableFrom(typeof(IEnumerator)), "Return type must be IEnumerator.");
			YarnCommandHandler value = (T context, string[] arguments) => (IEnumerator)method.Invoke(null, new object[2] { context, arguments });
			yarnCommands.Add(method.Name, value);
		}
		catch (VerifyException ex)
		{
			ex.Report(method.Name);
		}
	}

	private static void Verify(bool expected, string error)
	{
		if (!expected)
		{
			throw new VerifyException(error);
		}
	}
}
