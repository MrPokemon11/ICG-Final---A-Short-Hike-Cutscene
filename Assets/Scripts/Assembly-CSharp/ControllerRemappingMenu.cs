using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InControl;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;

public class ControllerRemappingMenu : MonoBehaviour
{
	public static bool remapWarning;

	public GameObject listenForBindingsPrefab;

	public FocusableUserInput input;

	private UI ui;

	private Dictionary<PlayerAction, BindingSource> pendingChanges;

	private CollectionListUI bindingsListMenu;

	private GameActionSet sharedActionSet => GameUserInput.sharedActionSet;

	private void Start()
	{
		ui = Singleton<ServiceLocator>.instance.Locate<UI>();
		pendingChanges = new Dictionary<PlayerAction, BindingSource>();
		bindingsListMenu = GetComponent<CollectionListUI>();
		bindingsListMenu.Setup(sharedActionSet.Actions, SetupBindingElement);
		if (!remapWarning)
		{
			remapWarning = true;
			ui.CreateSimpleDialogue(I18n.STRINGS.changesAfterMenu);
		}
	}

	private void Update()
	{
		if (input.GetCancelButton().ConsumePress() || input.menuButton.ConsumePress())
		{
			TryClosingMenu();
		}
	}

	private void TryClosingMenu()
	{
		if (pendingChanges.Count == 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if ((1u & ((!IsPendingBindingNull(sharedActionSet.button1)) ? 1u : 0u) & ((!IsPendingBindingNull(sharedActionSet.button2)) ? 1u : 0u) & ((!IsPendingBindingNull(sharedActionSet.menuButton)) ? 1u : 0u)) == 0)
		{
			ui.CreateSimpleDialogue(I18n.STRINGS.unboundControls).RegisterOnDestroyCallback(delegate
			{
				LinearMenu menu = null;
				menu = ui.CreateUndismissableSimpleMenu(new string[2]
				{
					I18n.STRINGS.editBindings,
					I18n.STRINGS.discardChanges
				}, new Action[2]
				{
					delegate
					{
						menu.Kill();
					},
					delegate
					{
						UnityEngine.Object.Destroy(base.gameObject);
						menu.Kill();
					}
				});
			});
		}
		else
		{
			ui.CreateSimpleDialogue(I18n.STRINGS.appliedBindings);
			ApplyPendingBindings();
			UnityEngine.Object.Destroy(base.gameObject);
			GameObject inputLock = GameUserInput.CreateInputGameObjectWithPriority(1000);
			Timer.Register(0.1f, delegate
			{
				UnityEngine.Object.Destroy(inputLock);
			});
		}
	}

	private void SetupBindingElement(PlayerAction action, CollectionListUIElement element)
	{
		string text = PlayerActionToTranslatedName(action);
		BindingSource originalBinding = action.GetGamepadBinding();
		BindingSource value;
		string text2 = ((!pendingChanges.TryGetValue(action, out value)) ? GetBindingReadableName(originalBinding) : (GetBindingReadableName(value) + " " + I18n.STRINGS.pending));
		string text3 = "<color=yellow>" + text + ":</color> " + text2;
		element.text.text = text3;
		element.image.gameObject.SetActive(value: false);
		element.ClearConfirmActions();
		element.onConfirm += delegate
		{
			GameObject listenWindow = ui.AddUI(listenForBindingsPrefab.Clone());
			sharedActionSet.ListenOptions.IncludeKeys = false;
			sharedActionSet.ListenOptions.IncludeMouseButtons = false;
			sharedActionSet.ListenOptions.IncludeMouseScrollWheel = false;
			sharedActionSet.ListenOptions.AllowDuplicateBindingsPerSet = true;
			sharedActionSet.ListenOptions.IncludeUnknownControllers = true;
			sharedActionSet.ListenOptions.OnBindingAdded = delegate(PlayerAction playerAction, BindingSource bindingSource)
			{
				BindingSource gamepadBinding = playerAction.GetGamepadBinding();
				RemoveBindingFromAllActions(gamepadBinding);
				pendingChanges[playerAction] = gamepadBinding;
				playerAction.ReplaceBinding(playerAction.GetGamepadBinding(), originalBinding);
			};
			sharedActionSet.ListenOptions.OnBindingEnded = delegate
			{
				RefreshBindingElements();
				UnityEngine.Object.Destroy(listenWindow);
			};
			action.ListenForBindingReplacing(originalBinding);
			StartCoroutine(ListenForBindingsWindowRoutine(action, listenWindow.GetComponentInChildren<TMP_Text>()));
		};
	}

	private void RemoveBindingFromAllActions(BindingSource binding)
	{
		for (int i = 0; i < sharedActionSet.Actions.Count; i++)
		{
			PlayerAction playerAction = sharedActionSet.Actions[i];
			if (pendingChanges.TryGetValue(playerAction, out var value))
			{
				if (value == binding)
				{
					pendingChanges[playerAction] = null;
				}
			}
			else if (playerAction.UnfilteredBindings.FirstOrDefault((BindingSource b) => !b.BindingSourceType.IsMouseOrKeyboard()) == binding)
			{
				pendingChanges[sharedActionSet.Actions[i]] = null;
			}
		}
	}

	private IEnumerator ListenForBindingsWindowRoutine(PlayerAction action, TMP_Text windowText)
	{
		int countdown = 5;
		while (countdown > 0 && windowText != null)
		{
			windowText.text = string.Format(I18n.STRINGS.listeningForInput, countdown);
			yield return new WaitForSecondsRealtime(1f);
			countdown--;
		}
		if (windowText != null)
		{
			action.StopListeningForBinding();
		}
	}

	private void RefreshBindingElements()
	{
		for (int i = 0; i < sharedActionSet.Actions.Count; i++)
		{
			SetupBindingElement(sharedActionSet.Actions[i], bindingsListMenu.GetElement(i));
		}
	}

	private void ApplyPendingBindings()
	{
		foreach (KeyValuePair<PlayerAction, BindingSource> pendingChange in pendingChanges)
		{
			if (pendingChange.Value == null)
			{
				pendingChange.Key.RemoveBinding(pendingChange.Key.GetGamepadBinding());
			}
			else
			{
				pendingChange.Key.ReplaceBinding(pendingChange.Key.GetGamepadBinding(), pendingChange.Value);
			}
		}
		GameActionSetSettings.Save(sharedActionSet);
		pendingChanges.Clear();
	}

	private bool IsPendingBindingNull(PlayerAction action)
	{
		if (pendingChanges.TryGetValue(action, out var value) && value == null)
		{
			return true;
		}
		return false;
	}

	private static string GetBindingReadableName(BindingSource binding)
	{
		if (binding == null)
		{
			return "-";
		}
		string text;
		if (binding is UnknownDeviceBindingSource binding2)
		{
			text = binding2.GetName();
		}
		else
		{
			if (!(binding is DeviceBindingSource binding3))
			{
				Debug.LogWarning("We should only be dealing with controller bindings?");
				return binding.Name;
			}
			text = binding3.GetName(InputManager.ActiveDevice);
		}
		return GameUserInput.WrapButtonNameForText(BindingSourceType.DeviceBindingSource, text);
	}

	private string PlayerActionToTranslatedName(PlayerAction action)
	{
		return InputControlTypeToTranslatedName(Enum.Parse<InputControlType>(action.Name));
	}

	public static string InputControlTypeToTranslatedName(InputControlType target)
	{
		switch (target)
		{
		case InputControlType.Action1:
			return I18n.STRINGS.jumpSelect;
		case InputControlType.Action2:
			return I18n.STRINGS.useItemBack;
		case InputControlType.Action3:
			return I18n.STRINGS.run;
		case InputControlType.Action4:
			return I18n.STRINGS.quickMenu;
		case InputControlType.Menu:
		case InputControlType.Command:
			return I18n.STRINGS.start;
		case InputControlType.LeftBumper:
			return I18n.STRINGS.leftBumper;
		case InputControlType.RightBumper:
			return I18n.STRINGS.rightBumper;
		default:
			return I18n.Localize(Regex.Replace(target.ToString(), "([a-z])([A-Z0-9])", "$1 $2").Trim());
		}
	}
}
