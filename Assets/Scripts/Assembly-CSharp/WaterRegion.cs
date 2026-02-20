using UnityEngine;

public class WaterRegion : MonoBehaviour
{
	public float waterHeightOffset;

	public FishingEnvironment fishingEnvironment;

	private MeshCollider meshCollider;

	private Vector2 waveLength;

	private float waveSpeed;

	private float waveHeight;

	public bool isRiver { get; private set; }

	public Renderer waterRenderer { get; private set; }

	public WaterCurrent current { get; private set; }

	private void Awake()
	{
		waterRenderer = GetComponent<Renderer>();
		if (waterRenderer == null)
		{
			waterRenderer = GetComponentInParent<Renderer>();
		}
		meshCollider = GetComponent<MeshCollider>();
		current = GetComponent<WaterCurrent>();
		isRiver = meshCollider != null;
		if (!isRiver)
		{
			waveLength.x = waterRenderer.sharedMaterial.GetFloat("_WaveLengthX");
			waveLength.y = waterRenderer.sharedMaterial.GetFloat("_WaveLengthY");
			waveSpeed = waterRenderer.sharedMaterial.GetFloat("_WaveSpeed");
			waveHeight = waterRenderer.sharedMaterial.GetFloat("_WaveHeight");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		other.GetComponent<IFloater>()?.RegisterWaterRegion(this);
	}

	private void OnTriggerExit(Collider other)
	{
		other.GetComponent<IFloater>()?.UnregisterWaterRegion(this);
	}

	public float GetWaterY(Vector3 atPosition)
	{
		if (isRiver)
		{
			Vector3 origin = new Vector3(atPosition.x, meshCollider.bounds.max.y + 0.1f, atPosition.z);
			if (meshCollider.Raycast(new Ray(origin, Vector3.down), out var hitInfo, meshCollider.bounds.size.y))
			{
				return hitInfo.point.y + waterHeightOffset;
			}
			return -1000f;
		}
		float shaderTime = GlobalShaderParameters.shaderTime;
		float num = Mathf.Sin(atPosition.x * waveLength.x + atPosition.z * waveLength.y + shaderTime * waveSpeed);
		float num2 = waveHeight * num;
		return waterRenderer.transform.position.y + num2 + waterHeightOffset;
	}
}
