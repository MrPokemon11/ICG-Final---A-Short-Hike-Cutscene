using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class VirtualCameraRegion : MonoBehaviour
{
	private class CameraRegionUpdater : MonoBehaviour
	{
		public int MAX_REGIONS_PER_FRAME = 5;

		public static CameraRegionUpdater instance;

		public List<VirtualCameraRegion> objects = new List<VirtualCameraRegion>();

		public static CameraRegionUpdater Initalize()
		{
			if (instance == null)
			{
				instance = new GameObject("CameraRegionUpdater").AddComponent<CameraRegionUpdater>();
			}
			return instance;
		}

		private void Start()
		{
			StartCoroutine(IterativeUpdate());
		}

		private IEnumerator IterativeUpdate()
		{
			int num = 0;
			while (true)
			{
				for (int i = 0; i < objects.Count; i++)
				{
					VirtualCameraRegion virtualCameraRegion = objects[i];
					if (virtualCameraRegion.isActiveAndEnabled)
					{
						virtualCameraRegion.ManualUpdate();
						num++;
					}
					if (num > MAX_REGIONS_PER_FRAME)
					{
						yield return null;
						num = 0;
					}
				}
			}
		}
	}

	public CinemachineVirtualCameraBase virtualCamera;

	public List<Collider> additionalColliders = new List<Collider>();

	private Collider myCollider;

	private GameObject vCamGameObj;

	private Transform _cachedEditorTarget;

	private LevelController levelController;

	private Bounds cachedBounds;

	private List<Bounds> additionalCachedBounds = new List<Bounds>();

	private Transform target
	{
		get
		{
			if (Application.isPlaying)
			{
				Player player = levelController.player;
				if (!(player != null))
				{
					return null;
				}
				return player.transform;
			}
			if (_cachedEditorTarget == null)
			{
				_cachedEditorTarget = Object.FindObjectOfType<Player>().transform;
			}
			return _cachedEditorTarget;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			levelController = Singleton<ServiceLocator>.instance.Locate<LevelController>();
			CameraRegionUpdater.Initalize().objects.Add(this);
		}
		myCollider = GetComponent<Collider>();
		cachedBounds = myCollider.bounds;
		for (int i = 0; i < additionalColliders.Count; i++)
		{
			additionalCachedBounds.Add(additionalColliders[i].bounds);
		}
		virtualCamera.enabled = true;
		vCamGameObj = virtualCamera.gameObject;
	}

	private void OnEnable()
	{
		vCamGameObj.SetActive(ShouldActivateCamera());
	}

	private void OnDisable()
	{
		vCamGameObj.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (Application.isPlaying && CameraRegionUpdater.instance != null)
		{
			CameraRegionUpdater.instance.objects.Remove(this);
		}
	}

	private void ManualUpdate()
	{
		bool flag = ShouldActivateCamera();
		if (vCamGameObj.activeSelf != flag)
		{
			vCamGameObj.SetActive(flag);
		}
	}

	private bool ShouldActivateCamera()
	{
		bool flag = myCollider.IsPointWithin(target.transform.position, cachedBounds);
		int num = 0;
		while (!flag && num < additionalColliders.Count)
		{
			flag = additionalColliders[num].IsPointWithin(target.transform.position, additionalCachedBounds[num]);
			num++;
		}
		return flag;
	}
}
