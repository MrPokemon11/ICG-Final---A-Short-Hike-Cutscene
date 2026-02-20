public interface IAchievementBackend
{
	void Initialize();

	void EnsureAchievement(string name);

	void SetLeaderboard(string boardname, int value);

	void OnSceneLoad();
}
