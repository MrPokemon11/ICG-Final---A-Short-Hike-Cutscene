using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ActiveMusicSet
{
	private bool enabled;

	private float volume;

	private float lifeAfterDeath;

	private float? customKillTime;

	private MusicManager manager;

	private Dictionary<MusicLayer, ActiveMusicLayer> musicLayers = new Dictionary<MusicLayer, ActiveMusicLayer>();

	private ICollection<ActiveMusicLayer> musicLayerValues;

	public bool alive { get; private set; }

	public bool startedPlaying { get; private set; }

	public ActiveMusicLayer baseLayer { get; private set; }

	public AudioMixerGroup mixerGroup => manager.mixerGroup;

	public MusicSet musicSetData { get; private set; }

	public float globalVolume => manager.globalFadeCurve.Evaluate(volume);

	public ActiveMusicSet(MusicSet musicSet, MusicManager manager)
	{
		this.manager = manager;
		musicSetData = musicSet;
		musicLayerValues = musicLayers.Values;
		MusicLayer[] layers = musicSet.layers;
		foreach (MusicLayer musicLayer in layers)
		{
			musicLayers.Add(musicLayer, new ActiveMusicLayer(musicLayer, this));
		}
		baseLayer = musicLayers[musicSetData.baseLayer];
		baseLayer.isBaseLayer = true;
		foreach (IMusicLayerController musicLayerController in manager.musicLayerControllers)
		{
			OnLayerControllerAdded(musicLayerController);
		}
		manager.onLayerControllerAdded += OnLayerControllerAdded;
		manager.onLayerControllerRemoved += OnLayerControllerRemoved;
		OnLayerControllersUpdated();
	}

	private void OnLayerControllerRemoved(IMusicLayerController controller)
	{
		if (musicLayers.ContainsKey(controller.musicLayer))
		{
			musicLayers[controller.musicLayer].RemoveController(controller);
		}
	}

	private void OnLayerControllerAdded(IMusicLayerController controller)
	{
		if (musicLayers.ContainsKey(controller.musicLayer))
		{
			musicLayers[controller.musicLayer].AddController(controller);
		}
	}

	public void Play()
	{
		enabled = true;
		alive = true;
		lifeAfterDeath = musicSetData.silenceAfterFinishing;
	}

	public void Retire()
	{
		enabled = false;
	}

	public void TrimRetirementTime(float killTime)
	{
		customKillTime = killTime;
		lifeAfterDeath = 0f;
	}

	public void SetPitch(float pitch)
	{
		foreach (ActiveMusicLayer musicLayerValue in musicLayerValues)
		{
			musicLayerValue.SetPitch(pitch);
		}
	}

	private void OnLayerControllersUpdated()
	{
		foreach (ActiveMusicLayer musicLayerValue in musicLayerValues)
		{
			musicLayerValue.ClearControllers();
		}
		foreach (IMusicLayerController musicLayerController in manager.musicLayerControllers)
		{
			if (musicLayers.ContainsKey(musicLayerController.musicLayer))
			{
				musicLayers[musicLayerController.musicLayer].AddController(musicLayerController);
			}
		}
	}

	public void Update()
	{
		if (!alive)
		{
			Debug.LogWarning("This shouldn't be updated when not alive!");
		}
		if (enabled)
		{
			bool flag = false;
			foreach (ActiveMusicLayer value in musicLayers.Values)
			{
				if (value.isAudible)
				{
					flag = true;
					break;
				}
			}
			if (!manager.AreOtherSetsAlive(this) && flag)
			{
				if (!startedPlaying)
				{
					RebootMusicLayers();
				}
				startedPlaying = true;
				volume += ((musicSetData.fadeInTime == 0f) ? 1f : (Time.deltaTime / musicSetData.fadeInTime));
				volume = Mathf.Min(volume, 1f);
			}
		}
		else
		{
			float num = (customKillTime.HasValue ? customKillTime.Value : musicSetData.fadeOutTime);
			volume -= ((num == 0f) ? 1f : (Time.deltaTime / num));
			if (volume <= 0f)
			{
				volume = 0f;
				lifeAfterDeath -= Time.deltaTime;
				if (lifeAfterDeath <= 0f || !startedPlaying)
				{
					Kill();
				}
			}
		}
		foreach (ActiveMusicLayer value2 in musicLayers.Values)
		{
			value2.Update();
		}
	}

	private void RebootMusicLayers()
	{
		foreach (ActiveMusicLayer value in musicLayers.Values)
		{
			value.SetSampleTime(0);
		}
	}

	private void Kill()
	{
		alive = false;
		startedPlaying = false;
		customKillTime = null;
	}
}
