using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishItemActions : MonoBehaviour, IActionableItem
{
	public delegate void FishMenuCallback(Fish fish, CollectionListUIElement element, GameObject fishMenu);

	public GameObject collectionUIPrefab;

	public List<ItemAction> GetMenuActions(bool held)
	{
		ItemAction item = new ItemAction(I18n.STRINGS.seeCollection, delegate
		{
			CreateFishMenu(collectionUIPrefab, ShowFishInventoryMenu);
			return true;
		});
		return new List<ItemAction> { item };
	}

	public static GameObject CreateFishMenu(GameObject menuPrefab, FishMenuCallback onSelect)
	{
		GameObject ui = menuPrefab.Clone();
		ui.GetComponent<CollectionListUI>().Setup(Singleton<GlobalData>.instance.gameData.inventory.GetAllFish(), delegate(Fish fish, CollectionListUIElement element)
		{
			element.text.text = string.Format("{0} ({1} cm)", fish.GetTitleWithSize(), fish.size.ToString("0.0"));
			SetupFishSprite(fish, element.image);
			element.onConfirm += delegate
			{
				onSelect(fish, element, ui);
			};
		});
		Singleton<GameServiceLocator>.instance.ui.AddUI(ui);
		return ui;
	}

	public static void SetupFishSprite(Fish fish, Image image)
	{
		LayoutElement component = image.GetComponent<LayoutElement>();
		Vector2 vector = ((component != null) ? new Vector2(component.preferredWidth, component.preferredHeight) : image.rectTransform.sizeDelta);
		Sprite sprite = fish.GetSprite();
		float num = vector.y * sprite.rect.width / sprite.rect.height;
		num *= Mathf.Lerp(0.8f, 1.2f, Mathf.InverseLerp(fish.species.size.min, fish.species.size.max, fish.size));
		image.sprite = sprite;
		Vector2 sizeDelta = new Vector2(num, vector.y);
		if ((bool)component)
		{
			component.preferredWidth = sizeDelta.x;
			component.preferredHeight = sizeDelta.y;
		}
		else
		{
			image.rectTransform.sizeDelta = sizeDelta;
		}
	}

	private void ShowFishInventoryMenu(Fish fish, CollectionListUIElement element, GameObject fishMenu)
	{
		LinearMenu menu = null;
		menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(new string[1] { I18n.STRINGS.release }, new Action[1]
		{
			delegate
			{
				Singleton<GlobalData>.instance.gameData.inventory.RemoveFish(fish);
				menu.Kill();
				UnityEngine.Object.Destroy(fishMenu);
			}
		});
		element.PositionSimpleMenuAbove(menu.gameObject);
	}
}
