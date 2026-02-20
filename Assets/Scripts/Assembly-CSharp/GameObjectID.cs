using UnityEngine;

public class GameObjectID : MonoBehaviour
{
	public string id;

	public void SaveBoolForID(string prefix, bool value)
	{
		if (!string.IsNullOrEmpty(id))
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(prefix + id, value);
		}
	}

	public bool GetBoolForID(string prefix)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return Singleton<GlobalData>.instance.gameData.tags.GetBool(prefix + id);
		}
		return false;
	}

	public void SaveIntForID(string prefix, int value)
	{
		if (!string.IsNullOrEmpty(id))
		{
			Singleton<GlobalData>.instance.gameData.tags.SetInt(prefix + id, value);
		}
	}

	public int GetIntForID(string prefix)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return Singleton<GlobalData>.instance.gameData.tags.GetInt(prefix + id);
		}
		return 0;
	}
}
