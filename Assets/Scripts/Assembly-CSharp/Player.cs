using System;
using System.Collections.Generic;
using Cinemachine;
using QuickUnityTools.Input;
using UnityEngine;

public class Player : PhysicsMovement, ICanFace, IFloater, IPlayerAnimatable
{
	public enum JitterFixConfiguration
	{
		Automatic = 0,
		Disabled = 1,
		Enabled = 2
	}

	public class PlayerInput
	{
		private FocusableUserInput input;

		public bool hasFocus => input.hasFocus;

		public PlayerInput()
		{
			input = new GameObject("PlayerInput").AddComponent<GameUserInput>();
		}

		public Vector2 GetMovement()
		{
			return input.leftStick.vector;
		}

		public Vector2 GetLookDirection()
		{
			return input.rightStick.vector;
		}

		public bool IsJumpHeld()
		{
			return input.GetJumpButton().isPressed;
		}

		public bool IsJumpTapped()
		{
			return input.GetJumpButton().ConsumePress();
		}

		public bool IsMenuTapped()
		{
			return input.WasOpenMenuPressed();
		}

		public bool ConsumeInteractTapped()
		{
			return input.GetInteractButton().ConsumePress();
		}

		public bool ConsumeUseItemTapped()
		{
			return input.GetUseItemButton().ConsumePress();
		}

		public bool IsInteractHeld()
		{
			return input.GetInteractButton().isPressed;
		}

		public bool IsUseItemHeld()
		{
			return input.GetUseItemButton().isPressed;
		}

		public bool IsRunHeld()
		{
			return input.GetRunButton().isPressed;
		}

		public bool IsDropTapped()
		{
			return input.leftBumper.wasPressed;
		}
	}

	public const float PICK_UP_TIME = 0.075f;

	public const string HELD_ITEM_TAG = "HeldItem";

	public const int AUTO_JITTER_FIX_FPS = 50;

	private const bool GLIDE_UNTIL_GROUNDED = false;

	private const float MAX_TIME_FROZEN = 0.5f;

	private static int CHARACTER_Y_ID = Shader.PropertyToID("_CharacterY");

	private static int GROUND_Y_ID = Shader.PropertyToID("_GroundY");

	private static bool _JITTER_FIX = false;

	private static JitterFixConfiguration _JITTER_CONFIG = JitterFixConfiguration.Automatic;

	[Header("Player Settings")]
	public CollectableItem featherCollectable;

	public float staminaRegainSpeed = 8f;

	public float lookDistance = 10f;

	public float groundedRaycastDistance = 1.4f;

	public float pullMoveSpeed = 1f;

	public float pullMoveForce = 1000f;

	public float walkToSuccessRadius = 3f;

	public float spotShadowRaycastDistance = 35f;

	public float featherStaminaOverkill = 1f;

	[Header("Render Settings")]
	public Color fullShirtColor;

	public Color emptyShirtColor;

	public float shirtColorLerpSpeed = 1f;

	public Renderer[] shirtRenderers;

	public float headOffset = 1.5f;

	public LayerMask spotShadowRaycastLayers;

	[Header("Jump Settings")]
	public float jumpSpeed = 20f;

	public float silverFeatherExtraJumpMultiplier = 0.5f;

	public float notGroundedJumpTimeAllowance = 0.15f;

	public float jumpCooldown = 0.35f;

	[Header("Run Settings")]
	public float runMultiplier = 1.5f;

	public float runStaminaDrain = 1f;

	public float runStaminaFreezeCooldownTime = 0.25f;

	public bool debugSpeed;

	[Header("Glide Settings")]
	public float glideAfterTouchingGroundTime = 0.5f;

	public float holdGlideButtonTime = 0.2f;

	public float glideTiltSpeed = 30f;

	public float maxGlideSpeed = 60f;

	public float maxGlideAngle = 60f;

	public float minGlideFallSpeed = -2.5f;

	public float glideDrag = 0.05f;

	public float maxHorizontalTilt = 15f;

	public float glideTurnSpeed = 2f;

	public float updraftMovementForce = 25f;

	public float updraftDrag = 0.5f;

	[Header("Climb Settings")]
	public float climbRaycastDistance = 2f;

	public float climbSpeed = 2f;

	public float climbTimePerFeather = 1f;

	public float dropItemFeatherStamina = 0.75f;

	public float silverFeatherExtraClimbSpeedMultiplier = 0.2f;

	public float heavyClimbTime = 1f;

	public LayerMask climbableMask;

	[Header("Swim Settings")]
	public float floatForce = 10f;

	public float floatDrag = 1f;

	public float maxFloatRiseSpeed = 3f;

	public float swimStrokeBoost = 5f;

	public float swimDrag = 1f;

	public float swimTurnSpeed = 3f;

