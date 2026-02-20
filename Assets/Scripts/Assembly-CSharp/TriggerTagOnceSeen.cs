using UnityEngine;

public class TriggerTagOnceSeen : MonoBehaviour
{
	private const float CHECK_TIME = 2.5f;

	public string tagRequired;

	public string tagToTrigger;

	private Timer checkTimer;

	private void OnBecameVisible()
	{
		if (!Singleton<GlobalData>.instance.gameData.tags.GetBool(tagToTrigger) && (string.IsNullOrEmpty(tagRequired) || Singleton<GlobalData>.instance.gameData.tags.GetBool(tagRequired)))
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(tagToTrigger);
		}
	}
}
