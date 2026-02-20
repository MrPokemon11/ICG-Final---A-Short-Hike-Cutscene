using UnityEngine;

public class TriggerTagOnceOffscreen : MonoBehaviour
{
	private const float CHECK_TIME = 2.5f;

	public string tagRequired;

	public string tagToTrigger;

	public float requiredDistance = 100f;

	private Timer checkTimer;

	private void OnBecameInvisible()
	{
		if (!(Singleton<GlobalData>.instance == null) && !Singleton<GlobalData>.instance.gameData.tags.GetBool(tagToTrigger) && Singleton<GlobalData>.instance.gameData.tags.GetBool(tagRequired))
		{
			Timer.Cancel(checkTimer);
			checkTimer = this.RegisterTimer(2.5f, OnCheck, isLooped: true);
		}
	}

	private void OnCheck()
	{
		if ((Singleton<GameServiceLocator>.instance.levelController.player.transform.position - base.transform.position).sqrMagnitude > requiredDistance.Sqr())
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(tagToTrigger);
			Timer.Cancel(checkTimer);
		}
	}
}
