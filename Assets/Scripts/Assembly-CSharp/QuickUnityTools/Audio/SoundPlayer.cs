using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	[ResourceSingleton("SoundPlayer")]
	public class SoundPlayer : Singleton<SoundPlayer>
	{
		public AudioMixerGroup soundMixerGroup;

		private Dictionary<string, AudioClip> loadedSounds = new Dictionary<string, AudioClip>();

		public AudioSource PlayVaried(AudioClip soundName, float spread = 0.3f)
		{
			AudioSource audioSource = Play(soundName);
			audioSource.pitch += UnityEngine.Random.Range((0f - spread) / 2f, spread / 2f);
			return audioSource;
		}

		public AudioSource Play(AudioClip clip, float volume = 1f)
		{
			return Play(clip, base.transform.position, volume);
		}

		public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 0f)
		{
			return PlayOneOff(clip, position, volume, spatialBlend);
		}

		public AudioSource PlayLooped(AudioClip clip, Vector3 position)
		{
			AudioSource audioSource = CreateAudioObject(clip, position);
			audioSource.loop = true;
			audioSource.Play();
			return audioSource;
		}

		private AudioSource PlayOneOff(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 0f)
		{
			if (clip == null)
			{
				Debug.LogWarning("Audio clip is not assigned to a value!");
				return null;
			}
			AudioSource audioSource = CreateAudioObject(clip, position);
			audioSource.volume = volume;
			audioSource.spatialBlend = spatialBlend;
			audioSource.outputAudioMixerGroup = soundMixerGroup;
			audioSource.Play();
			Timer.Register(clip.length + 0.1f, delegate
			{
				if (audioSource != null)
				{
					UnityEngine.Object.Destroy(audioSource.gameObject);
				}
			}, isLooped: false, useUnscaledTime: true);
			return audioSource;
		}

		private AudioSource CreateAudioObject(AudioClip clip, Vector3 position)
		{
			GameObject obj = new GameObject("AudioSource (Temp)");
			obj.transform.position = position;
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			audioSource.clip = clip;
			return audioSource;
		}

		[Obsolete]
		public AudioSource Play(string soundName)
		{
			return Play(LookUpAudioClip(soundName));
		}

		[Obsolete]
		public AudioSource PlayVaried(string soundName, float spread = 0.3f)
		{
			return PlayVaried(LookUpAudioClip(soundName), spread);
		}

		private AudioClip LookUpAudioClip(string soundName)
		{
			if (loadedSounds.ContainsKey(soundName))
			{
				return loadedSounds[soundName];
			}
			AudioClip audioClip = Resources.Load<AudioClip>(soundName);
			if (audioClip == null)
			{
				Debug.LogWarning("Tried to play sound from string, but failed: " + soundName);
				return null;
			}
			loadedSounds.Add(soundName, audioClip);
			return audioClip;
		}
	}
}
