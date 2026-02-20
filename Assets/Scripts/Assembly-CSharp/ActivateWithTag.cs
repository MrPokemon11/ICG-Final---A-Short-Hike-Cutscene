using UnityEngine;

public class ActivateWithTag : MonoBehaviour
{
	public enum Configuration
	{
		DeactivateWhenOn = 0,
		ActivateWhenOn = 1
	}

	public string tagName;

	public Configuration setting;

	public string overrideTagName;

	public Configuration overrideSetting;

	private void Start()
	{
		if (!string.IsNullOrEmpty(overrideTagName))
		{
			Singleton<GlobalData>.instance.gameData.tags.WatchBool(overrideTagName, OnChange);
		}
		if (string.IsNullOrEmpty(tagName))
		{
			Debug.LogWarning("It's empty!", this);
			return;
		}
		Singleton<GlobalData>.instance.gameData.tags.WatchBool(tagName, OnChange);
		UpdateActive();
	}

	private void OnDestroy()
	{
		if (!(Singleton<GlobalData>.instance == null))
		{
			if (!string.IsNullOrEmpty(tagName))
			{
				Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(tagName, OnChange);
			}
			if (!string.IsNullOrEmpty(overrideTagName))
			{
				Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(tagName, OnChange);
			}
		}
	}

	private void OnChange(bool tagValue)
	{
		UpdateActive();
	}

	private void UpdateActive()
	{
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		bool flag = tags.GetBool(tagName);
		bool active = ((setting == Configuration.ActivateWhenOn) ? flag : (!flag));
		if (!string.IsNullOrEmpty(overrideTagName) && tags.GetBool(overrideTagName))
		{
			active = overrideSetting == Configuration.ActivateWhenOn;
		}
		base.gameObject.SetActive(active);
	}
}
