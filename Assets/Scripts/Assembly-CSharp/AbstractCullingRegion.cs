using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractCullingRegion : MonoBehaviour
{
	private static readonly Color[] DISTANCE_COLORS = new Color[4]
	{
		Color.green,
		Color.yellow,
		new Color(1f, 0.5f, 0f),
		Color.red
	};

	public float radius = 10f;

	public Vector3[] offsetSpheres = new Vector3[0];

	public float[] offsetSphereRadius = new float[0];

	private CullingRegionSphere[] spheres;

	protected int viewDistance { get; private set; }

	protected bool isVisible { get; private set; }

	protected virtual void Awake()
	{
		if (!GameSettings.useCullingGroups)
		{
			base.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (spheres == null)
		{
			spheres = CreateSpheres().ToArray();
			CullingRegionSphere[] array = spheres;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].onStateChanged += OnSphereChanged;
			}
		}
		Singleton<GameServiceLocator>.instance.cullingManager.RegisterSpheres(spheres);
	}

	private void OnDisable()
	{
		if (spheres != null && (bool)Singleton<GameServiceLocator>.instance && (bool)Singleton<GameServiceLocator>.instance.cullingManager)
		{
			Singleton<GameServiceLocator>.instance.cullingManager.UnregisterSpheres(spheres);
		}
	}

	protected virtual void Start()
	{
		SetVisibility(visible: false);
	}

	private void OnSphereChanged(CullingRegionSphere sphere)
	{
		int num = DISTANCE_COLORS.Length;
		bool visibility = false;
		for (int i = 0; i < spheres.Length; i++)
		{
			if (spheres[i].viewDistance < num)
			{
				num = spheres[i].viewDistance;
			}
			if (spheres[i].isVisible)
			{
				visibility = true;
			}
		}
		SetVisibility(visibility);
		viewDistance = num;
	}

	public virtual void SetVisibility(bool visible)
	{
		isVisible = visible;
	}

	public IEnumerable<CullingRegionSphere> CreateSpheres()
	{
		yield return new CullingRegionSphere(base.transform.position, radius);
		for (int i = 0; i < offsetSpheres.Length; i++)
		{
			yield return new CullingRegionSphere(base.transform.position + offsetSpheres[i], GetSphereRadius(i));
		}
	}

	public float GetSphereRadius(int offsetSphereIndex)
	{
		if (offsetSphereIndex < offsetSphereRadius.Length)
		{
			return offsetSphereRadius[offsetSphereIndex];
		}
		return radius;
	}

	public void OnDrawGizmos()
	{
		DrawGizmos(Color.black);
	}

	public void OnDrawGizmosSelected()
	{
		DrawGizmos(Color.blue);
	}

	private void DrawGizmos(Color color)
	{
		if (base.enabled)
		{
			if (!Application.isPlaying)
			{
				spheres = CreateSpheres().ToArray();
			}
			Gizmos.color = (isVisible ? DISTANCE_COLORS[viewDistance] : color);
			CullingRegionSphere[] array = spheres;
			foreach (CullingRegionSphere cullingRegionSphere in array)
			{
				Gizmos.DrawWireSphere(cullingRegionSphere.position, cullingRegionSphere.radius);
			}
		}
	}
}
