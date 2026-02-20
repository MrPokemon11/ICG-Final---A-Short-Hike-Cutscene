using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	public class SmoothLoopAudioSource : MonoBehaviour
	{
		private const int SECONDS_IN_MINUTE = 60;

		public SmoothLoopAudioClip music;

		private AudioMixerGroup _mixerGroup;

		private float _volume = 1f;

		private AudioSource[] audioSources;

		private double startDpsTime;

		private int nextAudioSourceIndex;

		private int numberOfLoopsScheduled;

		private Timer loopTimer;

		public AudioMixerGroup mixerGroup
		{
			get
			{
				return _mixerGroup;
			}
			set
			{
				_mixerGroup = value;
				if (audioSources != null)
				{
					for (int i = 0; i < audioSources.Length; i++)
					{
						audioSources[i].outputAudioMixerGroup = mixerGroup;
					}
				}
			}
		}

		public float volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
				if (audioSources != null)
				{
					AudioSource[] array = audioSources;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].volume = value;
					}
				}
			}
		}

		public bool isPlaying { get; private set; }

		private float introTime => music.beatsPerMeasure * music.introMeasures / music.beatsPerMinute * 60f;

		private float loopTime => music.length - introTime;

		private void Start()
		{
			audioSources = new AudioSource[2];
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i] = base.gameObject.AddComponent<AudioSource>();
				audioSources[i].clip = music.clip;
				audioSources[i].outputAudioMixerGroup = mixerGroup;
				audioSources[i].volume = volume;
			}
		}

		public void Play()
		{
			Stop();
			startDpsTime = AudioSettings.dspTime;
			isPlaying = true;
			audioSources[0].Play();
			audioSources[1].PlayScheduled(startDpsTime + (double)music.length);
			audioSources[1].time = introTime;
			nextAudioSourceIndex = 0;
			numberOfLoopsScheduled = 1;
			loopTimer = this.RegisterTimer(music.length, ScheduleNextLoop);
		}

		public void Stop()
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Stop();
			}
			if (loopTimer != null)
			{
				loopTimer.Cancel();
			}
			isPlaying = false;
		}

		private void ScheduleNextLoop()
		{
			audioSources[nextAudioSourceIndex].PlayScheduled(startDpsTime + (double)music.length + (double)(loopTime * (float)numberOfLoopsScheduled));
			audioSources[nextAudioSourceIndex].time = introTime;
			nextAudioSourceIndex = (nextAudioSourceIndex + 1) % audioSources.Length;
			numberOfLoopsScheduled++;
			loopTimer = this.RegisterTimer(loopTime, ScheduleNextLoop);
		}
	}
}
