using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKAnimator : MonoBehaviour, ITalkingAnimator, IEmotionAnimator, IPoseAnimator, ICanLook
{
	public abstract class LimbIK
	{
		public enum Side
		{
			Right = 0,
			Left = 1
		}

		protected PlayerIKAnimator ikController;

		protected Side side;

		protected AudioClip placeSound;

		protected Vector3? destinationPos;

		private float maxStepCountdown;

		private Vector3 currentPos;

		private float stepCountdown;

		private Vector3 stepStartOffset;

		private float ikBlend;

		private bool _isTouching;

		protected Animator animator;

		protected Transform endJoint;

		public bool isTouching => _isTouching;

		public bool ikActive
		{
			get
			{
				if (ikController.ikActive && enabled)
				{
					return destinationPos.HasValue;
				}
				return false;
			}
		}

		public bool enabled { get; private set; }

		protected bool isStepping => stepCountdown > 0f;

		protected abstract AvatarIKGoal ikGoal { get; }

		public LimbIK(PlayerIKAnimator ikController, Side side, AudioClip placeSound)
		{
			this.ikController = ikController;
			this.side = side;
			this.placeSound = placeSound;
			animator = ikController.GetComponent<Animator>();
			endJoint = animator.GetBoneTransform(GetEndBone(side));
			enabled = true;
		}

		protected abstract Vector3? GetLatestDestinationPosition(out bool isTouching);

		protected abstract HumanBodyBones GetEndBone(Side side);

		public bool TakeStep(float stepInterval)
		{
			if (ikActive && (double)Vector3.SqrMagnitude(destinationPos.Value - currentPos) > 0.1)
			{
				stepCountdown = stepInterval;
				maxStepCountdown = stepInterval;
				stepStartOffset = currentPos - ikController.transform.position;
				return true;
			}
			return false;
		}

		public void SetEnabled(bool enabled, bool animate = true)
		{
			this.enabled = enabled;
			if (!animate)
			{
				ikBlend = (enabled ? 1 : 0);
			}
		}

		public virtual void Update()
		{
			bool flag = ikActive;
			destinationPos = GetLatestDestinationPosition(out _isTouching);
			if ((bool)ikController.player.movingPlatform)
			{
				currentPos += ikController.player.movingPlatform.velocity * Time.deltaTime;
			}
			if (!ikActive)
			{
				stepCountdown = 0f;
				destinationPos = null;
			}
			else if (!flag)
			{
				currentPos = destinationPos.Value;
			}
			if (stepCountdown > 0f)
			{
				Vector3 value = destinationPos.Value;
				Vector3 a = ikController.transform.position + stepStartOffset;
				stepCountdown -= Time.deltaTime;
				float num = 1f - stepCountdown / maxStepCountdown;
				currentPos = Vector3.Lerp(a, value, num) + GetLimbLiftOffset(num);
				if (stepCountdown <= 0f && _isTouching)
				{
					OnLimbTouchGround();
				}
			}
			ikBlend = Mathf.MoveTowards(ikBlend, ikActive ? 1 : 0, 1f / ikController.ikBlendTime * Time.deltaTime);
		}

		protected virtual void OnLimbTouchGround()
		{
			PlaySound(placeSound, 1f);
		}

		protected void PlaySound(AudioClip clip, float pitch)
		{
			ikController.audioSource.Stop();
			ikController.audioSource.pitch = pitch;
			ikController.audioSource.clip = clip;
			ikController.audioSource.Play();
		}

		protected virtual Vector3 GetLimbLiftOffset(float normalizedAnimationTime)
		{
			return ikController.footLiftCurve.Evaluate(normalizedAnimationTime) * Vector3.up * ikController.footLiftDistance;
		}

		public virtual void OnAnimatorIK()
		{
			animator.SetIKPositionWeight(ikGoal, ikBlend);
			animator.SetIKPosition(ikGoal, currentPos);
		}
	}

	public class HandIK : LimbIK
	{
		public bool isReachingLow { get; private set; }

		protected override AvatarIKGoal ikGoal
		{
			get
			{
				if (side != Side.Left)
				{
					return AvatarIKGoal.RightHand;
				}
				return AvatarIKGoal.LeftHand;
			}
		}

		public HandIK(PlayerIKAnimator ikController, Side side, AudioClip placeSound)
			: base(ikController, side, placeSound)
		{
		}

		protected override HumanBodyBones GetEndBone(Side side)
		{
			if (side != Side.Left)
			{
				return HumanBodyBones.RightHand;
			}
			return HumanBodyBones.LeftHand;
		}

		protected override Vector3? GetLatestDestinationPosition(out bool isTouching)
		{
			Vector3? result = RaycastForArm(ikController.climbingReachOffset);
			isTouching = result.HasValue;
			isReachingLow = false;
			if (!isTouching && ikController.player.isClimbing)
			{
				Vector3? vector = RaycastForArm((0f - ikController.climbingReachOffset) / 2f);
				if (vector.HasValue)
				{
					isReachingLow = true;
					result = vector;
					isTouching = vector.HasValue;
				}
			}
			return result;
		}

		protected override void OnLimbTouchGround()
		{
			if (ikController.player.isClimbing)
			{
				PlaySound(placeSound, Mathf.Min(1f + 0.05f * (float)ikController.handSoundsSinceGrounded, 2f));
				ikController.handSoundsSinceGrounded++;
			}
		}

		public Vector3? RaycastForArm(float yOffset)
		{
			Transform transform = ikController.transform;
			Vector3 center = ikController.bodyCollider.bounds.center;
			Vector3 vector = transform.right * ((side == Side.Right) ? 1 : (-1));
			if (!Physics.Raycast(new Ray(center + vector * ikController.rightSideOffset + yOffset * Vector3.up, ikController.transform.forward), out var hitInfo, ikController.armRaycastDistance, ikController.groundRaycastLayers.value))
			{
				return null;
			}
			return hitInfo.point;
		}

		protected override Vector3 GetLimbLiftOffset(float normalizedAnimationTime)
		{
			return -ikController.player.transform.forward * ikController.armReachCurve.Evaluate(normalizedAnimationTime) * ikController.armReachDistance;
		}
	}

	public class FootIK : LimbIK
	{
		private Collider touchingCollider;

		protected override AvatarIKGoal ikGoal
		{
			get
			{
				if (side != Side.Left)
				{
					return AvatarIKGoal.RightFoot;
				}
				return AvatarIKGoal.LeftFoot;
			}
		}

		public FootIK(PlayerIKAnimator ikController, Side side, AudioClip placeSound)
			: base(ikController, side, placeSound)
		{
		}

		protected override Vector3? GetLatestDestinationPosition(out bool isTouching)
		{
			Transform transform = ikController.transform;
			Bounds bounds = ikController.bodyCollider.bounds;
			float num = Mathf.Clamp01(ikController.relativePlayerSpeed / ikController.maxFootFrontOffsetSpeed);
			Vector3 center = bounds.center;
			Vector3 vector = transform.right * ((side == Side.Right) ? 1 : (-1));
			Vector3 vector2 = center + vector * ikController.rightSideOffset + ikController.player.GetDesiredMovementVector().normalized * ikController.footFrontOffset * num;
			Vector3 vector3 = Vector3.down + (ikController.player.isClimbing ? ikController.player.transform.forward : Vector3.zero);
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(new Ray(vector2, vector3), out hitInfo, ikController.footRaycastDistance, ikController.groundRaycastLayers.value);
			touchingCollider = hitInfo.collider;
			Vector3 value = (flag ? (hitInfo.point + ikController.footGroundOffset * -vector3) : (vector2 + Vector3.down * bounds.extents.y));
			isTouching = flag;
			return value;
		}

		protected override void OnLimbTouchGround()
		{
			if (!ikController.player.isGrounded)
			{
				return;
			}
			bool flag = false;
			WaterRegion waterRegion = ikController.player.waterRegion;
			if ((bool)waterRegion && destinationPos.HasValue)
			{
				float waterY = waterRegion.GetWaterY(destinationPos.Value);
				if (destinationPos.Value.y < waterY)
				{
					flag = true;
				}
			}
			if (flag && destinationPos.HasValue)
			{
				ikController.footSplashes.transform.position = destinationPos.Value;
				ikController.footSplashes.Play();
				PlaySound(ikController.waterStepSounds.PickRandom(), UnityEngine.Random.value * 0.25f + 1f);
			}
			else
			{
				base.OnLimbTouchGround();
				PlaceFootprint();
			}
		}

		public override void OnAnimatorIK()
		{
			base.OnAnimatorIK();
			if (base.ikActive)
			{
				animator.SetIKRotationWeight(ikGoal, 1f);
				float angle = (float)((side == Side.Left) ? 1 : (-1)) * ikController.footRotationCorrection;
				animator.SetIKRotation(ikGoal, Quaternion.AngleAxis(angle, ikController.transform.right) * Quaternion.LookRotation(ikController.transform.forward));
			}
			else
			{
				animator.SetIKRotationWeight(ikGoal, 0f);
			}
		}

		protected override Vector3 GetLimbLiftOffset(float normalizedAnimationTime)
		{
			if (ikController.player.isClimbing)
			{
				return -ikController.player.transform.forward * ikController.armReachCurve.Evaluate(normalizedAnimationTime) * ikController.armReachDistance;
			}
			return base.GetLimbLiftOffset(normalizedAnimationTime);
		}

		protected override HumanBodyBones GetEndBone(Side side)
		{
			if (side != Side.Left)
			{
				return HumanBodyBones.RightFoot;
			}
			return HumanBodyBones.LeftFoot;
		}

		private void PlaceFootprint()
		{
			if ((bool)ikController.footsteps)
			{
				Color? footprintColor = GetFootprintColor();
				if (footprintColor.HasValue)
				{
					ikController.footsteps.SpawnFootstep(footprintColor.Value, destinationPos.Value, ikController.transform.forward);
				}
			}
		}

		private Color? GetFootprintColor()
		{
			if (touchingCollider == null)
			{
				return null;
			}
			Terrain component = touchingCollider.GetComponent<Terrain>();
			if (component != null && destinationPos.HasValue)
			{
				Vector3 vector = component.transform.InverseTransformPoint(destinationPos.Value);
				Vector2 vector2 = new Vector2(vector.x / component.terrainData.size.x, vector.z / component.terrainData.size.z);
				Texture2D alphamapTexture = component.terrainData.GetAlphamapTexture(0);
				Vector2Int vector2Int = new Vector2Int(alphamapTexture.width, alphamapTexture.height);
				Vector2Int vector2Int2 = new Vector2Int((int)(vector2.x * (float)vector2Int.x), (int)(vector2.y * (float)vector2Int.y));
				if (vector2Int2.x >= 0 && vector2Int2.x < vector2Int.x && vector2Int2.y >= 0 && vector2Int2.y < vector2Int.y)
				{
					Color pixel = alphamapTexture.GetPixel(vector2Int2.x, vector2Int2.y);
					Color footprintColorFromChannels = GetFootprintColorFromChannels(pixel);
					if (footprintColorFromChannels.a > 0.01f)
					{
						return footprintColorFromChannels;
					}
				}
			}
			return null;
		}

		private Color GetFootprintColorFromChannels(Vector4 splatmapChannels)
		{
			return ikController.footprintChannelColors[GetIndexFromSplatmapChannels(splatmapChannels)];
		}

		private int GetIndexFromSplatmapChannels(Vector4 map)
		{
			if (map.x > 0.5f)
			{
				return 0;
			}
			if (map.y > 0.5f)
			{
				return 1;
			}
			if (map.z > 0.5f)
			{
				return 2;
			}
			return 3;
		}
	}

	public const float FISHING_SLEEP_TIME = 38f;

	public const float FISHING_SIT_TIME = 5f;

	private static int GROUNDED_ID = Animator.StringToHash("Grounded");

	private static int FALL_SPEED_ID = Animator.StringToHash("FallSpeed");

	private static int SPEED_ID = Animator.StringToHash("Speed");

	private static int GLIDING_ID = Animator.StringToHash("Gliding");

	private static int SWIMMING_ID = Animator.StringToHash("Swimming");

	private static int CLIMBING_ID = Animator.StringToHash("Climbing");

	private static int TILTED_ID = Animator.StringToHash("Tilted");

	private static int MOVEMENT_FORCE_ID = Animator.StringToHash("MovementForce");

	private static int ARM_SPEED_ID = Animator.StringToHash("ArmSpeed");

	public bool ikActive;

	[Header("Links")]
	public Animator stretchAnimator;

	public Animator headAnimator;

	[Header("Effect Links")]
	public ParticleSystem footSplashes;

	public FootstepManager footsteps;

	[Header("Limb IK Settings")]
	public float ikBlendTime = 0.1f;

	public float stepInterval = 0.125f;

	public LayerMask groundRaycastLayers;

	public float rightSideOffset = 0.6f;

	public float climbStepInterval = 0.125f;

	[Header("Hand IK Settings")]
	public float climbingReachOffset = 1.2f;

	public AnimationCurve armReachCurve;

	public float armReachDistance = 0.5f;

	public float armRaycastDistance = 3f;

	[Header("Foot IK Settings")]
	public float footRaycastDistance = 2f;

	public float footFrontOffset = 1.2f;

	public float maxFootFrontOffsetSpeed = 15f;

	public float footGroundOffset = 0.15f;

	public AnimationCurve footLiftCurve;

	public float footLiftDistance = 0.5f;

	public Color[] footprintChannelColors = new Color[4];

	public float footRotationCorrection;

	[Header("Head IK Settings")]
	public float headRotateSpeed = 8f;

	public AnimationCurve headBobCurve;

	public float headBobHeight = 0.15f;

	public float headLookDownSpeed = 1f;

	public float headUpFactorLerpSpeed = 10f;

	[Header("Sounds")]
	public AudioSource audioSource;

	public AudioClip footstep1;

	public AudioClip footstep2;

	public AudioClip handstep1;

	public AudioClip handstep2;

	public AudioClip[] waterStepSounds;

	public AudioClip[] splashes;

	public float splashVolume = 0.3f;

	protected Animator animator;

	protected IPlayerAnimatable player;

	protected Collider bodyCollider;

	private List<LimbIK> limbs = new List<LimbIK>();

	private LimbIK leftFoot;

	private LimbIK rightFoot;

	private HandIK leftHand;

	private HandIK rightHand;

	private float ikEnabledResetCountdown;

	private LimbIK.Side stepSide;

	private bool isArmRunningAnimationPlaying;

	private float? previousArmAnimationNormalizedTime;

	private Vector3 lookDirection;

	private Vector3 localStartPos;

	private bool headIKEnabled = true;

	private float maxHeadBobCountdown;

	private float headBobCountdown;

	private float headIKBlend;

	private int handSoundsSinceGrounded;

	private float softHeadLookDown;

	private float softHeadUpFactor;

	private SortedList<StackResourceSortingKey, Emotion> emotionStack;

	private IList<Emotion> emotionList;

	private bool isPosing;

	private Dictionary<int, Timer> animatorActionCooldown;

	private bool isTalking;

	private Timer stepTimer;

	private Action cachedOnStepTimerDelegate;

	private float relativePlayerSpeed;

	public Transform lookAt { get; set; }

	public bool isTouchingWall
	{
		get
		{
			if (!leftHand.isTouching)
			{
				return rightHand.isTouching;
			}
			return true;
		}
	}

	bool ITalkingAnimator.isTalking => isTalking;

	private bool isReachingLow
	{
		get
		{
			if (!leftHand.isReachingLow)
			{
				return rightHand.isReachingLow;
			}
			return true;
		}
	}

	private void Awake()
	{
		player = GetComponentInParent<IPlayerAnimatable>();
		animator = GetComponent<Animator>();
		cachedOnStepTimerDelegate = OnStepTimer;
		bodyCollider = player.collider;
		leftFoot = new FootIK(this, LimbIK.Side.Left, footstep1);
		leftHand = new HandIK(this, LimbIK.Side.Left, handstep1);
		rightFoot = new FootIK(this, LimbIK.Side.Right, footstep2);
		rightHand = new HandIK(this, LimbIK.Side.Right, handstep2);
		limbs.Add(leftHand);
		limbs.Add(leftFoot);
		limbs.Add(rightHand);
		limbs.Add(rightFoot);
		lookDirection = base.transform.forward;
		localStartPos = base.transform.localPosition;
		animatorActionCooldown = new Dictionary<int, Timer>();
		emotionStack = new SortedList<StackResourceSortingKey, Emotion>();
		emotionList = emotionStack.Values;
	}

	private void OnEnable()
	{
		Timer.FlagToRecycle(stepTimer);
		stepTimer = this.RegisterTimer(stepInterval, cachedOnStepTimerDelegate);
	}

	private void OnDisable()
	{
		Timer.Cancel(stepTimer);
	}

	private void Update()
	{
		relativePlayerSpeed = player.relativeVelocity.SetY(0f).magnitude;
		animator.SetBool(GROUNDED_ID, player.isGrounded);
		animator.SetFloat(FALL_SPEED_ID, player.relativeVelocity.y);
		animator.SetFloat(SPEED_ID, relativePlayerSpeed);
		animator.SetBool(GLIDING_ID, player.isGliding);
		animator.SetBool(SWIMMING_ID, player.isSwimming);
		animator.SetBool(CLIMBING_ID, player.isClimbing);
		animator.SetBool(TILTED_ID, isReachingLow);
		animator.SetBool(MOVEMENT_FORCE_ID, player.GetDesiredMovementVector() != Vector3.zero);
		animator.SetFloat(ARM_SPEED_ID, player.isRunning ? 1f : 0.7f);
		stretchAnimator.SetBool(GLIDING_ID, player.isGliding);
		ikActive = !player.isGliding && !player.isSwimming;
		SyncStepsWithArmAnimation();
		foreach (LimbIK limb in limbs)
		{
			limb.Update();
		}
		if (headBobCountdown > 0f)
		{
			headBobCountdown -= Time.deltaTime;
			float time = headBobCountdown / maxHeadBobCountdown;
			float num = headBobHeight * Mathf.Min(relativePlayerSpeed, 1f);
			base.transform.localPosition = localStartPos + Vector3.up * headBobCurve.Evaluate(time) * num;
		}
		if (ikEnabledResetCountdown > 0f && !isPosing)
		{
			ikEnabledResetCountdown -= Time.deltaTime;
			if (ikEnabledResetCountdown <= 0f)
			{
				leftHand.SetEnabled(enabled: true);
				rightHand.SetEnabled(enabled: true);
				leftFoot.SetEnabled(enabled: true);
				rightFoot.SetEnabled(enabled: true);
				headIKEnabled = true;
			}
		}
		lookDirection = Vector3.RotateTowards(lookDirection, GetLookDirection(), headRotateSpeed * Time.deltaTime, 1f);
		headIKBlend = Mathf.MoveTowards(headIKBlend, (ikActive && headIKEnabled) ? 1 : 0, 1f / ikBlendTime * Time.deltaTime);
		if (player.isGrounded)
		{
			handSoundsSinceGrounded = 0;
		}
	}

	private void OnAnimatorIK()
	{
		if (!animator)
		{
			return;
		}
		foreach (LimbIK limb in limbs)
		{
			limb.OnAnimatorIK();
		}
		animator.SetLookAtWeight(headIKBlend);
		animator.SetLookAtPosition(base.transform.position + lookDirection * 100f);
	}

	private Vector3 GetLookDirection()
	{
		if ((bool)lookAt)
		{
			return lookAt.transform.position - base.transform.position;
		}
		if (player.isMounted)
		{
			return player.transform.forward;
		}
		if (player.GetInputLookDirection() != Vector2.zero)
		{
			Vector3 rhs = player.GetWorldLookDirection();
			if (rhs.sqrMagnitude > 1f)
			{
				rhs = rhs.normalized;
			}
			float num = Vector3.Dot(player.transform.forward, rhs);
			float num2 = Vector3.Dot(player.transform.right, rhs);
			return player.transform.TransformDirection(Quaternion.Euler(num * 50f, num2 * 90f, 0f) * Vector3.forward);
		}
		if (player.nearbyInteractable != null)
		{
			return player.nearbyInteractable.position - base.transform.position;
		}
		Vector3 vector = player.GetDesiredMovementVector().normalized;
		if (vector == Vector3.zero)
		{
			vector = player.transform.forward;
		}
		float b = 0f;
		if (player.relativeVelocity.sqrMagnitude > 5f.Sqr())
		{
			b = Mathf.Max(player.isGrounded ? (-0.2f) : (-1f), player.relativeVelocity.normalized.y);
		}
		if (leftHand.ikActive || rightHand.ikActive)
		{
			b = 1f;
		}
		softHeadLookDown = Mathf.MoveTowards(softHeadLookDown, isReachingLow ? (-0.5f) : 0f, Time.deltaTime * headLookDownSpeed);
		if (softHeadLookDown != 0f)
		{
			b = softHeadLookDown;
		}
		softHeadUpFactor = Mathf.Lerp(softHeadUpFactor, b, headUpFactorLerpSpeed * Time.deltaTime);
		return vector + softHeadUpFactor * Vector3.up;
	}

	public void FlapWings()
	{
		stretchAnimator.SetTrigger("Flap");
		animator.SetTrigger("Flap");
		ikEnabledResetCountdown = 0.35f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
	}

	public void SwipeArms()
	{
		animator.SetTrigger("Swipe");
		ikEnabledResetCountdown = 0.4f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void SetFishingTime(float idleFishTime, bool allowSleeping)
	{
		int value = 0;
		if (idleFishTime > 38f && allowSleeping)
		{
			value = 3;
			leftHand.SetEnabled(enabled: false);
			rightHand.SetEnabled(enabled: false);
			leftFoot.SetEnabled(enabled: false);
			rightFoot.SetEnabled(enabled: false);
			headIKEnabled = false;
			isPosing = true;
		}
		else if (idleFishTime > 15f)
		{
			value = 2;
			leftHand.SetEnabled(enabled: false);
			rightHand.SetEnabled(enabled: false);
			leftFoot.SetEnabled(enabled: false);
			rightFoot.SetEnabled(enabled: false);
			headIKEnabled = false;
			isPosing = true;
		}
		else if (idleFishTime > 5f)
		{
			value = 1;
			leftHand.SetEnabled(enabled: false);
			rightHand.SetEnabled(enabled: false);
			leftFoot.SetEnabled(enabled: false);
			rightFoot.SetEnabled(enabled: false);
			headIKEnabled = true;
			isPosing = true;
		}
		else if (isPosing)
		{
			value = 0;
			leftHand.SetEnabled(enabled: true);
			rightHand.SetEnabled(enabled: true);
			leftFoot.SetEnabled(enabled: true);
			rightFoot.SetEnabled(enabled: true);
			headIKEnabled = true;
			isPosing = false;
		}
		animator.SetInteger("FishingIdleStage", value);
	}

	public void Dig()
	{
		animator.SetTrigger("Dig");
		ikEnabledResetCountdown = 1.1f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		leftFoot.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void PullRecoil()
	{
		animator.Play("PullRecoil", 0);
		animator.Play("PullRecoil", 1);
		ikEnabledResetCountdown = 1.1f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		leftFoot.SetEnabled(enabled: false, animate: false);
		rightFoot.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void PullRecoilAndTrip()
	{
		animator.Play("PullTrip", 0);
		animator.Play("PullTrip", 1);
		ikEnabledResetCountdown = 3f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		leftFoot.SetEnabled(enabled: false, animate: false);
		rightFoot.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void PickUp()
	{
		animator.SetTrigger("PickUp");
		ikEnabledResetCountdown = 0.3f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void Scoop()
	{
		animator.SetTrigger("Scoop");
		ikEnabledResetCountdown = 1f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void Pour()
	{
		animator.SetTrigger("Pour");
		ikEnabledResetCountdown = 1f;
		leftHand.SetEnabled(enabled: false, animate: false);
		rightHand.SetEnabled(enabled: false, animate: false);
		headIKEnabled = false;
	}

	public void SetPulling(bool isPulling)
	{
		animator.SetBool("Pulling", isPulling);
	}

	public void SetBoatSit(bool isMounted)
	{
		isPosing = isMounted;
		animator.SetBool("BoatSit", isMounted);
		leftHand.SetEnabled(!isMounted);
		rightHand.SetEnabled(!isMounted);
		leftFoot.SetEnabled(!isMounted);
		rightFoot.SetEnabled(!isMounted);
	}

	public Action Pose(Pose pose)
	{
		stretchAnimator.SetBool(pose.ToString(), value: true);
		animator.SetBool(pose.ToString(), value: true);
		leftHand.SetEnabled(enabled: false);
		rightHand.SetEnabled(enabled: false);
		leftFoot.SetEnabled(enabled: false);
		rightFoot.SetEnabled(enabled: false);
		headIKEnabled = false;
		isPosing = true;
		return delegate
		{
			stretchAnimator.SetBool(pose.ToString(), value: false);
			animator.SetBool(pose.ToString(), value: false);
			leftHand.SetEnabled(enabled: true);
			rightHand.SetEnabled(enabled: true);
			leftFoot.SetEnabled(enabled: true);
			rightFoot.SetEnabled(enabled: true);
			headIKEnabled = true;
			isPosing = false;
		};
	}

	public StackResourceSortingKey ShowEmotion(Emotion emotion, int priority = 0)
	{
		StackResourceSortingKey stackResourceSortingKey = null;
		stackResourceSortingKey = new StackResourceSortingKey(priority, delegate(StackResourceSortingKey removeKey)
		{
			if (emotionStack.Remove(removeKey))
			{
				UpdateAnimatorEmotions(animator);
				UpdateAnimatorEmotions(headAnimator);
			}
			else
			{
				Debug.LogWarning("Trying to release an emotion that's already free!");
			}
		});
		emotionStack.Add(stackResourceSortingKey, emotion);
		UpdateAnimatorEmotions(animator);
		UpdateAnimatorEmotions(headAnimator);
		return stackResourceSortingKey;
	}

	private void UpdateAnimatorEmotions(Animator animator)
	{
		Emotion emotion = (Emotion)(-1);
		if (emotionList.Count > 0)
		{
			emotion = emotionList[emotionList.Count - 1];
		}
		foreach (Emotion value in Enum.GetValues(typeof(Emotion)))
		{
			animator.SetBool(value.ToString(), value == emotion);
		}
	}

	public void SetTalking(bool isTalking)
	{
		this.isTalking = isTalking;
		headAnimator.SetBool("Talking", isTalking);
		animator.SetBool("Talking", isTalking);
	}

	public void SetNPC(bool isNPC)
	{
		animator.SetBool("NPC", isNPC);
	}

	public void EnableWeaponTrails(int enabled)
	{
		bool emitting = ((enabled != 0) ? true : false);
		if (player.heldItem != null)
		{
			TrailRenderer componentInChildren = player.heldItem.GetComponentInChildren<TrailRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.emitting = emitting;
			}
		}
	}

	public void ActivateHoldableEffect(int parameter)
	{
		if (!animatorActionCooldown.ContainsKey(parameter) || animatorActionCooldown[parameter].IsDone())
		{
			if (animatorActionCooldown.ContainsKey(parameter))
			{
				Timer.FlagToRecycle(animatorActionCooldown[parameter]);
			}
			animatorActionCooldown[parameter] = this.RegisterTimer(0.1f, delegate
			{
			});
			(player.heldItem?.GetComponent<IHoldableAction>())?.ActivateAction(parameter);
		}
	}

	public void OnBeforeSwimArmStroke()
	{
		splashes.PickRandom().Play().volume = splashVolume;
	}

	public void OnSwimArmStroke()
	{
		player.OnSwimArmStroke();
	}

	private void OnStepTimer()
	{
		float num = stepInterval;
		if (!isArmRunningAnimationPlaying)
		{
			stepSide = ((stepSide != LimbIK.Side.Left) ? LimbIK.Side.Left : LimbIK.Side.Right);
			num = Mathf.Lerp(stepInterval * 2f, stepInterval, relativePlayerSpeed / player.maximumWalkSpeed);
			if (player.isClimbing)
			{
				num = climbStepInterval;
			}
			ActivateFootStep(stepSide, num);
		}
		Timer.FlagToRecycle(stepTimer);
		stepTimer = this.RegisterTimer(num, cachedOnStepTimerDelegate);
	}

	private void ActivateFootStep(LimbIK.Side side, float stepLength)
	{
		bool flag = false;
		if (side == LimbIK.Side.Left)
		{
			flag |= leftFoot.TakeStep(stepLength);
			rightHand.TakeStep(stepLength);
		}
		else
		{
			flag |= rightFoot.TakeStep(stepLength);
			leftHand.TakeStep(stepLength);
		}
		if (flag)
		{
			headBobCountdown = stepLength;
			maxHeadBobCountdown = stepLength;
		}
	}

	private void SyncStepsWithArmAnimation()
	{
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(1);
		AnimatorStateInfo nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(1);
		isArmRunningAnimationPlaying = currentAnimatorStateInfo.IsName("ArmsRun") && nextAnimatorStateInfo.shortNameHash == 0;
		if (isArmRunningAnimationPlaying)
		{
			float num = currentAnimatorStateInfo.normalizedTime % 1f;
			if (previousArmAnimationNormalizedTime.HasValue)
			{
				if (previousArmAnimationNormalizedTime.Value > 0.5f && num < 0.5f)
				{
					ActivateFootStep(LimbIK.Side.Left, currentAnimatorStateInfo.length / 2f);
				}
				else if (previousArmAnimationNormalizedTime.Value < 0.5f && num > 0.5f)
				{
					ActivateFootStep(LimbIK.Side.Right, currentAnimatorStateInfo.length / 2f);
				}
			}
			previousArmAnimationNormalizedTime = num;
		}
		else
		{
			previousArmAnimationNormalizedTime = null;
		}
	}
}
