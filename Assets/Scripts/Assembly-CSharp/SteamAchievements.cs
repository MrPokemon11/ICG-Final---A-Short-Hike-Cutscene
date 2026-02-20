using System.Collections;
using Steamworks;
using UnityEngine;

public class SteamAchievements : IAchievementBackend
{
	private class CoroutineRunner : MonoBehaviour
	{
	}

	private CoroutineRunner runner;

	private CallResult<LeaderboardFindResult_t> findAndSetLeaderboardCallResult;

	private CallResult<LeaderboardScoreUploaded_t> uploadLeaderboardCallResult;

	private int? setLeaderboardValue;

	public void Initialize()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogWarning("Steam failed to initalize!");
			return;
		}
		findAndSetLeaderboardCallResult = CallResult<LeaderboardFindResult_t>.Create(OnSteamLeaderboard);
		uploadLeaderboardCallResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);
	}

	public void OnSceneLoad()
	{
	}

	public void EnsureAchievement(string name)
	{
		if (SteamManager.Initialized)
		{
			SteamUserStats.SetAchievement(name);
			SteamUserStats.StoreStats();
		}
	}

	public void SetLeaderboard(string boardname, int value)
	{
		if (SteamManager.Initialized)
		{
			if (runner == null)
			{
				runner = new GameObject("SteamAchievementRunner").AddComponent<CoroutineRunner>();
			}
			runner.StartCoroutine(SetLeaderboardWhenFree(boardname, value));
		}
	}

	private IEnumerator SetLeaderboardWhenFree(string boardname, int value)
	{
		while (setLeaderboardValue.HasValue)
		{
			yield return null;
		}
		setLeaderboardValue = value;
		SteamAPICall_t hAPICall = SteamUserStats.FindLeaderboard(boardname);
		findAndSetLeaderboardCallResult.Set(hAPICall);
		Debug.Log("Finding leaderboard " + boardname);
	}

	private void OnSteamLeaderboard(LeaderboardFindResult_t param, bool bIOFailure)
	{
		if (param.m_bLeaderboardFound == 0 || bIOFailure)
		{
			Debug.LogError("Could not find the leaderboard!");
			setLeaderboardValue = null;
			return;
		}
		if (!setLeaderboardValue.HasValue)
		{
			Debug.LogError("There was no value to set on the leaderboard! Something went wrong.");
			return;
		}
		SteamAPICall_t hAPICall = SteamUserStats.UploadLeaderboardScore(param.m_hSteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, setLeaderboardValue.Value, null, 0);
		uploadLeaderboardCallResult.Set(hAPICall);
		Debug.Log("Setting leaderboard value " + setLeaderboardValue.Value);
		setLeaderboardValue = null;
	}

	private void OnScoreUploaded(LeaderboardScoreUploaded_t param, bool bIOFailure)
	{
		if (param.m_bSuccess == 0 || bIOFailure)
		{
			Debug.LogError("Score uploading failed!");
		}
		string text = param.m_bScoreChanged.ToString();
		int nScore = param.m_nScore;
		Debug.Log("Score changed: " + text + " Score tried: " + nScore);
	}
}
