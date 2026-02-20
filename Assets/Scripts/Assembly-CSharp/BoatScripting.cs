using System;
using System.Collections;
using System.Collections.Generic;
using QuickUnityTools.Input;
using UnityEngine;

public class BoatScripting : MonoBehaviour
{
	public class WaitForRaceNode : CustomYieldInstruction
	{
		public enum WaitResult
		{
			Continue = 0,
			Cancel = 1,
			Reset = 2
		}

		private BoatScripting scripting;

		private Transform checkpoint;

		private bool cancelled;

		public WaitResult continueRaceResult { get; private set; }

		public override bool keepWaiting
		{
			get
			{
				if (cancelled)
				{
					return false;
				}
				if (!((scripting.boat.transform.position - checkpoint.transform.position).sqrMagnitude < scripting.checkpointRadius.Sqr()))
				{
					if (!scripting.boat.mounted && (scripting.player.transform.position - scripting.boat.transform.position).sqrMagnitude > 625f)
					{
						Singleton<GlobalData>.instance.gameData.tags.SetBool(scripting.ranAwayFromRaceTag);
						continueRaceResult = WaitResult.Cancel;
						return false;
					}
					return true;
				}
				continueRaceResult = WaitResult.Continue;
				return false;
			}
		}

		public WaitForRaceNode(BoatScripting scripting, Transform checkpoint)
		{
			this.scripting = scripting;
			this.checkpoint = checkpoint;
		}

		public void Cancel(WaitResult result)
		{
			cancelled = true;
			continueRaceResult = result;
		}
	}

	private const float CANCEL_RACE_RADIUS_SQR = 625f;

	private const float ABANDON_DEER_RADIUS_SQR = 40000f;

	public Motorboat boat;

	public Transform playerBoatStart;

	[Header("Rendering")]
	public Renderer oceanRenderer;

	public GameObject longOceanObject;

	public VirtualCameraRegion[] boatOnlyCameraRegions;

	[Header("Kid State Scripting")]
	public string insideBoatTag = "DeerInBoat";

	public string fastBoatTag = "BoatMovedFast";

	public string abandondedKidTag = "AbandonedKid";

	public NPCIKAnimator boatKidAnimator;

	public float kidAbandondedTime = 5f;

	public float kidAbandondedUpdateInterval = 1f;

	public Transform[] boatResetVisibilityTransforms;

	[Header("Race")]
	public string beforeRaceStartYarnNode = "BeforeBoatRaceStart";

	public string finishedRaceYarnNode = "FinishedBoatRaceStart";

	public string boatRaceActiveTag = "BoatRaceActive";

	public string ranAwayFromRaceTag = "RanAwayBoatRace";

	public string boatRaceWonTag = "BoatRaceWon";

	public string boatRaceBestTimeTag = "BoatRaceBestTime";

	public string boatRaceBestTimeTextTag = "BoatRaceBestTimeText";

	public string boatRaceTimeTextTag = "BoatRaceTimeText";

	public string boatRacePenaltiesTag = "BoatRacePenalties";

	public float achievementRaceTime = 45f;

	public float checkpointRadius = 15f;

	public float initialBestRaceTime = 60f;

	public Transform raceStart;

	public GameObject goalUIPrefab;

	public GameObject boatArrowUIPrefab;

	public GameObject raceObjectsParent;

	public Transform checkpointParent;

	public MusicSet raceMusic;

	public AudioClip countdownSound;

	public AudioClip countdownDone;

	public AudioClip startSound;

	public AudioClip winnerSound;

	public AudioClip checkpointSound;

	public GameObject timerUIPrefab;

	public Transform cameraTarget;

	public GameObject confettiPrefab;

	[Header("Race Camera")]
	public float boatCameraSmoothTime = 0.4f;

	public float boatCameraCheckpointPull = 0.1f;

	public float boatCameraVelocityFactor = 0.2f;

	public float boatCameraMaxPull = 30f;

	public float boatCameraNextCheckpointDistance = 60f;

	public float boatCameraNextCheckpointFactor = 0.5f;

	[Header("Ramp Jump")]
	public string crashedBoatYarnNode = "CrashedBoatStart";

	public string crashedBoatYarnNode2 = "CrashedBoat2";

	public string crashedBoatYarnNode3 = "CrashedBoat3";

	public GameObject rampJumpCamera;

	public bool saveJumpData;

	public ParticleSystem backBubblesBoat;

