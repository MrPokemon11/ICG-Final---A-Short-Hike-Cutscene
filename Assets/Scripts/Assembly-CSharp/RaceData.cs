using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RaceData
{
	public string id;

	public string requireTag;

	public int allowedFeathers;

	public Transform playerStart;

	public Transform playerStartAlt;

	public RaceEndDestination raceEnd;

	public List<PlayerReplayData> playerReplays;

	private PlayerReplayData _ghostData;

	public float bestTime
	{
		get
		{
			if (!ghostData.isEmpty)
			{
				return ghostData.lastFrame.time;
			}
			return 10000f;
		}
	}

	public bool attempted
	{
		get
		{
			return Singleton<GlobalData>.instance.gameData.tags.GetBool(id + "_Attempted");
		}
		set
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool(id + "_Attempted", value);
		}
	}

	public int victories
	{
		get
		{
			return (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat(id + "_Victories");
		}
		set
		{
			Singleton<GlobalData>.instance.gameData.tags.SetFloat(id + "_Victories", value);
		}
	}

	public PlayerReplayData ghostData
	{
		get
		{
			InitalizeGhostData();
			return _ghostData;
		}
		set
		{
			InitalizeGhostData();
			_ghostData.frames = value.frames.ToList();
			Singleton<GlobalData>.instance.gameData.playerReplayData[id] = value.frames.ToList();
		}
	}

	private void InitalizeGhostData()
	{
		if (_ghostData == null)
		{
			_ghostData = ScriptableObject.CreateInstance<PlayerReplayData>();
			if (Singleton<GlobalData>.instance.gameData.playerReplayData.ContainsKey(id))
			{
				_ghostData.frames = Singleton<GlobalData>.instance.gameData.playerReplayData[id].ToList();
			}
		}
	}

	public PlayerReplayData GetReplayData()
	{
		if (victories < playerReplays.Count)
		{
			return playerReplays[victories];
		}
		if (ghostData.frames.Count > 0)
		{
			return ghostData;
		}
		return playerReplays.LastOrDefault();
	}

	public Transform GetPlayerStartPosition()
	{
		return GetPlayerStartPosition(GetReplayData());
	}

	public Transform GetPlayerStartPosition(PlayerReplayData replayData)
	{
		float sqrMagnitude = (replayData.frames[0].position - playerStart.position).SetY(0f).sqrMagnitude;
		float sqrMagnitude2 = (replayData.frames[0].position - playerStartAlt.position).SetY(0f).sqrMagnitude;
		if ((bool)playerStartAlt && sqrMagnitude < sqrMagnitude2)
		{
			return playerStartAlt;
		}
		return playerStart;
	}
}
