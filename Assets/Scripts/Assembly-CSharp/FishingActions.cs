using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using Cinemachine;
using QuickUnityTools.Audio;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.Audio;

public class FishingActions : MonoBehaviour, IHoldableAction
{
	public const int WATER_LAYER = 4;

	public static bool ALWAYS_TRIP = false;

	public static float FISH_CHEAT_MULTIPLIER = 1f;

	public const float BAIT_RARE_MULTIPLIER = 2f;

	public const float BAIT_NIBBLE_MULTIPLIER = 1.25f;

	public const float BAIT_ENCOUNTER_MULTIPLIER = 2f;

	[Header("Rod Visuals")]
	public float rodHoldAngle = 80f;

	public float rodHandRotationPercent = 0.4f;

	public Transform meshParent;

	public float sleepRodHoldAngle = 20f;

	public AnimationCurve rodSleepCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float rodAngleLerpSpeed = 10f;

	[Header("Wire Visuals")]
	public float minWireDistance = 2f;

	public float restLengthChangeSpeed = 0.1f;

	public float restLengthLimpChangeSpeed = 0.01f;

	[Header("Cast Settings")]
	public float castSpeed = 60f;

	public Transform spawnPosition;

	public GameObject endSpawnPrefab;

	public float maxDistance = 20f;

	public float snapDistance = 70f;

	public float pullStrength = 50f;

	public CollectableItem baitItem;

	public AnimationCurve encounterLikelihoodBonus;

	public float encounterLikelihoodBonusTime;

	[Header("Water Visuals")]
	public float nibbleForce = 15f;

	public GameObject biteParticlesPrefab;

	public GameObject waterRingPrefab;

	[Header("Struggle Settings")]
	public float randomPositionRadius = 25f;

	public float randomPositionRiverRadius = 10f;

	public float randomPositionRaycastFailRadius = 3f;

	public float randomPositionRaycastDistance = 25f;

	public LayerMask randomPositionRaycastLayers;

	public float biteSpeed = 2f;

	public float biteStruggleSpeed = 5f;

	public AnimationCurve struggleSpeedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	public float struggleSpeedCurvePeriod = 8f;

	public float strugglePitchLerpSpeed = 20f;

	public float biteTimeout = 1f;

	public float bittenDecalPeriod = 0.25f;

	public GameObject waterTrailPrefab;

	[Header("Catch Settings")]
	public float catchPointsRequired = 10f;

	public float maxCatchPointsPerSecond = 5f;

	public float catchPointsPerStruggle = 1f;

	public float catchPointsDecayPerSecond = 1f;

	public float failCatchPoints = -2f;

	private Transform caughtFish;

	[Header("Fish Settings")]
	public GameObject fishPrefab;

	public float rodEncounterChanceMultiplier = 1f;

	public float rodNibbleChanceMultiplier = 1f;

	public float rodRareChanceMultiplier = 1f;

	[Header("Audio Settings")]
	public AudioClip castSound;

	public AudioClip pullSound;

	public AudioClip biteSound;

	public AudioClip[] nibbleSounds;

	public AudioClip struggleSound;

	public AudioClip snapSound;

	public AudioMixerSnapshot defaultSnapshot;

	public AudioMixerSnapshot fishingSnapshot;

	public float snapshotFadeTime = 15f;

	public float snapshotReturnTime = 2f;

	private bool _allowSleeping = true;

	private Holdable holdable;

	private FishingLine fishingLine;

	private float currentRodAngle;

	private Rigidbody bobber;

	private Floater bobberFloater;

	private FishingBobber fishingBobber;

	private ICinemachineCamera bobberCamera;

	private CinemachineTargetGroup bobberCameraTargetGroup;

	private Player playerHolding;

	private bool triggerSnapshot;

	private float idleFishTime;

	private bool lineBaited;

	private Timer encounterCheckTimer;

	private Timer nibbleCheckTimer;

	private int requiredNibbles;

	private int nibbles;

	private float encounterStartTime;

	private bool isBitten;

	private float biteCountdown;

	private float bittenDecalCountdown;

	private bool isStruggling;

	private float bobberSplineTime;

	private BezierSpline bobberSpline;

	private AudioSource struggleSoundSource;