	public float waterCurrentForce = 20f;

	[Header("Player Links")]
	public PlayerIKAnimator ikAnimator;

	public ParticleSystem featherParticles;

	public Material shirtMaterial;

	public Projector projector;

	public Transform handTransform;

	public Transform hatParent;

	public Transform cameraTarget;

	public CollectableItem runningShoes;

	public CollectableItem silverFeather;

	[Header("Sound")]
	public AudioSource playerSounds;

	public AudioSource airGlideSource;

	public AudioClip featherLoseSound;

	public AudioClip coldFeatherLoseSound;

	public AudioClip swipeSound;

	public AudioClip pickUpSound;

	public AudioClip dropSound;

	public AudioClip stashSound;

	public float airGlideVolumeSpeed = 2f;

	private float glideStartCountdown = 1f;

	private float glideTiltPercent;

	private float glideSpeed;

	private float startDrag;

	private float horizontalTilt;

	private float timeSinceAirbourne;

	private bool isFacingWall;

	private int _maxFeathers;

	private int? _allowedFeathers;

	private List<IInteractable> interactables = new List<IInteractable>();

	private List<WaterRegion> regions = new List<WaterRegion>();

	private List<Updraft> updrafts = new List<Updraft>();

	private CollectableItem lastHeldItem;

	private IHeavyItem heavyHeldItem;

	private float lastJumpTime = float.NegativeInfinity;

	private Transform turnToFaceTransform;

	private Vector3? turnToFacePosition;

	private float timeFrozen;

	private Color shirtColor;

	private float airGlideMaxVolume;

	private Timer useItemCooldown;

	private Timer freezeMovementTimer;

	private bool hasRunningShoes;

	private int silverFeathers;

	private float heavyClimbTimeRemaining;

	private float defaultMoveFriction;

	private Vector3? cachedDesiredMovementDirection;

	private float walkToEndTime;

	private float? walkToOverrideSpeed;

	private float runStaminaCooldown;

	private float boundsMinYOffset;

	private float boundsExtentsY;

	private ExclaimationBubble bubble;

	private PlayerEffects effects;

	private FPSCounter fpsCounter;

	public static JitterFixConfiguration jitterFixConfiguration
	{
		get
		{
			return _JITTER_CONFIG;
		}
		set
		{
			ConfigureJitterFix(value);
		}
	}

	public static bool jitterFix
	{
		get
		{
			return _JITTER_FIX;
		}
		private set
		{
			EnableJitterFix(value);
		}
	}

	public int feathers { get; private set; }

	public float featherStamina { get; private set; }

	public bool feathersRegenerate { get; set; }

	public int maxFeathers
	{
		get
		{
			if (!allowedFeathers.HasValue)
			{
				return _maxFeathers;
			}
			return Math.Min(_maxFeathers, allowedFeathers.Value);
		}
	}

	public int? allowedFeathers
	{
		get
		{
			return _allowedFeathers;
		}
		set
		{
			_allowedFeathers = value;
			feathers = Math.Min(feathers, maxFeathers);
		}
	}

	public Transform pullingAgainst { get; set; }

	public Transform walkFacingTarget { get; set; }

	public bool disableInteraction { get; set; }

	public bool isMounted => mountedVehicle != null;

	public float waterY { get; private set; }

	public Rigidbody movingPlatform { get; set; }

	public bool feetInWater => base.transform.position.y + boundsMinYOffset < waterY;

	public bool isClimbing { get; private set; }

	public bool isGliding
	{
		get
		{
			if (glideStartCountdown <= 0f && !isFacingWall && !isSwimming)
			{
				return !pullingAgainst;
			}
			return false;
		}
	}

	public bool isSwimming
	{
		get
		{
			if (!base.isGrounded && isInWater)
			{
				return !isClimbing;
			}
			return false;
		}
	}

	public bool isSliding
	{
		get
		{
			if (!base.isGrounded && isFacingWall && !isClimbing)
			{
				return base.body.linearVelocity.y < -1f;
			}
			return false;
		}
	}

	public bool isInWater => regions.Count > 0;

	public bool isRunning
	{
		get
		{
			if (isTryingToRun)
			{
				return feathers > 0;
			}
			return false;
		}
	}

	public Holdable heldItem { get; private set; }

	public IInteractable nearbyInteractable
	{
		get
		{
			if (!base.isGrounded || interactables.Count <= 0)
			{
				return null;
			}
			return interactables[0];
		}
	}

	public WaterRegion waterRegion
	{
		get
		{
			if (regions.Count <= 0)
			{
				return null;
			}
			return regions[0];
		}
	}

	public Vector3 headTarget => base.transform.position + Vector3.up * headOffset;

	public Vector3 startPosition { get; set; }

