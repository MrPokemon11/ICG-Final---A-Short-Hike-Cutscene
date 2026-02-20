using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsAdapter
{
	public interface IPlayerPrefs
	{
		void Initalize();

		void SetString(string key, string value);

		string GetString(string key, string defaultValue);

		void SetFloat(string key, float value);

		float GetFloat(string key, float defaultValue);

		void SetInt(string key, int value);

		int GetInt(string key, int defaultValue);

		void Save();
	}

	public class FilePlayerPrefs : IPlayerPrefs
	{
		[Serializable]
		public class Data
		{
			public Dictionary<string, float> floats = new Dictionary<string, float>();

			public Dictionary<string, string> strings = new Dictionary<string, string>();

			public Dictionary<string, int> ints = new Dictionary<string, int>();
		}

		private const string PREFS_FILE = "PlayerPrefsFileEdition.prefs";

		private Data data;

		public void Initalize()
		{
			data = FileSystem.LoadObjectUnsafe<Data>("PlayerPrefsFileEdition.prefs");
			if (data == null)
			{
				data = new Data();
			}
		}

		public void ReloadAsync(Action onFinish)
		{
			FileSystem.LoadObject("PlayerPrefsFileEdition.prefs", delegate(Data result)
			{
				data = result;
				if (data == null)
				{
					data = new Data();
				}
				onFinish?.Invoke();
			});
		}

		public string GetString(string key, string defaultValue)
		{
			return GetOrDefault(data.strings, key, defaultValue);
		}

		public void SetString(string key, string value)
		{
			data.strings[key] = value;
		}

		private T GetOrDefault<T>(Dictionary<string, T> dict, string key, T defaultValue)
		{
			if (!dict.ContainsKey(key))
			{
				return defaultValue;
			}
			return dict[key];
		}

		public void Save()
		{
			FileSystem.SaveObject("PlayerPrefsFileEdition.prefs", data, 10000, null);
		}

		public void SetFloat(string key, float value)
		{
			data.floats[key] = value;
		}

		public float GetFloat(string key, float defaultValue)
		{
			return GetOrDefault(data.floats, key, defaultValue);
		}

		public void SetInt(string key, int value)
		{
			data.ints[key] = value;
		}

		public int GetInt(string key, int defaultValue)
		{
			return GetOrDefault(data.ints, key, defaultValue);
		}
	}

	public class StandardPlayerPrefs : IPlayerPrefs
	{
		public void Initalize()
		{
		}

		public string GetString(string key, string defaultValue)
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}

		public void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public void Save()
		{
			PlayerPrefs.Save();
		}

		public void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public float GetFloat(string key, float defaultValue)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		public void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public int GetInt(string key, int defaultValue)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}
	}

	private static IPlayerPrefs prefs;

	public static void Initalize()
	{
		if (prefs == null)
		{
			prefs = CrossPlatform.CreatePlayerPrefs();
			prefs.Initalize();
		}
	}

	public static void SetString(string key, string value)
	{
		prefs.SetString(key, value);
	}

	public static string GetString(string key, string defaultValue = null)
	{
		return prefs.GetString(key, defaultValue);
	}

	public static void SetFloat(string key, float value)
	{
		prefs.SetFloat(key, value);
	}

	public static float GetFloat(string key, float defaultValue = 0f)
	{
		return prefs.GetFloat(key, defaultValue);
	}

	public static void SetInt(string key, int value)
	{
		prefs.SetInt(key, value);
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		return prefs.GetInt(key, defaultValue);
	}

	public static void Save()
	{
		prefs.Save();
	}
}