	private WaterTrail waterTrail;

	private StackResourceSortingKey eyesEmotionKey;

	private StackResourceSortingKey struggleEmotionKey;

	private StackResourceSortingKey sleepEmotionKey;

	private Timer closeEyesTimer;

	private float catchPoints;

	private List<GameObject> orphanedGameObjects = new List<GameObject>();

	public bool isCast => bobber != null;

	public bool tutorialMode { get; set; }

	public FishSpecies fishEncounter { get; set; }

	public bool allowSleeping
	{
		get
		{
			return _allowSleeping;
		}
		set
		{
			_allowSleeping = value;
			if (!value)
			{
				idleFishTime = 5f;
				StackResourceSortingKey.Release(sleepEmotionKey);
				sleepEmotionKey = null;
			}
		}
	}

	private FishingEnvironment fishingEnvironment => fishingBobber?.fishingEnvironment;

	private void Awake()
	{
		holdable = GetComponent<Holdable>();
		fishingLine = GetComponentInChildren<FishingLine>();
		currentRodAngle = rodHoldAngle;
	}

	private void OnDestroy()
	{
		if (isCast)
		{
			PullLine();
		}
	}

	public void ActivateAction(int parameter)
	{
		if (parameter == 0)
		{
			if (isBitten)
			{
				Struggle();
			}
			else if (isCast)
			{
				PullLine();
			}
			else if (!caughtFish)
			{
				CastLine();
			}
		}
		if (parameter == 1 && (bool)playerHolding && idleFishTime > 38f)
		{
			StackResourceSortingKey.Release(sleepEmotionKey);
			sleepEmotionKey = playerHolding.ikAnimator.ShowEmotion(Emotion.EyesClosed);
		}
	}

	private void Update()
	{
		if ((bool)holdable.anchoredTo)
		{
			Player.PlayerInput input = holdable.anchoredTo.input;
			bool flag = input.GetMovement().sqrMagnitude > 0.1f || input.IsJumpHeld() || input.IsRunHeld() || input.IsUseItemHeld() || input.IsInteractHeld();
			if (isCast && bobberFloater.inWater && !isBitten && (!flag || tutorialMode) && holdable.anchoredTo.body.linearVelocity.SetY(0f).sqrMagnitude < 1f)
			{
				idleFishTime += Time.deltaTime;
			}
			else
			{
				idleFishTime = 0f;
				StackResourceSortingKey.Release(sleepEmotionKey);
				sleepEmotionKey = null;
			}
			holdable.anchoredTo.ikAnimator.SetFishingTime(idleFishTime, allowSleeping);
		}
		if (isBitten)
		{
			bobberSpline.MoveAlongSpline(ref bobberSplineTime, struggleSpeedCurve.Evaluate(Time.time / struggleSpeedCurvePeriod % 1f) * (isStruggling ? biteStruggleSpeed : biteSpeed) * Time.deltaTime);
			bobber.transform.position = bobberSpline.GetPoint(bobberSplineTime);
			if (bobberSplineTime >= 1f)
			{
				bobberSplineTime = 0f;
			}
			if (!isStruggling)
			{
				biteCountdown -= Time.deltaTime;
				if (biteCountdown <= 0f && !tutorialMode)
				{
					LoseFish();
				}
				bittenDecalCountdown -= Time.deltaTime;
				if (bittenDecalCountdown <= 0f)
				{
					bittenDecalCountdown = bittenDecalPeriod;
					SpawnFishingLineWaterEffect();
				}
			}
		}
		if (isStruggling)
		{
			Vector3 inputWorldDirection = playerHolding.GetInputWorldDirection();
			float num = ((inputWorldDirection == Vector3.zero) ? 180f : Vector3.Angle(inputWorldDirection, playerHolding.transform.position - bobber.transform.position));
			bool flag2 = num < 90f;
			if (eyesEmotionKey == null && flag2)
			{
				eyesEmotionKey = playerHolding.ikAnimator.ShowEmotion(Emotion.EyesClosed);
			}
			else if (eyesEmotionKey != null && !flag2)
			{
				eyesEmotionKey.ReleaseResource();
				eyesEmotionKey = null;
			}
			float t = catchPoints / catchPointsRequired;
			float t2 = Mathf.InverseLerp(90f, 30f, (inputWorldDirection != Vector3.zero) ? num : 90f);
			float b = Mathf.Lerp(0.8f, 1.3f, t) + Mathf.Lerp(-0.2f, 0.2f, t2);
			struggleSoundSource.pitch = Mathf.Lerp(struggleSoundSource.pitch, b, strugglePitchLerpSpeed * Time.deltaTime);
			waterTrail.transform.position = GetLineWaterIntersection();
			catchPoints -= catchPointsDecayPerSecond * Time.deltaTime;
			if (num < 90f)
			{
				catchPoints += Mathf.InverseLerp(90f, 0f, num) * maxCatchPointsPerSecond * Time.deltaTime;
			}
			if (catchPoints > catchPointsRequired)
			{
				CatchFish();
			}
			else if (!tutorialMode && catchPoints < failCatchPoints)
			{
				LoseFish();
			}
		}
		if (triggerSnapshot && bobberFloater.inWater)
		{
			fishingSnapshot.TransitionTo(snapshotFadeTime);
		}
		if (isCast && playerHolding.isSwimming)
		{
			PullLine();
		}
	}

