using UnityEngine;

public class WaterTrail : MonoBehaviour
{
	public float yOffset = 0.4f;

	public ParticleSystem splashParticles;

	public bool adoptColor = true;

	private WaterRegion _region;

	public TrailRenderer trail { get; private set; }

	public WaterRegion region
	{
		get
		{
			return _region;
		}
		set
		{
			_region = value;
			Material material = _region.waterRenderer.material;
			if (!region.isRiver)
			{
				SetWaterProperties(material);
			}
		}
	}

	private void Awake()
	{
		trail = GetComponent<TrailRenderer>();
	}

	private void LateUpdate()
	{
		if ((bool)region)
		{
			if (!region.isRiver)
			{
				base.transform.position = base.transform.position.SetY(region.transform.position.y + yOffset);
			}
			else
			{
				base.transform.position = base.transform.position.SetY(region.GetWaterY(base.transform.position) + yOffset);
			}
		}
	}

	public void SetWaterProperties(Material waterMaterial)
	{
		Renderer componentInChildren = GetComponentInChildren<Renderer>();
		if (adoptColor)
		{
			componentInChildren.material.SetColor("_Color", waterMaterial.GetColor("_FoamColor"));
		}
		componentInChildren.material.SetFloat("_WaveHeight", waterMaterial.GetFloat("_WaveHeight"));
		componentInChildren.material.SetFloat("_WaveSpeed", waterMaterial.GetFloat("_WaveSpeed"));
		componentInChildren.material.SetFloat("_WaveLengthX", waterMaterial.GetFloat("_WaveLengthX"));
		componentInChildren.material.SetFloat("_WaveLengthY", waterMaterial.GetFloat("_WaveLengthY"));
	}

	public void Kill()
	{
		TrailRenderer component = GetComponent<TrailRenderer>();
		component.emitting = false;
		component.autodestruct = true;
		if ((bool)splashParticles)
		{
			splashParticles.Stop();
		}
	}
}
