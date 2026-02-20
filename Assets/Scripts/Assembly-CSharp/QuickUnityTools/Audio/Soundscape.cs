using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	public class Soundscape : MonoBehaviour
	{
		[Serializable]
		public class SoundscapeLayer
		{
			public AudioClip clip;

			public float volume = 1f;

			public bool fadeIn = true;

			public AudioSource existingSource;
		}

		private class AmbientSoundManager : Singleton<AmbientSoundManager>
		{
			private static SoundscapeLayer[] EMPTY_LIST = new SoundscapeLayer[0];

			private bool initalized;

			private AudioMixerGroup mixerGroup;

			private float fadeTime = 1f;

			private Dictionary<AudioClip, AudioSource> audioSources = new Dictionary<AudioClip, AudioSource>();

			private List<Coroutine> soundFades = new List<Coroutine>();

			private SortedList<PrioritySortingKey, Soundscape> ambientSoundsStack = new SortedList<PrioritySortingKey, Soundscape>();

			private IList<Soundscape> ambientSoundsList;

			private SoundscapeLayer[] currentSounds
			{
				get
				{
					if (ambientSoundsList.Count <= 0)
					{
						return null;
					}
					return ambientSoundsList[0].soundsInScene;
				}
			}

			public void Initalize(Soundscape soundGroup)
			{
				if (!initalized)
				{
					mixerGroup = soundGroup.mixerGroup;
					ambientSoundsList = ambientSoundsStack.Values;
					UpdateSounds(soundGroup.soundsInScene);
					initalized = true;
				}
			}

			public void RemoveSoundscape(PrioritySortingKey key)
			{
				ambientSoundsStack.Remove(key);
				UpdateSounds(currentSounds);
			}

			public PrioritySortingKey AddSoundscape(Soundscape sounds)
			{
				PrioritySortingKey prioritySortingKey = new PrioritySortingKey(sounds.priority);
				ambientSoundsStack.Add(prioritySortingKey, sounds);
				UpdateSounds(currentSounds);
				return prioritySortingKey;
			}

			public void UpdateSounds(SoundscapeLayer[] newSounds)
			{
				CancelPreviousFades();
				if (newSounds == null)
				{
					newSounds = EMPTY_LIST;
				}
				SoundscapeLayer[] array = newSounds;
				foreach (SoundscapeLayer soundscapeLayer in array)
				{
					if (!audioSources.ContainsKey(soundscapeLayer.clip))
					{
						AudioSource audioSource = ((soundscapeLayer.existingSource != null) ? soundscapeLayer.existingSource : new GameObject(soundscapeLayer.clip.name).AddComponent<AudioSource>());
						audioSource.transform.parent = base.transform;
						audioSource.clip = soundscapeLayer.clip;
						audioSource.volume = (soundscapeLayer.fadeIn ? 0f : soundscapeLayer.volume);
						audioSource.loop = true;
						audioSource.outputAudioMixerGroup = mixerGroup;
						audioSource.Play();
						audioSources[soundscapeLayer.clip] = audioSource;
					}
					AudioSource audioSource2 = audioSources[soundscapeLayer.clip];
					if (audioSource2.volume != soundscapeLayer.volume)
					{
						FadeTowardsVolume(audioSource2, soundscapeLayer.volume);
					}
				}
				AudioSource[] array2 = (from s in audioSources
					where !newSounds.Any((SoundscapeLayer newSound) => newSound.clip == s.Key)
					select s.Value).ToArray();
				foreach (AudioSource audioSource3 in array2)
				{
					audioSource3.gameObject.AddComponent<FadeOutAudioSource>();
					audioSources.Remove(audioSource3.clip);
				}
			}

			private void FadeTowardsVolume(AudioSource source, float volume)
			{
				soundFades.Add(StartCoroutine(FadeSourceVolumeRoutine(source, volume, fadeTime)));
			}

			private IEnumerator FadeSourceVolumeRoutine(AudioSource source, float targetVolume, float fadeTime)
			{
				float originalVolume = source.volume;
				float startTime = Time.time;
				while (Time.time - startTime < fadeTime)
				{
					source.volume = Mathf.Lerp(originalVolume, targetVolume, (Time.time - startTime) / fadeTime);
					yield return null;
				}
				source.volume = targetVolume;
			}

			private void CancelPreviousFades()
			{
				foreach (Coroutine soundFade in soundFades)
				{
					StopCoroutine(soundFade);
				}
				soundFades.Clear();
			}
		}

		public SoundscapeLayer[] soundsInScene;

		public AudioMixerGroup mixerGroup;

		public int priority;

		private PrioritySortingKey stackKey;

		private void Awake()
		{
			Singleton<AmbientSoundManager>.instance.Initalize(this);
		}

		protected void OnEnable()
		{
			Asserts.WeakAssertTrue(stackKey == null, "The key should be null!");
			stackKey = Singleton<AmbientSoundManager>.instance.AddSoundscape(this);
		}

		protected void OnDisable()
		{
			Asserts.WeakAssertTrue(stackKey != null, "The key should be present!");
			if (Singleton<AmbientSoundManager>.instance != null)
			{
				Singleton<AmbientSoundManager>.instance.RemoveSoundscape(stackKey);
				stackKey = null;
			}
		}
	}
}
