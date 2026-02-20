using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	public class AudioClipMusicAsset : MusicStack.IMusicAsset
	{
		public class AudioClipMusicPlayer : MusicStack.IMusicPlayer
		{
			private AudioSource source;

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

			public AudioMixerGroup outputAudioMixerGroup
			{
				set
				{
					source.outputAudioMixerGroup = value;
				}
			}

			public AudioClipMusicPlayer(AudioClip clip)
			{
				GameObject gameObject = new GameObject("AudioClipMusicPlayer (" + clip?.ToString() + ")");
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.clip = clip;
				audioSource.loop = true;
				source = audioSource;
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

		private AudioClip clip;

		public AudioClipMusicAsset(AudioClip clip)
		{
			this.clip = clip;
		}

		public int GetMusicID()
		{
			return clip.GetInstanceID();
		}

		public MusicStack.IMusicPlayer CreatePlayer()
		{
			return new AudioClipMusicPlayer(clip);
		}
	}
}
