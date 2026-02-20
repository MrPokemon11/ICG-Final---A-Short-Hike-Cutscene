using UnityEngine;

public class CullingRegion : AbstractCullingRegion
{
	public Renderer[] renderers;

	public override void SetVisibility(bool visible)
	{
		base.SetVisibility(visible);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (renderer != null)
			{
				renderer.enabled = visible;
			}
		}
	}
}
