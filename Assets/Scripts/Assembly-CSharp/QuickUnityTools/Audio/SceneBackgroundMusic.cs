using UnityEngine;

namespace QuickUnityTools.Audio
{
	public class SceneBackgroundMusic : MonoBehaviour
	{
		public BasicMusicStackElement musicConfig;

		private PrioritySortingKey musicStackKey;

		private void Start()
		{
			musicStackKey = Singleton<MusicStack>.instance.AddToMusicStack(musicConfig);
		}

		private void OnDestroy()
		{
			if (Singleton<MusicStack>.instance != null)
			{
				Singleton<MusicStack>.instance.RemoveFromMusicStack(musicStackKey);
			}
		}
	}
}
