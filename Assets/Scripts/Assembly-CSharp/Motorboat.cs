using System;
using QuickUnityTools.Audio;
using UnityEngine;

public class Motorboat : Vehicle
{
	private static bool shownDismountPrompt;

	[Header("Motorboat Physics")]
	public float gentleForce = 150f;

	public float motorForce = 400f;

	public float forwardDrag = 0.5f;

	public float sideDrag = 1f;

	public float sideLossToForwardPercent = 0.9f;

	public Range turnSpeed = new Range(0.75f, 1.6f);

	public Range turnSpeedVelocity = new Range(10f, 40f);

	public float turnRampUpAcceleration = 1000f;

	public float gentleMaxSpeed = 25f;

	public float motorMaxSpeed = 80f;

	public float limitAccelerationSpeed = 60f;

	public float limitedAcceleration = 1f;

	public AnimationCurve throttlePower;

	public float throttlePowerTime = 3f;

	public Collider frontCollider;

	public Collider[] playerIgnoreColliders;

	public Collider groundCollider;

	public float unmountCooldownTime = 0.15f;

	public float unmountedMass = 50f;

	public float mountedMass = 10f;

	public float waterNormalSampleLength = 1f;

	public bool debugSpeed;

	public LayerMask groundRaycastLayers;

	public float groundRaycastLength = 1f;

	[Header("Tank Controls")]
	public bool tankControls = true;

	public float tankControlsTurnAngle = 45f;

	public float tankControlsTurnAcceleration = 10f;

	[Header("Motorboat Animation")]
	public float tiltLerpSpeed = 1f;

	public float maxHorizontalTilt = 40f;

	public float maxTiltSpeed = 60f;

	public float maxForwardTilt = 30f;

	public float forwardTiltSpeedFactor = 0.5f;

	public float forwardTiltAccelerationFactor = 2f;

	public float forwardTiltYSpeedFactor = 1f;

	public float minForwardTiltSpeed = 20f;

	public float maxForwardTiltSpeed = 60f;

	public float motorMeshTurnFactor = 0.1f;

	public AnimationCurve wiggleBoatAnimation;

	public float wiggleBoatAnimationTime = 1f;

	public float playerLerpLength = 1f;

	[Header("Motorboat Effects")]
	public WaterTrail waterTrail;

	public float waterTrailOffset;

	public Range waterTrailScrollSpeed = new Range(0.01f, 0.1f);

	public ParticleSystem bubbleParticles;

	public ParticleSystem backFlingParticles;

	public ParticleSystem frontLeftFoam;

	public ParticleSystem frontRightFoam;

	public ParticleSystem leftWaterFan;

	public ParticleSystem rightWaterFan;

	public ParticleSystem bigSplashParticles;

	public Range speedToEffectStrength = new Range(10f, 70f);

	public float bubbleOffset = 0.1f;

	public Transform frontWaterCheck;

	public Transform backWaterCheck;

	public float fanDisappearSpeed = 15f;

	public Range bigSplashRequiredYVelocity = new Range(-40f, -10f);

	[Header("Motorboat Audio")]
	public AudioSource engineAudio;

	public float enginePitchMultiplier = 1f;

	public float maxSpeedPitch = 1.2f;

	public float minSpeedPitch = 0.9f;

	public Range speedToPitchRange = new Range(10f, 70f);

	public float idlePitch = 0.7f;

	public float pitchChangeSpeed = 1f;

	public float motorCooldownTime = 5f;

	public float motorPowerUpSpeed = 3f;

	public AnimationCurve motorCooldownPitchMultiplier;

	public AnimationCurve motorCooldownVolumeMultiplier;

	public float engineMaxVolume = 0.8f;

	public AudioSource waterFlowAudio;

	public Range waterFlowPitchRange = new Range(0.7f, 1.8f);

	public AnimationCurve waterFlowVolumeOverSpeed;

	public AudioClip[] impactSounds;

	public Range impactSoundForceRange = new Range(150f, 800f);

	public float impactForceRandomization = 200f;

	public Range impactForcePitch = new Range(0.9f, 1.1f);

	public AudioSource waterRushAudio;

