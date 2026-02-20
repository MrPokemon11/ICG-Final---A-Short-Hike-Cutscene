using UnityEngine;

public class WaterDecal : MonoBehaviour
{
	public float yOffset = 0.4f;

	public float driftSpeed = 1f;

	public float adoptColorFactor = 1f;

	public bool adoptFoamColor = true;

	public bool randomRotation = true;

	private WaterRegion _region;

	private Vector3 velocity;

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
				base.transform.position = base.transform.position.SetY(region.transform.position.y + yOffset);
				return;
			}
			base.transform.position = base.transform.position.SetY(region.GetWaterY(base.transform.position) + yOffset);
			WaterCurrent component = region.GetComponent<WaterCurrent>();
			if ((bool)component)
			{
				velocity = component.GetCurrent(base.transform.position) * driftSpeed;
			}
		}
	}

	private void Start()
	{
		if (randomRotation)
		{
			base.transform.eulerAngles = base.transform.eulerAngles.SetY(Random.value * 360f);
		}
	}

	private void Update()
	{
		base.transform.position += velocity * Time.deltaTime;
	}

	public void OnAnimationFinish()
	{
		Object.Destroy(base.gameObject);
	}

	public void SetWaterProperties(Material waterMaterial)
	{
		Renderer componentInChildren = GetComponentInChildren<Renderer>();
		if (adoptColorFactor > 0f)
		{
			Color color = (adoptFoamColor ? waterMaterial.GetColor("_FoamColor") : waterMaterial.GetColor("_Color"));
			Vector4 vector = Vector4.Lerp(componentInChildren.material.GetColor("_Color"), color, adoptColorFactor);
			componentInChildren.material.SetColor("_Color", vector);
		}
		componentInChildren.material.SetFloat("_WaveHeight", waterMaterial.GetFloat("_WaveHeight"));
		componentInChildren.material.SetFloat("_WaveSpeed", waterMaterial.GetFloat("_WaveSpeed"));
		componentInChildren.material.SetFloat("_WaveLengthX", waterMaterial.GetFloat("_WaveLengthX"));
		componentInChildren.material.SetFloat("_WaveLengthY", waterMaterial.GetFloat("_WaveLengthY"));
	}
}
