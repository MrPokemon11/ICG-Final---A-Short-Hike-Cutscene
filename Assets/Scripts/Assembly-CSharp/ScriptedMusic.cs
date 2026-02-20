using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedMusic
{
	private MusicSet musicSet;

	private StackResourceSortingKey musicKey;

	private List<IMusicLayerController> controllers;

	private int priority;

	public bool isPlaying => musicKey != null;

	public ScriptedMusic(MusicSet musicSet, int priority = 100)
	{
		this.priority = priority;
		this.musicSet = musicSet;
		controllers = new List<IMusicLayerController>();
	}

	public void Play()
	{
		if (musicKey == null)
		{
			musicKey = Singleton<MusicManager>.instance.RegisterMusicSet(priority, musicSet);
			MusicLayer[] layers = musicSet.layers;
			for (int i = 0; i < layers.Length; i++)
			{
				SimpleMusicController simpleMusicController = new SimpleMusicController(layers[i]);
				controllers.Add(simpleMusicController);
				Singleton<MusicManager>.instance.RegisterLayerController(simpleMusicController);
			}
		}
	}

	public void PlayOnce(float fadeOutTimeFromEnd = 1f)
	{
		if (musicKey == null)
		{
			Play();
			Singleton<MusicManager>.instance.StartCoroutine(PlayOnceMusicRoutine(fadeOutTimeFromEnd));
		}
	}

	public void SetPitch(float pitch)
	{
		Singleton<MusicManager>.instance.GetActiveMusicSet(musicSet).SetPitch(pitch);
	}

	private IEnumerator PlayOnceMusicRoutine(float fadeOutTimeFromEnd)
	{
		Singleton<MusicManager>.instance.TrimRetiredActiveMusicSets(1f);
		ActiveMusicSet activeSet = Singleton<MusicManager>.instance.GetActiveMusicSet(musicSet);
		yield return new WaitUntil(() => activeSet.startedPlaying || musicKey == null);
		yield return new WaitUntil(() => musicSet.baseLayer.loadedClip != null);
		AudioClip clip = musicSet.baseLayer.loadedClip;
		int halfClipSamples = clip.samples / 2;
		yield return new WaitUntil(() => activeSet.baseLayer.timeSamples > (float)halfClipSamples || musicKey == null);
		float fadeOutSamples = (float)clip.samples - fadeOutTimeFromEnd * (float)clip.frequency;
		yield return new WaitUntil(() => activeSet.baseLayer.timeSamples < (float)halfClipSamples || activeSet.baseLayer.timeSamples > fadeOutSamples || musicKey == null);
		Stop();
	}

	public void Stop()
	{
		if (musicKey == null)
		{
			return;
		}
		musicKey?.ReleaseResource();
		musicKey = null;
		IMusicLayerController[] listCopy = controllers.ToArray();
		Singleton<MusicManager>.instance.RegisterTimer(musicSet.fadeOutTime, delegate
		{
			IMusicLayerController[] array = listCopy;
			foreach (IMusicLayerController controller in array)
			{
				Singleton<MusicManager>.instance.UnregisterLayerController(controller);
			}
		});
	}

	public void Load()
	{
		musicSet.baseLayer.StartLoadingClip();
		Singleton<MusicManager>.instance.StartCoroutine(LoadAudioDataAfterLoadingClip());
	}

	private IEnumerator LoadAudioDataAfterLoadingClip()
	{
		yield return new WaitUntil(() => musicSet.baseLayer.loadedClip != null);
		musicSet.baseLayer.loadedClip.LoadAudioData();
	}
}