	public AnimationCurve waterRushAudioVolume;

	public AudioClip waterSplashSound;

	public Range waterSplashVolume = new Range(0.25f, 0.75f);

	public AudioClip[] waterTurnSounds;

	public float waterTurnSoundVolume = 0.5f;

	public float waterTurnSoundAngle = 120f;

	public Range scrapingPitchRange = new Range(0.8f, 1.2f);

	public Range scrapingSpeedRange = new Range(0f, 10f);

	public AudioSource scrapingSound;

	public AudioSource bumpSounds;

	public AudioClip honkSound;

	[Header("Motorboat Honking")]
	public float honkRadius = 50f;

	[Header("Motorboat Links")]
	public string noBoatKeyYarnNode = "NoBoatKeyStart";

	public string boatAnchoredTag = "BoatAnchored";

	public string dismountCounterTag = "DismountCounter";

	public CollectableItem boatKey;

	public Renderer boatRenderer;

	public Transform boatMesh;

	public Transform boatMotor;

	public Transform animatedBoatMesh;

	public GameObject dismountPromptUI;

	public FlexibleTriggerInteractable interactableTrigger;

	public float interactableDismountCooldown = 2f;

	private Floater floater;

	private StandUpStraight straightener;

	private Material trailMaterial;

	private ParticleSystem.Particle[] particles;

	private float horizontalTilt;

	private float forwardTilt;

	private float lastTurnAngle;

	private float throttleHeld;

	private float motorRotation;

	private float wiggleBoatAnimationCountdown;

	private float playerLerpCountdown;

	private Vector3 playerLerpStartLocalPosition;

	private Quaternion playerLerpStartRotation;

	private WaterRegion lastWaterRegion;

	private float motorCooldown;

	private Action undoPose;

	private float unmountCountdown;

	private AudioSource turnSoundSource;

	private bool disableParticleEffects;

	private float fanDisappearCountdown;

	private bool isGrounded;

	private NPCIKAnimator[] nearbyNPCs = new NPCIKAnimator[5];

	private AudioSource honkSource;

	private Timer honkCooldownTimer;

	private Vector3 startPos;

	private Quaternion startRot;

	private float tankControlsTurnSpeed;

	private Timer dismountPromptCooldown;

	private Timer interactableDismountCooldownTimer;

	private float fluffEmissionRate;

	private float fluffParticleSpeed;

	private float waterFanParticleSpeed;

	private float backFlingEmissionRate;

	private float bubbleEmissionRate;

	private float horizontalTiltSignedPercent;

	private float bigSplashParticleSpeed;

	public Transform autoSteerTowards { get; set; }

	public float prevSpeed { get; private set; }

	public bool isMidair
	{
		get
		{
			if (!isGrounded)
			{
				return !floater.inWater;
			}
			return false;
		}
	}

	public event Action<float, Collision> onBumpedBoat;

	protected override void Start()
	{
		base.Start();
		floater = GetComponent<Floater>();
		straightener = GetComponent<StandUpStraight>();
		trailMaterial = waterTrail.GetComponent<TrailRenderer>().material;
		floater.onWaterRegionChanged += OnWaterRegionChanged;
		waterTrail.transform.parent = null;
		particles = new ParticleSystem.Particle[bubbleParticles.main.maxParticles];
		fluffEmissionRate = frontLeftFoam.emission.rateOverTimeMultiplier;
		fluffParticleSpeed = frontLeftFoam.main.startSpeedMultiplier;
		waterFanParticleSpeed = leftWaterFan.main.startSpeedMultiplier;
		bigSplashParticleSpeed = leftWaterFan.main.startSpeedMultiplier;
		backFlingEmissionRate = backFlingParticles.emission.rateOverTimeMultiplier;
		bubbleEmissionRate = bubbleParticles.emission.rateOverTimeMultiplier;
		frontCollider.enabled = false;
		base.body.mass = unmountedMass;
		UpdateJitterFix(Player.jitterFix);
		Player.onJitterFixChanged += UpdateJitterFix;
		startPos = base.transform.position;
		startRot = base.transform.rotation;
		Anchor(value: true);
	}