	public Vector3 rampTeleportVelocity;

	public Vector3 rampTeleportPosition;

	public Quaternion rampTeleportRotation;

	public AudioClip boatCrashSound;

	public AudioClip engineRevSound;

	[Header("Broken Boat")]
	public string brokenEngineTag = "BrokenBoatEngine";

	public string trackedBrokenBoatYarnNode = "BrokenBoatNoticedStart";

	public string brokenEngineTrackedTag = "BrokenBoatTracked";

	public float brokenEngineTrackingDistance = 150f;

	public AudioClip normalEngineSound;

	public AudioClip brokenEngineSound;

	[Header("Fixing Boat")]
	public string fixedBoatTag = "BoatIsFixed";

	public string dadFixedBoatYarnNode = "DadFixedBoatStart";

	public string dadFixingBoatPositionTag = "DadDeerFixingBoat";

	public GameObject normalDadPosition;

	public GameObject fixingDadPosition;

	public string duckFixedBoatYarnNode = "DuckFixedBoatStart";

	public string duckFixingBoatPositionTag = "DuckFixingBoat";

	public GameObject normalDuckPosition;

	public GameObject fixingDuckPosition;

	public Transform boatDuckPlace;

	public Transform playerDuckPlace;

	[Header("Dad Book")]
	public string dadBookTimeoutTag = "DadIsDoneBookChat";

	private Transform lookAtTarget;

	private bool animateKid;

	private Coroutine kidAnimationCoroutine;

	private Timer abandonKidTimer;

	private List<BoatRaceCheckpoint> checkpoints = new List<BoatRaceCheckpoint>();

	private List<List<Renderer>> checkpointRenderers = new List<List<Renderer>>();

	private WaitForRaceNode waitForRaceNode;

	private Player player;

	private Coroutine cleanUpRaceCorountine;

	private ScriptedMusic music;

	private TimerUI timerUI;

	private int penalties;

	private BoatGoalArrowUI boatArrow;

	private float brokenBoatTotalMovement;

	private bool boatBroken;

