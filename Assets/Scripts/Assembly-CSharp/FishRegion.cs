using System;
using System.Collections.Generic;
using UnityEngine;

public class FishRegion : MonoBehaviour
{
	public const float WHILE_FISHING_CHANCE = 0.25f;

	public const float FISH_DISTANCE_SQR = 40000f;

	public FishingEnvironment fishingEnvironment;

	public WaterRegion waterRegion;

	[Header("Effects")]
	public int maxEffects = 3;

	public GameObject[] fishEffectPrefabs;

	public float centralizePosition = 0.5f;

	public float forceFishRadius;

	public Range updateFrequency = new Range(1f, 10f);

	private Camera mainCamera;

	private List<GameObject> currentEffects = new List<GameObject>();

	private Player player;

	private Action cachedUpdateFishRegionDelegate;

	private Timer timer;

	private void Start()
	{
		cachedUpdateFishRegionDelegate = UpdateFishRegion;
		timer = this.RegisterTimer(updateFrequency.Random(), cachedUpdateFishRegionDelegate);
		mainCamera = Camera.main;
		player = Singleton<GameServiceLocator>.instance.levelController.player;
	}

	private Vector3 GetEffectPosition(GameObject prefab)
	{
		Vector3 vector = base.transform.position;
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			vector = component.RandomWithin();
		}
		SphereCollider component2 = GetComponent<SphereCollider>();
		if ((bool)component2)
		{
			vector = component2.RandomWithin();
		}
		if ((bool)prefab.GetComponent<FishShadow>() && forceFishRadius > 0.01f)
		{
			vector = base.transform.position + (vector - base.transform.position).SetY(0f).normalized * forceFishRadius;
		}
		return vector + (base.transform.position - vector) * centralizePosition;
	}

	private void OnDrawGizmos()
	{
		if (forceFishRadius > 0.01f)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, forceFishRadius);
		}
	}

	private void UpdateFishRegion()
	{
		if (!((base.transform.position - mainCamera.transform.position).sqrMagnitude > 40000f) && mainCamera.IsPointInView(base.transform.position, 0.25f) && currentEffects.Count < maxEffects)
		{
			FishingActions fishingActions = player.heldItem?.GetComponent<FishingActions>();
			if (!fishingActions || !fishingActions.isCast || UnityEngine.Random.value < 0.25f)
			{
				GameObject gameObject = fishEffectPrefabs.PickRandom();
				GameObject currentEffect = gameObject.CloneAt(GetEffectPosition(gameObject));
				WaterDecal component = currentEffect.GetComponent<WaterDecal>();
				if ((bool)component)
				{
					component.region = waterRegion;
				}
				FishShadow component2 = currentEffect.GetComponent<FishShadow>();
				if ((bool)component2)
				{
					component2.Face(base.transform.position);
				}
				currentEffects.Add(currentEffect);
				currentEffect.RegisterOnDestroyCallback(delegate
				{
					currentEffects.Remove(currentEffect);
				});
			}
		}
		Timer.FlagToRecycle(timer);
		timer = this.RegisterTimer(updateFrequency.Random(), cachedUpdateFishRegionDelegate);
	}
}
