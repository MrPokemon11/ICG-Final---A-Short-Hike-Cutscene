using System.Linq;
using UnityEngine;

public class SetTagOnceAllOffscreen : MonoBehaviour
{
	public string[] requiredTags;

	public Renderer[] requiredOffscreenRenderers;

	public string setTag;

	private Timer checkTimer;

	private void Start()
	{
		CheckForRequirements(checkRenderers: false);
		if (!Singleton<GlobalData>.instance.gameData.tags.GetBool(setTag))
		{
			RegisterWatchers();
		}
	}

	private void RegisterWatchers()
	{
		string[] array = requiredTags;
		foreach (string text in array)
		{
			Singleton<GlobalData>.instance.gameData.tags.WatchBool(text, OnTagChange);
		}
	}

	private void OnDestroy()
	{
		UnregisterWatchers();
	}

	private void UnregisterWatchers()
	{
		if (!(Singleton<GlobalData>.instance == null))
		{
			string[] array = requiredTags;
			foreach (string text in array)
			{
				Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(text, OnTagChange);
			}
		}
	}

	private void OnTagChange(bool value)
	{
		CheckForRequirements(checkRenderers: true);
	}

	private void CheckForRequirements(bool checkRenderers)
	{
		Tags tagData = Singleton<GlobalData>.instance.gameData.tags;
		if (requiredTags.All((string t) => tagData.GetBool(t)))
		{
			if (!checkRenderers || requiredOffscreenRenderers.All((Renderer r) => !r.isVisible))
			{
				TriggerTag();
				return;
			}
			Timer.Cancel(checkTimer);
			checkTimer = this.RegisterTimer(1f, DelayedCheckRenderers, isLooped: true);
		}
	}

	private void DelayedCheckRenderers()
	{
		if (requiredOffscreenRenderers.All((Renderer r) => !r.isVisible))
		{
			TriggerTag();
		}
	}

	private void TriggerTag()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool(setTag);
		UnregisterWatchers();
		Timer.Cancel(checkTimer);
	}
}
