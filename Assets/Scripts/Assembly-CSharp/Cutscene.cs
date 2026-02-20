using System.Collections;
using UnityEngine;

public abstract class Cutscene : MonoBehaviour
{
	public string playOnceTag;

	public virtual void Start()
	{
		if (!string.IsNullOrEmpty(playOnceTag) && Singleton<GlobalData>.instance.gameData.tags.GetBool(playOnceTag))
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (!string.IsNullOrEmpty(playOnceTag))
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(playOnceTag);
		}
		StartCoroutine(CutsceneRoutine());
	}

	protected abstract IEnumerator CutsceneRoutine();
}
