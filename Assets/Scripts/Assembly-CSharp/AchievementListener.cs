using UnityEngine;

public class AchievementListener : ServiceMonoBehaviour
{
	private const string BACKWARDS_CHECK_NOV16 = "backwardsCheckNov16th";

	public CollectableItem featherItem;

	public CollectableItem silverFeatherItem;

	public int totalFeathers = 20;

	public int totalSilverFeathers = 2;

	public int totalSaplings = 27;

	private void Start()
	{
		GlobalData.GameData gameData = Singleton<GlobalData>.instance.gameData;
		gameData.WatchCollected(featherItem, OnFeatherItemChanged);
		gameData.WatchCollected(silverFeatherItem, OnFeatherItemChanged);
		gameData.tags.WatchInt("TOTAL_SPROUTS_DUDE", OnSaplingWatered);
		if (!gameData.tags.GetBool("backwardsCheckNov16th"))
		{
			gameData.tags.SetBool("backwardsCheckNov16th");
			int num = gameData.tags.CountTagsStartingWith("Sapling");
			gameData.tags.SetInt("TOTAL_SPROUTS_DUDE", num);
			Debug.Log("Watered Plants: " + num);
			if (gameData.tags.GetBool("FoxClimbedToTop"))
			{
				Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.PhotoTaken);
			}
		}
	}

	private void OnSaplingWatered(int total)
	{
		if (total >= totalSaplings)
		{
			Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.AllPlants);
		}
	}

	private void OnFeatherItemChanged(int obj)
	{
		GlobalData.GameData gameData = Singleton<GlobalData>.instance.gameData;
		if (gameData.GetCollected(featherItem) >= totalFeathers && gameData.GetCollected(silverFeatherItem) >= totalSilverFeathers)
		{
			Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.AllFeathers);
		}
		if (gameData.GetCollected(featherItem) >= 10)
		{
			Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.TenFeathers);
		}
	}
}
