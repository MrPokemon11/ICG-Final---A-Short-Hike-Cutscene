using UnityEngine;

namespace QuickUnityTools.Audio
{
	[CreateAssetMenu]
	public class SmoothLoopAudioClip : ScriptableObject
	{
		public AudioClip clip;

		public float introMeasures = 1f;

		public float beatsPerMeasure = 4f;

		public float beatsPerMinute = 130f;

		public float length => clip.length;
	}
}
