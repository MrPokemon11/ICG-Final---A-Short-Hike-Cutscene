using System;
using UnityEngine;

namespace QuickUnityTools.Audio
{
	[Serializable]
	public class BasicMusicStackElement : IMusicStackElement
	{
		public AudioClip music;

		public SmoothLoopAudioClip smoothLoopMusic;

		public MusicStackPriorty priority;

		public MusicStack.Transition transitionIn = MusicStack.Transition.CROSS_FADE;

		public MusicStack.Transition transitionOut = MusicStack.Transition.CROSS_FADE;

		public float desiredVolume = 1f;

		public BasicMusicStackElement(AudioClip music)
		{
			this.music = music;
		}

		public MusicStack.IMusicAsset GetMusicAsset()
		{
			if (smoothLoopMusic != null)
			{
				return new SmoothLoopMusicAsset(smoothLoopMusic);
			}
			if (!(music != null))
			{
				return null;
			}
			return new AudioClipMusicAsset(music);
		}

		public MusicStack.Transition GetReleaseControlTransition()
		{
			return transitionOut;
		}

		public MusicStack.Transition GetTakeControlTransition()
		{
			return transitionIn;
		}

		public float GetDesiredVolume()
		{
			return desiredVolume;
		}

		public MusicStackPriorty GetPriority()
		{
			return priority;
		}
	}
}
