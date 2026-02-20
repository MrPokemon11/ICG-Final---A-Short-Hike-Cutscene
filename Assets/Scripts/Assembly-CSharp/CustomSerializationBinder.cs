using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using InControl;
using QuickUnityTools.Input;
using UnityEngine;

public class CustomSerializationBinder : SerializationBinder
{
	public static List<Type> SAFE_TYPES;

	public static Dictionary<string, Type> SAFE_TYPE_DICT;

	static CustomSerializationBinder()
	{
		SAFE_TYPES = new List<Type>
		{
			typeof(Dictionary<string, float>),
			typeof(Dictionary<string, string>),
			typeof(Dictionary<string, int>),
			typeof(Dictionary<string, bool>),
			typeof(KeyValuePair<string, float>),
			typeof(KeyValuePair<string, string>),
			typeof(KeyValuePair<string, int>),
			typeof(KeyValuePair<string, bool>),
			Type.GetType("System.Collections.Generic.GenericEqualityComparer`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"),
			Type.GetType("System.Collections.Generic.InternalStringComparer"),
			typeof(GlobalData.GameData),
			typeof(Dictionary<string, List<PlayerReplayFrame>>),
			typeof(KeyValuePair<string, List<PlayerReplayFrame>>),
			typeof(List<PlayerReplayFrame>),
			typeof(PlayerReplayFrame),
			typeof(Vector3),
			typeof(Quaternion),
			typeof(PlayerReplayFrame.Event),
			typeof(List<string>),
			typeof(GlobalData.CollectionInventory),
			typeof(List<Fish>),
			typeof(Dictionary<string, Fish>),
			typeof(KeyValuePair<string, Fish>),
			typeof(Fish),
			typeof(Tags),
			typeof(HashSet<string>),
			typeof(InputControlType),
			typeof(PlayerPrefsAdapter.FilePlayerPrefs.Data),
			typeof(GameActionSetSettings)
		};
		SAFE_TYPE_DICT = new Dictionary<string, Type>();
		foreach (Type sAFE_TYPE in SAFE_TYPES)
		{
			SAFE_TYPE_DICT.Add(sAFE_TYPE.FullName, sAFE_TYPE);
		}
	}

	public override Type BindToType(string assemblyName, string typeName)
	{
		if (SAFE_TYPE_DICT.TryGetValue(typeName, out var value))
		{
			return value;
		}
		throw new ArgumentOutOfRangeException(typeName);
	}
}
