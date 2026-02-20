using System;
using UnityEngine;

public class MotorboatMusicController : MonoBehaviour
{
	[Header("Links")]
	public Motorboat boat;

	public MusicSet musicSet;

	[Header("Settings")]
	public int musicPriority = 10;

	public string brokenEngineTag = "BrokenBoatEngine";

	[Header("Layer Level Up Settings")]
	public float boatWarmUpMusicSpeedThreshold = 40f;

	public Range warmUpSpeed = new Range(0.5f, 1.5f);

	public Range coolDownSpeed = new Range(-1f, -0.5f);

	public Vector2[] layerCoolDownWarmUpMultipliers = new Vector2[3]
	{
		new Vector2(1f, 1f),
		new Vector2(0.5f, 0.5f),
		new Vector2(1f, 1f)
	};

	[Header("Layer Volume Settings")]
	public float layerVolumeFadeSpeed = 0.18f;

	public Vector2[] layerVolumeFadeSpeedMultipliers = new Vector2[3]
	{
		new Vector2(1f, 0.5f),
		new Vector2(1f, 1f),
		new Vector2(1f, 1f)
	};

	public Range topLayerVolumeSpeedFactor = new Range(0.4f, 1f);

	private MusicManager manager;

	private StackResourceSortingKey musicStackKey;

	private SimpleMusicController[] layerControllers;

	private float musicWarmUp;

	private int musicLevel;

	private float[] layerFadeLevels;

	private bool brokenBoat;

	private void Start()
	{
		boat.onMounted += OnMountedBoat;
		manager = Singleton<MusicManager>.instance;
		layerControllers = new SimpleMusicController[musicSet.layers.Length];
		for (int i = 0; i < musicSet.layers.Length; i++)
		{
			layerControllers[i] = new SimpleMusicController(musicSet.layers[i]);
			manager.RegisterLayerController(layerControllers[i]);
		}
		layerFadeLevels = new float[layerControllers.Length];
		OnBoatBroken(Singleton<GlobalData>.instance.gameData.tags.GetBool(brokenEngineTag));
		Singleton<GlobalData>.instance.gameData.tags.WatchBool(brokenEngineTag, OnBoatBroken);
	}

	private void OnDestroy()
	{
		if (manager != null && layerControllers != null)
		{
			StackResourceSortingKey.Release(musicStackKey);
			for (int i = 0; i < layerControllers.Length; i++)
			{
				manager.UnregisterLayerController(layerControllers[i]);
			}
		}
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(brokenEngineTag, OnBoatBroken);
		}
	}

	private void OnBoatBroken(bool broken)
	{
		brokenBoat = broken;
	}

	private void OnMountedBoat(bool mounted)
	{
		if (mounted)
		{
			musicStackKey = manager.RegisterMusicSet(musicPriority, musicSet);
			return;
		}
		musicStackKey.ReleaseResource();
		musicStackKey = null;
	}

	public void Update()
	{
		UpdateMusicLevel();
		float num = Mathf.InverseLerp(boat.gentleMaxSpeed, boat.motorMaxSpeed, boat.prevSpeed);
		for (int i = 0; i < layerControllers.Length; i++)
		{
			bool flag = musicLevel >= i + 1;
			layerFadeLevels[i] = Mathf.MoveTowards(layerFadeLevels[i], flag ? 1 : 0, Time.deltaTime * layerVolumeFadeSpeed * (flag ? layerVolumeFadeSpeedMultipliers[i].y : layerVolumeFadeSpeedMultipliers[i].x));
			float num2 = ((i < layerControllers.Length - 1) ? layerFadeLevels[i + 1] : 0f);
			layerControllers[i].volume = layerFadeLevels[i] * topLayerVolumeSpeedFactor.Lerp(Mathf.Clamp01(num + num2));
		}
	}

	private void UpdateMusicLevel()
	{
		Vector2 vector = layerCoolDownWarmUpMultipliers[Math.Min(musicLevel, layerCoolDownWarmUpMultipliers.Length - 1)];
		float num;
		if (boat.prevSpeed > boatWarmUpMusicSpeedThreshold && !brokenBoat && manager.currentMusicSet == musicSet)
		{
			num = warmUpSpeed.Lerp(Mathf.InverseLerp(boatWarmUpMusicSpeedThreshold, boat.motorMaxSpeed, boat.prevSpeed));
			num *= vector.y;
		}
		else
		{
			num = coolDownSpeed.Lerp(Mathf.InverseLerp(0f, boatWarmUpMusicSpeedThreshold, boat.prevSpeed));
			num *= vector.x;
		}
		musicWarmUp += Time.deltaTime * num;
		if (musicWarmUp > 1f && musicLevel < layerControllers.Length)
		{
			musicWarmUp = 0f;
			musicLevel++;
		}
		else if (musicWarmUp < 0f && musicLevel > 0)
		{
			musicWarmUp = 1f;
			musicLevel--;
		}
		musicWarmUp = Mathf.Clamp01(musicWarmUp);
	}
}