	public PlayerInput input { get; private set; }

	public Vector3? walkTo { get; private set; }

	public bool disableMenu { get; set; }

	public Vehicle mountedVehicle { get; set; }

	float IPlayerAnimatable.maximumWalkSpeed => base.maxSpeed;

	Collider IPlayerAnimatable.collider => base.myCollider;

	GameObject IPlayerAnimatable.heldItem => heldItem?.gameObject;

	Transform IPlayerAnimatable.nearbyInteractable => nearbyInteractable?.transform;

	Vector3 IPlayerAnimatable.relativeVelocity
	{
		get
		{
			if (!(movingPlatform != null))
			{
				return base.body.linearVelocity;
			}
			return base.body.linearVelocity - movingPlatform.linearVelocity;
		}
	}

	private float glideAngle => Mathf.LerpUnclamped(0f, 60f, glideTiltPercent);

	private bool hasFeathers => feathers > 0;

	private bool isTryingToRun
	{
		get
		{
			if (hasRunningShoes && base.isGrounded && input.IsRunHeld())
			{
				return input.GetMovement() != Vector2.zero;
			}
			return false;
		}
	}

	private bool isInUpdraft => updrafts.Count > 0;

	private Updraft updraft
	{
		get
		{
			if (updrafts.Count <= 0)
			{
				return null;
			}
			return updrafts[0];
		}
	}

	private bool isHeavy
	{
		get
		{
			if (heavyHeldItem != null)
			{
				return heavyHeldItem.isHeavy;
			}
			return false;
		}
	}

	private bool justLeftGround => timeSinceAirbourne < notGroundedJumpTimeAllowance;

	private bool jumpOnCooldown => Time.time < lastJumpTime + jumpCooldown;

	private Vector3? turnToFace
	{
		get
		{
			if (!turnToFacePosition.HasValue)
			{
				if (!(turnToFaceTransform != null))
				{
					return null;
				}
				return turnToFaceTransform.transform.position;
			}
			return turnToFacePosition.Value;
		}
	}

	Transform IPlayerAnimatable.transform => base.transform;

	public static event Action<bool> onJitterFixChanged;

	public event Action<Holdable> onHeldItemChanged;

	public event Action onAttemptHeavyAction;

	public event Action<Holdable> onHoldableUsed;

	public event Action onWingsFlapped;

	public event Action onGroundJumped;

	Vector2 IPlayerAnimatable.GetInputLookDirection()
	{
		return input.GetLookDirection();
	}

	protected override void Awake()
	{
		base.Awake();
		boundsMinYOffset = base.myCollider.bounds.min.y - base.transform.position.y;
		boundsExtentsY = base.myCollider.bounds.extents.y;
	}

	private void Start()
	{
		startPosition = base.transform.position;
		startDrag = base.body.linearDamping;
		input = new PlayerInput();
		effects = GetComponentInChildren<PlayerEffects>();
		fpsCounter = Singleton<ServiceLocator>.instance.Locate<FPSCounter>();
		bubble = Singleton<ServiceLocator>.instance.Locate<UI>().AddExclaimationBubble();
		bubble.target = base.transform;
		feathersRegenerate = true;
		shirtColor = fullShirtColor;
		defaultMoveFriction = movingFriction;
		Material material = new Material(projector.material);
		material.name += " (Instance)";
		projector.material = material;
		Singleton<GlobalData>.instance.gameData.WatchCollected(runningShoes, OnRunningShoesUpdated);
		OnRunningShoesUpdated(Singleton<GlobalData>.instance.gameData.GetCollected(runningShoes));
		Singleton<GlobalData>.instance.gameData.WatchCollected(featherCollectable, OnGoldenFeathersUpdated);
		OnGoldenFeathersUpdated(Singleton<GlobalData>.instance.gameData.GetCollected(featherCollectable));
		Singleton<GlobalData>.instance.gameData.tags.WatchString("WornHat", OnHatUpdated);
		OnHatUpdated(Singleton<GlobalData>.instance.gameData.tags.GetString("WornHat"));
		Singleton<GlobalData>.instance.gameData.WatchCollected(silverFeather, OnSilverFeathersUpdated);
		OnSilverFeathersUpdated(Singleton<GlobalData>.instance.gameData.GetCollected(silverFeather));
		string text = Singleton<GlobalData>.instance.gameData.tags.GetString("HeldItem");
		if (text != null)
		{
			GameObject gameObject = CollectableItem.Load(text).worldPrefab.Clone();
			PickUp(gameObject.GetComponent<Holdable>(), animate: false);
		}
		EnableJitterFix(jitterFix);
	}

	public void RegisterInteractable(IInteractable interactable)
	{
		for (int i = 0; i < interactables.Count; i++)
		{
			if (interactable.priority > interactables[i].priority)
			{
				interactables.Insert(i, interactable);
				return;
			}
		}
		interactables.Add(interactable);
	}

