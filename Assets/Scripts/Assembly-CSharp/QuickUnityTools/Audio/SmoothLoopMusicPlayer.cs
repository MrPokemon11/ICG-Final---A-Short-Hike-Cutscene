using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	public class SmoothLoopMusicPlayer : MusicStack.IMusicPlayer
	{
		private SmoothLoopAudioSource source;

		public AudioMixerGroup outputAudioMixerGroup
		{
			set
			{
				source.mixerGroup = value;
			}
		}

		public float volume
		{
			get
			{
				return source.volume;
			}
			set
			{
				source.volume = value;
			}
		}

		public bool isPlaying => source.isPlaying;

		public SmoothLoopMusicPlayer(SmoothLoopAudioClip clip)
		{
			GameObject gameObject = new GameObject("SmoothLoopMusicPlayer (" + clip?.ToString() + ")");
			SmoothLoopAudioSource smoothLoopAudioSource = gameObject.AddComponent<SmoothLoopAudioSource>();
			smoothLoopAudioSource.music = clip;
			source = smoothLoopAudioSource;
			Object.DontDestroyOnLoad(gameObject);
		}

		public void CleanUp()
		{
			Object.Destroy(source.gameObject);
		}

		public void Play()
		{
			source.Play();
		}

		public void Stop()
		{
			source.Stop();
		}
	}
}
