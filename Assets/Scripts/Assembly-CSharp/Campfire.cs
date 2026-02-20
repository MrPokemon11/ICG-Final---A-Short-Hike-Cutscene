using UnityEngine;

public class Campfire : MonoBehaviour, IWaterable
{
	private const string EXTINGUISH_TAG = "CampfiresExtinguished";

	private const int EXTINGUISH_ACHIEVEMENT_COUNT = 4;

	public ParticleSystem fireParticles;

	public ParticleSystem smokeParticles;

	public ParticleSystem trailParticles;

	public string dialogueNode;

	public Transform speaker;

	private Timer relightTimer;

	private float originalRate;

	private bool isLit;

	private void Start()
	{
		originalRate = smokeParticles.emission.rateOverTimeMultiplier;
		SetLit(lit: true);
	}

	public void SetLit(bool lit)
	{
		isLit = lit;
		ParticleSystem.EmissionModule emission = fireParticles.emission;
		emission.enabled = lit;
		emission = smokeParticles.emission;
		emission.rateOverTimeMultiplier = originalRate * (lit ? 1f : 0.25f);
		emission = trailParticles.emission;
		emission.enabled = !lit;
	}

	public void Water()
	{
		if (isLit)
		{
			SetLit(lit: false);
			if (!string.IsNullOrEmpty(dialogueNode))
			{
				Singleton<GameServiceLocator>.instance.dialogue.StartConversation(dialogueNode, speaker);
			}
			Timer.Cancel(relightTimer);
			relightTimer = this.RegisterTimer(5f, CheckToRelight, isLooped: true);
			int num = Singleton<GlobalData>.instance.gameData.tags.GetInt("CampfiresExtinguished") + 1;
			Singleton<GlobalData>.instance.gameData.tags.SetInt("CampfiresExtinguished", num);
			if (num == 4)
			{
				Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.ExtinguishCampfires);
			}
		}
	}

	private void CheckToRelight()
	{
		if (!Camera.main.IsPointInView(base.transform.position))
		{
			Timer.Cancel(relightTimer);
			SetLit(lit: true);
		}
	}
}
