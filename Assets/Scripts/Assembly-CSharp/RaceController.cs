using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.Audio;

public class RaceController : MonoBehaviour
{
	[Header("Links")]
	public PlayerReplay playerReplay;

	public PlayerRecorder recorder;

	public RaceCoordinator coordinator;

	public GameObject goalUIPrefab;

	public GameObject opponentUIPrefab;

	[Header("Yarn Write Tags")]
	public string beginRaceTag = "BeginRace";

	public string resetRaceTag = "ResetRace";

	[Header("Yarn Read/Write Tags")]
	public string raceIdTag = "RaceId";

	public string discussedRaceTag = "RaceResultDiscussed";

	public string usedVehicleTag = "UsedVehicle";

	[Header("Yarn Read Tags")]
	public string startedRaceTag = "StartedRace";

	public string finishedRaceTag = "FinishedRace";

	public string raceWonTag = "WonRace";

	public string raceWonSeconds = "WonRaceTime";

	public string raceVictoriesTag = "RaceVictories";

	public string hardcodedReplaysTag = "HardcodedReplays";

	public string isSwimmingTag = "IsSwimming";

	public string abandonRaceTag = "RaceAbandoned";

	[Header("Effects")]
	public float waitAfterRaceTime = 300f;

	public float errorBeepStamina = 0.25f;

	public AudioClip errorClip;

	public TrailRenderer enemyTrail;

	public GameObject winnerParticles;

	public MusicSet raceMusic;

	public AudioClip countdownSound;

	public AudioClip countdownDone;

	public AudioClip startSound;

	public AudioClip winnerSound;

	public AudioMixerSnapshot quietAudio;

	public AudioMixerSnapshot normalAudio;

	private IInteractable rangedInteractable;

	private DialogueInteractable dialogue;

	private NPCFacer facer;

	private Player player;

	private Tags tags;

	private int allowedFeathers = 3;

	private Transform playerRaceStart;

	private RaceEndDestination raceEnd;

	private bool raceActive;

	private bool raceFinished;

	private bool restarting;

	private float? playerWinTime;

	private float? enemyWinTime;

	private bool abandonRace;

	private List<Action> poseFreeActions = new List<Action>();

	private List<StackResourceSortingKey> emotionKeys = new List<StackResourceSortingKey>();

	private float prevFeatherStamina;

	private ScriptedMusic music;

	private float raceFinishTime;

	public bool raceFinishedAndDiscussed => tags.GetBool(discussedRaceTag);

	public RaceData currentRaceData { get; private set; }

	public bool isBusy
	{
		get
		{
			if (!raceActive && !restarting)
			{
				if (raceFinished)
				{
					return !raceFinishedAndDiscussed;
				}
				return false;
			}
			return true;
		}
	}

