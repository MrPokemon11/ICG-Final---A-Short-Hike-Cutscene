using System.Collections.Generic;
using UnityEngine;

public class MusicSetRegion : MonoBehaviour
{
	public MusicSet musicSet;

	public int priority;

	public MusicLayer[] baseMusicLayers;

	private StackResourceSortingKey musicKey;

	private List<SimpleMusicController> baseMusicLayerControllers = new List<SimpleMusicController>();

	private void Start()
	{
		MusicLayer[] array = baseMusicLayers;
		for (int i = 0; i < array.Length; i++)
		{
			SimpleMusicController simpleMusicController = new SimpleMusicController(array[i]);
			baseMusicLayerControllers.Add(simpleMusicController);
			Singleton<MusicManager>.instance.RegisterLayerController(simpleMusicController);
		}
	}

	private void OnDestroy()
	{
		if (!(Singleton<MusicManager>.instance != null))
		{
			return;
		}
		foreach (SimpleMusicController baseMusicLayerController in baseMusicLayerControllers)
		{
			Singleton<MusicManager>.instance.UnregisterLayerController(baseMusicLayerController);
		}
		if (musicKey != null)
		{
			Unregister();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>() && musicKey == null)
		{
			musicKey = Singleton<MusicManager>.instance.RegisterMusicSet(priority, musicSet);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<Player>() && musicKey != null)
		{
			Unregister();
		}
	}

	private void Unregister()
	{
		musicKey.ReleaseResource();
		musicKey = null;
	}
}
