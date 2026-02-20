using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

[Serializable]
public class Tags : IDeserializationCallback
{
	private Dictionary<string, bool> bools = new Dictionary<string, bool>();

	private Dictionary<string, int> ints = new Dictionary<string, int>();

	private Dictionary<string, float> floats = new Dictionary<string, float>();

	private Dictionary<string, string> strings = new Dictionary<string, string>();

	[NonSerialized]
	private Dictionary<string, Action<bool>> boolWatchers = new Dictionary<string, Action<bool>>();

	[NonSerialized]
	private Dictionary<string, Action<int>> intWatchers = new Dictionary<string, Action<int>>();

	[NonSerialized]
	private Dictionary<string, Action<string>> stringWatchers = new Dictionary<string, Action<string>>();

	[NonSerialized]
	private Dictionary<string, Action<float>> floatWatchers = new Dictionary<string, Action<float>>();

	public void OnDeserialization(object sender)
	{
		boolWatchers = new Dictionary<string, Action<bool>>();
		intWatchers = new Dictionary<string, Action<int>>();
		stringWatchers = new Dictionary<string, Action<string>>();
		floatWatchers = new Dictionary<string, Action<float>>();
	}

	public void Clear()
	{
		bools.Clear();
		ints.Clear();
		floats.Clear();
		strings.Clear();
	}

	public Tags Clone()
	{
		return new Tags
		{
			bools = bools.ToDictionary((KeyValuePair<string, bool> p) => p.Key.ToString(), (KeyValuePair<string, bool> p) => p.Value),
			ints = ints.ToDictionary((KeyValuePair<string, int> p) => p.Key.ToString(), (KeyValuePair<string, int> p) => p.Value),
			strings = strings.ToDictionary((KeyValuePair<string, string> p) => p.Key.ToString(), (KeyValuePair<string, string> p) => p.Value.ToString()),
			floats = floats.ToDictionary((KeyValuePair<string, float> p) => p.Key.ToString(), (KeyValuePair<string, float> p) => p.Value)
		};
	}

	public bool GetBool(string tag)
	{
		if (!bools.ContainsKey(tag))
		{
			return false;
		}
		return bools[tag];
	}

	public bool HasBool(string tag)
	{
		return bools.ContainsKey(tag);
	}

	public void SetBool(string tag, bool value = true)
	{
		bools[tag] = value;
		if (boolWatchers.ContainsKey(tag))
		{
			boolWatchers[tag](value);
		}
	}

	public int GetInt(string tag)
	{
		if (!ints.ContainsKey(tag))
		{
			return 0;
		}
		return ints[tag];
	}

	public void SetInt(string tag, int num)
	{
		ints[tag] = num;
		if (intWatchers.ContainsKey(tag))
		{
			intWatchers[tag](num);
		}
	}

	public string GetString(string tag)
	{
		if (!strings.ContainsKey(tag))
		{
			return null;
		}
		return strings[tag];
	}

	public bool HasString(string variableName)
	{
		return strings.ContainsKey(variableName);
	}

	public void SetString(string tag, string value)
	{
		if (value == null)
		{
			strings.Remove(tag);
		}
		else
		{
			strings[tag] = value;
		}
		if (stringWatchers.ContainsKey(tag))
		{
			stringWatchers[tag](value);
		}
	}

	public void SetFloat(string tag, float number)
	{
		floats[tag] = number;
		if (floatWatchers.ContainsKey(tag))
		{
			floatWatchers[tag](number);
		}
	}

	public bool HasFloat(string variableName)
	{
		return floats.ContainsKey(variableName);
	}

	public float GetFloat(string tag)
	{
		if (!floats.ContainsKey(tag))
		{
			return 0f;
		}
		return floats[tag];
	}

	public void WatchBool(string tag, Action<bool> onChange)
	{
		if (!boolWatchers.ContainsKey(tag))
		{
			boolWatchers[tag] = null;
		}
		Dictionary<string, Action<bool>> dictionary = boolWatchers;
		dictionary[tag] = (Action<bool>)Delegate.Combine(dictionary[tag], onChange);
	}

