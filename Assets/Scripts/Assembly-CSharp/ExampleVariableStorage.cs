using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class ExampleVariableStorage : VariableStorageBehaviour
{
	[Serializable]
	public class DefaultVariable
	{
		public string name;

		public string value;

		public Value.Type type;
	}

	private Dictionary<string, Value> variables = new Dictionary<string, Value>();

	public DefaultVariable[] defaultVariables = new DefaultVariable[0];

	[Header("Optional debugging tools")]
	public Text debugTextView;

	private void Awake()
	{
		ResetToDefaults();
	}

	public override void ResetToDefaults()
	{
		Clear();
		DefaultVariable[] array = defaultVariables;
		foreach (DefaultVariable defaultVariable in array)
		{
			object value;
			switch (defaultVariable.type)
			{
			case Value.Type.Number:
			{
				float result2 = 0f;
				float.TryParse(defaultVariable.value, out result2);
				value = result2;
				break;
			}
			case Value.Type.String:
				value = defaultVariable.value;
				break;
			case Value.Type.Bool:
			{
				bool result = false;
				bool.TryParse(defaultVariable.value, out result);
				value = result;
				break;
			}
			case Value.Type.Variable:
				Debug.LogErrorFormat("Can't set variable {0} to {1}: You can't set a default variable to be another variable, because it may not have been initialised yet.", defaultVariable.name, defaultVariable.value);
				continue;
			case Value.Type.Null:
				value = null;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			Value value2 = new Value(value);
			SetValue("$" + defaultVariable.name, value2);
		}
	}

	public override void SetValue(string variableName, Value value)
	{
		variables[variableName] = new Value(value);
	}

	public override Value GetValue(string variableName)
	{
		if (!variables.ContainsKey(variableName))
		{
			return Value.NULL;
		}
		return variables[variableName];
	}

	public override void Clear()
	{
		variables.Clear();
	}

	private void Update()
	{
		if (!(debugTextView != null))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, Value> variable in variables)
		{
			stringBuilder.AppendLine($"{variable.Key} = {variable.Value}");
		}
		debugTextView.text = stringBuilder.ToString();
	}
}
