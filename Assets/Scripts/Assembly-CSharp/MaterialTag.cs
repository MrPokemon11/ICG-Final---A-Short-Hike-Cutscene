using UnityEngine;

public class MaterialTag : MonoBehaviour
{
	public string materialIndexTag;

	public Material[] materialList;

	public int rendererMaterialIndex;

	private void Start()
	{
		UpdateHatColor(Singleton<GlobalData>.instance.gameData.tags.GetFloat(materialIndexTag));
		Singleton<GlobalData>.instance.gameData.tags.WatchFloat(materialIndexTag, UpdateHatColor);
	}

	private void UpdateHatColor(float colorIndex)
	{
		Renderer component = GetComponent<Renderer>();
		int num = (int)colorIndex;
		Material material = materialList[num];
		Material[] materials = component.materials;
		materials[rendererMaterialIndex] = material;
		component.materials = materials;
	}

	private void OnDestroy()
	{
		Singleton<GlobalData>.instance.gameData.tags.UnwatchFloat(materialIndexTag, UpdateHatColor);
	}
}
