using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishEncyclopedia : MonoBehaviour, IActionableItem
{
	public GameObject collectionUIPrefab;

	public GameObject dialogueBoxPrefab;

	public List<ItemAction> GetMenuActions(bool held)
	{
		ItemAction item = new ItemAction(I18n.STRINGS.viewRecords, delegate
		{
			GlobalData.CollectionInventory inventory = Singleton<GlobalData>.instance.gameData.inventory;
			GameObject ui = collectionUIPrefab.Clone();
			CollectionListUI component = ui.GetComponent<CollectionListUI>();
			FishSpecies[] source = FishSpecies.LoadAll();
			IOrderedEnumerable<(FishSpecies, bool)> data = from t in Enumerable.Concat<(FishSpecies, bool)>(second: (from species in source
					where inventory.GetBiggestFishRecord(species, rare: true) != null
					select (species: species, true)).ToList(), first: source.Select((FishSpecies species) => (species: species, rare: false)))
				orderby inventory.GetCatchCount(t.Item1) descending, t.Item2 ? 1 : 0
				select t;
			component.Setup<(FishSpecies, bool)>(data, delegate((FishSpecies species, bool) tuple, CollectionListUIElement element)
			{
				Fish biggestFishRecord = Singleton<GlobalData>.instance.gameData.inventory.GetBiggestFishRecord(tuple.species, tuple.Item2);
				if (biggestFishRecord != null)
				{
					string title = biggestFishRecord.GetTitle();
					element.text.text = string.Format("{0} <color=#BA8>({1} cm)</color>", title, biggestFishRecord.size.ToString("0.0"));
					FishItemActions.SetupFishSprite(biggestFishRecord, element.image);
				}
				else
				{
					element.Setup(tuple.species.sprite, "???");
					element.image.color = Color.black;
				}
				element.onConfirm += delegate
				{
					ShowFishInventoryMenu(tuple.species, element, ui);
				};
			});
			Singleton<GameServiceLocator>.instance.ui.AddUI(ui);
			return true;
		});
		return new List<ItemAction> { item };
	}

	private void ShowFishInventoryMenu(FishSpecies fish, CollectionListUIElement element, GameObject fishMenu)
	{
		LinearMenu menu = null;
		menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(new string[1] { I18n.STRINGS.seeNotes }, new Action[1]
		{
			delegate
			{
				GameObject gameObject = dialogueBoxPrefab.Clone();
				UI.SetGenericText(gameObject, I18n.Localize(fish.journalInfo));
				Singleton<GameServiceLocator>.instance.ui.AddUI(gameObject);
				menu.Kill();
			}
		});
		element.PositionSimpleMenuAbove(menu.gameObject);
	}
}
