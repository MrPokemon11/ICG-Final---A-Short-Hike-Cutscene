using UnityEngine;

public class AchievementManager : ServiceMonoBehaviour
{
	private const string LEADERBOARD_TAG_PREFIX = "Leaderboard_";

	private const string ACHIEVEMENT_TAG_PREFIX = "Achieved_";

	private static IAchievementBackend achievementBackend;

	private void Awake()
	{
		if (achievementBackend == null)
		{
			achievementBackend = CrossPlatform.CreateAchievementBackend(this);
			achievementBackend.Initialize();
		}
		else
		{
			achievementBackend.OnSceneLoad();
		}
	}

	public void EnsureAchievement(Achievement achievement)
	{
		EnsureAchievement(achievement.ToString());
	}

	public bool HasAchievement(Achievement achievement)
	{
		return 1 == PlayerPrefsAdapter.GetInt("Achieved_" + achievement);
	}

	public void SetLeaderboard(string boardname, int value)
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool("DisableLeaderboardsCheats"))
		{
			Debug.LogWarning("Won't upload score due to a certain cheat used on this file!");
			return;
		}
		Singleton<GlobalData>.instance.gameData.tags.SetInt("Leaderboard_" + boardname, value);
		achievementBackend.SetLeaderboard(boardname, value);
	}

	private void EnsureAchievement(string name)
	{
		Debug.Log("Achievement: " + name);
		OverrideAchievementFlag(name, value: true);
		achievementBackend.EnsureAchievement(name);
	}

	public static void OverrideAchievementFlag(string name, bool value)
	{
		PlayerPrefsAdapter.SetInt("Achieved_" + name, value ? 1 : 0);
	}
}
