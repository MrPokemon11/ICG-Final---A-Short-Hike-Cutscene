using System.Collections.Generic;
using UnityEngine;

public class DistanceMusicLayerController : MonoBehaviour, IMusicLayerController
{
	private class DistanceMusicLayerControllerUpdater : MonoBehaviour
	{
		public static DistanceMusicLayerControllerUpdater instance;

		public List<DistanceMusicLayerController> objects = new List<DistanceMusicLayerController>();

		public static DistanceMusicLayerControllerUpdater Initalize()
		{
			if (instance == null)
			{
				instance = new GameObject("DistanceMusicLayerControllerUpdater").AddComponent<DistanceMusicLayerControllerUpdater>();
			}
			return instance;
		}

		private void Update()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				DistanceMusicLayerController distanceMusicLayerController = objects[i];
				if (distanceMusicLayerController.isActiveAndEnabled)
				{
					distanceMusicLayerController.ManualUpdate();
				}
			}
		}
	}

	public float distance;

	public AnimationCurve falloffCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private MusicLayer _musicLayer;

	private Player player;

	private bool isActivated;

	private float _volume;

	public MusicLayer musicLayer => _musicLayer;

	public float volume => _volume;

	public void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		DistanceMusicLayerControllerUpdater.Initalize().objects.Add(this);
	}

	private void OnDestroy()
	{
		if (Singleton<MusicManager>.instance != null)
		{
			Singleton<MusicManager>.instance.UnregisterLayerController(this);
		}
		if (DistanceMusicLayerControllerUpdater.instance != null)
		{
			DistanceMusicLayerControllerUpdater.instance.objects.Remove(this);
		}
	}

	private void ManualUpdate()
	{
		float sqrMagnitude = (base.transform.position - player.transform.position).sqrMagnitude;
		bool flag = sqrMagnitude < distance.Sqr();
		if (flag && !isActivated)
		{
			Singleton<MusicManager>.instance.RegisterLayerController(this);
			isActivated = true;
		}
		else if (!flag && isActivated)
		{
			Singleton<MusicManager>.instance.UnregisterLayerController(this);
			isActivated = false;
			_volume = 0f;
		}
		if (isActivated)
		{
			_volume = falloffCurve.Evaluate(1f - Mathf.Sqrt(sqrMagnitude) / distance);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, distance);
	}
}
