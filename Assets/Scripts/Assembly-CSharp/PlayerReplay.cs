using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplay : DummyPlayer, IFloater
{
	[Header("NPC Settings")]
	public Pose idlePose = Pose.StretchCalm;

	public bool faceDuringDialogue = true;

	[Header("Replay Settings")]
	public PlayerReplayData data;

	public bool looping = true;

	[Header("Water Settings")]
	public float floatForce = 80f;

	public float maxFloatRiseSpeed = 1f;

	public float floatDrag = 8f;

	private bool _isSwimming;

	private bool _isGliding;

	private bool _isClimbing;

	private bool _isRunning;

	private bool _isSliding;

	private Vector3 _velocity;

	private float _waterY;

	private float time;

	private int lastFrame;

	private PlayerReplayFrame startFrame;

	private List<WaterRegion> waterRegions = new List<WaterRegion>();

	private Quaternion originalAnimatorRotation;

	private bool defaultKinematic;

	private PlayerEffects effects;

	private DialogueInteractable dialogue;

	public override WaterRegion waterRegion
	{
		get
		{
			if (waterRegions.Count <= 0)
			{
				return null;
			}
			return waterRegions[0];
		}
	}

	public override bool isSwimming
	{
		get
		{
			if (!isPlaying)
			{
				if (waterRegions.Count > 0)
				{
					return !base.isGrounded;
				}
				return false;
			}
			return _isSwimming;
		}
	}

	public override bool isGliding => _isGliding;

	public override bool isClimbing => _isClimbing;

	public override bool isRunning => _isRunning;

	public override bool isSliding => _isSliding;

	public override float waterY => _waterY;

	public override Vector3 relativeVelocity
	{
		get
		{
			if (!isPlaying)
			{
				return base.body.velocity;
			}
			return _velocity;
		}
	}

	public bool isPlaying { get; private set; }

	public Vector3? walkTo { get; set; }

	public event Action onStop;

	protected override void Awake()
	{
		base.Awake();
		effects = GetComponentInChildren<PlayerEffects>();
		dialogue = GetComponent<DialogueInteractable>();
		originalAnimatorRotation = base.animator.transform.localRotation;
		defaultKinematic = base.body.isKinematic;
		if ((bool)dialogue)
		{
			dialogue.onConversationStart += OnConversationStart;
		}
	}

	private void OnConversationStart(IConversation obj)
	{
		if (faceDuringDialogue)
		{
			GetComponent<NPCFacer>()?.TurnToFace(Singleton<GameServiceLocator>.instance.levelController.player.transform);
		}
	}

	protected override void Update()
	{
		base.Update();
		bool flag = (bool)dialogue && dialogue.isConversationActive;
		UpdateIdlePose(!isSwimming && !flag && !isPlaying && (rangedInteractable == null || !rangedInteractable.isRegistered), idlePose);
		_waterY = ((waterRegion == null) ? (-1000f) : waterRegion.GetWaterY(base.transform.position));
		if (walkTo.HasValue && (base.transform.position - walkTo.Value).SetY(0f).sqrMagnitude < 1f)
		{
			walkTo = null;
		}
	}

	protected override void FixedUpdate()
	{
		if (!isPlaying && !myRenderer.isVisible)
		{
			return;
		}
		base.FixedUpdate();
		if (!isPlaying && waterRegions.Count > 0)
		{
			Vector3 velocity = base.body.velocity;
			if (velocity.y < maxFloatRiseSpeed)
			{
				float maxDelta = Mathf.Max(0f, waterY - base.transform.position.y) * floatForce * Time.fixedDeltaTime / base.body.mass;
				velocity.y = Mathf.MoveTowards(velocity.y, maxFloatRiseSpeed, maxDelta);
			}
			velocity.y *= 1f - Time.fixedDeltaTime * floatDrag;
			base.body.velocity = velocity;
		}
	}

	private void LateUpdate()
	{
		if (data == null || !isPlaying)
		{
			return;
		}
		PlayerReplayFrame frameData;
		if (time >= 0f)
		{
			frameData = data.GetFrame(time, lastFrame);
			if (frameData.index > lastFrame)
			{
				HandleEvents(data.frames[lastFrame].eventFlags);
				lastFrame = frameData.index;
			}
		}
		else
		{
			frameData = PlayerReplayFrame.Lerp(startFrame, data.frames[0], 1f - time / startFrame.time);
			lastFrame = 0;
		}
		SetFrameData(frameData);
		time += Time.deltaTime;
		if (time > data.lastFrame.time)
		{
			if (looping)
			{
				time = 0f;
				lastFrame = 0;
			}
			else
			{
				Stop();
			}
		}
	}

	private void SetFrameData(PlayerReplayFrame frame)
	{
		base.transform.position = frame.position;
		base.transform.rotation = frame.rotation;
		base.animator.transform.localRotation = frame.animatorRotation;
		_velocity = frame.velocity;
		_isSwimming = frame.isSwimming;
		_isGliding = frame.isGliding;
		_isClimbing = frame.isClimbing;
		_isRunning = frame.isRunning;
		_isSliding = frame.isSliding;
	}

	public void Play(float startTime = 0f)
	{
		isPlaying = true;
		time = startTime;
		base.body.isKinematic = true;
		startFrame = PlayerReplayFrame.FromTransform(base.transform, startTime, -1);
		walkTo = null;
	}

	public void Stop()
	{
		isPlaying = false;
		base.body.isKinematic = defaultKinematic;
		base.animator.transform.localRotation = originalAnimatorRotation;
		_velocity = Vector3.zero;
		_isSwimming = false;
		_isGliding = false;
		_isClimbing = false;
		_isRunning = false;
		_isSliding = false;
		this.onStop?.Invoke();
	}

	public override Vector3 GetDesiredMovementVector()
	{
		if (isPlaying)
		{
			if (!(_velocity.sqrMagnitude > 1f))
			{
				return Vector3.zero;
			}
			return _velocity;
		}
		if (walkTo.HasValue)
		{
			return (walkTo.Value - base.transform.position).normalized;
		}
		return Vector3.zero;
	}

	public override bool CheckIfGrounded()
	{
		if (!isPlaying && !myRenderer.isVisible)
		{
			return true;
		}
		return base.CheckIfGrounded();
	}

	private void HandleEvents(PlayerReplayFrame.Event eventFlags)
	{
		if (eventFlags.HasFlag(PlayerReplayFrame.Event.FlapWings))
		{
			effects.FlapWings();
		}
		if (eventFlags.HasFlag(PlayerReplayFrame.Event.Jump))
		{
			effects.Jump();
		}
	}

	public void RegisterWaterRegion(WaterRegion waterRegion)
	{
		waterRegions.Add(waterRegion);
	}

	public void UnregisterWaterRegion(WaterRegion waterRegion)
	{
		waterRegions.Remove(waterRegion);
	}
}
