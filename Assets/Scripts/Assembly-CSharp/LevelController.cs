using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class LevelController : ServiceMonoBehaviour
{
	public const string BEAT_GAME_TAG = "WonGameNiceJob";

	public const string SAVE_POINT_TAG = "SavePointLawl";

	public const string SPEEDRUN_TIME = "SpeedRunTime";

	public const string SAVE_POS = "SavePos";

	public static bool LOAD_FROM_EXACT_SPOT = true;

	public static bool loadSaveRegion;

	public static int? loadDoor;

	private static bool _speedrunClock;

	public float autoSaveMinutes = 10f;

	public Transform defaultSpawn;

	public GameObject speedrunCanvas;

	public TMP_Text speedrunText;

	public AudioMixerSnapshot defaultSnapshot;

	public GameObject savingBoxPrefab;

	public GameObject cinemaCamera;

	private float elapsedGameTime;

	private float autoSaveCountdown;

	public static bool speedrunClockActive
	{
		get
		{
			return _speedrunClock;
		}
		set
		{
			_speedrunClock = value;
			LevelController levelController = Singleton<ServiceLocator>.instance.Locate<LevelController>(allowFail: true);
			if ((bool)levelController)
			{
				levelController.UpdateSpeedrunClockUI();
			}
		}
	}

	public Player player { get; private set; }

	public Transform cameraTarget
	{
		get
		{
			if (!(player != null))
			{
				return null;
			}
			return player.transform;
		}
	}

	public bool gameRunning { get; set; }

	private void Awake()
	{
		player = UnityEngine.Object.FindObjectOfType<Player>();
		autoSaveCountdown = autoSaveMinutes * 60f;
		Singleton<GlobalData>.instance.onBeforeSave += OnBeforeSave;
		if (loadDoor.HasValue)
		{
			Singleton<GameServiceLocator>.instance.transitionAnimation.FadeInOnStart();
			Vector3 position = defaultSpawn.transform.position;
			TeleportPoint teleportPoint = UnityEngine.Object.FindObjectsOfType<TeleportPoint>().FirstOrDefault((TeleportPoint t) => t.doorId == loadDoor.Value);
			if ((bool)teleportPoint)
			{
				position = ((teleportPoint.landpointPoint != null) ? teleportPoint.landpointPoint.position : teleportPoint.transform.position);
			}
			player.body.position = position;
			loadDoor = null;
		}
		else if (loadSaveRegion)
		{
			Vector3 position2 = defaultSpawn.transform.position;
			Vector3? storedLastPosition = GetStoredLastPosition();
			SaveRegion storedSaveRegion = SaveRegion.GetStoredSaveRegion();
			if ((bool)storedSaveRegion)
			{
				position2 = storedSaveRegion.GetNearestLoadPosition(storedLastPosition);
			}
			if (LOAD_FROM_EXACT_SPOT && storedLastPosition.HasValue)
			{
				Vector3 spawnPos = storedLastPosition.Value + Vector3.up;
				if (IsSpawnPositionSafe(spawnPos))
				{
					position2 = storedLastPosition.Value;
				}
			}
			player.transform.position = position2;
			Physics.SyncTransforms();
		}
		elapsedGameTime = Singleton<GlobalData>.instance.gameData.tags.GetFloat("SpeedRunTime");
		UpdateSpeedrunClockUI();
		if (!Singleton<GlobalData>.instance.gameData.tags.GetBool("WonGameNiceJob"))
		{
			gameRunning = true;
		}
		defaultSnapshot.TransitionTo(1f);
		if (GameSettings.prewarmMusic)
		{
			MusicSetRegion[] array = UnityEngine.Object.FindObjectsOfType<MusicSetRegion>();
			for (int num = 0; num < array.Length; num++)
			{
				MusicLayer[] layers = array[num].musicSet.layers;
				for (int num2 = 0; num2 < layers.Length; num2++)
				{
					layers[num2].StartLoadingClip();
				}
			}
		}
		Singleton<GlobalData>.instance.gameData.EnsureAllCollectedInitalized();
	}

	private void StoreLastPosition()
	{
		Vector3 vector = player.transform.position;
		if (player.movingPlatform != null)
		{
			Vehicle component = player.movingPlatform.GetComponent<Vehicle>();
			if ((bool)component)
			{
				Vector3? savePosition = component.GetSavePosition();
				if (savePosition.HasValue)
				{
					vector = savePosition.Value;
				}
			}
		}
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		tags.SetFloat("SavePosX", vector.x);
		tags.SetFloat("SavePosY", vector.y);
		tags.SetFloat("SavePosZ", vector.z);
		Singleton<GlobalData>.instance.gameData.savedMap = SceneManager.GetActiveScene().name;
	}

	private Vector3? GetStoredLastPosition()
	{
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		if (!tags.HasFloat("SavePosX"))
		{
			return null;
		}
		return new Vector3(tags.GetFloat("SavePosX"), tags.GetFloat("SavePosY"), tags.GetFloat("SavePosZ"));
	}

	private bool IsSpawnPositionSafe(Vector3 spawnPos)
	{
		player.GetComponent<CapsuleCollider>().ToWorldSpaceCapsule(spawnPos, out var point, out var point2, out var radius);
		point2.y += 0.25f;
		int layerMask = -513;
		if (Physics.OverlapCapsule(point, point2, radius, layerMask, QueryTriggerInteraction.Ignore).Any())
		{
			return false;
		}
		return true;
	}

	private void OnDestroy()
	{
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.onBeforeSave -= OnBeforeSave;
		}
	}

	private void OnBeforeSave()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetFloat("SpeedRunTime", elapsedGameTime);
		StoreLastPosition();
	}

	private void Update()
	{
		if (gameRunning)
		{
			elapsedGameTime += Time.unscaledDeltaTime;
		}
		if (speedrunClockActive)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedGameTime);
			speedrunText.text = timeSpan.ToString("hh':'mm':'ss'.'fff");
		}
		autoSaveCountdown -= Time.unscaledDeltaTime;
		if (autoSaveCountdown <= 0f)
		{
			TryAutoSave();
		}
	}

	private void TryAutoSave()
	{
		if (GameSettings.autosave && player.input.hasFocus && player.body.linearVelocity.sqrMagnitude < 1f && !player.disableInteraction && player.isGrounded && FileSystem.canSave)
		{
			autoSaveCountdown = autoSaveMinutes * 60f;
			LevelUI ui = Singleton<GameServiceLocator>.instance.levelUI;
			ui.ShowSaveIcon(show: true);
			SaveQueue saveQueue = Singleton<GlobalData>.instance.SaveGameAsync();
			WaitFor.WithCoroutine(this, delegate
			{
				ui.ShowSaveIcon(show: false);
			}, saveQueue, new TimerWaitable(this.RegisterTimer(1.5f)));
		}
	}

	private void UpdateSpeedrunClockUI()
	{
		speedrunCanvas.SetActive(speedrunClockActive);
	}

	public void SaveAndQuit()
	{
		SaveQueue saveQueue = Singleton<GlobalData>.instance.SaveGameAsync();
		AsyncOperation operation = SceneManager.LoadSceneAsync("TitleScene");
		operation.allowSceneActivation = false;
		Timer timer = this.RegisterTimer(speedrunClockActive ? 0.01f : 1.2f, delegate
		{
		}, isLooped: false, useUnscaledTime: true);
		GameObject gameObject = Singleton<GameServiceLocator>.instance.ui.AddUI(savingBoxPrefab.Clone());
		GameObject warningBox = null;
		if (!FileSystem.canSave)
		{
			warningBox = Singleton<GameServiceLocator>.instance.ui.CreateSimpleDialogue(I18n.STRINGS.noUserNoSaves);
			SavingDialogue component = gameObject.GetComponent<SavingDialogue>();
			if ((bool)component)
			{
				component.noString = true;
			}
		}
		WaitFor.WithCoroutine(this, delegate
		{
			operation.allowSceneActivation = true;
		}, new TimerWaitable(timer), new SimpleAsyncOperationBundle(operation), saveQueue, new CustomWaitable(() => warningBox == null));
	}

	public static void SaveAndShowCredits()
	{
		Singleton<GlobalData>.instance.SaveGame(delegate
		{
			SceneManager.LoadScene("CreditsScene");
		});
	}
}