	private void Awake()
	{
		tags = Singleton<GlobalData>.instance.gameData.tags;
	}

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		facer = playerReplay.GetComponent<NPCFacer>();
		dialogue = playerReplay.GetComponent<DialogueInteractable>();
		rangedInteractable = playerReplay.GetComponent<IInteractable>();
		dialogue.onConversationStart += OnConversationStart;
	}

	private void OnDestroy()
	{
		if (music != null && music.isPlaying)
		{
			music.Stop();
		}
	}

	private void Update()
	{
		if (raceActive)
		{
			if (player.isMounted)
			{
				tags.SetBool(usedVehicleTag);
			}
			if (!playerWinTime.HasValue && (player.transform.position - raceEnd.transform.position).sqrMagnitude < raceEnd.radius.Sqr())
			{
				RecordPlayerFinishTime();
			}
			if (!enemyWinTime.HasValue && (playerReplay.transform.position - raceEnd.transform.position).sqrMagnitude < raceEnd.radius.Sqr())
			{
				RecordEnemyFinishTime();
			}
			if (player.feathers == 0 && prevFeatherStamina >= errorBeepStamina && player.featherStamina < errorBeepStamina)
			{
				errorClip.Play();
			}
			prevFeatherStamina = player.featherStamina;
		}
		else if (raceFinished && Time.time - raceFinishTime > waitAfterRaceTime && !raceFinishedAndDiscussed)
		{
			tags.SetBool(discussedRaceTag);
		}
	}

	private void RecordPlayerFinishTime()
	{
		recorder.ForceCurrentFrame();
		recorder.StopRecording();
		playerWinTime = Time.time;
		if (!enemyWinTime.HasValue)
		{
			WinnerEffects(player.transform);
		}
	}

	private void RecordEnemyFinishTime()
	{
		enemyWinTime = Time.time;
		if (!playerWinTime.HasValue)
		{
			WinnerEffects(playerReplay.transform);
		}
	}

	public void SetupRaceData(RaceData raceData)
	{
		currentRaceData = raceData;
		playerReplay.data = raceData.GetReplayData();
		allowedFeathers = raceData.allowedFeathers;
		raceEnd = raceData.raceEnd;
		playerRaceStart = raceData.GetPlayerStartPosition(playerReplay.data);
		raceFinished = false;
		tags.SetBool(finishedRaceTag, value: false);
		tags.SetBool(discussedRaceTag, value: false);
		tags.SetBool(startedRaceTag, value: false);
		tags.SetBool(usedVehicleTag, value: false);
		tags.SetFloat(hardcodedReplaysTag, raceData.playerReplays.Count);
		tags.SetString(raceIdTag, currentRaceData.id);
		if (!Camera.main.IsPointInView(base.transform.position))
		{
			enemyTrail.Clear();
		}
		ClearCharacterAnimations();
	}

	public void ClearRaceData()
	{
		if (raceActive)
		{
			Debug.LogError("Cannot clear the race data while its running!");
		}
		else
		{
			currentRaceData = null;
		}
	}

	public void AbandonRace()
	{
		if (raceActive)
		{
			abandonRace = true;
		}
	}

	public void RestartCurrentRace()
	{
		if (currentRaceData != null)
		{
			SetupRaceData(currentRaceData);
			RestartRace();
		}
	}

	private void WinnerEffects(Transform transform)
	{
		winnerParticles.CloneAt(transform.position);
		winnerSound.Play();
	}

	private void OnDrawGizmos()
	{
		if ((bool)playerReplay && (bool)playerReplay.data)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(playerReplay.data.frames[0].position, 1f);
		}
		if ((bool)playerRaceStart)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(playerRaceStart.transform.position, 1f);
		}
	}

	private void OnConversationStart(IConversation conversation)
	{
		tags.SetFloat(raceVictoriesTag, currentRaceData.victories);
		tags.SetBool(isSwimmingTag, playerReplay.isSwimming);
		ClearCharacterAnimations();
		conversation.onConversationFinish += OnConversationFinish;
	}

	private void OnConversationFinish()
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(beginRaceTag))
		{
			BeginRace();
		}
		else if (Singleton<GlobalData>.instance.gameData.tags.GetBool(resetRaceTag))
		{
			string id = Singleton<GlobalData>.instance.gameData.tags.GetString(raceIdTag);
			RaceData raceData = coordinator.raceData.FirstOrDefault((RaceData r) => r.id == id);
			if (raceData == null)
			{
				raceData = currentRaceData;
			}
			SetupRaceData(raceData);
			RestartRace();
		}
	}

	private void BeginRace()
	{
		if (!raceActive && currentRaceData != null)
		{
			StartCoroutine(RaceCoroutine());
		}
	}

	private void RestartRace()
	{
		if (restarting)
		{
			Debug.LogWarning("Cannot restart while restarting!");
			return;
		}
		GameUserInput inputLock = GameUserInput.CreateInput(base.gameObject);
		TransitionAnimation transitionAnimation = Singleton<GameServiceLocator>.instance.transitionAnimation;
		quietAudio.TransitionTo(1f);
		restarting = true;
		transitionAnimation.Begin(delegate
		{
			player.body.position = playerRaceStart.transform.position;
			playerReplay.transform.position = playerReplay.data.frames[0].position;
			enemyTrail.Clear();
			player.TurnToFace(playerReplay.transform);
			facer.TurnToFace(player.transform);
			Camera.main.transform.position = player.transform.position;
		}, delegate
		{
			normalAudio.TransitionTo(1f);
			this.RegisterTimer(1f, delegate
			{
				restarting = false;
				BeginRace();
				UnityEngine.Object.Destroy(inputLock);
			});
		});
	}

	private IEnumerator RaceCoroutine()
	{
		rangedInteractable.enabled = false;
		GameUserInput inputLook = GameUserInput.CreateInput(base.gameObject);
		raceActive = true;
		playerWinTime = null;
		enemyWinTime = null;
		player.allowedFeathers = allowedFeathers;
		currentRaceData.attempted = true;
		tags.SetBool(startedRaceTag);
		StackResourceSortingKey silenceKey = Singleton<MusicManager>.instance.RegisterSilenece(50);
		Singleton<MusicManager>.instance.TrimRetiredActiveMusicSets(3f);
		music = new ScriptedMusic(raceMusic, 200);
		music.Load();
		Coroutine coroutine = StartCoroutine(WalkPlayersToStartingLine());
		countdownSound.Play();
		yield return new WaitForSeconds(1f);
		countdownSound.Play();
		yield return new WaitForSeconds(1f);
		countdownSound.Play();
		yield return new WaitForSeconds(0.75f);
		playerReplay.Play(-0.25f);
		yield return new WaitForSeconds(0.25f);
		StopCoroutine(coroutine);
		ClearCharacterAnimations();
		player.WalkTo(null);
		UnityEngine.Object.Destroy(inputLook);
		startSound.Play();
		countdownDone.Play();
		recorder.NewRecording();
		recorder.StartRecording();
		RaceGoalUI goalUI = Singleton<GameServiceLocator>.instance.ui.AddUI(goalUIPrefab.Clone()).GetComponent<RaceGoalUI>();
		goalUI.destination = raceEnd.transform;
		RaceGoalUI opponentUI = Singleton<GameServiceLocator>.instance.ui.AddUI(opponentUIPrefab.Clone()).GetComponent<RaceGoalUI>();
		opponentUI.destination = playerReplay.transform;
		float startTime = Time.time;
		yield return new WaitForSeconds(0.1f);
		silenceKey.ReleaseResource();
		music.Play();
		abandonRace = false;
		yield return new WaitUntil(() => !playerReplay.isPlaying || abandonRace);
		recorder.StopRecording();
		music.Stop();
		player.allowedFeathers = null;
		rangedInteractable.enabled = true;
		raceFinished = true;
		raceFinishTime = Time.time;
		raceActive = false;
		UnityEngine.Object.Destroy(goalUI.gameObject);
		UnityEngine.Object.Destroy(opponentUI.gameObject);
		bool flag = tags.GetBool(usedVehicleTag);
		bool flag2 = !abandonRace && !flag && playerWinTime.HasValue && (!enemyWinTime.HasValue || playerWinTime.Value <= enemyWinTime.Value);
		tags.SetBool(raceWonTag, flag2);
		tags.SetBool(finishedRaceTag);
		tags.SetBool(abandonRaceTag, abandonRace);
		if (abandonRace)
		{
			playerReplay.Stop();
			yield break;
		}
		if (!enemyWinTime.HasValue)
		{
			RecordEnemyFinishTime();
		}
		if (flag2)
		{
			float num = playerWinTime.Value - startTime;
			tags.SetFloat(raceWonSeconds, Mathf.CeilToInt(num));
			currentRaceData.victories++;
			if (currentRaceData.ghostData.frames.Count == 0 || recorder.replayData.lastFrame.time < currentRaceData.bestTime)
			{
				currentRaceData.ghostData = recorder.replayData;
			}
			coordinator.CheckForAchievements();
			Singleton<GameServiceLocator>.instance.achievements.SetLeaderboard(currentRaceData.id, Mathf.RoundToInt(num * 1000f));
		}
		else
		{
			poseFreeActions.Add(playerReplay.OverridePose(Pose.RaiseArms));
			emotionKeys.Add(playerReplay.animator.ShowEmotion(Emotion.Happy));
		}
		facer.TurnToFace(player.transform);
	}

	private void ClearCharacterAnimations()
	{
		foreach (Action poseFreeAction in poseFreeActions)
		{
			poseFreeAction();
		}
		poseFreeActions.Clear();
		foreach (StackResourceSortingKey emotionKey in emotionKeys)
		{
			emotionKey.ReleaseResource();
		}
		emotionKeys.Clear();
	}

	private IEnumerator WalkPlayersToStartingLine()
	{
		PlayerReplayFrame firstFrame = playerReplay.data.frames[0];
		playerReplay.walkTo = firstFrame.position;
		player.WalkTo(playerRaceStart.position);
		float time = Time.time;
		bool playerWalked = false;
		bool enemyWalked = false;
		while (Time.time < time + 4f && !(playerWalked && enemyWalked))
		{
			if (!player.walkTo.HasValue && !playerWalked)
			{
				player.TurnToFace(player.transform.position + firstFrame.rotation * Vector3.forward);
				poseFreeActions.Add(player.ikAnimator.Pose(Pose.RaceReady));
				playerWalked = true;
			}
			if (!playerReplay.walkTo.HasValue && !enemyWalked)
			{
				facer.TurnToFace(firstFrame.rotation * Vector3.forward);
				enemyWalked = true;
			}
			yield return null;
		}
		player.TurnToFace(player.transform.position + firstFrame.rotation * Vector3.forward);
		yield return new WaitForSeconds(0.25f);
		poseFreeActions.Add(playerReplay.OverridePose(Pose.RaceReady));
	}
}
