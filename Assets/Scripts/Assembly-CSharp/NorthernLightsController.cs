using UnityEngine;

public class NorthernLightsController : MonoBehaviour
{
	public Renderer[] meshes;

	public float fadeTime = 2f;

	private float brightness;

	private MaterialPropertyBlock propertyBlock;

	public bool enableLights { get; set; }

	private void Awake()
	{
		propertyBlock = new MaterialPropertyBlock();
		DisableMeshes();
	}

	private void LateUpdate()
	{
		float num = (enableLights ? 1 : 0);
		if (brightness != num)
		{
			brightness = Mathf.MoveTowards(brightness, num, Time.deltaTime / fadeTime);
			if (brightness > 0f && !meshes[0].enabled)
			{
				EnableMeshes();
			}
			else if (brightness == 0f && meshes[0].enabled)
			{
				DisableMeshes();
			}
			Renderer[] array = meshes;
			foreach (Renderer obj in array)
			{
				obj.GetPropertyBlock(propertyBlock);
				Color color = propertyBlock.GetColor("_Color");
				color = color.SetA(brightness);
				propertyBlock.SetColor("_Color", color);
				obj.SetPropertyBlock(propertyBlock);
			}
		}
	}

	private void EnableMeshes()
	{
		Renderer[] array = meshes;
		foreach (Renderer mesh in array)
		{
			mesh.enabled = true;
			this.RegisterTimer(0.01f, delegate
			{
				mesh.GetComponent<Animator>().enabled = true;
			});
		}
	}

	private void DisableMeshes()
	{
		Renderer[] array = meshes;
		foreach (Renderer obj in array)
		{
			obj.enabled = false;
			obj.GetComponent<Animator>().enabled = false;
		}
	}
}
