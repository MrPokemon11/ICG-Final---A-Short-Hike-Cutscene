using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class YarnToGlobalDataAdapter : VariableStorageBehaviour
{
	public HashSet<string> yarnTags = new HashSet<string>();

	private Tags tags => Singleton<GlobalData>.instance.gameData.tags;

	public override Value GetValue(string variableName)
	{
		variableName = ProcessVariableName(variableName);
		if (tags.HasString(variableName))
		{
			return new Value(tags.GetString(variableName));
		}
		if (tags.HasFloat(variableName))
		{
			return new Value(tags.GetFloat(variableName));
		}
		if (tags.HasBool(variableName))
		{
			return new Value(tags.GetBool(variableName));
		}
		return Value.NULL;
	}

	public override void SetValue(string variableName, Value value)
	{
		variableName = ProcessVariableName(variableName);
		yarnTags.Add(variableName);
		if (value.type == Value.Type.Bool)
		{
			tags.RemoveValuesWithTag(variableName);
			tags.SetBool(variableName, value.AsBool);
		}
		else if (value.type == Value.Type.String)
		{
			tags.RemoveValuesWithTag(variableName);
			tags.SetString(variableName, value.stringValue);
		}
		else
		{
			tags.RemoveValuesWithTag(variableName);
			tags.SetFloat(variableName, value.AsNumber);
		}
	}

	private static string ProcessVariableName(string variableName)
	{
		if (variableName.StartsWith("$_"))
		{
			return variableName.Remove(0, 2);
		}
		return variableName;
	}

	public override float GetNumber(string variableName)
	{
		Debug.LogWarning("This should be deprecated?", this);
		return GetValue(variableName).AsNumber;
	}

	public override void SetNumber(string variableName, float number)
	{
		Debug.LogWarning("This should be deprecated?", this);
		SetValue(variableName, new Value(number));
	}

	public override void Clear()
	{
		foreach (string yarnTag in yarnTags)
		{
			tags.RemoveValuesWithTag(yarnTag);
		}
		yarnTags.Clear();
	}

	public override void ResetToDefaults()
	{
	}
}
