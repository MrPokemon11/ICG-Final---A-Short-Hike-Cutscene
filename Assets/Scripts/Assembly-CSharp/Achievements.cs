using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Achievements
{
	private static Dictionary<Achievement, AchievementData> data = new Dictionary<Achievement, AchievementData>();

	public static IEnumerable<Achievement> all => ((Achievement[])Enum.GetValues(typeof(Achievement))).Where((Achievement a) => a < Achievement.Platinum);

	public static AchievementData GetData(this Achievement achievement)
	{
		if (!data.ContainsKey(achievement))
		{
			data.Add(achievement, Resources.Load<AchievementData>("Achievements/" + achievement));
		}
		return data[achievement];
	}
}
