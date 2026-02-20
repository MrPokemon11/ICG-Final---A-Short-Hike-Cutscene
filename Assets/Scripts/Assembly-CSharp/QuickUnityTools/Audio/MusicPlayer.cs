using System;
using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	[ResourceSingleton("MusicPlayer")]
	[Obsolete("The MusicPlayerStack is the newer version of this class.")]
	public class MusicPlayer : Singleton<MusicPlayer>
	{
		private AudioSource fadeOutAudio;

		public AudioMixerGroup musicMixerGroup;

		public float fadeInRate = 0.25f;

		public float fadeOutRate = 0.5f;

		private float _musicVolume = 0.45f;

		public AudioSource currentAudio { get; private set; }

		private bool isFadingOutAudioClip => fadeOutAudio != null;

		public float musicVolume
		{
			get
			{
				return _musicVolume;
			}
			set
			{
				_musicVolume = value;
				currentAudio.volume = _musicVolume;
				fadeOutAudio.volume = 0f;
			}
		}

		private void Update()
		{
			if (fadeOutAudio != null)
			{
				if (fadeOutAudio.volume > 0.1f * musicVolume)
				{
					fadeOutAudio.volume -= fadeOutRate * musicVolume * Time.deltaTime;
				}
				else
				{
					UnityEngine.Object.Destroy(fadeOutAudio);
					fadeOutAudio = null;
				}
			}
			if (currentAudio != null && currentAudio.volume < musicVolume)
			{
				currentAudio.volume = Mathf.MoveTowards(currentAudio.volume, musicVolume, fadeInRate * Time.deltaTime);
			}
		}

		public void TransitionToMusic(AudioClip newMusic)
		{
			if (currentAudio != null && currentAudio.clip == newMusic)
			{
				return;
			}
			bool flag = currentAudio != null;
			if (currentAudio != null)
			{
				BeginMusicFadeOut();
			}
			if (!(newMusic == null))
			{
				currentAudio = base.gameObject.AddComponent<AudioSource>();
				currentAudio.clip = newMusic;
				currentAudio.Play();
				currentAudio.outputAudioMixerGroup = musicMixerGroup;
				currentAudio.loop = true;
				if (flag)
				{
					currentAudio.volume = 0.1f * musicVolume;
				}
			}
		}

		public void FadeOutMusic()
		{
			TransitionToMusic(null);
		}

		public void StopMusic()
		{
			if (currentAudio != null)
			{
				UnityEngine.Object.Destroy(currentAudio);
				currentAudio = null;
			}
			if (isFadingOutAudioClip)
			{
				UnityEngine.Object.Destroy(fadeOutAudio);
				fadeOutAudio = null;
			}
		}

		private void BeginMusicFadeOut()
		{
			if (isFadingOutAudioClip)
			{
				UnityEngine.Object.Destroy(fadeOutAudio);
				fadeOutAudio = null;
			}
			fadeOutAudio = currentAudio;
			currentAudio = null;
		}
	}
}