	private void OnDestroy()
	{
		Player.onJitterFixChanged -= UpdateJitterFix;
	}

	public override void Interact()
	{
		if (Singleton<GlobalData>.instance.gameData.GetCollected(boatKey) > 0)
		{
			base.Interact();
		}
		else
		{
			Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(noBoatKeyYarnNode, base.player.transform);
		}
	}

	public void ResetPosition()
	{
		Place(startPos, startRot);
		Anchor(value: true);
	}

	public void Place(Vector3 pos, Quaternion rot)
	{
		base.body.position = pos;
		base.body.rotation = rot;
		base.transform.position = pos;
		base.transform.rotation = rot;
		base.body.velocity = Vector3.zero;
	}

	public void SetVelocity(Vector3 velocity)
	{
		base.body.velocity = velocity;
		prevSpeed = velocity.magnitude;
	}

	public void Anchor(bool value)
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool(boatAnchoredTag, value);
		base.body.constraints = (value ? ((RigidbodyConstraints)42) : RigidbodyConstraints.None);
	}

	private void UpdateJitterFix(bool enabled)
	{
		base.body.interpolation = ((!enabled) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
	}

	private void OnWaterRegionChanged(WaterRegion region)
	{
		if (region != null)
		{
			waterTrail.region = region;
			if (base.body.velocity.y < bigSplashRequiredYVelocity.max)
			{
				float num = 1f - bigSplashRequiredYVelocity.InverseLerp(base.body.velocity.y);
				waterSplashSound.Play().volume = waterSplashVolume.Lerp(num);
				ParticleSystem.MainModule main = bigSplashParticles.main;
				main.startSpeedMultiplier = bigSplashParticleSpeed * Mathf.Lerp(0.5f, 1f, num);
				bigSplashParticles.Play();
			}
		}
	}

	protected override void Enter()
	{
		base.Enter();
		if (!engineAudio.isPlaying)
		{
			engineAudio.Play();
		}
		if (!waterFlowAudio.isPlaying)
		{
			waterFlowAudio.Play();
		}
		if (!waterRushAudio.isPlaying)
		{
			waterRushAudio.Play();
		}
		frontCollider.enabled = true;
		base.body.mass = mountedMass;
		Anchor(value: false);
		wiggleBoatAnimationCountdown = wiggleBoatAnimationTime;
		playerLerpStartLocalPosition = base.transform.InverseTransformPoint(base.player.transform.position);
		playerLerpStartRotation = base.player.transform.rotation;
		playerLerpCountdown = playerLerpLength;
		base.player.DropOrStashHeldItem();
		base.player.ikAnimator.SetBoatSit(isMounted: true);
		Collider[] array = playerIgnoreColliders;
		foreach (Collider collider in array)
		{
			Physics.IgnoreCollision(base.player.myCollider, collider, ignore: true);
		}
		Timer.Cancel(interactableDismountCooldownTimer);
		interactableTrigger.enabled = false;
		if (!shownDismountPrompt || Singleton<GlobalData>.instance.gameData.tags.GetInt(dismountCounterTag) < 2)
		{
			ShowDismountPrompt();
		}
	}

	protected override void Exit()
	{
		base.Exit();
		frontCollider.enabled = false;
		base.body.mass = unmountedMass;
		wiggleBoatAnimationCountdown = wiggleBoatAnimationTime;
		unmountCountdown = unmountCooldownTime;
		base.player.ikAnimator.SetBoatSit(isMounted: false);
		Collider[] array = playerIgnoreColliders;
		foreach (Collider collider in array)
		{
			Physics.IgnoreCollision(base.player.myCollider, collider, ignore: false);
		}
		base.player.transform.position = mountPosition.position;
		base.player.body.velocity = base.body.velocity;
		base.player.body.position = mountPosition.position;
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		tags.SetInt(dismountCounterTag, tags.GetInt(dismountCounterTag) + 1);
		interactableDismountCooldownTimer = this.RegisterTimer(interactableDismountCooldown, delegate
		{
			interactableTrigger.enabled = true;
		});
	}

	protected override void Update()
	{
		base.Update();
		if (base.mounted)
		{
			if ((input.button4.ConsumePress() || input.menuButton.wasPressed) && playerLerpCountdown <= 0f)
			{
				Exit();
			}
			if (input.button3.wasPressed)
			{
				honkSource = honkSound.Play();
				AlertHonkables();
			}
			if (!input.button3.isPressed && (bool)honkSource)
			{
				honkSource.gameObject.AddComponent<FadeOutAudioSource>().fadeOutTime = 0.1f;
				honkSource = null;
			}
			if (prevSpeed < gentleMaxSpeed && (input.button2.isPressed || input.button3.isPressed) && Singleton<GlobalData>.instance.gameData.tags.GetInt(dismountCounterTag) < 1)
			{
				ShowDismountPrompt();
			}
		}
		else if (base.transform.position.y < 1f)
		{
			base.transform.position = base.transform.position.SetY(1f);
			base.body.velocity = base.body.velocity.SetY(0f);
		}
		bool flag = base.player.groundHit.HasValue && base.player.groundHit.Value.collider == groundCollider && !base.mounted;
		if (base.player.movingPlatform != base.body && flag)
		{
			base.player.movingPlatform = base.body;
		}
		else if (base.player.movingPlatform == base.body && !flag)
		{
			base.player.movingPlatform = null;
		}
		if (wiggleBoatAnimationCountdown > 0f)
		{
			animatedBoatMesh.localEulerAngles = new Vector3(0f, 0f, wiggleBoatAnimation.Evaluate(1f - wiggleBoatAnimationCountdown / wiggleBoatAnimationTime));
			wiggleBoatAnimationCountdown -= Time.deltaTime;
			if (wiggleBoatAnimationCountdown <= 0f)
			{
				animatedBoatMesh.localEulerAngles = Vector3.zero;
			}
		}
		UpdateSoundEffects();
		UpdateWaterParticleEffects();
		isGrounded = !base.mounted || Physics.Raycast(new Ray(base.transform.position, Vector3.down), groundRaycastLength, groundRaycastLayers);
		straightener.enabled = !isMidair;
	}

	private void ShowDismountPrompt()
	{
		if (dismountPromptCooldown == null)
		{
			Singleton<GameServiceLocator>.instance.ui.AddUI(dismountPromptUI.Clone());
			shownDismountPrompt = true;
			Timer.Cancel(dismountPromptCooldown);
			dismountPromptCooldown = this.RegisterTimer(10f, delegate
			{
				dismountPromptCooldown = null;
			});
		}
	}

	private void AlertHonkables()
	{
		int num = NPCIKAnimator.FindNearby(base.transform.position, honkRadius, nearbyNPCs);
		if (num == 0 || honkCooldownTimer != null)
		{
			return;
		}
		base.player.ikAnimator.lookAt = nearbyNPCs[0].transform;
		StackResourceSortingKey playerHappyKey = base.player.ikAnimator.ShowEmotion(Emotion.Happy);
		honkCooldownTimer = this.RegisterTimer(1.5f, delegate
		{
			playerHappyKey.ReleaseResource();
			honkCooldownTimer = null;
			base.player.ikAnimator.lookAt = null;
		});
		for (int num2 = 0; num2 < num; num2++)
		{
			NPCIKAnimator nPCIKAnimator = nearbyNPCs[num2];
			ICanFace canFace = nPCIKAnimator.GetComponentInParent<ICanFace>();
			if (canFace != null)
			{
				canFace.TurnToFace(base.transform);
			}
			ICanLook canLook = nPCIKAnimator.GetComponent<ICanLook>();
			if (canLook != null)
			{
				canLook.lookAt = base.transform;
			}
			NPCMovement componentInParent = nPCIKAnimator.GetComponentInParent<NPCMovement>();
			if ((bool)componentInParent)
			{
				componentInParent.PauseAndFace(base.player.transform, 1.5f);
			}
			StackResourceSortingKey emotionKey = nPCIKAnimator.ShowEmotion(Emotion.Happy);
			Action poseKey = nPCIKAnimator.Pose(Pose.Wave);
			Timer timer = null;
			timer = this.RegisterTimer(1.25f, delegate
			{
				if (canFace != null)
				{
					canFace.FaceDefault();
				}
				if (canLook != null)
				{
					canLook.lookAt = null;
				}
				emotionKey.ReleaseResource();
				poseKey();
				Timer.FlagToRecycle(timer);
			});
		}
	}

	private void UpdateSoundEffects()
	{
		if (engineAudio.isPlaying || waterFlowAudio.isPlaying || waterFlowAudio.isPlaying || scrapingSound.isPlaying)
		{
			bool isPressed = input.button1.isPressed;
			bool flag = isPressed && isGrounded;
			if (isPressed)
			{
				motorCooldown = Mathf.MoveTowards(motorCooldown, motorCooldownTime, motorPowerUpSpeed);
			}
			else if (motorCooldown > 0f)
			{
				motorCooldown -= Time.deltaTime;
			}
			float time = 1f - motorCooldown / motorCooldownTime;
			engineAudio.volume = motorCooldownVolumeMultiplier.Evaluate(time) * engineMaxVolume;
			if (engineAudio.volume < 0.01f && !base.mounted)
			{
				engineAudio.Stop();
			}
			float num = speedToPitchRange.InverseLerp(prevSpeed);
			if (engineAudio.isPlaying)
			{
				float num2 = Mathf.Lerp(isPressed ? minSpeedPitch : idlePitch, maxSpeedPitch, num) * (flag ? 1.6f : 1f);
				num2 *= motorCooldownPitchMultiplier.Evaluate(time);
				engineAudio.pitch = Mathf.MoveTowards(engineAudio.pitch, num2, pitchChangeSpeed * Time.deltaTime) * enginePitchMultiplier;
			}
			waterFlowAudio.pitch = waterFlowPitchRange.Lerp(num);
			waterFlowAudio.volume = waterFlowVolumeOverSpeed.Evaluate(prevSpeed / speedToPitchRange.max);
			waterRushAudio.volume = waterRushAudioVolume.Evaluate(prevSpeed / speedToPitchRange.max);
			if (waterFlowAudio.volume < 0.01f && !base.mounted)
			{
				waterFlowAudio.Stop();
			}
			if (waterRushAudio.volume < 0.01f && !base.mounted)
			{
				waterRushAudio.Stop();
			}
			scrapingSound.pitch = scrapingPitchRange.Lerp(scrapingSpeedRange.InverseLerp(prevSpeed));
			bool flag2 = flag;
			if (flag2 && !scrapingSound.isPlaying)
			{
				scrapingSound.Play();
			}
			else if (!flag2 && scrapingSound.isPlaying)
			{
				scrapingSound.Stop();
			}
		}
	}

	public void SetEngineCooldown(int normalizedCooldown)
	{
		motorCooldown = (float)normalizedCooldown * motorCooldownTime;
	}

	private void LateUpdate()
	{
		if (floater.waterRegion != null)
		{
			lastWaterRegion = floater.waterRegion;
		}
		if ((bool)lastWaterRegion && bubbleParticles.particleCount > 0)
		{
			int num = bubbleParticles.GetParticles(particles);
			for (int i = 0; i < num; i++)
			{
				Vector3 position = particles[i].position;
				float num2 = lastWaterRegion.GetWaterY(position) + bubbleOffset;
				if (position.y < num2)
				{
					position.y = num2;
					particles[i].position = position;
				}
			}
			bubbleParticles.SetParticles(particles, num);
		}
		if (base.mounted)
		{
			Vector3 vector = mountPosition.position;
			Quaternion quaternion = Quaternion.LookRotation(base.transform.forward.SetY(0f));
			if (playerLerpCountdown > 0f)
			{
				playerLerpCountdown -= Time.deltaTime;
				float t = 1f - playerLerpCountdown / playerLerpLength;
				vector = Vector3.Lerp(base.transform.TransformPoint(playerLerpStartLocalPosition), vector, t);
				quaternion = Quaternion.Lerp(playerLerpStartRotation, quaternion, t);
			}
			base.player.transform.position = vector;
			base.player.transform.rotation = quaternion;
		}
		if (unmountCountdown > 0f)
		{
			base.player.body.position = mountPosition.position.SetY(base.player.body.position.y);
			base.player.transform.position = mountPosition.position.SetY(base.player.body.position.y);
		}
		unmountCountdown -= Time.deltaTime;
		if (debugSpeed)
		{
			Debug.Log(prevSpeed);
		}
	}

	private void FixedUpdate()
	{
		if (!base.mounted && !boatRenderer.isVisible)
		{
			if (boatRenderer.isVisible)
			{
				prevSpeed = base.body.velocity.magnitude;
			}
			return;
		}
		if (unmountCountdown > 0f)
		{
			base.player.body.position = mountPosition.position.SetY(base.player.body.position.y);
			base.player.transform.position = mountPosition.position.SetY(base.player.body.position.y);
			base.player.body.velocity = base.body.velocity;
		}
		Vector3 vector = (base.mounted ? GetInputWorldDirection() : Vector3.zero);
		Vector3 vector2 = vector;
		float num = base.body.velocity.magnitude;
		if (num > 5f || base.mounted)
		{
			Vector3 vector3 = Vector3.Project(base.body.velocity, base.transform.forward) * (1f - Time.fixedDeltaTime * forwardDrag);
			Vector3 vector4 = Vector3.Project(base.body.velocity, base.transform.right);
			float num2 = vector4.magnitude * (Time.fixedDeltaTime * sideDrag);
			vector4 *= 1f - Time.fixedDeltaTime * sideDrag;
			vector3 += vector3.normalized * num2 * sideLossToForwardPercent;
			Vector3 vector5 = Vector3.Project(base.body.velocity, base.transform.up);
			base.body.velocity = vector3 + vector4 + vector5;
		}
		if (base.mounted)
		{
			base.player.body.position = mountPosition.position;
			base.body.AddForce(base.transform.forward * gentleForce * vector.magnitude * (tankControls ? input.leftStick.vector.y : 1f));
			float num3 = (input.button1.isPressed ? 1 : 0) + (input.button2.isPressed ? (-1) : 0);
			if (num3 != 0f && !isMidair)
			{
				throttleHeld += Time.fixedDeltaTime;
				float num4 = throttlePower.Evaluate(Mathf.InverseLerp(0f, throttlePowerTime, throttleHeld));
				base.body.AddForce(base.transform.forward * motorForce * num4 * num3);
			}
			else
			{
				throttleHeld = 0f;
			}
			if (vector2 != Vector3.zero || lastTurnAngle != 0f)
			{
				float num5 = Mathf.Lerp(turnSpeed.min, turnSpeed.max, Mathf.InverseLerp(turnSpeedVelocity.min, turnSpeedVelocity.max, num));
				float num6 = Vector3.Angle(base.transform.forward.SetY(0f), vector2) * Mathf.Sign(Vector3.Cross(base.transform.forward.SetY(0f), vector2).y);
				if (tankControls)
				{
					tankControlsTurnSpeed = Mathf.MoveTowards(tankControlsTurnSpeed, input.leftStick.vector.x * tankControlsTurnAngle, Time.deltaTime * tankControlsTurnAcceleration);
					num6 = tankControlsTurnSpeed;
				}
				float target = num5 * num6 * (isMidair ? 0.25f : 1f);
				float num7 = Mathf.MoveTowards(lastTurnAngle, target, turnRampUpAcceleration * Time.fixedDeltaTime);
				base.body.MoveRotation(base.body.rotation * Quaternion.AngleAxis(num7 * Time.fixedDeltaTime, Vector3.up));
				lastTurnAngle = num7;
				float b = (0f - num6) * motorMeshTurnFactor;
				motorRotation = Mathf.Lerp(motorRotation, b, Time.fixedDeltaTime * tiltLerpSpeed * 2f);
				boatMotor.transform.localEulerAngles = new Vector3(-90f, 0f, motorRotation);
				if (num7 > waterTurnSoundAngle && turnSoundSource == null)
				{
					turnSoundSource = waterTurnSounds.PickRandom().Play();
					turnSoundSource.volume = waterTurnSoundVolume;
				}
			}
			num = base.body.velocity.magnitude;
			if (num - prevSpeed > 0f)
			{
				float num8 = (input.button1.isPressed ? motorMaxSpeed : gentleMaxSpeed);
				if (num > num8)
				{
					float num9 = Mathf.Max(num8, prevSpeed);
					base.body.velocity = base.body.velocity / num * num9;
					num = num9;
				}
				else if (num > limitAccelerationSpeed)
				{
					float num10 = Mathf.Min(num, prevSpeed + limitedAcceleration * Time.fixedDeltaTime);
					base.body.velocity = base.body.velocity / num * num10;
					num = num10;
				}
			}
		}
		float b2 = 0f;
		float b3 = 0f;
		if (base.mounted)
		{
			float num11 = Mathf.InverseLerp(0f, maxTiltSpeed, num);
			horizontalTiltSignedPercent = Vector3.Cross(vector2, base.transform.forward).y;
			b2 = horizontalTiltSignedPercent * maxHorizontalTilt * num11;
			float num12 = num - prevSpeed;
			num11 = Mathf.InverseLerp(minForwardTiltSpeed, maxForwardTiltSpeed, num);
			b3 = Mathf.Lerp(0f, maxForwardTilt, num11 * forwardTiltSpeedFactor + num12 * forwardTiltAccelerationFactor + base.body.velocity.y * forwardTiltYSpeedFactor * num11);
		}
		horizontalTilt = Mathf.Lerp(horizontalTilt, b2, Time.fixedDeltaTime * tiltLerpSpeed);
		forwardTilt = Mathf.Lerp(forwardTilt, b3, Time.fixedDeltaTime * tiltLerpSpeed);
		boatMesh.localEulerAngles = boatMesh.localEulerAngles.SetZ(horizontalTilt).SetX(0f - forwardTilt);
		Vector3 worldUpVector = Vector3.up;
		if (base.mounted && floater.waterRegion != null)
		{
			Vector3 normalized = base.transform.forward.SetY(0f).normalized;
			float waterY = floater.waterRegion.GetWaterY(base.transform.position + normalized * waterNormalSampleLength * 0.5f);
			float waterY2 = floater.waterRegion.GetWaterY(base.transform.position - normalized * waterNormalSampleLength * 0.5f);
			float num13 = (waterY - waterY2) / waterNormalSampleLength;
			float num14 = Mathf.Sqrt(1f / (1f + num13 * num13));
			float num15 = num13 * num14;
			worldUpVector = Vector3.Cross(normalized * num14 + Vector3.up * num15, Vector3.Cross(normalized, Vector3.down));
		}
		straightener.worldUpVector = worldUpVector;
		prevSpeed = base.body.velocity.magnitude;
	}

	private void UpdateWaterParticleEffects()
	{
		if (!boatRenderer.isVisible)
		{
			if (!disableParticleEffects)
			{
				bubbleParticles.Emission(enabled: false);
				leftWaterFan.gameObject.SetActive(value: false);
				rightWaterFan.gameObject.SetActive(value: false);
				frontLeftFoam.gameObject.SetActive(value: false);
				frontRightFoam.gameObject.SetActive(value: false);
				backFlingParticles.gameObject.SetActive(value: false);
				waterTrail.trail.emitting = false;
				disableParticleEffects = true;
			}
			return;
		}
		if (disableParticleEffects)
		{
			bubbleParticles.Emission(enabled: true);
			leftWaterFan.gameObject.SetActive(value: true);
			rightWaterFan.gameObject.SetActive(value: true);
			frontLeftFoam.gameObject.SetActive(value: true);
			frontRightFoam.gameObject.SetActive(value: true);
			backFlingParticles.gameObject.SetActive(value: true);
			waterTrail.trail.emitting = true;
			disableParticleEffects = false;
		}
		float num = prevSpeed;
		float num2 = speedToEffectStrength.InverseLerp(num);
		float num3 = (floater.inWater ? ((floater.waterRegion.GetWaterY(frontWaterCheck.position) > frontWaterCheck.position.y) ? 1 : 0) : 0);
		float num4 = Mathf.Clamp01(horizontalTiltSignedPercent);
		float num5 = Mathf.Clamp01(horizontalTiltSignedPercent * -1f);
		waterTrail.transform.position = base.transform.position - base.transform.forward * waterTrailOffset;
		trailMaterial.mainTextureOffset += new Vector2((0f - Mathf.Lerp(waterTrailScrollSpeed.min, waterTrailScrollSpeed.max, num2)) * 0.01f, 0f);
		waterTrail.trail.emitting = num > 5f && floater.inWater;
		ParticleSystem.MainModule main;
		ParticleSystem.EmissionModule emission;
		if (prevSpeed > fanDisappearSpeed || fanDisappearCountdown > 0f)
		{
			if (prevSpeed > fanDisappearSpeed)
			{
				fanDisappearCountdown = 5f;
			}
			else
			{
				fanDisappearCountdown -= Time.deltaTime;
			}
			if (!leftWaterFan.gameObject.activeSelf)
			{
				leftWaterFan.gameObject.SetActive(value: true);
				rightWaterFan.gameObject.SetActive(value: true);
			}
			main = leftWaterFan.main;
			main.startSpeedMultiplier = waterFanParticleSpeed * Mathf.Clamp01(num2 + num2 * num4 * 0.5f);
			emission = leftWaterFan.emission;
			emission.enabled = num3 > 0f;
			main = rightWaterFan.main;
			main.startSpeedMultiplier = waterFanParticleSpeed * Mathf.Clamp01(num2 + num2 * num5 * 0.5f);
			emission = rightWaterFan.emission;
			emission.enabled = num3 > 0f;
		}
		else if (leftWaterFan.gameObject.activeSelf)
		{
			leftWaterFan.gameObject.SetActive(value: false);
			rightWaterFan.gameObject.SetActive(value: false);
		}
		emission = frontLeftFoam.emission;
		emission.rateOverTimeMultiplier = fluffEmissionRate * num2 * num3;
		main = frontLeftFoam.main;
		main.startSpeedMultiplier = fluffParticleSpeed * (Mathf.Clamp01(num2 * 2f) * 0.75f + num5 * 0.4f);
		emission = frontRightFoam.emission;
		emission.rateOverTimeMultiplier = fluffEmissionRate * num2 * num3;
		main = frontRightFoam.main;
		main.startSpeedMultiplier = fluffParticleSpeed * (Mathf.Clamp01(num2 * 2f) * 0.75f + num4 * 0.4f);
		num3 = (num3 = (floater.inWater ? ((floater.waterRegion.GetWaterY(backWaterCheck.position) > backWaterCheck.position.y) ? 1 : 0) : 0));
		float num6 = Mathf.Clamp01(Mathf.Clamp01(num2 - 0.4f) / 0.6f + (float)(input.button1.isPressed ? 1 : 0)) * num3;
		emission = backFlingParticles.emission;
		emission.rateOverTimeMultiplier = backFlingEmissionRate * num6;
		emission = bubbleParticles.emission;
		emission.rateOverTimeMultiplier = bubbleEmissionRate * num6;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.gameObject.tag == "Player") && collision.impulse.sqrMagnitude > impactSoundForceRange.min.Sqr())
		{
			float value = collision.impulse.magnitude + UnityEngine.Random.Range(0f - impactForceRandomization, impactForceRandomization);
			float num = Mathf.Clamp01(impactSoundForceRange.InverseLerp(value));
			int num2 = Mathf.FloorToInt((float)impactSounds.Length * num * 0.99f);
			bumpSounds.pitch = impactForcePitch.Random();
			bumpSounds.PlayOneShot(impactSounds[num2]);
			this.onBumpedBoat?.Invoke(num, collision);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.down * groundRaycastLength);
	}

	public Vector3 GetInputWorldDirection()
	{
		if (autoSteerTowards != null)
		{
			return (autoSteerTowards.position - base.transform.position).SetY(0f).normalized;
		}
		Vector2 vector = input.leftStick.vector;
		Vector3 normalized = Camera.main.transform.forward.SetY(0f).normalized;
		Vector3 vector2 = -Vector3.Cross(normalized, Vector3.up);
		return normalized * vector.y + vector2 * vector.x;
	}
}
