using System.Collections.Generic;
using System.Linq;
using SubjectNerd.Utilities;
using UnityEngine;

public class RaceCoordinator : MonoBehaviour
{
	public RaceController raceController;

	public Transform racer;

	public Renderer racerRenderer;

	public float updateCheckFrequency = 2.13f;

	public float seenMoveTime = 30f;

	[Reorderable]
	public List<RaceData> raceData;

	private float lastSeenTime;

	public void Start()
	{
		UpdateRacerPosition(forceUpdate: true);
		this.RegisterTimer(updateCheckFrequency, UpdateRacerPosition, isLooped: true);
	}

	private void UpdateRacerPosition()
	{
		UpdateRacerPosition(false);
	}

	private void UpdateRacerPosition(bool forceUpdate = false)
	{
		if (racerRenderer.isVisible)
		{
			lastSeenTime = Time.time;
		}
		if (!forceUpdate && (raceController.isBusy || racerRenderer.isVisible || Time.time - lastSeenTime < seenMoveTime))
		{
			return;
		}
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		RaceData raceData = this.raceData.Where((RaceData d) => string.IsNullOrEmpty(d.requireTag) || tags.GetBool(d.requireTag)).MinValue((RaceData d) => (d.GetPlayerStartPosition(d.GetReplayData()).position - player.transform.position).sqrMagnitude);
		if (raceData == null)
		{
			racer.gameObject.SetActive(value: false);
		}
		else if (raceData != raceController.currentRaceData)
		{
			racer.gameObject.SetActive(value: true);
			Vector3 position = raceData.GetPlayerStartPosition().position;
			if (!Camera.main.IsPointInView(position) || forceUpdate)
			{
				PlaceRacer(raceData);
			}
		}
	}

	public void PlaceRacer(RaceData raceData)
	{
		PlayerReplayData replayData = raceData.GetReplayData();
		if ((bool)replayData)
		{
			racer.transform.position = replayData.frames[0].position;
			racer.transform.rotation = replayData.frames[0].rotation;
			raceController.SetupRaceData(raceData);
		}
	}

	public void CallAbandonRace()
	{
		raceController.AbandonRace();
	}

	public void CallRestartRace()
	{
		raceController.AbandonRace();
		raceController.RestartCurrentRace();
	}

	private void OnDrawGizmos()
	{
		foreach (RaceData raceDatum in raceData)
		{
			foreach (PlayerReplayData playerReplay in raceDatum.playerReplays)
			{
				if ((bool)playerReplay)
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawWireSphere(playerReplay.frames[0].position, 1f);
				}
			}
			if ((bool)raceDatum.playerStart)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(raceDatum.playerStart.transform.position, 1f);
			}
			if ((bool)raceDatum.playerStartAlt)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(raceDatum.playerStartAlt.transform.position, 1f);
			}
		}
	}

	public void CheckForAchievements()
	{
		if (raceData.All((RaceData r) => r.victories > 0))
		{
			Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.AllRaces);
		}
	}
}
