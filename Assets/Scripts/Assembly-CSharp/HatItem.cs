using System;
using System.Collections.Generic;
using UnityEngine;

public class HatItem : MonoBehaviour, IActionableItem
{
	public const string HAT_TAG = "WornHat";

	public CollectableItem associatedItem;

	public List<ItemAction> GetMenuActions(bool held)
	{
		List<ItemAction> list = new List<ItemAction>();
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		if (tags.GetString("WornHat") == associatedItem.name)
		{
			Func<bool> action = delegate
			{
				tags.SetString("WornHat", null);
				return true;
			};
			list.Add(new ItemAction(I18n.STRINGS.takeOff, action));
		}
		else
		{
			Func<bool> action2 = delegate
			{
				tags.SetString("WornHat", associatedItem.name);
				return true;
			};
			list.Add(new ItemAction(I18n.STRINGS.wear, action2));
		}
		return list;
	}
}
