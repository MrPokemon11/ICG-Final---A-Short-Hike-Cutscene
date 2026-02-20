using UnityEngine;

namespace QuickUnityTools.Audio
{
	public static class Volume
	{
		public enum Channel
		{
			Master = 0,
			Music = 1,
			Ambience = 2,
			SoundEffects = 3
		}

		private static float GetDefaultChannelDB(Channel channel)
		{
			return channel switch
			{
				Channel.Master => -6f, 
				Channel.Music => -5f, 
				_ => 0f, 
			};
		}

		public static void SetVolume(Channel channel, float percent)
		{
			float defaultChannelDB = GetDefaultChannelDB(channel);
			SetVolume(channel.ToString() + "Volume", percent, defaultChannelDB);
		}

		private static void SetVolume(string parameter, float percent, float normal)
		{
			float value = (-80f - normal) / Mathf.Log(0.01f) * Mathf.Log(Mathf.Max(0.01f, percent)) + normal;
			Singleton<SoundPlayer>.instance.soundMixerGroup.audioMixer.SetFloat(parameter, value);
		}

		public static float GetVolume(Channel channel)
		{
			string name = channel.ToString() + "Volume";
			Singleton<SoundPlayer>.instance.soundMixerGroup.audioMixer.GetFloat(name, out var value);
			float defaultChannelDB = GetDefaultChannelDB(channel);
			float num = (-80f - defaultChannelDB) / Mathf.Log(0.01f);
			return Mathf.Exp((value - defaultChannelDB) / num);
		}
	}
}
