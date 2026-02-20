using System.Collections.Generic;
using UnityEngine;

public class ActiveMusicLayer
{
	public static bool RESYNC_LAYERS = true;

	public static bool DEBUG_AUDIO = false;

	public const int AUDIO_DESYNC_THRESHOLD = 1000;

	public List<IMusicLayerController> controllers = new List<IMusicLayerController>();

	public bool isBaseLayer;

	private MusicLayer musicLayerData;

	private ActiveMusicSet activeMusicSet;

	private AudioSource audioSource;

	private float localVolume;

	private float pitch = 1f;

	public bool isAudible
	{
		get
		{
			if (audioSource != null && audioSource.clip.loadState == AudioDataLoadState.Loaded)
			{
				return localVolume > 0f;
			}
			return false;
		}
	}

	public float timeSamples => (!(audioSource == null)) ? audioSource.timeSamples : 0;

	public float time
	{
		get
		{
			if (!(audioSource == null))
			{
				return audioSource.time;
			}
			return 0f;
		}
	}

	private bool shouldPlay
	{
		get
		{
			if (controllers.Count <= 0)
			{
				return isBaseLayer;
			}
			return true;
		}
	}

	public ActiveMusicLayer(MusicLayer layer, ActiveMusicSet set)
	{
		musicLayerData = layer;
		activeMusicSet = set;
	}

	public void ClearControllers()
	{
		controllers.Clear();
	}

	public void AddController(IMusicLayerController controller)
	{
		controllers.Add(controller);
	}

	public void RemoveController(IMusicLayerController controller)
	{
		controllers.Remove(controller);
	}

	public void Update()
	{
		UpdateAudioSourcePlayState();
		if (!(audioSource != null))
		{
			return;
		}
		localVolume = 0f;
		foreach (IMusicLayerController controller in controllers)
		{
			localVolume = Mathf.Max(controller.volume, localVolume);
		}
		localVolume *= musicLayerData.normalVolume;
		audioSource.volume = localVolume * activeMusicSet.globalVolume;
	}

	private void UpdateAudioSourcePlayState()
	{
		if (shouldPlay)
		{
			if (!EnsureAudioSourceExists())
			{
				return;
			}
			if (audioSource.clip.loadState == AudioDataLoadState.Unloaded)
			{
				audioSource.clip.LoadAudioData();
			}
			else if (audioSource.clip.loadState == AudioDataLoadState.Loaded && !audioSource.isPlaying)
			{
				float num = ((!isBaseLayer) ? 1 : 0);
				audioSource.time = (activeMusicSet.baseLayer.audioSource.time + num) % audioSource.clip.length;
				audioSource.PlayScheduled(AudioSettings.dspTime + (double)num);
			}
			else
			{
				if (audioSource.clip.loadState != AudioDataLoadState.Loaded || !audioSource.isPlaying)
				{
					return;
				}
				int value = audioSource.timeSamples - activeMusicSet.baseLayer.audioSource.timeSamples;
				if (Mathf.Abs(value) > 1000 && RESYNC_LAYERS)
				{
					if (DEBUG_AUDIO)
					{
						Debug.LogWarning(audioSource.clip.name + " re-synced! Offset: " + value);
					}
					audioSource.timeSamples = activeMusicSet.baseLayer.audioSource.timeSamples % audioSource.clip.samples;
				}
			}
		}
		else if (audioSource != null && audioSource.isPlaying)
		{
			audioSource.Stop();
		}
	}

	public void SetPitch(float pitch)
	{
		this.pitch = pitch;
		if (audioSource != null)
		{
			audioSource.pitch = pitch;
		}
	}

	private bool EnsureAudioSourceExists()
	{
		if (musicLayerData.loadedClip == null)
		{
			musicLayerData.StartLoadingClip();
			return false;
		}
		if (audioSource != null)
		{
			return true;
		}
		audioSource = Singleton<MusicManager>.instance.gameObject.AddComponent<AudioSource>();
		audioSource.clip = musicLayerData.loadedClip;
		audioSource.loop = true;
		audioSource.pitch = pitch;
		audioSource.outputAudioMixerGroup = activeMusicSet.mixerGroup;
		audioSource.Stop();
		return true;
	}

	public void SetSampleTime(int time)
	{
		if (audioSource != null)
		{
			audioSource.timeSamples = time;
		}
	}
}
