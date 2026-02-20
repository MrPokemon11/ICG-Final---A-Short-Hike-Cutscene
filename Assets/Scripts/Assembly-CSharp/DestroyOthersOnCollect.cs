using UnityEngine;

public class DestroyOthersOnCollect : MonoBehaviour
{
	public string sharedTag;

	private bool dontDestroySelf;

	private void Start()
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(sharedTag))
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GetComponent<ICollectable>().onCollect += OnCollect;
		Singleton<GlobalData>.instance.gameData.tags.WatchBool(sharedTag, OnTagChanged);
	}

	private void OnTagChanged(bool value)
	{
		if (value && !dontDestroySelf)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnCollect()
	{
		dontDestroySelf = true;
		Singleton<GlobalData>.instance.gameData.tags.SetBool(sharedTag);
	}
}
