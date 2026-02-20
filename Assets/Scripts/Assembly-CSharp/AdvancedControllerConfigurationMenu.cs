using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using QuickUnityTools.Input;
using UnityEngine;

public class AdvancedControllerConfigurationMenu : MonoBehaviour
{
	public static Action onIconsUpdated;

	public GameObject bindingsMenuPrefab;

	public GameObject testInputPrefab;

	public GameObject collectionViewPrefab;

	private UI ui;

	private void Start()
	{
		ui = Singleton<ServiceLocator>.instance.Locate<UI>();
		ShowMenu();
	}

	private void ShowMenu()
	{
		List<string> list = new List<string>();
		List<Action> list2 = new List<Action>();
		list.Add(I18n.STRINGS.customize);
		list2.Add(delegate
		{
			ui.AddUI(bindingsMenuPrefab.Clone());
		});
		list.Add(I18n.STRINGS.revertCustomizations);
		list2.Add(delegate
		{
			GameUserInput.sharedActionSet.ResetGamepadBindings();
			GameUserInput.sharedActionSet.customButtonHandles = new string[4];
			GameActionSetSettings.Save(GameUserInput.sharedActionSet);
			ui.CreateSimpleDialogue(I18n.STRINGS.resetControllerBindings);
		});
		list.Add(I18n.STRINGS.testInputs);
		list2.Add(delegate
		{
			ui.AddUI(testInputPrefab.Clone());
		});
		list.Add(I18n.STRINGS.forceCustomButtonIcons);
		list2.Add(delegate
		{
			SetButtonIcons();
		});
		ShowMenu(list, list2);
	}

	private static LinearMenu ShowMenu(IEnumerable<string> names, IEnumerable<Action> actions)
	{
		LinearMenu linearMenu = Singleton<ServiceLocator>.instance.Locate<UI>().CreateSimpleMenu(names.ToArray(), actions.ToArray());
		OptionsMenu.PositionMenu(linearMenu.transform);
		return linearMenu;
	}

	public void SetButtonIcons()
	{
		GameObject gameObject = ui.AddUI(collectionViewPrefab.Clone());
		gameObject.GetComponent<CollectionListUI>().Setup(new InputControlType[4]
		{
			InputControlType.Action1,
			InputControlType.Action2,
			InputControlType.Action3,
			InputControlType.Action4
		}, SetupButtonIconElement);
		gameObject.RegisterOnDestroyCallback(delegate
		{
			GameActionSetSettings.Save(GameUserInput.sharedActionSet);
			onIconsUpdated?.Invoke();
		});
	}

	private static void SetupButtonIconElement(InputControlType type, CollectionListUIElement element)
	{
		string customHandle = GameUserInput.sharedActionSet.GetCustomHandle(type);
		customHandle = ((customHandle == null) ? "-" : customHandle);
		string text = $"<color=yellow>{ControllerRemappingMenu.InputControlTypeToTranslatedName(type)}:</color> {GameUserInput.WrapButtonNameForText(BindingSourceType.DeviceBindingSource, customHandle)}";
		element.text.text = text;
		element.image.gameObject.SetActive(value: false);
		element.ClearConfirmActions();
		element.onConfirm += delegate
		{
			ChooseButtonIcon(type, element);
		};
	}

	private static void ChooseButtonIcon(InputControlType forType, CollectionListUIElement element)
	{
		List<string> list = new List<string>();
		List<Action> list2 = new List<Action>();
		LinearMenu menu = null;
		foreach (string icon in new List<string> { "A", "B", "X", "Y", "Square", "Triangle", "Circle", null })
		{
			list.Add((icon != null) ? GameUserInput.WrapButtonNameForText(BindingSourceType.DeviceBindingSource, icon) : I18n.STRINGS.defaultIcon);
			list2.Add(delegate
			{
				GameUserInput.sharedActionSet.SetCustomHandle(forType, icon);
				SetupButtonIconElement(forType, element);
				menu.Kill();
			});
		}
		menu = ShowMenu(list, list2);
	}
}
