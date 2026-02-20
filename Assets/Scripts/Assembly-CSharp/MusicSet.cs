using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Music/Music Set")]
public class MusicSet : ScriptableObject
{
	public float fadeInTime = 2f;

	public float fadeOutTime = 2f;

	public float silenceAfterFinishing = 5f;

	public bool playOnce;

	[FormerlySerializedAs("unitLayer")]
	public MusicLayer baseLayer;

	public MusicLayer[] layers;

	public static MusicSet Load(string name)
	{
		return Resources.Load<MusicSet>(name);
	}
}
