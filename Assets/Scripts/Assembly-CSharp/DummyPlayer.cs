using System;
using UnityEngine;

public class DummyPlayer : PhysicsMovement, IPlayerAnimatable
{
	public Renderer myRenderer;

	protected RangedInteractable rangedInteractable;

	protected DialogueInteractable dialogueInteractable;

	protected Player player;

	private Transform _nearbyInteractable;

	private Action idlePoseReleaseKey;

	private Action setPoseReleaseKey;

	public virtual GameObject heldItem => null;

	public virtual Transform nearbyInteractable => _nearbyInteractable;

	public virtual WaterRegion waterRegion => null;

	public virtual bool isSwimming => false;

	public virtual bool isGliding => false;

	public virtual bool isClimbing => false;

	public virtual bool isRunning => false;

	public virtual bool isSliding => false;

	public bool isMounted => false;

	public virtual Vector3 relativeVelocity => base.body.linearVelocity;

	public virtual float waterY => -1000f;

	Collider IPlayerAnimatable.collider => base.myCollider;

	float IPlayerAnimatable.maximumWalkSpeed => base.maxSpeed;

	public Rigidbody movingPlatform => null;

	public Vehicle mountedVehicle => null;

	public PlayerIKAnimator animator { get; private set; }

	Transform IPlayerAnimatable.transform => base.transform;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponentInChildren<PlayerIKAnimator>();
		rangedInteractable = GetComponent<RangedInteractable>();
		dialogueInteractable = GetComponent<DialogueInteractable>();
		player = Singleton<GameServiceLocator>.instance.levelController.player;
	}

	private void Start()
	{
		animator.SetNPC(isNPC: true);
	}

	protected override void Update()
	{
		base.Update();
		animator.enabled = myRenderer.isVisible;
		if ((bool)rangedInteractable)
		{
			_nearbyInteractable = (rangedInteractable.isRegistered ? player.transform : null);
		}
	}

	public override Vector3 GetDesiredMovementVector()
	{
		return Vector3.zero;
	}

	public Vector2 GetInputLookDirection()
	{
		return Vector3.zero;
	}

	public Vector3 GetWorldLookDirection()
	{
		return Vector3.zero;
	}

	public void OnSwimArmStroke()
	{
	}

	public Action OverridePose(Pose pose)
	{
		if (idlePoseReleaseKey != null)
		{
			idlePoseReleaseKey();
			idlePoseReleaseKey = null;
		}
		setPoseReleaseKey?.Invoke();
		Action action = animator.Pose(pose);
		setPoseReleaseKey = delegate
		{
			setPoseReleaseKey = null;
			action();
		};
		return setPoseReleaseKey;
	}

	protected void UpdateIdlePose(bool isIdle, Pose idlePose)
	{
		if (dialogueInteractable != null)
		{
			isIdle &= !dialogueInteractable.isConversationActive;
		}
		if (setPoseReleaseKey != null)
		{
			isIdle = false;
		}
		if (isIdle && idlePoseReleaseKey == null)
		{
			idlePoseReleaseKey = animator.Pose(idlePose);
		}
		else if (!isIdle && idlePoseReleaseKey != null)
		{
			idlePoseReleaseKey();
			idlePoseReleaseKey = null;
		}
	}
}
