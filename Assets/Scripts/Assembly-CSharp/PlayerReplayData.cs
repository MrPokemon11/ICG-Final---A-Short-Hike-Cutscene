using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayData : ScriptableObject
{
	public List<PlayerReplayFrame> frames = new List<PlayerReplayFrame>();

	public bool isEmpty => frames.Count == 0;

	public PlayerReplayFrame lastFrame
	{
		get
		{
			return frames[frames.Count - 1];
		}
		set
		{
			frames[frames.Count - 1] = value;
		}
	}

	public void RecordFrame(Player player, float time)
	{
		if (frames.Count > 0 && time <= frames[frames.Count - 1].time)
		{
			Debug.LogError("You can only record frames that are further in time!");
		}
		else
		{
			frames.Add(PlayerReplayFrame.FromPlayer(player, time, frames.Count));
		}
	}

	public PlayerReplayFrame GetFrame(float time)
	{
		return GetFrame(time, 0);
	}

	public PlayerReplayFrame GetFrame(float time, int lastFrame)
	{
		for (int i = lastFrame; i < frames.Count; i++)
		{
			if (frames[i].time >= time)
			{
				int index = Math.Max(0, i - 1);
				PlayerReplayFrame a = frames[index];
				PlayerReplayFrame b = frames[i];
				return PlayerReplayFrame.Lerp(lerp: Mathf.InverseLerp(a.time, b.time, time), a: a, b: b);
			}
		}
		return frames[frames.Count - 1];
	}
}
