using System.Collections.Generic;
using UnityEngine;

public class FeatherUIController : MonoBehaviour
{
	public GameObject featherUIPrefab;

	public CollectableItem goldenFeatherItem;

	public float featherFreezeAnimationInterval = 0.08f;

	public AudioClip freezeClip;

	public float featherShakeStamina = 0.5f;

	private List<FeatherUI> featherUIs = new List<FeatherUI>();

	private Player player;

	private bool prevFeathersRegenerate = true;

	private float prevFeatherStamina;

	public void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		Singleton<GlobalData>.instance.gameData.WatchCollected(goldenFeatherItem, OnGoldenFeathersChanged);
		OnGoldenFeathersChanged(Singleton<GlobalData>.instance.gameData.GetCollected(goldenFeatherItem));
	}

	private void Update()
	{
		for (int i = 0; i < featherUIs.Count; i++)
		{
			float fill = 0f;
			if (i == player.feathers - 1)
			{
				fill = player.featherStamina;
			}
			else if (i < player.feathers)
			{
				fill = 1f;
			}
			FeatherUI featherUI = featherUIs[i];
			featherUI.SetDisabled(i >= player.allowedFeathers);
			featherUI.SetFill(fill);
			featherUI.Shaking(player.feathersRegenerate && player.feathers == 0 && player.featherStamina < featherShakeStamina);
			featherUI.ManualUpdate();
		}
		if (player.feathersRegenerate != prevFeathersRegenerate)
		{
			UpdateFeathersFrozen();
			if (!player.feathersRegenerate)
			{
				freezeClip.Play();
			}
		}
		prevFeathersRegenerate = player.feathersRegenerate;
	}

	private void UpdateFeathersFrozen()
	{
		for (int i = 0; i < featherUIs.Count; i++)
		{
			int cachedIndex = i;
			this.RegisterTimer(featherFreezeAnimationInterval * (float)i, delegate
			{
				if (cachedIndex < featherUIs.Count)
				{
					featherUIs[cachedIndex].SetFrozen(!player.feathersRegenerate);
				}
			});
		}
	}

	private void OnDestroy()
	{
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.gameData.UnwatchCollected(goldenFeatherItem, OnGoldenFeathersChanged);
		}
	}

	private void OnGoldenFeathersChanged(int number)
	{
		if (number < featherUIs.Count)
		{
			foreach (FeatherUI featherUI in featherUIs)
			{
				Object.Destroy(featherUI.gameObject);
			}
			featherUIs.Clear();
		}
		while (featherUIs.Count < number)
		{
			GameObject gameObject = featherUIPrefab.Clone();
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			featherUIs.Add(gameObject.GetComponent<FeatherUI>());
		}
		UpdateFeathersFrozen();
	}
}