	public void UnregisterInteractable(IInteractable interactable)
	{
		interactables.Remove(interactable);
	}

	public void RegisterWaterRegion(WaterRegion waterRegion)
	{
		regions.Add(waterRegion);
	}

	public void UnregisterWaterRegion(WaterRegion waterRegion)
	{
		regions.Remove(waterRegion);
	}

	public void RegisterUpdraft(Updraft updraft)
	{
		updrafts.Add(updraft);
	}

	public void UnregisterUpdraft(Updraft updraft)
	{
		updrafts.Remove(updraft);
	}

	public void ShowTrail(float time)
	{
		effects.ShowBodyTrail(time);
	}

	public void PickUp(Holdable holdable, bool animate = true)
	{
		if (heldItem != null)
		{
			DropOrStashHeldItem();
		}
		heldItem = holdable;
		heavyHeldItem = holdable.GetComponent<IHeavyItem>();
		Singleton<GlobalData>.instance.gameData.tags.SetString("HeldItem", heldItem.associatedItem.name);
		this.onHeldItemChanged?.Invoke(heldItem);
		if (animate)
		{
			ikAnimator.PickUp();
		}
		float duration = (animate ? 0.075f : 0f);
		this.RegisterTimer(duration, delegate
		{
			if (heldItem == holdable)
			{
				holdable.ParentToPlayer(this);
				if (animate)
				{
					pickUpSound.Play();
				}
			}
		});
	}

	public void DropItem(bool playSound = true)
	{
		if (!(heldItem == null))
		{
			heldItem.ReleaseFromPlayer();
			ikAnimator.EnableWeaponTrails(0);
			if (playSound)
			{
				dropSound.Play();
			}
			if (isClimbing)
			{
				heldItem.transform.position = base.transform.position + boundsCenterOffset;
				heldItem.transform.forward = base.transform.right;
			}
			heavyHeldItem = null;
			heldItem = null;
			Singleton<GlobalData>.instance.gameData.tags.SetString("HeldItem", null);
			this.onHeldItemChanged?.Invoke(heldItem);
		}
	}

	public void StashHeldItem()
	{
		Asserts.NotNull(heldItem);
		lastHeldItem = heldItem.associatedItem;
		Holdable holdable = heldItem;
		DropItem(playSound: false);
		stashSound.Play();
		Singleton<GlobalData>.instance.gameData.AddCollected(holdable.associatedItem, 1, equipAction: true);
		UnityEngine.Object.Destroy(holdable.gameObject);
	}

	public void TurnToFace(Transform target)
	{
		turnToFaceTransform = target;
	}

	public void TurnToFace(Vector3 position)
	{
		turnToFacePosition = position;
	}

	public void FaceDefault()
	{
	}

	public void WalkTo(Vector3? walkTo, float? speed = null, float estimatedTime = 3f)
	{
		this.walkTo = walkTo;
		walkToOverrideSpeed = speed;
		walkToEndTime = Time.time + estimatedTime;
	}

	public void RestoreFeathers()
	{
		feathers = maxFeathers;
		featherStamina = 1f;
	}

	public void RegainFeatherStamina(float amount)
	{
		featherStamina += amount;
		if (featherStamina > 1f)
		{
			if (feathers < maxFeathers)
			{
				feathers = Math.Min(maxFeathers, feathers + 1);
				featherStamina -= 1f;
			}
			else
			{
				featherStamina = 1f;
			}
		}
	}

	private void OnHatUpdated(string hatName)
	{
		hatParent.DestroyChildren();
		if (!string.IsNullOrEmpty(hatName))
		{
			CollectableItem collectableItem = CollectableItem.Load(hatName);
			if ((bool)collectableItem)
			{
				collectableItem.worldPrefab.Clone().transform.SetParent(hatParent, worldPositionStays: false);
			}
		}
	}

	private void OnGoldenFeathersUpdated(int number)
	{
		_maxFeathers = number;
		if (feathers > maxFeathers)
		{
			feathers = maxFeathers;
		}
	}

	private void OnSilverFeathersUpdated(int number)
	{
		silverFeathers = number;
	}

	private void OnRunningShoesUpdated(int number)
	{
		hasRunningShoes = number > 0;
	}

	public void OnSwimArmStroke()
	{
		if (input.hasFocus)
		{
			base.body.AddForce(base.transform.forward * swimStrokeBoost, ForceMode.Impulse);
		}
	}

