using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class PropertyBlockSetter : MonoBehaviour
{
	private static MaterialPropertyBlock sharedBlock;

	[FormerlySerializedAs("key")]
	public string floatKey = "_PalletOffset";

	[Range(0f, 1f)]
	[FormerlySerializedAs("value")]
	public float floatValue;

	public string colorKey = "";

	public Color colorValue = Color.white;

	private void Awake()
	{
		if (sharedBlock == null)
		{
			sharedBlock = new MaterialPropertyBlock();
		}
		sharedBlock.Clear();
		if (!string.IsNullOrEmpty(floatKey))
		{
			sharedBlock.SetFloat(floatKey, floatValue);
		}
		if (!string.IsNullOrEmpty(colorKey))
		{
			sharedBlock.SetColor(colorKey, colorValue);
		}
		GetComponent<Renderer>().SetPropertyBlock(sharedBlock);
	}
}
