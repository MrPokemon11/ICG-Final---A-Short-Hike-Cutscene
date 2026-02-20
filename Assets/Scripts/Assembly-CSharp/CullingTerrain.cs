using UnityEngine;

public class CullingTerrain : AbstractCullingRegion
{
	public static float ERROR_MULTIPLIER = 1f;

	private static readonly float[] ERROR_LEVELS = new float[4] { 10f, 50f, 1000f, 500f };

	public Terrain terrain;

	protected override void Awake()
	{
		base.Awake();
		if (GameSettings.useBakedTerrain)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		terrain.enabled = base.isVisible;
		if (base.viewDistance < ERROR_LEVELS.Length && base.viewDistance >= 0)
		{
			terrain.heightmapPixelError = ERROR_LEVELS[base.viewDistance] * ERROR_MULTIPLIER;
		}
		else
		{
			Debug.LogWarning("Invalid distance: " + base.viewDistance);
		}
	}
}
