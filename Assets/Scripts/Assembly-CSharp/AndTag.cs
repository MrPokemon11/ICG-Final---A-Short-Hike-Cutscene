using UnityEngine;

public class AndTag : MonoBehaviour
{
	public string[] tags;

	public string setTag;

	private void Start()
	{
		string[] array = tags;
		foreach (string text in array)
		{
			Singleton<GlobalData>.instance.gameData.tags.WatchBool(text, OnChange);
		}
	}

	private void OnDestroy()
	{
		string[] array = tags;
		foreach (string text in array)
		{
			Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(text, OnChange);
		}
	}

	private void OnChange(bool obj)
	{
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		bool value = true;
		string[] array = this.tags;
		foreach (string text in array)
		{
			if (!tags.GetBool(text))
			{
				value = false;
				break;
			}
		}
		tags.SetBool(setTag, value);
	}
}
