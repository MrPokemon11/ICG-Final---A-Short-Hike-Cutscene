using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMusicLayers : MonoBehaviour
{
	private class PlayerMusicLayerController : IMusicLayerController
	{
		private MusicLayer layer;

		private Player player;

		private float fadeTime;

		private float delayLength;

		private Func<Player, bool> isLayerActive;

		private float _volume;

		private float delayCounter;

		public MusicLayer musicLayer => layer;

		public float volume => _volume;

		public PlayerMusicLayerController(Player player, MusicLayer layer, Func<Player, bool> isLayerActive, float fadeTime, float delayLength)
		{
			this.layer = layer;
			this.player = player;
			this.isLayerActive = isLayerActive;
			this.fadeTime = fadeTime;
			this.delayLength = delayLength;
		}

		public void Update()
		{
			float num = (isLayerActive(player) ? 1 : 0);
			if (_volume == 0f && num == 1f && delayCounter < delayLength)
			{
				delayCounter += Time.deltaTime;
				return;
			}
			_volume = Mathf.MoveTowards(_volume, num, Time.deltaTime / fadeTime);
			delayCounter = 0f;
		}
	}

	public float glideLayerFadeTime = 2f;

	public float glideLayerDelay = 1f;

	public MusicLayer[] glidingLayers;

	private Player player;

	private List<PlayerMusicLayerController> layerControllers = new List<PlayerMusicLayerController>();

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		MusicLayer[] array = glidingLayers;
		foreach (MusicLayer layer in array)
		{
			PlayerMusicLayerController playerMusicLayerController = new PlayerMusicLayerController(player, layer, (Player player) => player.isGliding, glideLayerFadeTime, glideLayerDelay);
			layerControllers.Add(playerMusicLayerController);
			Singleton<MusicManager>.instance.RegisterLayerController(playerMusicLayerController);
		}
	}

	private void Update()
	{
		foreach (PlayerMusicLayerController layerController in layerControllers)
		{
			layerController.Update();
		}
	}
}
