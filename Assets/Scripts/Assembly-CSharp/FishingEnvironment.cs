using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishingEnvironment : ScriptableObject
{
	public float rareLikelihood = 0.1f;

	public float encounterChancePerSecond = 0.1f;

	public FishProbability[] fish;

	private Dictionary<WaterRegion, float> lastCatchTime = new Dictionary<WaterRegion, float>();

	public float GetEncounterChance(WaterRegion waterRegion)
	{
		return 0f;
	}

	public void ReportCatch(WaterRegion region)
	{
		lastCatchTime[region] = Time.time;
	}
}