	private void FixedUpdate()
	{
		float num = minWireDistance;
		if ((bool)bobber)
		{
			Vector3 vector = spawnPosition.position - bobber.transform.position;
			float magnitude = vector.magnitude;
			if (!isBitten)
			{
				num = Mathf.Min(magnitude, maxDistance);
			}
			if (magnitude > maxDistance)
			{
				float num2 = magnitude - maxDistance;
				Vector3 force = vector / magnitude * num2 * pullStrength;
				bobber.AddForce(force);
			}
			if (magnitude > snapDistance)
			{
				PullLine();
			}
		}
		float num3 = num / (float)(fishingLine.segments - 1);
		float num4 = (((bool)bobber && num3 < fishingLine.restLength) ? restLengthLimpChangeSpeed : restLengthChangeSpeed);
		fishingLine.restLength = Mathf.MoveTowards(fishingLine.restLength, num3, num4 * Time.fixedDeltaTime);
	}

	private void LateUpdate()
	{
		if ((bool)holdable.anchoredTo)
		{
			float b = Mathf.Lerp(rodHoldAngle, sleepRodHoldAngle, rodSleepCurve.Evaluate(idleFishTime / 38f));
			currentRodAngle = Mathf.Lerp(currentRodAngle, b, rodAngleLerpSpeed * Time.deltaTime);
			Vector3 forward = holdable.anchoredTo.transform.forward;
			forward = Quaternion.AngleAxis(0f - currentRodAngle, holdable.anchoredTo.transform.right) * forward;
			meshParent.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(forward, base.transform.forward, rodHandRotationPercent), Vector3.up);
		}
	}

	private void SpawnFishingLineWaterEffect()
	{
		if (bobberFloater.waterRegion != null)
		{
			GameObject obj = waterRingPrefab.CloneAt(bobberFloater.transform.position);
			obj.transform.position = GetLineWaterIntersection();
			obj.GetComponent<WaterDecal>().region = bobberFloater.waterRegion;
		}
	}

	private Vector3 GetLineWaterIntersection()
	{
		Vector3 position = bobberFloater.transform.position;
		float num = bobberFloater.waterRegion.GetWaterY(position) - position.y;
		Vector3 vector = spawnPosition.position - position;
		float num2 = vector.SetY(0f).magnitude / vector.y * num;
		return position + vector.SetY(0f).normalized * num2;
	}

	private void CastLine()
	{
		if (isCast)
		{
			Debug.LogWarning("Cannot cast the line when it's already out.");
		}
		else
		{
			if (!holdable.anchoredTo)
			{
				return;
			}
			GameObject gameObject = endSpawnPrefab.CloneAt(spawnPosition.position);
			bobber = gameObject.GetComponent<Rigidbody>();
			bobberFloater = bobber.GetComponent<Floater>();
			fishingBobber = bobber.GetComponent<FishingBobber>();
			bobber.linearVelocity = holdable.anchoredTo.transform.forward * castSpeed;
			bobber.AddTorque(Random.insideUnitSphere * 720f, ForceMode.Impulse);
			fishingLine.endConstraint = gameObject.transform;
			playerHolding = holdable.anchoredTo;
			playerHolding.disableInteraction = true;
			lineBaited = false;
			if (FishingBait.isBaitActive && Singleton<GlobalData>.instance.gameData.GetCollected(baitItem) > 0)
			{
				Singleton<GlobalData>.instance.gameData.AddCollected(baitItem, -1);
				lineBaited = true;
			}
			castSound.Play();
			triggerSnapshot = true;
			playerHolding.ikAnimator.lookAt = bobber.transform;
			ICinemachineCamera activeVirtualCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
			if ((bool)(activeVirtualCamera.VirtualCameraGameObject.GetComponent<ICinemachineCamera>() as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>())
			{
				GameObject gameObject2 = activeVirtualCamera.VirtualCameraGameObject.Clone();
				bobberCamera = gameObject2.GetComponent<ICinemachineCamera>();
				bobberCamera.Priority = Mathf.Min(900, activeVirtualCamera.Priority + 1);
				bobberCameraTargetGroup = new GameObject("Fishing Target Group").AddComponent<CinemachineTargetGroup>();
				bobberCameraTargetGroup.AddMember(holdable.anchoredTo.transform, 1f, 1f);
				bobberCameraTargetGroup.AddMember(bobber.transform, 1f, 1f);
				bobberCamera.Follow = bobberCameraTargetGroup.transform;
				CinemachineVirtualCamera cinemachineVirtualCamera = bobberCamera as CinemachineVirtualCamera;
				CinemachineFramingTransposer cinemachineComponent = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
				if ((bool)cinemachineComponent)
				{
					cinemachineComponent.m_MaximumFOV = cinemachineVirtualCamera.m_Lens.FieldOfView;
					cinemachineComponent.m_MinimumFOV = cinemachineVirtualCamera.m_Lens.FieldOfView;
					cinemachineComponent.m_MaxDollyIn = 2.5f;
					cinemachineComponent.m_MaxDollyOut = 20f;
					cinemachineComponent.m_GroupFramingSize = 0.7f;
				}
			}
			fishEncounter = null;
			Timer.Cancel(encounterCheckTimer);
			encounterCheckTimer = this.RegisterTimer(1f, CheckForEncounters, isLooped: true);
		}
	}

	private void CheckForEncounters()
	{
		if (!fishingEnvironment || (bool)fishEncounter || tutorialMode)
		{
			return;
		}
		float value = Random.value;
		float num = fishingEnvironment.encounterChancePerSecond * FISH_CHEAT_MULTIPLIER * (lineBaited ? 2f : 1f) * rodEncounterChanceMultiplier * encounterLikelihoodBonus.Evaluate(Mathf.Clamp01(idleFishTime / encounterLikelihoodBonusTime));
		if (value < num)
		{
			fishEncounter = fishingEnvironment.fish.Select((FishProbability f) => f.species).ToList().PickRandomWithWeights(fishingEnvironment.fish.Select((FishProbability f) => f.likelihood).ToList());
			requiredNibbles = Random.Range(fishEncounter.minRequiredNibbles, fishEncounter.maxRequiredNibbles + 1);
			nibbles = 0;
			encounterStartTime = Time.time;
			Timer.Cancel(nibbleCheckTimer);
			nibbleCheckTimer = this.RegisterTimer(fishEncounter.nibbleCheckPeriod, CheckForNibbles, isLooped: true);
		}
	}

	private void CheckForNibbles()
	{
		if (tutorialMode)
		{
			return;
		}
		if (!fishEncounter)
		{
			Timer.Cancel(nibbleCheckTimer);
		}
		else
		{
			if (isBitten || !bobberFloater.inWater)
			{
				return;
			}
			if (Time.time > encounterStartTime + fishEncounter.encounterTime)
			{
				fishEncounter = null;
				Timer.Cancel(nibbleCheckTimer);
				return;
			}
			float value = Random.value;
			float num = fishEncounter.nibbleChance * FISH_CHEAT_MULTIPLIER * (lineBaited ? 1.25f : 1f) * rodNibbleChanceMultiplier;
			if (value < num)
			{
				nibbles++;
				if (nibbles == requiredNibbles)
				{
					Bite();
				}
				else
				{
					Nibble();
				}
			}
		}
	}

	public void Nibble()
	{
		if (isCast)
		{
			fishingLine.restLength = Mathf.Lerp(fishingLine.restLength, minWireDistance / (float)fishingLine.segments, 0.15f);
			bobber.AddForce(Vector3.down * nibbleForce, ForceMode.Impulse);
			bobberFloater.SpawnWaterRipple();
			nibbleSounds.PickRandom().Play().pitch += (Random.value - 0.5f) * 0.5f;
		}
	}

	public void Bite()
	{
		if (isBitten)
		{
			Debug.LogWarning("Cannot bite when already bitten!");
			return;
		}
		isBitten = true;
		bobber.isKinematic = true;
		bobber.position += Vector3.down * 1f;
		biteSound.Play();
		bobberFloater.SpawnWaterRipple();
		biteParticlesPrefab.CloneAt(bobber.transform.position);
		fishingLine.SetTotalRestLength(minWireDistance);
		bittenDecalCountdown = bittenDecalPeriod;
		bobberSpline = CreateBobberBittenSpline();
		bobberSplineTime = 0f;
		biteCountdown = biteTimeout;
	}

	private BezierSpline CreateBobberBittenSpline()
	{
		BezierSpline bezierSpline = new GameObject("Bobber Spline").AddComponent<BezierSpline>();
		bezierSpline.Initialize(10);
		bezierSpline[0].position = bobber.position;
		bezierSpline[9].position = bobber.position;
		for (int i = 1; i < 9; i++)
		{
			bezierSpline[i].position = GetRandomBitePosition();
		}
		bezierSpline.AutoConstructSpline();
		return bezierSpline;
	}

	private void Struggle()
	{
		if (!holdable.anchoredTo)
		{
			return;
		}
		if (!isStruggling)
		{
			isStruggling = true;
			catchPoints = 0f;
			waterTrail = waterTrailPrefab.CloneAt(bobber.transform.position).GetComponent<WaterTrail>();
			waterTrail.region = bobberFloater.waterRegion;
			playerHolding.pullingAgainst = bobberFloater.transform;
			playerHolding.ikAnimator.SetPulling(isPulling: true);
			if (struggleSoundSource == null)
			{
				struggleSoundSource = Singleton<SoundPlayer>.instance.PlayLooped(struggleSound, base.transform.position);
			}
		}
		catchPoints += catchPointsPerStruggle;
		struggleSoundSource.pitch += 0.5f;
		CloseEyes(0.4f);
	}

	private void CloseEyes(float time)
	{
		if (closeEyesTimer != null)
		{
			Timer.Cancel(closeEyesTimer);
			StackResourceSortingKey.Release(struggleEmotionKey);
			closeEyesTimer = null;
			struggleEmotionKey = null;
		}
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		struggleEmotionKey = player.ikAnimator.ShowEmotion(Emotion.EyesClosed);
		closeEyesTimer = this.RegisterTimer(time, delegate
		{
			struggleEmotionKey.ReleaseResource();
			struggleEmotionKey = null;
			closeEyesTimer = null;
		});
	}

	private Vector3 GetRandomBitePosition()
	{
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 vector = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
		Vector3 vector2 = bobber.position + vector * (bobberFloater.waterRegion.isRiver ? randomPositionRiverRadius : randomPositionRadius);
		if (Physics.Raycast(new Ray(vector2 + Vector3.up * randomPositionRaycastDistance, Vector3.down), out var hitInfo, randomPositionRaycastDistance, randomPositionRaycastLayers, QueryTriggerInteraction.Collide) && hitInfo.collider.gameObject.layer == 4)
		{
			return vector2;
		}
		return bobber.position + vector * randomPositionRaycastFailRadius;
	}

	private void LoseFish()
	{
		bobberFloater.SpawnWaterRipple();
		biteParticlesPrefab.CloneAt(bobber.transform.position);
		PullLine();
	}

	private void CatchFish()
	{
		bobberFloater.SpawnWaterRipple();
		biteParticlesPrefab.CloneAt(bobber.transform.position);
		float num = fishingEnvironment.rareLikelihood * fishEncounter.rareLikelihood * (lineBaited ? 2f : 1f) * rodRareChanceMultiplier;
		bool rare = Random.value < num;
		caughtFish = fishPrefab.CloneAt(bobber.position).transform;
		caughtFish.GetComponent<CaughtFishAnimation>().fish = new Fish(fishEncounter, rare);
		fishingLine.endConstraint = caughtFish;
		float duration = 1.5f;
		if (!tutorialMode && (Random.value < 0.35f || ALWAYS_TRIP))
		{
			duration = 3.4f;
			playerHolding.ikAnimator.PullRecoilAndTrip();
		}
		else
		{
			playerHolding.ikAnimator.PullRecoil();
		}
		this.RegisterTimer(0.25f, delegate
		{
			CloseEyes(0.65f);
		});
		GameUserInput inputLock = GameUserInput.CreateInput(base.gameObject);
		this.RegisterTimer(duration, delegate
		{
			Object.Destroy(inputLock);
		});
		PullLine();
	}

	private void Unbite()
	{
		isStruggling = false;
		if (struggleSoundSource != null)
		{
			Object.Destroy(struggleSoundSource.gameObject);
			struggleSoundSource = null;
		}
		if ((bool)waterTrail)
		{
			waterTrail.Kill();
			waterTrail = null;
		}
		playerHolding.pullingAgainst = null;
		playerHolding.ikAnimator.SetPulling(isPulling: false);
		StackResourceSortingKey.Release(eyesEmotionKey);
		eyesEmotionKey = null;
		isBitten = false;
		bobber.isKinematic = false;
		Object.Destroy(bobberSpline.gameObject);
		fishEncounter = null;
	}

	private void PullLine()
	{
		if (!isCast)
		{
			Debug.LogWarning("Cannot pull the line when the bobber isn't out.");
			return;
		}
		if (isBitten)
		{
			if (!caughtFish)
			{
				snapSound.Play();
			}
			Unbite();
		}
		else if (lineBaited)
		{
			Singleton<GlobalData>.instance.gameData.AddCollected(baitItem, 1);
		}
		pullSound.Play();
		fishingLine.SetTotalRestLength(minWireDistance);
		triggerSnapshot = false;
		defaultSnapshot.TransitionTo(snapshotReturnTime);
		if (playerHolding.ikAnimator.lookAt == bobber.transform)
		{
			playerHolding.ikAnimator.lookAt = null;
		}
		playerHolding.disableInteraction = false;
		idleFishTime = 0f;
		playerHolding.ikAnimator.SetFishingTime(0f, allowSleeping: true);
		StackResourceSortingKey.Release(sleepEmotionKey);
		sleepEmotionKey = null;
		playerHolding = null;
		ICinemachineCamera cinemachineCamera = bobberCamera;
		CinemachineTargetGroup cinemachineTargetGroup = bobberCameraTargetGroup;
		GameObject gameObject = bobber.gameObject;
		Timer.Cancel(encounterCheckTimer);
		Timer.Cancel(nibbleCheckTimer);
		if (fishingLine.endConstraint == bobber.transform)
		{
			fishingLine.endConstraint = null;
		}
		bobber = null;
		bobberCamera = null;
		bobberCameraTargetGroup = null;
		if (cinemachineCamera != null)
		{
			cinemachineCamera.VirtualCameraGameObject.SetActive(value: false);
			cinemachineTargetGroup.enabled = false;
		}
		gameObject.gameObject.SetActive(value: false);
		CleanUp(gameObject.gameObject);
		if (cinemachineCamera != null)
		{
			CleanUp(cinemachineCamera.VirtualCameraGameObject);
			CleanUp(cinemachineTargetGroup.gameObject);
		}
	}

	private void CleanUp(GameObject gameObject)
	{
		orphanedGameObjects.Add(gameObject);
		Timer.Register(6f, delegate
		{
			Debug.Log("clean up object");
			if (orphanedGameObjects.Contains(gameObject))
			{
				orphanedGameObjects.Remove(gameObject);
				Object.Destroy(gameObject);
			}
		});
		if (orphanedGameObjects.Count > 9)
		{
			Debug.Log("clean up pops off");
			GameObject obj = orphanedGameObjects[0];
			orphanedGameObjects.RemoveAt(0);
			Object.Destroy(obj);
		}
	}
}
