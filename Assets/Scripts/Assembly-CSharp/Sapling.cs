using UnityEngine;

public class Sapling : MonoBehaviour, IWaterable
{
	public const string TOTAL_WATERED = "TOTAL_SPROUTS_DUDE";

	public const string TAG = "Sapling";

	public GameObject sapling;

	public GameObject flower;

	public Collider trigger;

	public ParticleSystem poof;

	public float activationDelay = 0.5f;

	public AudioClip growSound;

	private GameObjectID id;

	private bool activated;

	private void Start()
	{
		id = GetComponent<GameObjectID>();
		if (id.GetBoolForID("Sapling"))
		{
			ActivateFlower();
			return;
		}
		sapling.SetActive(value: true);
		flower.SetActive(value: false);
	}

	private void ActivateFlower()
	{
		sapling.SetActive(value: false);
		flower.SetActive(value: true);
		trigger.enabled = false;
		activated = true;
	}

	public void Water()
	{
		if (!activated)
		{
			activated = true;
			id.SaveBoolForID("Sapling", value: true);
			Tags tags = Singleton<GlobalData>.instance.gameData.tags;
			tags.SetInt("TOTAL_SPROUTS_DUDE", tags.GetInt("TOTAL_SPROUTS_DUDE") + 1);
			this.RegisterTimer(activationDelay, delegate
			{
				poof.Play();
				growSound.Play();
				GetComponent<Animator>().SetTrigger("Grow");
			});
		}
	}
}
