using UnityEngine;

public class TriggerTag : MonoBehaviour
{
	public string boolTag;

	public bool destroyAfterTriggered;

	private void Start()
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(boolTag) && destroyAfterTriggered)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>())
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(boolTag);
			if (destroyAfterTriggered)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
