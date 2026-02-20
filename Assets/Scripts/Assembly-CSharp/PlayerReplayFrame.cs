using System;
using UnityEngine;

[Serializable]
public struct PlayerReplayFrame
{
	[Flags]
	public enum Event
	{
		Jump = 1,
		FlapWings = 2
	}

	public int index;

	public float time;

	public Vector3 position;

	public Quaternion rotation;

	public Quaternion animatorRotation;

	public Vector3 velocity;

	public bool isSwimming;

	public bool isGliding;

	public bool isClimbing;

	public bool isRunning;

	public bool isSliding;

	public Event eventFlags;

	public static PlayerReplayFrame FromPlayer(Player player, float time, int index)
	{
		PlayerReplayFrame result = default(PlayerReplayFrame);
		result.position = player.transform.position;
		result.rotation = player.transform.rotation;
		result.animatorRotation = player.ikAnimator.transform.localRotation;
		result.velocity = player.body.velocity;
		result.time = time;
		result.index = index;
		result.isSwimming = player.isSwimming;
		result.isGliding = player.isGliding;
		result.isClimbing = player.isClimbing;
		result.isRunning = player.isRunning;
		result.isSliding = player.isSliding;
		result.eventFlags = (Event)0;
		return result;
	}

	public static PlayerReplayFrame FromTransform(Transform transform, float time, int index)
	{
		PlayerIKAnimator componentInChildren = transform.GetComponentInChildren<PlayerIKAnimator>();
		PlayerReplayFrame result = default(PlayerReplayFrame);
		result.position = transform.position;
		result.rotation = transform.rotation;
		result.animatorRotation = (componentInChildren ? componentInChildren.transform.localRotation : Quaternion.identity);
		result.velocity = Vector3.zero;
		result.time = time;
		result.index = index;
		result.isSwimming = false;
		result.isGliding = false;
		result.isClimbing = false;
		result.isRunning = false;
		result.isSliding = false;
		result.eventFlags = (Event)0;
		return result;
	}

	public static PlayerReplayFrame Lerp(PlayerReplayFrame a, PlayerReplayFrame b, float lerp)
	{
		PlayerReplayFrame result = default(PlayerReplayFrame);
		result.time = Mathf.Lerp(a.time, b.time, lerp);
		result.index = Math.Max(a.index, b.index);
		result.position = Vector3.Lerp(a.position, b.position, lerp);
		result.rotation = Quaternion.Lerp(a.rotation, b.rotation, lerp);
		result.animatorRotation = Quaternion.Lerp(a.animatorRotation, b.animatorRotation, lerp);
		result.velocity = Vector3.Lerp(a.velocity, b.velocity, lerp);
		result.isSwimming = a.isSwimming;
		result.isGliding = a.isGliding;
		result.isClimbing = a.isClimbing;
		result.isRunning = a.isRunning;
		result.isSliding = a.isSliding;
		result.eventFlags = (Event)0;
		return result;
	}
}
