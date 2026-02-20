using System.Collections.Generic;
using UnityEngine;

public class FishingBait : MonoBehaviour, IActionableItem
{
	public const string ACTIVE_TAG = "BaitActive";

	public static bool isBaitActive
	{
		get
		{
			Tags tags = Singleton<GlobalData>.instance.gameData.tags;
			if (!tags.HasBool("BaitActive"))
			{
				tags.SetBool("BaitActive");
			}
			return tags.GetBool("BaitActive");
		}
		set
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool("BaitActive", value);
		}
	}

	public List<ItemAction> GetMenuActions(bool held)
	{
		List<ItemAction> list = new List<ItemAction>();
		if (isBaitActive)
		{
			list.Add(new ItemAction(I18n.STRINGS.stopBait, delegate
			{
				isBaitActive = !isBaitActive;
				return false;
			}));
			list.Add(new ItemAction(I18n.STRINGS.keepBait, () => false));
		}
		else
		{
			list.Add(new ItemAction(I18n.STRINGS.startBait, delegate
			{
				isBaitActive = !isBaitActive;
				return false;
			}));
			list.Add(new ItemAction(I18n.STRINGS.keepNotBait, () => false));
		}
		return list;
	}
}