	public void UnwatchBool(string tag, Action<bool> onChange)
	{
		if (boolWatchers.ContainsKey(tag))
		{
			Dictionary<string, Action<bool>> dictionary = boolWatchers;
			dictionary[tag] = (Action<bool>)Delegate.Remove(dictionary[tag], onChange);
			if (boolWatchers[tag] == null)
			{
				boolWatchers.Remove(tag);
			}
		}
	}

	public void WatchFloat(string tag, Action<float> onChange)
	{
		if (!floatWatchers.ContainsKey(tag))
		{
			floatWatchers[tag] = null;
		}
		Dictionary<string, Action<float>> dictionary = floatWatchers;
		dictionary[tag] = (Action<float>)Delegate.Combine(dictionary[tag], onChange);
	}

	public void UnwatchFloat(string tag, Action<float> onChange)
	{
		if (floatWatchers.ContainsKey(tag))
		{
			Dictionary<string, Action<float>> dictionary = floatWatchers;
			dictionary[tag] = (Action<float>)Delegate.Remove(dictionary[tag], onChange);
			if (floatWatchers[tag] == null)
			{
				floatWatchers.Remove(tag);
			}
		}
	}

	public void WatchInt(string tag, Action<int> onChange)
	{
		if (!intWatchers.ContainsKey(tag))
		{
			intWatchers[tag] = null;
		}
		Dictionary<string, Action<int>> dictionary = intWatchers;
		dictionary[tag] = (Action<int>)Delegate.Combine(dictionary[tag], onChange);
	}

	public void UnwatchInt(string tag, Action<int> onChange)
	{
		if (intWatchers.ContainsKey(tag))
		{
			Dictionary<string, Action<int>> dictionary = intWatchers;
			dictionary[tag] = (Action<int>)Delegate.Remove(dictionary[tag], onChange);
			if (intWatchers[tag] == null)
			{
				intWatchers.Remove(tag);
			}
		}
	}

	public void WatchString(string tag, Action<string> onChange)
	{
		if (!stringWatchers.ContainsKey(tag))
		{
			stringWatchers[tag] = null;
		}
		Dictionary<string, Action<string>> dictionary = stringWatchers;
		dictionary[tag] = (Action<string>)Delegate.Combine(dictionary[tag], onChange);
	}

	public void UnwatchString(string tag, Action<string> onChange)
	{
		if (stringWatchers.ContainsKey(tag))
		{
			Dictionary<string, Action<string>> dictionary = stringWatchers;
			dictionary[tag] = (Action<string>)Delegate.Remove(dictionary[tag], onChange);
			if (stringWatchers[tag] == null)
			{
				stringWatchers.Remove(tag);
			}
		}
	}

	public void RemoveValuesWithTag(string tag)
	{
		bools.Remove(tag);
		ints.Remove(tag);
		floats.Remove(tag);
		strings.Remove(tag);
	}

	public int CountTagsStartingWith(string prefix)
	{
		return bools.Count((KeyValuePair<string, bool> pair) => pair.Key.StartsWith(prefix)) + ints.Count((KeyValuePair<string, int> pair) => pair.Key.StartsWith(prefix)) + floats.Count((KeyValuePair<string, float> pair) => pair.Key.StartsWith(prefix)) + strings.Count((KeyValuePair<string, string> pair) => pair.Key.StartsWith(prefix));
	}

	public string DumpTagData()
	{
		string text = "";
		foreach (KeyValuePair<string, bool> @bool in bools)
		{
			text = text + @bool.Key + ": " + @bool.Value + "\n";
		}
		foreach (KeyValuePair<string, int> @int in ints)
		{
			text = text + @int.Key + ": " + @int.Value + "\n";
		}
		foreach (KeyValuePair<string, float> @float in floats)
		{
			text = text + @float.Key + ": " + @float.Value + "\n";
		}
		foreach (KeyValuePair<string, string> @string in strings)
		{
			text = text + @string.Key + ": " + @string.Value + "\n";
		}
		return text;
	}
}