	private void Awake()
	{
		boat.onMounted += OnBoatMounted;
		boat.onBumpedBoat += OnBumpedBoat;
		checkpoints = new List<BoatRaceCheckpoint>();
		BoatRaceCheckpoint[] componentsInChildren = checkpointParent.GetComponentsInChildren<BoatRaceCheckpoint>();
		foreach (BoatRaceCheckpoint boatRaceCheckpoint in componentsInChildren)
		{
			checkpoints.Add(boatRaceCheckpoint);
			List<Renderer> list = new List<Renderer>();
			checkpointRenderers.Add(list);
			foreach (Transform child in boatRaceCheckpoint.transform.parent.GetChildren())
			{
				if (child != boatRaceCheckpoint.transform)
				{
					Renderer component = child.GetComponent<Renderer>();
					if ((bool)component)
					{
						list.Add(component);
					}
				}
			}
		}
		raceObjectsParent.SetActive(value: false);
	}

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		Singleton<GlobalData>.instance.gameData.tags.SetBool(boatRaceActiveTag, value: false);
		Singleton<GlobalData>.instance.gameData.tags.SetBool(ranAwayFromRaceTag, value: false);
		Singleton<GlobalData>.instance.gameData.tags.WatchBool(brokenEngineTag, OnBoatBroken);
		OnBoatBroken(Singleton<GlobalData>.instance.gameData.tags.GetBool(brokenEngineTag));
		Singleton<GlobalData>.instance.gameData.tags.SetBool(dadFixingBoatPositionTag, value: false);
		Singleton<GlobalData>.instance.gameData.tags.SetBool(duckFixingBoatPositionTag, value: false);
		Singleton<GlobalData>.instance.gameData.tags.SetBool(dadBookTimeoutTag, value: false);
		Singleton<GlobalData>.instance.gameData.tags.WatchBool(dadBookTimeoutTag, OnBookChatDiscussed);
	}

	private void OnDestroy()
	{
		if (music != null && music.isPlaying)
		{
			music.Stop();
		}
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(brokenEngineTag, OnBoatBroken);
			Singleton<GlobalData>.instance.gameData.tags.UnwatchBool(dadBookTimeoutTag, OnBookChatDiscussed);
		}
	}

	private void OnBookChatDiscussed(bool value)
	{
		if (value)
		{
			this.RegisterTimer(60f, delegate
			{
				Singleton<GlobalData>.instance.gameData.tags.SetBool(dadBookTimeoutTag, value: false);
			});
		}
	}

	private void OnBumpedBoat(float percentStrength, Collision collision)
	{
		if (percentStrength > 0.6f && animateKid)
		{
			StackResourceSortingKey key = boatKidAnimator.ShowEmotion(Emotion.Surprise);
			this.RegisterTimer(0.6f, delegate
			{
				key.ReleaseResource();
			});
		}
		if (percentStrength > 0.3f && !collision.gameObject.GetComponent<BoatRaceCheckpoint>() && timerUI != null && Singleton<GlobalData>.instance.gameData.tags.GetBool(fixedBoatTag))
		{
			timerUI.time += 10f;
			penalties++;
			timerUI.Flash();
		}
	}

	private void OnBoatMounted(bool mounted)
	{
		if (mounted && Singleton<GlobalData>.instance.gameData.tags.GetBool(insideBoatTag))
		{
			animateKid = true;
			if (kidAnimationCoroutine == null)
			{
				kidAnimationCoroutine = StartCoroutine(DeerKidAnimationRoutine());
			}
		}
		else
		{
			animateKid = false;
		}
		if (mounted)
		{
			Timer.Cancel(abandonKidTimer);
			Timer.FlagToRecycle(abandonKidTimer);
			abandonKidTimer = null;
		}
		else if (!mounted && Singleton<GlobalData>.instance.gameData.tags.GetBool(insideBoatTag))
		{
			abandonKidTimer = this.RegisterTimer(kidAbandondedTime, CheckOnAbandondedKid);
		}
		longOceanObject.SetActive(mounted);
		oceanRenderer.enabled = !mounted;
		VirtualCameraRegion[] array = boatOnlyCameraRegions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = mounted;
		}
		TrackBrokenBoatMovement();
	}

	private void OnBoatBroken(bool broken)
	{
		boat.engineAudio.Stop();
		boat.engineAudio.clip = (broken ? brokenEngineSound : normalEngineSound);
		if (boat.mounted)
		{
			boat.engineAudio.Play();
		}
		TrackBrokenBoatMovement();
		boatBroken = broken;
	}

	private void TrackBrokenBoatMovement()
	{
		if (boat.mounted && Singleton<GlobalData>.instance.gameData.tags.GetBool(brokenEngineTag) && !Singleton<GlobalData>.instance.gameData.tags.GetBool(brokenEngineTrackedTag))
		{
			StartCoroutine(TrackBrokenBoatRoutine());
		}
	}

	private IEnumerator TrackBrokenBoatRoutine()
	{
		while (boat.mounted)
		{
			if (boat.engineAudio.isPlaying)
			{
				brokenBoatTotalMovement += boat.prevSpeed * Time.deltaTime;
			}
			if (brokenBoatTotalMovement > brokenEngineTrackingDistance)
			{
				Singleton<GameServiceLocator>.instance.dialogue.StartConversation(trackedBrokenBoatYarnNode, boatKidAnimator.transform.parent);
				Singleton<GlobalData>.instance.gameData.tags.SetBool(brokenEngineTrackedTag);
				break;
			}
			yield return null;
		}
	}

	private void CheckOnAbandondedKid()
	{
		Timer.FlagToRecycle(abandonKidTimer);
		abandonKidTimer = null;
		if ((boat.transform.position - player.transform.position).sqrMagnitude > 40000f && !boat.boatRenderer.isVisible)
		{
			bool flag = true;
			Camera main = Camera.main;
			Transform[] array = boatResetVisibilityTransforms;
			foreach (Transform transform in array)
			{
				if (main.IsPointInView(transform.position))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Singleton<GlobalData>.instance.gameData.tags.SetBool(abandondedKidTag);
				boat.ResetPosition();
				return;
			}
		}
		abandonKidTimer = this.RegisterTimer(kidAbandondedUpdateInterval, CheckOnAbandondedKid);
	}

	private IEnumerator DeerKidAnimationRoutine()
	{
		if (lookAtTarget == null)
		{
			lookAtTarget = new GameObject("Kid Look At").transform;
		}
		bool fastBoatTagValue = Singleton<GlobalData>.instance.gameData.tags.GetBool(fastBoatTag);
		Action poseKey = null;
		StackResourceSortingKey emotionKey = null;
		while (animateKid)
		{
			lookAtTarget.position = boatKidAnimator.transform.position + boatKidAnimator.transform.forward + boat.body.velocity.SetY(0f) * 100f;
			if (boatKidAnimator.lookAt == null)
			{
				boatKidAnimator.lookAt = lookAtTarget;
			}
			bool flag = boat.body.velocity.sqrMagnitude > boat.limitAccelerationSpeed.Sqr() && !boatBroken;
			if (flag && !fastBoatTagValue)
			{
				Singleton<GlobalData>.instance.gameData.tags.SetBool(fastBoatTag);
				fastBoatTagValue = true;
			}
			if (flag && poseKey == null)
			{
				poseKey = boatKidAnimator.Pose(Pose.RaiseArms);
				emotionKey = boatKidAnimator.ShowEmotion(Emotion.Happy);
			}
			else if (!flag && poseKey != null)
			{
				poseKey();
				emotionKey.ReleaseResource();
				poseKey = null;
			}
			yield return null;
		}
		poseKey?.Invoke();
		emotionKey?.ReleaseResource();
		boatKidAnimator.lookAt = null;
		kidAnimationCoroutine = null;
	}

	public void GetIntoBoat()
	{
		ResetBoat(delegate
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(insideBoatTag);
		});
	}

	public void GetOffBoat()
	{
		ResetBoat(delegate
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(insideBoatTag, value: false);
		});
	}

	public void ResetBoat()
	{
		ResetBoat(null);
	}

	public void ResetAndFixBoatDad()
	{
		ResetBoat(delegate
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(brokenEngineTag, value: false);
			Singleton<GlobalData>.instance.gameData.tags.SetBool(dadFixingBoatPositionTag);
		}, delegate
		{
			StartCoroutine(AfterBoatFixedRoutine(dadFixedBoatYarnNode, dadFixingBoatPositionTag, fixingDadPosition.transform, normalDadPosition.transform));
		});
	}

	public void ResetAndFixBoatDuck()
	{
		GameUserInput inputLock = GameUserInput.CreateInput(base.gameObject);
		Singleton<GameServiceLocator>.instance.transitionAnimation.Begin(delegate
		{
			player.body.position = playerDuckPlace.transform.position;
			player.body.rotation = playerDuckPlace.transform.rotation;
			Camera.main.transform.position = player.transform.position;
			boat.Place(boatDuckPlace.position, boatDuckPlace.rotation);
			boat.Anchor(value: true);
			Singleton<GlobalData>.instance.gameData.tags.SetBool(brokenEngineTag, value: false);
			Singleton<GlobalData>.instance.gameData.tags.SetBool(duckFixingBoatPositionTag);
		}, delegate
		{
			this.RegisterTimer(1f, delegate
			{
				UnityEngine.Object.Destroy(inputLock);
				StartCoroutine(AfterBoatFixedRoutine(duckFixedBoatYarnNode, duckFixingBoatPositionTag, fixingDuckPosition.transform, normalDuckPosition.transform));
			});
		});
	}

	private IEnumerator AfterBoatFixedRoutine(string conversationNode, string positionTag, Transform fixingPosition, Transform originalPosition)
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool(fixedBoatTag);
		IConversation conversation = Singleton<GameServiceLocator>.instance.dialogue.StartConversation(conversationNode, fixingPosition);
		yield return new WaitUntil(() => !conversation.isAlive);
		while (Camera.main.IsPointInView(fixingPosition.position) || Camera.main.IsPointInView(originalPosition.position))
		{
			yield return new WaitForSeconds(10f);
		}
		Singleton<GlobalData>.instance.gameData.tags.SetBool(positionTag, value: false);
	}

	private GameUserInput ResetBoat(Action onSwitch, Action onDone = null)
	{
		GameUserInput inputLock = GameUserInput.CreateInput(base.gameObject);
		Singleton<GameServiceLocator>.instance.transitionAnimation.Begin(delegate
		{
			PlaceBoatAndPlayerHome();
			onSwitch?.Invoke();
		}, delegate
		{
			this.RegisterTimer(1f, delegate
			{
				UnityEngine.Object.Destroy(inputLock);
				onDone?.Invoke();
			});
		});
		return inputLock;
	}

	public void StartChallenge()
	{
		StartCoroutine(ChallengeCoroutine());
	}

	public void CancelChallenge()
	{
		if (waitForRaceNode != null)
		{
			waitForRaceNode.Cancel(WaitForRaceNode.WaitResult.Cancel);
		}
	}

	public void ResetChallenge()
	{
		if (waitForRaceNode != null)
		{
			waitForRaceNode.Cancel(WaitForRaceNode.WaitResult.Reset);
		}
	}

	private IEnumerator ChallengeCoroutine()
	{
		GameObject inputLock = GameUserInput.CreateInputGameObjectWithPriority(10);
		bool transitionDone = false;
		Singleton<GameServiceLocator>.instance.transitionAnimation.Begin(delegate
		{
			boat.Interact();
			boat.Place(raceStart.position, raceStart.rotation);
			ResetRaceObjects();
		}, delegate
		{
			transitionDone = true;
		});
		yield return new WaitUntil(() => transitionDone);
		yield return new WaitForSeconds(0.5f);
		UnityEngine.Object.Destroy(inputLock);
		float bestRaceTime = Singleton<GlobalData>.instance.gameData.tags.GetFloat(boatRaceBestTimeTag);
		bestRaceTime = ((bestRaceTime == 0f) ? initialBestRaceTime : bestRaceTime);
		Singleton<GlobalData>.instance.gameData.tags.SetString(boatRaceBestTimeTextTag, ConvertRaceTimeToDialogueString(bestRaceTime));
		DialogueController dialogue = Singleton<GameServiceLocator>.instance.dialogue;
		IConversation conversation = dialogue.StartConversation(beforeRaceStartYarnNode, boatKidAnimator.transform.parent);
		yield return new WaitUntil(() => !conversation.isAlive);
		StackResourceSortingKey silenceKey = Singleton<MusicManager>.instance.RegisterSilenece(50);
		Singleton<MusicManager>.instance.TrimRetiredActiveMusicSets(3f);
		music = new ScriptedMusic(raceMusic);
		music.Load();
		timerUI = Singleton<GameServiceLocator>.instance.ui.AddUI(timerUIPrefab.Clone()).GetComponent<TimerUI>();
		penalties = 0;
		inputLock = GameUserInput.CreateInputGameObjectWithPriority(10);
		yield return new WaitForSeconds(0.5f);
		yield return Countdown();
		UnityEngine.Object.Destroy(inputLock);
		BoatCameraTarget cameraTargetFocuser = base.gameObject.GetComponent<BoatCameraTarget>();
		if (cameraTargetFocuser == null)
		{
			cameraTargetFocuser = base.gameObject.AddComponent<BoatCameraTarget>().Setup(this);
		}
		else
		{
			cameraTargetFocuser.Revive();
		}
		silenceKey.ReleaseResource();
		music.Play();
		timerUI.Begin();
		RaceGoalUI goal = Singleton<GameServiceLocator>.instance.ui.AddUI(goalUIPrefab.Clone()).GetComponent<RaceGoalUI>();
		goal.disappearOnScreen = false;
		goal.offset = Vector2.up * 25f;
		boatArrow = Singleton<GameServiceLocator>.instance.ui.AddUI(boatArrowUIPrefab.Clone()).GetComponent<BoatGoalArrowUI>();
		boatArrow.destinationOffset = Vector2.up * 25f;
		Singleton<GlobalData>.instance.gameData.tags.SetBool(boatRaceActiveTag);
		Singleton<GlobalData>.instance.gameData.tags.SetBool(ranAwayFromRaceTag, value: false);
		Action cleanUpRace = delegate
		{
			UnityEngine.Object.Destroy(goal.gameObject);
			UnityEngine.Object.Destroy(boatArrow.gameObject);
			UnityEngine.Object.Destroy(timerUI.gameObject);
			music.Stop();
			Singleton<GlobalData>.instance.gameData.tags.SetBool(boatRaceActiveTag, value: false);
			foreach (BoatRaceCheckpoint checkpoint2 in checkpoints)
			{
				checkpoint2.Finished();
			}
			cameraTargetFocuser.Kill();
		};
		for (int i = 0; i < checkpoints.Count; i++)
		{
			BoatRaceCheckpoint checkpoint = checkpoints[i];
			checkpoint.Awaken();
			cameraTargetFocuser.SetCheckpoint(checkpoint.transform, (i < checkpoints.Count - 1) ? checkpoints[i + 1].transform : null);
			goal.destination = checkpoint.transform;
			boatArrow.destination = checkpoint.transform;
			waitForRaceNode = new WaitForRaceNode(this, checkpoint.transform);
			yield return waitForRaceNode;
			checkpoint.Finished();
			switch (waitForRaceNode.continueRaceResult)
			{
			case WaitForRaceNode.WaitResult.Continue:
				checkpointSound.Play();
				break;
			case WaitForRaceNode.WaitResult.Cancel:
				cleanUpRace();
				yield break;
			case WaitForRaceNode.WaitResult.Reset:
				cleanUpRace();
				StartChallenge();
				yield break;
			}
		}
		winnerSound.Play();
		confettiPrefab.CloneAt(player.transform.position).transform.parent = boat.transform;
		float time = timerUI.time;
		bool flag = time < bestRaceTime;
		if (flag)
		{
			Singleton<GlobalData>.instance.gameData.tags.SetFloat(boatRaceBestTimeTag, time);
			Singleton<GameServiceLocator>.instance.achievements.SetLeaderboard("BoatChallenge", Mathf.RoundToInt(time * 1000f));
		}
		if (time < achievementRaceTime)
		{
			Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.FastBoat);
		}
		Singleton<GlobalData>.instance.gameData.tags.SetBool(boatRaceWonTag, flag);
		Singleton<GlobalData>.instance.gameData.tags.SetString(boatRaceTimeTextTag, ConvertRaceTimeToDialogueString(time));
		Singleton<GlobalData>.instance.gameData.tags.SetFloat(boatRacePenaltiesTag, penalties);
		cleanUpRace();
		inputLock = GameUserInput.CreateInputGameObjectWithPriority(10);
		float originalDrag = boat.body.drag;
		boat.body.drag = 2f;
		yield return new WaitForSeconds(1.5f);
		boat.body.drag = originalDrag;
		UnityEngine.Object.Destroy(inputLock);
		boatKidAnimator.lookAt = Singleton<GameServiceLocator>.instance.levelController.player.transform;
		conversation = dialogue.StartConversation(finishedRaceYarnNode, boatKidAnimator.transform.parent);
		yield return new WaitUntil(() => !conversation.isAlive);
		boatKidAnimator.lookAt = null;
	}

	private string ConvertRaceTimeToDialogueString(float seconds)
	{
		return seconds.ToString("0.00");
	}

	private void ResetRaceObjects()
	{
		raceObjectsParent.SetActive(value: true);
		for (int i = 0; i < checkpoints.Count; i++)
		{
			checkpoints[i].Reset();
			foreach (Renderer item in checkpointRenderers[i])
			{
				item.gameObject.SetActive(value: true);
			}
		}
		if (cleanUpRaceCorountine != null)
		{
			StopCoroutine(cleanUpRaceCorountine);
		}
		cleanUpRaceCorountine = StartCoroutine(CleanUpRaceObjects());
	}

	private IEnumerator CleanUpRaceObjects()
	{
		bool done = false;
		while (!done)
		{
			done = true;
			for (int i = 0; i < checkpoints.Count; i++)
			{
				if (checkpoints[i].isFinished)
				{
					foreach (Renderer item in checkpointRenderers[i])
					{
						if (item.gameObject.activeSelf && !item.isVisible)
						{
							item.gameObject.SetActive(value: false);
						}
						else
						{
							done = false;
						}
						yield return null;
					}
				}
				else
				{
					done = false;
				}
				yield return null;
			}
		}
		Debug.Log("done cleanup");
	}

	private IEnumerator Countdown()
	{
		countdownSound.Play();
		yield return new WaitForSeconds(1f);
		countdownSound.Play();
		yield return new WaitForSeconds(1f);
		countdownSound.Play();
		yield return new WaitForSeconds(1f);
		countdownDone.Play();
		startSound.Play();
	}

	private void PlaceBoatAndPlayerHome()
	{
		player.body.position = playerBoatStart.transform.position;
		player.body.rotation = playerBoatStart.transform.rotation;
		Camera.main.transform.position = player.transform.position;
		boat.ResetPosition();
	}

	public IEnumerator SlowMotionRampRoutine()
	{
		if (!saveJumpData)
		{
			Transform obj = boat.transform;
			Vector3 position = (boat.body.position = rampTeleportPosition);
			obj.position = position;
			Transform obj2 = boat.transform;
			Quaternion rotation = (boat.body.rotation = rampTeleportRotation);
			obj2.rotation = rotation;
			boat.SetVelocity(rampTeleportVelocity);
		}
		else
		{
			rampTeleportPosition = boat.body.position;
			rampTeleportRotation = boat.body.rotation;
			rampTeleportVelocity = boat.body.velocity;
		}
		ActiveMusicSet activeMusic = null;
		MusicSet currentMusicSet = Singleton<MusicManager>.instance.currentMusicSet;
		if (currentMusicSet != null)
		{
			activeMusic = Singleton<MusicManager>.instance.GetActiveMusicSet(currentMusicSet);
		}
		Time.timeScale = 0.15f;
		activeMusic?.SetPitch(0.55f);
		boat.enginePitchMultiplier = 0.5f;
		rampJumpCamera.SetActive(value: true);
		StackResourceSortingKey emotionKeyKid = boatKidAnimator.ShowEmotion(Emotion.Surprise);
		StackResourceSortingKey emotionKeyPlayer = player.ikAnimator.ShowEmotion(Emotion.Surprise);
		Action poseKey = boatKidAnimator.Pose(Pose.RaiseArms);
		player.ikAnimator.lookAt = Camera.main.transform;
		boatKidAnimator.lookAt = Camera.main.transform;
		if (boatArrow != null)
		{
			boatArrow.gameObject.SetActive(value: false);
		}
		BoatCameraTarget component = base.gameObject.GetComponent<BoatCameraTarget>();
		if ((bool)component)
		{
			component.SetCheckpoint(null, null);
			component.ResetOffset();
		}
		StartCoroutine(EmitBubblesRoutine());
		GameObject inputLockPriority = GameUserInput.CreateInputGameObjectWithPriority(10);
		yield return new WaitForSecondsRealtime(4.5f);
		emotionKeyKid.ReleaseResource();
		emotionKeyPlayer.ReleaseResource();
		poseKey();
		player.ikAnimator.lookAt = null;
		boatKidAnimator.lookAt = null;
		Time.timeScale = 1f;
		activeMusic?.SetPitch(1f);
		boat.enginePitchMultiplier = 1f;
		rampJumpCamera.SetActive(value: false);
		yield return new WaitUntil(() => !boat.isMidair);
		boatCrashSound.Play();
		CancelChallenge();
		if (music != null)
		{
			music.Stop();
		}
		boat.SetEngineCooldown(0);
		Singleton<MusicManager>.instance.TrimRetiredActiveMusicSets(0.01f);
		yield return new WaitForSecondsRealtime(1.5f);
		UnityEngine.Object.Destroy(inputLockPriority);
		IConversation conversation = Singleton<GameServiceLocator>.instance.dialogue.StartConversation(crashedBoatYarnNode, boatKidAnimator.transform.parent);
		yield return new WaitUntil(() => !conversation.isAlive);
		Singleton<GlobalData>.instance.gameData.tags.SetBool(brokenEngineTag);
		GameUserInput inputLock = GameUserInput.CreateInput(base.gameObject);
		yield return new WaitForSeconds(0.4f);
		engineRevSound.Play();
		yield return new WaitForSeconds(1f);
		conversation = Singleton<GameServiceLocator>.instance.dialogue.StartConversation(crashedBoatYarnNode2, boatKidAnimator.transform.parent);
		yield return new WaitUntil(() => !conversation.isAlive);
		yield return new WaitForSeconds(0.4f);
		engineRevSound.Play().pitch = 0.8f;
		yield return new WaitForSeconds(1f);
		engineRevSound.Play().pitch = 1.2f;
		yield return new WaitForSeconds(0.5f);
		boat.SetEngineCooldown(1);
		yield return new WaitForSeconds(0.5f);
		conversation = Singleton<GameServiceLocator>.instance.dialogue.StartConversation(crashedBoatYarnNode3, boatKidAnimator.transform.parent);
		yield return new WaitUntil(() => !conversation.isAlive);
		UnityEngine.Object.Destroy(inputLock);
	}

	private IEnumerator EmitBubblesRoutine()
	{
		float bubblesPerSecond = 120f;
		float counter = 0f;
		float bubbleTime = 0f;
		while (counter < 3f)
		{
			bubbleTime += Time.deltaTime;
			int num = Mathf.FloorToInt(bubblesPerSecond * bubbleTime);
			bubbleTime -= (float)num / bubblesPerSecond;
			if (num > 0)
			{
				backBubblesBoat.Emit(num);
			}
			counter += Time.unscaledDeltaTime;
			yield return null;
		}
	}
}
