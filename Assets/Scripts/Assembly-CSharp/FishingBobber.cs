using System.Collections.Generic;
using UnityEngine;

public class FishingBobber : MonoBehaviour
{
	private Floater floater;

	private List<FishingEnvironment> fishingEnvironments = new List<FishingEnvironment>();

	public FishingEnvironment fishingEnvironment
	{
		get
		{
			if (fishingEnvironments.Count <= 0)
			{
				return floater.waterRegion?.fishingEnvironment;
			}
			return fishingEnvironments[0];
		}
	}

	private void Awake()
	{
		floater = GetComponent<Floater>();
	}

	private void OnTriggerEnter(Collider other)
	{
		FishRegion component = other.GetComponent<FishRegion>();
		if ((bool)component)
		{
			fishingEnvironments.Add(component.fishingEnvironment);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		FishRegion component = other.GetComponent<FishRegion>();
		if ((bool)component)
		{
			fishingEnvironments.Remove(component.fishingEnvironment);
		}
	}
}
