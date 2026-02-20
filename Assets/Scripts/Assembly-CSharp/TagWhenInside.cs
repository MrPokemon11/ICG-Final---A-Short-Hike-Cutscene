using UnityEngine;

public class TagWhenInside : MonoBehaviour
{
	public string tagID;

	public Rigidbody detectBody;

	private void Awake()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool(tagID, value: false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Rigidbody>() == detectBody && detectBody != null)
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(tagID);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponentInParent<Rigidbody>() == detectBody && detectBody != null)
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(tagID, value: false);
		}
	}
}
