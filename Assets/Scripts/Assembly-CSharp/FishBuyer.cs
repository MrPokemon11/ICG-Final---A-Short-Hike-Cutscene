using System;
using System.Collections.Generic;
using UnityEngine;

public class FishBuyer : MonoBehaviour
{
	private const string SOLD_FISH = "Sold_";

	public GameObject collectionUIPrefab;

	public CollectableItem baitItem;

	public CollectableItem coinItem;

	public AudioClip saleSoundEffect;

	[Header("Yarn Nodes")]
	public string sellNode = "BuyFish";

	public string cancelNode = "BuyFishCancel";

	[Header("Yarn Read Tags")]
	public string alreadySoldTag = "FishAlreadySold";

	public string nameTag = "FishName";

	public string priceTag = "FishPrice";

	public string rareTag = "FishRare";

	[Header("Yarn Write Tags")]
	public string soldBaitTag = "BaitSold";

	public void SellFish()
	{
		FishItemActions.CreateFishMenu(collectionUIPrefab, ShowSubmenu).GetComponent<KillOnBackButton>().onKill += delegate
		{
			Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(cancelNode, base.transform);
		};
	}

	public static string GetFishSoldTag(FishSpecies species, bool rare)
	{
		return "Sold_" + species.name + (rare ? "_Rare" : "");
	}

	private void ShowSubmenu(Fish fish, CollectionListUIElement element, GameObject fishMenu)
	{
		LinearMenu menu = null;
		GlobalData.GameData data = Singleton<GlobalData>.instance.gameData;
		string specificFishAlreadySoldTag = GetFishSoldTag(fish.species, fish.rare);
		List<string> list = new List<string>();
		List<Action> list2 = new List<Action>();
		int baitWorth = 1 + ((fish.sizeCategory != Fish.SizeCategory.Normal) ? 1 : 0) + (fish.rare ? 2 : 0);
		if (data.tags.GetBool(soldBaitTag) && data.tags.GetBool(specificFishAlreadySoldTag))
		{
			string format = ((baitWorth == 1) ? I18n.STRINGS.sellForBait : I18n.STRINGS.sellForBaitPlural);
			list.Add(string.Format(format, baitWorth));
			list2.Add(delegate
			{
				data.inventory.RemoveFish(fish);
				data.AddCollected(baitItem, baitWorth);
				menu.Kill();
				fishMenu.GetComponent<CollectionListUI>().RemoveElement(element.gameObject);
				saleSoundEffect.Play();
			});
		}
		else
		{
			list.Add(I18n.STRINGS.sell);
			list2.Add(delegate
			{
				data.inventory.RemoveFish(fish);
				menu.Kill();
				UnityEngine.Object.Destroy(fishMenu);
				data.tags.SetBool(alreadySoldTag, data.tags.GetBool(specificFishAlreadySoldTag));
				string title = fish.GetTitle();
				title = title.ToLower();
				data.tags.SetString(nameTag, title);
				data.tags.SetFloat(priceTag, fish.species.price * ((!fish.rare) ? 1 : 4));
				data.tags.SetBool(rareTag, fish.rare);
				Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(sellNode, base.transform);
				data.tags.SetBool(specificFishAlreadySoldTag);
			});
		}
		menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(list.ToArray(), list2.ToArray());
		element.PositionSimpleMenuAbove(menu.gameObject);
	}

	private void PayCoins()
	{
		GlobalData.GameData gameData = Singleton<GlobalData>.instance.gameData;
		gameData.AddCollected(coinItem, (int)gameData.tags.GetFloat(priceTag));
	}
}
