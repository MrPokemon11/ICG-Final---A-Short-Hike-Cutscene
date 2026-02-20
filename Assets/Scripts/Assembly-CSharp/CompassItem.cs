using System.Collections.Generic;
using UnityEngine;

public class CompassItem : MonoBehaviour, IActionableItem
{
	public const string SHOW_COMPASS_TAG = "ShowCompass";

	public List<ItemAction> GetMenuActions(bool held)
	{
		bool shown = Singleton<GlobalData>.instance.gameData.tags.GetBool("ShowCompass");
		string text = (shown ? I18n.STRINGS.hide : I18n.STRINGS.show);
		return new List<ItemAction>
		{
			new ItemAction(text, delegate
			{
				Singleton<GlobalData>.instance.gameData.tags.SetBool("ShowCompass", !shown);
				return true;
			})
		};
	}
}