	protected override void Update()
	{
		ResetCache();
		base.Update();
		if (!base.isGrounded)
		{
			timeSinceAirbourne += Time.deltaTime;
		}
		else
		{
			timeSinceAirbourne = 0f;
		}
		bool flag = interactables.Count > 0 && !disableInteraction && (base.isGrounded || isSwimming);
		bubble.Show(flag && input.hasFocus);
		if (flag && input.ConsumeInteractTapped())
		{
			interactables[0].Interact();
		}
		else if (input.ConsumeUseItemTapped())
		{
			if (heldItem != null && (base.isGrounded || (!isGliding && heldItem.canUseWhileJumping)))
			{
				UseItem();
			}
			else if (heldItem == null && base.isGrounded && lastHeldItem != null && Singleton<GlobalData>.instance.gameData.GetCollected(lastHeldItem) > 0)
			{
				Holdable.EquipFromInventory(this, lastHeldItem);
			}
		}
		if (input.IsMenuTapped() && !disableMenu)
		{
			Singleton<GameServiceLocator>.instance.levelUI.OpenPauseMenu();
		}
		if ((input.IsDropTapped() || (isClimbing && featherStamina < dropItemFeatherStamina && (heavyHeldItem == null || !heavyHeldItem.isHeavy))) && !disableMenu)
		{
			DropOrStashHeldItem();
		}
		if (isRunning)
		{
			runStaminaCooldown = runStaminaFreezeCooldownTime;
		}
		else if (runStaminaCooldown > 0f)
		{
			runStaminaCooldown -= Time.deltaTime;
		}
		if (base.isGrounded || (isSwimming && base.body.linearVelocity.y < 1f && base.body.transform.position.y < waterY))
		{
			if (isTryingToRun)
			{
				DrainFeatherStamina(Time.deltaTime * runStaminaDrain);
			}
			else if (feathersRegenerate && runStaminaCooldown <= 0f)
			{
				RegainFeatherStamina(Time.deltaTime * staminaRegainSpeed);
			}
		}
		bool flag2 = (base.groundHit.HasValue || justLeftGround || isSwimming) && !jumpOnCooldown;
		if (input.IsJumpTapped() && !pullingAgainst)
		{
			if (!flag2 && feathers > 0)
			{
				if (isHeavy)
				{
					this.onAttemptHeavyAction?.Invoke();
				}
				else
				{
					float b = jumpSpeed * (1f + (float)silverFeathers * silverFeatherExtraJumpMultiplier);
					base.body.linearVelocity = base.body.linearVelocity.SetY(Mathf.Max(base.body.linearVelocity.y, b));
					effects.FlapWings(Mathf.Clamp(1f + 0.1f * (float)(maxFeathers - feathers), 0.5f, 4f));
					glideStartCountdown = holdGlideButtonTime;
					UseFeather();
					DropOrStashHeldItem();
					this.onWingsFlapped?.Invoke();
				}
			}
			else if (flag2)
			{
				effects.Jump();
				float num = jumpSpeed;
				if (isSwimming)
				{
					num *= 1.5f;
				}
				base.body.linearVelocity = base.body.linearVelocity.SetY(Mathf.Max(base.body.linearVelocity.y, num));
				lastJumpTime = Time.time;
				this.onGroundJumped?.Invoke();
			}
		}
		if (!base.isGrounded && input.IsJumpHeld() && !isClimbing && timeSinceAirbourne > glideAfterTouchingGroundTime)
		{
			glideStartCountdown -= Time.deltaTime;
		}
		else
		{
			_ = base.isGrounded;
			glideStartCountdown = holdGlideButtonTime * 0.5f;
		}
		float b2 = ((!isGliding) ? 0f : Vector3.Cross(GetDesiredRotateDirection(), base.transform.forward).y);
		horizontalTilt = Mathf.Lerp(horizontalTilt, b2, Time.deltaTime * glideTiltSpeed);
		float a = 0f;
		float magnitude = input.GetMovement().magnitude;
		magnitude = Mathf.Clamp01(magnitude * 1.17f);
		float b3 = ((!isGliding) ? 0f : Mathf.Lerp(a, 1f, 1f - magnitude));
		if (isGliding && isInUpdraft)
		{
			b3 = -1f;
		}
		glideTiltPercent = Mathf.Lerp(glideTiltPercent, b3, Time.deltaTime * glideTiltSpeed);
		float b4 = maxGlideSpeed * Mathf.Cos(glideAngle * (MathF.PI / 180f));
		glideSpeed = Mathf.Min(base.body.linearVelocity.magnitude, b4);
		glideSpeed = Mathf.Max(base.maxSpeed, glideSpeed);
		Vector3 eulerAngles = ikAnimator.transform.eulerAngles;
		eulerAngles.x = glideAngle;
		eulerAngles.z = horizontalTilt * maxHorizontalTilt;
		ikAnimator.transform.eulerAngles = eulerAngles;
		airGlideSource.pitch = 0.5f + glideTiltPercent * 0.25f + horizontalTilt * 0.1f;
		isClimbing = false;
		isFacingWall = false;
		if (Physics.Raycast(new Ray(base.transform.position + boundsCenterOffset + Vector3.down * boundsExtentsY * 0.85f, base.transform.forward), out var _, climbRaycastDistance, climbableMask.value))
		{
			isFacingWall = true;
			if (input.IsJumpHeld() && hasFeathers && heavyClimbTimeRemaining > 0f)
			{
				isClimbing = true;
			}
		}
		if (isClimbing)
		{
			DrainFeatherStamina(Time.deltaTime / climbTimePerFeather);
			if (isHeavy && heavyClimbTimeRemaining > 0f)
			{
				heavyClimbTimeRemaining -= Time.deltaTime;
				if (heavyClimbTimeRemaining <= 0f)
				{
					this.onAttemptHeavyAction?.Invoke();
				}
			}
		}
		else if (base.isGrounded)
		{
			heavyClimbTimeRemaining = heavyClimbTime;
		}
		base.body.linearDamping = startDrag;
		if (isGliding)
		{
			base.body.linearDamping = glideDrag;
		}
		else if (isSwimming && !movingPlatform)
		{
			base.body.linearDamping = swimDrag;
		}
		Color color = ((feathers == 0) ? emptyShirtColor : fullShirtColor);
		if (shirtColor != color)
		{
			shirtColor = Vector4.MoveTowards(shirtColor, color, shirtColorLerpSpeed * Time.deltaTime);
			Renderer[] array = shirtRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material.color = shirtColor;
			}
		}
		waterY = ((waterRegion == null) ? (-1000f) : waterRegion.GetWaterY(base.transform.position));
		RaycastHit hitInfo2;
		bool flag3 = Physics.Raycast(new Ray(base.transform.position, Vector3.down), out hitInfo2, spotShadowRaycastDistance, spotShadowRaycastLayers.value);
		projector.gameObject.SetActive(flag3);
		if (flag3)
		{
			projector.transform.position = projector.transform.position.SetY(hitInfo2.point.y + 0.05f);
			projector.material.SetFloat(CHARACTER_Y_ID, base.transform.position.y);
			projector.material.SetFloat(GROUND_Y_ID, hitInfo2.point.y);
		}
		if (turnToFace.HasValue && Vector3.Angle(base.transform.forward.SetY(0f), (turnToFace.Value - base.transform.position).SetY(0f)) < 5f)
		{
			turnToFaceTransform = null;
			turnToFacePosition = null;
		}
		if (!base.isGrounded && !isInWater && feathers == 0 && base.body.linearVelocity.sqrMagnitude < 0.5f)
		{
			timeFrozen += Time.deltaTime;
			if (timeFrozen > 0.5f)
			{
				base.body.linearVelocity = base.body.linearVelocity.SetY(jumpSpeed * 1.25f);
				feathers = maxFeathers;
				featherStamina = 1f;
			}
		}
		else
		{
			timeFrozen = 0f;
		}
		if (jitterFixConfiguration == JitterFixConfiguration.Automatic && (bool)fpsCounter)
		{
			bool flag4 = (float)fpsCounter.fps <= 50f * Time.timeScale;
			if (jitterFix != flag4)
			{
				jitterFix = flag4;
			}
		}
		if (debugSpeed)
		{
			Debug.Log(base.body.linearVelocity.SetY(0f).magnitude);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (isGliding)
		{
			float b = (0f - maxGlideSpeed) * Mathf.Sin(glideAngle * (MathF.PI / 180f));
			b = Mathf.Min(minGlideFallSpeed, b);
			base.body.linearVelocity = base.body.linearVelocity.SetY(Mathf.Max(base.body.linearVelocity.y, b));
			if (isInUpdraft)
			{
				Updraft updraft = this.updraft;
				base.body.AddForce(Vector3.up * updraft.upwardForce);
				Vector3 vector = (updraft.transform.position - base.transform.position).SetY(0f);
				base.body.AddForce(vector * updraft.inwardForce);
				base.body.linearVelocity = (base.body.linearVelocity * (1f - Time.fixedDeltaTime * updraft.drag)).SetY(base.body.linearVelocity.y);
			}
		}
		if (isClimbing)
		{
			float b2 = climbSpeed * (1f + (float)silverFeathers * silverFeatherExtraClimbSpeedMultiplier);
			base.body.linearVelocity = base.body.linearVelocity.SetY(Mathf.Max(base.body.linearVelocity.y, b2));
		}
		if (isInWater)
		{
			Vector3 velocity = base.body.linearVelocity;
			if (velocity.y < maxFloatRiseSpeed)
			{
				float maxDelta = Mathf.Max(0f, waterY - base.transform.position.y) * floatForce * Time.fixedDeltaTime / base.body.mass;
				velocity.y = Mathf.MoveTowards(velocity.y, maxFloatRiseSpeed, maxDelta);
			}
			velocity.y *= 1f - Time.fixedDeltaTime * floatDrag;
			base.body.linearVelocity = velocity;
			if ((bool)waterRegion.current)
			{
				base.body.AddForce(waterRegion.current.GetCurrent(base.transform.position) * waterCurrentForce);
			}
		}
		if (walkTo.HasValue && (Time.time > walkToEndTime || (base.transform.position - walkTo.Value).SetY(0f).sqrMagnitude < walkToSuccessRadius.Sqr()))
		{
			walkTo = null;
		}
	}

