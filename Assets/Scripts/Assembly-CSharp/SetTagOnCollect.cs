using UnityEngine;

public class SetTagOnCollect : MonoBehaviour
{
	public string dataTag;

	private void Start()
	{
		GetComponent<ICollectable>().onCollect += OnCollect;
	}

	private void OnCollect()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool(dataTag);
	}
}
