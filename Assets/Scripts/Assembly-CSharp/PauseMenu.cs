using System;
using System.Collections.Generic;
using System.Linq;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	public FocusableUserInput userInput;

	public LinearMenu mainMenu;

	public LinearMenu itemMenu;

	public Transform itemsContainer;

	public BasicMenuItem heldItemElement;

	public GameObject itemMenuItemPrefab;

	public GameObject optionsMenuPrefab;

	public TMP_Text descriptionText;

	public AudioClip errorSound;

	private Player player => Singleton<GameServiceLocator>.instance.levelController.player;

	private void OnEnable()
	{
		RebuildItemMenu();
		I18n.onLanguageChanged += OnLanguageChanged;
	}

	private void OnDisable()
	{
		I18n.onLanguageChanged -= OnLanguageChanged;
	}

	private void OnLanguageChanged()
	{
		List<GameObject> menuObjects = itemMenu.GetMenuObjects();
		if (menuObjects.Count > 0)
		{
			menuObjects[itemMenu.selectedIndex].GetComponent<BasicMenuItem>().Highlight();
		}
	}

	private void RebuildItemMenu()
	{
		itemsContainer.DestroyChildren();
		List<GameObject> list = (from item in Singleton<GlobalData>.instance.gameData.GetAllCollected()
			orderby item.priority descending
			select item).Select(delegate(CollectableItem item)
		{
			GameObject gameObject = itemMenuItemPrefab.Clone();
			gameObject.transform.SetParent(itemsContainer, worldPositionStays: false);
			ConfigureItemElementForItem(item, gameObject, held: false);
			return gameObject;
		}).ToList();
		if (player != null && player.heldItem != null)
		{
			ConfigureItemElementForItem(player.heldItem.associatedItem, heldItemElement.gameObject, held: true);
			list.Insert(0, heldItemElement.gameObject);
		}
		else
		{
			ConfigureItemElementForItem(null, heldItemElement.gameObject, held: true);
		}
		itemMenu.SetMenuObjects(list);
		if (list.Any() && !mainMenu.GetMenuObjects().Contains(itemMenu.gameObject))
		{
			List<GameObject> list2 = mainMenu.GetMenuObjects().ToList();
			list2.Insert(0, itemMenu.gameObject);
			mainMenu.SetMenuObjects(list2);
		}
		else if (!list.Any() && mainMenu.GetMenuObjects().Contains(itemMenu.gameObject))
		{
			List<GameObject> list3 = mainMenu.GetMenuObjects().ToList();
			list3.Remove(itemMenu.gameObject);
			mainMenu.SetMenuObjects(list3);
		}
		if (mainMenu.indexedMenuItem.gameObject != itemMenu.gameObject)
		{
			descriptionText.text = "";
		}
	}

	private void ConfigureItemElementForItem(CollectableItem item, GameObject newElement, bool held)
	{
		Image component = newElement.transform.Find("Icon").GetComponent<Image>();
		Image component2 = newElement.transform.Find("Dot").GetComponent<Image>();
		component.enabled = item != null;
		heldItemElement.GetComponent<Image>().enabled = item != null;
		component2.enabled = item != null && DoesItemHaveActions(item, held);
		if (!(item != null))
		{
			return;
		}
		component.sprite = item.icon;
		BasicMenuItem component3 = newElement.GetComponent<BasicMenuItem>();
		component3.onSelectionChanged += delegate(IMenuItem element, bool selected)
		{
			if (selected)
			{
				UpdateDescriptionTextForItem(item);
			}
		};
		component3.onConfirm.RemoveAllListeners();
		component3.onConfirm.AddListener(delegate
		{
			OnItemConfirm(item, newElement, held);
		});
	}

	private bool DoesItemHaveActions(CollectableItem item, bool held)
	{
		if (!item.worldPrefab)
		{
			return !string.IsNullOrEmpty(item.yarnNode);
		}
		return true;
	}

	private void OnItemConfirm(CollectableItem item, GameObject element, bool held)
	{
		LinearMenu menu = null;
		List<string> list = new List<string>();
		List<Action> list2 = new List<Action>();
		if ((bool)item.worldPrefab)
		{
			foreach (ItemAction action in item.worldPrefab.GetComponent<IActionableItem>().GetMenuActions(held))
			{
				list.Add(action.name);
				list2.Add(delegate
				{
					bool num = action.action();
					menu.Kill();
					if (num)
					{
						Close();
					}
					else
					{
						RebuildItemMenu();
					}
				});
			}
		}
		if (!string.IsNullOrEmpty(item.yarnNode))
		{
			list2.Add(delegate
			{
				Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(item.yarnNode, player.transform);
				menu.Kill();
				Close();
			});
			list.Add(I18n.Localize(item.yarnNodeTitle));
		}
		if (list.Count > 0)
		{
			menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(list.ToArray(), list2.ToArray());
			Vector3 vector = (element.transform.parent as RectTransform).TransformPointTo(element.transform.localPosition, base.transform as RectTransform);
			(menu.transform as RectTransform).localPosition = vector + Vector3.up * 12f;
		}
		else
		{
			errorSound.Play();
		}
	}

	private void UpdateDescriptionTextForItem(CollectableItem item)
	{
		int collected = Singleton<GlobalData>.instance.gameData.GetCollected(item);
		string text = ((collected <= 1) ? I18n.Localize(item.readableName) : string.Format(I18n.STRINGS.menuItemsOrder, collected.ToString(), I18n.Localize(item.readableNamePlural)));
		string text2 = TextReplacer.ReplaceVariables(I18n.Localize(item.description));
		if (item.name == "FishingRod" && GameUserInput.sharedActionSet.LastInputType.IsMouseOrKeyboard())
		{
			text2 = TextReplacer.ReplaceVariables(I18n.STRINGS.fishingRodKeyboardDescription);
		}
		string text3 = "<color=yellow>" + text + "</color>\n" + text2;
		descriptionText.text = text3;
	}

	private void Update()
	{
		if (userInput.GetCancelButton().ConsumePress() || userInput.WasOpenMenuPressed())
		{
			Close();
		}
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Quit()
	{
		Singleton<GameServiceLocator>.instance.levelController.SaveAndQuit();
	}

	public void ShowOptions()
	{
		optionsMenuPrefab.Clone();
	}
}