	private void LateUpdate()
	{
		ResetCache();
	}

	private void ResetCache()
	{
		cachedDesiredMovementDirection = null;
	}

	private static void ConfigureJitterFix(JitterFixConfiguration value)
	{
		_JITTER_CONFIG = value;
		switch (value)
		{
		case JitterFixConfiguration.Enabled:
			jitterFix = true;
			break;
		case JitterFixConfiguration.Disabled:
			jitterFix = false;
			break;
		}
	}

	private static void EnableJitterFix(bool enabled)
	{
		_JITTER_FIX = enabled;
		LevelController levelController = Singleton<ServiceLocator>.instance.Locate<LevelController>(allowFail: true);
		if (levelController != null)
		{
			levelController.player.body.interpolation = ((!enabled) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
			Camera.main.GetComponent<CinemachineBrain>().m_UpdateMethod = ((!enabled) ? CinemachineBrain.UpdateMethod.SmartUpdate : CinemachineBrain.UpdateMethod.FixedUpdate);
		}
		Player.onJitterFixChanged?.Invoke(enabled);
	}

	public void DropOrStashHeldItem()
	{
		if (!(heldItem == null))
		{
			if (!heldItem.cannotStash)
			{
				StashHeldItem();
			}
			else
			{
				DropItem();
			}
		}
	}

	private void UseFeather()
	{
		feathers--;
		featherParticles.Emit(1);
		if (feathers > 0)
		{
			featherStamina = 1f;
		}
	}

	private void DrainFeatherStamina(float amount)
	{
		featherStamina -= amount;
		if (featherStamina <= 0f)
		{
			if (feathers > 0)
			{
				UseFeather();
				featherStamina = 1f;
				PlayFeatherLoseSound();
			}
			if (featherStamina < 0f - featherStaminaOverkill)
			{
				featherStamina = 0f - featherStaminaOverkill;
			}
		}
	}

	public void UseItem()
	{
		if (heldItem == null)
		{
			Debug.LogWarning("Trying to use held item when none is held!");
		}
		else
		{
			if (useItemCooldown != null && !useItemCooldown.IsDone())
			{
				return;
			}
			switch (heldItem.useAction)
			{
			case Holdable.UseAction.Swing:
				swipeSound.Play();
				ikAnimator.SwipeArms();
				useItemCooldown = this.RegisterTimer(0.3f, delegate
				{
					useItemCooldown = null;
				});
				break;
			case Holdable.UseAction.Dig:
				if (!isFacingWall || !ikAnimator.isTouchingWall)
				{
					ikAnimator.Dig();
					useItemCooldown = this.RegisterTimer(1.1f, delegate
					{
						useItemCooldown = null;
					});
					Timer.Cancel(freezeMovementTimer);
					freezeMovementTimer = this.RegisterTimer(1f, delegate
					{
						freezeMovementTimer = null;
					});
				}
				break;
			case Holdable.UseAction.Bucket:
			{
				BucketActions component = heldItem.GetComponent<BucketActions>();
				Timer.Cancel(freezeMovementTimer);
				if (!component.filled || feetInWater)
				{
					component.scoopSound.Play();
					ikAnimator.Scoop();
					freezeMovementTimer = this.RegisterTimer(0.4f, delegate
					{
						freezeMovementTimer = null;
					});
				}
				else
				{
					ikAnimator.Pour();
					freezeMovementTimer = this.RegisterTimer(0.8f, delegate
					{
						freezeMovementTimer = null;
					});
				}
				useItemCooldown = this.RegisterTimer(1f, delegate
				{
					useItemCooldown = null;
				});
				break;
			}
			}
			this.onHoldableUsed?.Invoke(heldItem);
		}
	}

	public void FreezeMovement(float time)
	{
		freezeMovementTimer = this.RegisterTimer(time, delegate
		{
			freezeMovementTimer = null;
		});
		useItemCooldown = this.RegisterTimer(time, delegate
		{
			useItemCooldown = null;
		});
	}

	private void PlaySound(AudioClip sound, float pitch = 1f)
	{
		playerSounds.pitch = pitch;
		playerSounds.PlayOneShot(sound);
	}

	private void PlayFeatherLoseSound()
	{
		AudioSource audioSource = (feathersRegenerate ? featherLoseSound : coldFeatherLoseSound).Play();
		audioSource.pitch = 0.8f + (float)(maxFeathers - feathers) * 0.04f;
		audioSource.volume = 0.5f;
	}

	public override bool CheckIfGrounded()
	{
		if (!base.CheckIfGrounded())
		{
			return false;
		}
		return base.groundHit.Value.distance < groundedRaycastDistance;
	}

	protected override Vector3 GetDesiredVelocityInternal(Vector3 desiredDirection)
	{
		Vector3 desiredVelocityInternal = base.GetDesiredVelocityInternal(desiredDirection);
		if ((bool)movingPlatform)
		{
			desiredVelocityInternal += movingPlatform.linearVelocity.SetY(0f);
		}
		return desiredVelocityInternal;
	}

	public override Vector3 GetDesiredMovementVector()
	{
		if (cachedDesiredMovementDirection.HasValue)
		{
			return cachedDesiredMovementDirection.Value;
		}
		if (walkTo.HasValue)
		{
			return (walkTo.Value - base.transform.position).SetY(0f).normalized;
		}
		Vector3 vector = GetInputWorldDirection();
		if (isGliding && !isInUpdraft)
		{
			vector = base.transform.forward;
		}
		else if (isClimbing && vector == Vector3.zero)
		{
			vector = base.transform.forward;
		}
		cachedDesiredMovementDirection = vector;
		return vector;
	}

	protected override Vector3 GetDesiredRotateDirection()
	{
		if (turnToFace.HasValue)
		{
			return (turnToFace.Value - base.transform.position).SetY(0f).normalized;
		}
		if ((bool)pullingAgainst)
		{
			Vector3 inputWorldDirection = GetInputWorldDirection();
			if (inputWorldDirection != Vector3.zero)
			{
				return (((pullingAgainst.transform.position - base.transform.position).SetY(0f).normalized + inputWorldDirection * 0.6f) / 2f).normalized;
			}
			return Vector3.zero;
		}
		if ((bool)walkFacingTarget)
		{
			Vector3 inputWorldDirection2 = GetInputWorldDirection();
			if (inputWorldDirection2 != Vector3.zero)
			{
				return (((walkFacingTarget.transform.position - base.transform.position).SetY(0f).normalized + inputWorldDirection2) / 2f).normalized;
			}
			return Vector3.zero;
		}
		if (isGliding)
		{
			return GetInputWorldDirection();
		}
		return base.GetDesiredRotateDirection();
	}

	public Vector3 GetInputWorldDirection()
	{
		Vector2 movement = input.GetMovement();
		Vector3 normalized = Camera.main.transform.forward.SetY(0f).normalized;
		Vector3 vector = -Vector3.Cross(normalized, Vector3.up);
		return normalized * movement.y + vector * movement.x;
	}

	public Vector3 GetWorldLookDirection()
	{
		Vector2 lookDirection = input.GetLookDirection();
		Vector3 normalized = Camera.main.transform.forward.SetY(0f).normalized;
		Vector3 vector = -Vector3.Cross(normalized, Vector3.up);
		return normalized * lookDirection.y + vector * lookDirection.x;
	}

	public override float ResolveMovementForce()
	{
		if (isGliding && isInUpdraft)
		{
			return updraft.movementForce;
		}
		if ((bool)pullingAgainst)
		{
			return pullMoveForce;
		}
		return base.ResolveMovementForce();
	}

	public override float ResolveMaximumVelocity()
	{
		if (freezeMovementTimer != null && !freezeMovementTimer.IsDone())
		{
			return 0f;
		}
		if (walkTo.HasValue && walkToOverrideSpeed.HasValue)
		{
			return walkToOverrideSpeed.Value;
		}
		if ((bool)pullingAgainst)
		{
			float value = Vector3.Dot(GetInputWorldDirection(), (pullingAgainst.transform.position - base.transform.position).normalized);
			return Mathf.Lerp(pullMoveSpeed, base.maxSpeed / 2f, Mathf.InverseLerp(-1f, 1f, value));
		}
		if (isGliding)
		{
			return glideSpeed;
		}
		float num = 1f;
		if (isRunning)
		{
			num = runMultiplier;
		}
		return base.ResolveMaximumVelocity() * num;
	}

	public override float ResolveTurnSpeed()
	{
		if (isSwimming)
		{
			return swimTurnSpeed;
		}
		if (!isGliding)
		{
			return base.ResolveTurnSpeed();
		}
		return glideTurnSpeed;
	}
}
