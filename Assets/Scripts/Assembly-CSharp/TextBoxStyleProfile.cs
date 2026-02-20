using UnityEngine;

[CreateAssetMenu]
public class TextBoxStyleProfile : ScriptableObject
{
	public float pitch = 1f;

	public AudioClip[] customBeepClips = new AudioClip[0];

	public Color boxColor = Color.white;

	public Color textColor = Color.black;

	public static TextBoxStyleProfile Load(string name)
	{
		return Resources.Load<TextBoxStyleProfile>("TextBoxProfiles/" + name);
	}
}
