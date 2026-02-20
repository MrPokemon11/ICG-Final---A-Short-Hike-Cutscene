using UnityEngine;

[RequireComponent(typeof(GameObjectID))]
public class CoinDispenser : MonoBehaviour, IShovelWhackable
{
	private const string TAG = "COIN_DISPENSER";

	public GameObject[] spawnPrefabs;

	public int totalDispenses;

	public AudioClip hitSound;

	public string hitTrigger;

	public ParticleSystem particles;

	public string hitOnceTag = "HitMagicRock";

	private int dispenses;

	private GameObjectID id;

	private Animator animator;

	private void Start()
	{
		id = GetComponent<GameObjectID>();
		animator = GetComponent<Animator>();
		dispenses = id.GetIntForID("COIN_DISPENSER");
		UpdateEffects();
	}

	public void WhackWithShovel()
	{
		if (dispenses < totalDispenses)
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(hitOnceTag);
			Collider component = GetComponent<Collider>();
			Vector3 spawnPosition = component.bounds.center + component.bounds.extents.y * Vector3.up;
			Chest.SpawnRewards(spawnPrefabs, spawnPosition, null, 35f);
			dispenses++;
			id.SaveIntForID("COIN_DISPENSER", dispenses);
			animator.SetTrigger(hitTrigger);
			UpdateEffects();
		}
	}

	private void UpdateEffects()
	{
		if (dispenses >= totalDispenses)
		{
			particles?.gameObject.SetActive(value: false);
		}
	}
}
