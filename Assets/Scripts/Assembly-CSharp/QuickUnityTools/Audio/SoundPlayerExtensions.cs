using UnityEngine;

namespace QuickUnityTools.Audio
{
	public static class SoundPlayerExtensions
	{
		public static AudioSource Play(this AudioClip clip)
		{
			return Singleton<SoundPlayer>.instance.Play(clip);
		}
	}
}
