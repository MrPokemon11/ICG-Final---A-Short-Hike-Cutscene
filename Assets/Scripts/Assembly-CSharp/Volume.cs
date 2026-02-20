using System;
using QuickUnityTools.Audio;
using UnityEngine;

public static class Volume
{
	public enum Channel
	{
		Master = 0,
		Music = 1,
		Ambience = 2,
		SoundEffects = 3
	}

	public static void LoadVolumePrefs()
	{
		foreach (Channel value in Enum.GetValues(typeof(Channel)))
		{
			SetVolume(value, PlayerPrefsAdapter.GetFloat(value.ToString(), 1.1f));
		}
	}

	public static void SetVolume(Channel channel, float percent)
	{
		PlayerPrefsAdapter.SetFloat(channel.ToString(), percent);
		SetVolume(channel.ToString() + "Volume", percent);
	}

	private static void SetVolume(string parameter, float percent)
	{
		float value = -80f / Mathf.Log(0.01f) * Mathf.Log(Mathf.Max(0.01f, percent));
		Singleton<SoundPlayer>.instance.soundMixerGroup.audioMixer.SetFloat(parameter, value);
	}

	public static float GetVolume(Channel channel)
	{
		string name = channel.ToString() + "Volume";
		Singleton<SoundPlayer>.instance.soundMixerGroup.audioMixer.GetFloat(name, out var value);
		float num = -80f / Mathf.Log(0.01f);
		return Mathf.Exp(value / num);
	}
}
