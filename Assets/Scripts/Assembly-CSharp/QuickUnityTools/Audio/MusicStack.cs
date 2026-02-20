using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace QuickUnityTools.Audio
{
	[ResourceSingleton("MusicStack")]
	public class MusicStack : Singleton<MusicStack>
	{
		public class MusicPlayerController
		{
			private IMusicPlayer player;

			private float normalVolume;

			private float fadeDelayCountdown;

			private float destinationVolume;

			private float? fadeSpeed;

			public bool canCleanUp
			{
				get
				{
					if (!isPlaying)
					{
						return player.volume == 0f;
					}
					return false;
				}
			}

			private bool isPlaying => destinationVolume > 0f;

			public MusicPlayerController(IMusicPlayer player)
			{
				this.player = player;
				normalVolume = player.volume;
				player.volume = 0f;
			}

			public void StartMusic(float fadeInTime, float fadeInDelay, float? newVolume)
			{
				if (newVolume.HasValue)
				{
					normalVolume = newVolume.Value;
				}
				FadeTowardsVolume(normalVolume, fadeInTime, fadeInDelay);
			}

			public void StopMusic(float fadeOutTime)
			{
				FadeTowardsVolume(0f, fadeOutTime, 0f);
			}

			private void FadeTowardsVolume(float toVolume, float fadeTime, float fadeDelay)
			{
				fadeDelayCountdown = fadeDelay;
				destinationVolume = toVolume;
				fadeSpeed = ((fadeTime > 0f) ? new float?(Mathf.Abs(destinationVolume - player.volume) / fadeTime) : ((float?)null));
			}

			public void ManualUpdate()
			{
				if (fadeDelayCountdown > 0f)
				{
					fadeDelayCountdown -= Time.unscaledDeltaTime;
					return;
				}
				if (isPlaying && !player.isPlaying)
				{
					player.Play();
				}
				if (!isPlaying && player.isPlaying && player.volume <= 0f)
				{
					player.Stop();
				}
				float maxDelta = (fadeSpeed.HasValue ? (Time.unscaledDeltaTime * fadeSpeed.Value) : 1000f);
				player.volume = Mathf.MoveTowards(player.volume, destinationVolume, maxDelta);
			}

			public void CleanUp()
			{
				player.CleanUp();
			}
		}

		public interface IMusicAsset
		{
			IMusicPlayer CreatePlayer();

			int GetMusicID();
		}

		public interface IMusicPlayer
		{
			AudioMixerGroup outputAudioMixerGroup { set; }

			float volume { get; set; }

			bool isPlaying { get; }

			void Play();

			void Stop();

			void CleanUp();
		}

		[Serializable]
		public struct Transition
		{
			public float fadeOutTime;

			public float fadeInTime;

			public float fadeInDelay;

			public static readonly Transition INSTANT = default(Transition);

			public static readonly Transition CROSS_FADE = new Transition
			{
				fadeOutTime = 2f,
				fadeInTime = 2f
			};

			public static readonly Transition QUICK_OUT = new Transition
			{
				fadeOutTime = 0.1f,
				fadeInDelay = 0.5f
			};
		}

		public AudioMixerGroup mixerGroup;

		private SortedList<PrioritySortingKey, IMusicStackElement> musicStack = new SortedList<PrioritySortingKey, IMusicStackElement>();

		private Dictionary<int, MusicPlayerController> playerControllers = new Dictionary<int, MusicPlayerController>();

		private IList<IMusicStackElement> musicStackView;

		private Dictionary<int, MusicPlayerController>.ValueCollection musicControllersView;

		private IMusicStackElement currentMusic
		{
			get
			{
				if (musicStackView.Count != 0)
				{
					return musicStackView[0];
				}
				return null;
			}
		}

		private void Awake()
		{
			musicStackView = musicStack.Values;
			musicControllersView = playerControllers.Values;
		}

		public PrioritySortingKey AddToMusicStack(IMusicStackElement addedMusic)
		{
			IMusicStackElement musicStackElement = currentMusic;
			PrioritySortingKey prioritySortingKey = new PrioritySortingKey((int)addedMusic.GetPriority());
			musicStack.Add(prioritySortingKey, addedMusic);
			if (currentMusic != musicStackElement)
			{
				Asserts.AssertTrue(addedMusic == currentMusic, "Somehow we added to the music stack and yet a different music rose to the top?");
				TransitionToMusic(musicStackElement, currentMusic, currentMusic.GetTakeControlTransition());
			}
			return prioritySortingKey;
		}

		public void RemoveFromMusicStack(PrioritySortingKey key)
		{
			Asserts.AssertTrue(musicStack.ContainsKey(key), "This key is not in the music stack!");
			IMusicStackElement musicStackElement = musicStack[key];
			bool num = musicStackElement == currentMusic;
			musicStack.Remove(key);
			if (num)
			{
				TransitionToMusic(musicStackElement, currentMusic, musicStackElement.GetReleaseControlTransition());
			}
		}

		private void TransitionToMusic(IMusicStackElement oldElement, IMusicStackElement newElement, Transition transition)
		{
			IMusicAsset audioData = GetAudioData(oldElement);
			IMusicAsset audioData2 = GetAudioData(newElement);
			if (audioData != null)
			{
				playerControllers[audioData.GetMusicID()].StopMusic(transition.fadeOutTime);
			}
			if (audioData2 != null)
			{
				int musicID = audioData2.GetMusicID();
				if (playerControllers.ContainsKey(musicID))
				{
					playerControllers[musicID].StartMusic(transition.fadeInTime, transition.fadeInDelay, newElement.GetDesiredVolume());
					return;
				}
				IMusicPlayer musicPlayer = audioData2.CreatePlayer();
				musicPlayer.outputAudioMixerGroup = mixerGroup;
				MusicPlayerController musicPlayerController = new MusicPlayerController(musicPlayer);
				playerControllers.Add(musicID, musicPlayerController);
				musicPlayerController.StartMusic(transition.fadeInTime, transition.fadeInDelay, newElement.GetDesiredVolume());
			}
		}

		private void Update()
		{
			bool flag = false;
			foreach (MusicPlayerController item in musicControllersView)
			{
				item.ManualUpdate();
				flag |= item.canCleanUp;
			}
			if (flag)
			{
				playerControllers.BufferedForEach((KeyValuePair<int, MusicPlayerController> pair) => pair.Value.canCleanUp, delegate(KeyValuePair<int, MusicPlayerController> pair)
				{
					pair.Value.CleanUp();
					playerControllers.Remove(pair.Key);
				});
			}
		}

		private static IMusicAsset GetAudioData(IMusicStackElement element)
		{
			return element?.GetMusicAsset();
		}
	}
}
