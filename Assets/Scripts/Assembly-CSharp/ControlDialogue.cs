using System.Collections.Generic;
using InControl;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ControlDialogue : MonoBehaviour
{
	public enum ControlType
	{
		Keyboard = 1,
		Gamepad = 2
	}

	[FormerlySerializedAs("controller")]
	public ControlType controlType = ControlType.Gamepad;

	[FormerlySerializedAs("move")]
	public ControlDialogueRow moveRow;

	[FormerlySerializedAs("button1")]
	public ControlDialogueRow button1Row;

	[FormerlySerializedAs("button2")]
	public ControlDialogueRow button2Row;

	[FormerlySerializedAs("button3")]
	public ControlDialogueRow button3Row;

	[FormerlySerializedAs("button4")]
	public ControlDialogueRow button4Row;

	[TextArea]
	public string jumpButtonText;

	[TextArea]
	public string itemButtonText;

	[TextArea]
	public string runButtonText;

	[TextArea]
	public string interactButtonText;

	[TextArea]
	public string menuButtonText;

	private Dictionary<Button, ControlDialogueRow> buttonRows;

	private void Awake()
	{
		buttonRows = new Dictionary<Button, ControlDialogueRow>
		{
			{
				Button.Button1,
				button1Row
			},
			{
				Button.Button2,
				button2Row
			},
			{
				Button.Button3,
				button3Row
			},
			{
				Button.Button4,
				button4Row
			}
		};
	}

	public void Start()
	{
		UpdateButtonNames();
		UpdateButtonActions();
	}

	private void UpdateButtonActions()
	{
		FocusableUserInput inputWithFocus = Singleton<FocusableUserInputManager>.instance.inputWithFocus;
		foreach (ControlDialogueRow value in buttonRows.Values)
		{
			value.actions.text = "";
		}
		AddActionToRow(I18n.Localize(jumpButtonText), inputWithFocus.GetJumpButton().button);
		AddActionToRow(I18n.Localize(interactButtonText), inputWithFocus.GetInteractButton().button);
		AddActionToRow(I18n.Localize(itemButtonText), inputWithFocus.GetUseItemButton().button);
		AddActionToRow(I18n.Localize(runButtonText), inputWithFocus.GetRunButton().button);
		AddActionToRow(I18n.Localize(menuButtonText), inputWithFocus.GetMenuFaceButton().button);
	}

	private void AddActionToRow(string actionText, Button buttonRow)
	{
		if (!buttonRows.ContainsKey(buttonRow))
		{
			Debug.LogError("No row corresponding to " + buttonRow);
			return;
		}
		TMP_Text actions = buttonRows[buttonRow].actions;
		if (!string.IsNullOrEmpty(actions.text))
		{
			actions.text += "\n";
		}
		actions.text += actionText;
	}

	private void UpdateButtonNames()
	{
		BindingSourceType inputType = ((controlType != ControlType.Keyboard) ? BindingSourceType.DeviceBindingSource : BindingSourceType.KeyBindingSource);
		if (controlType == ControlType.Keyboard)
		{
			KeyboardInputDeviceMapping keyboardInputDeviceMapping = KeyboardInputDeviceMapping.LoadFromPlayerPrefs();
			button1Row.button.text = PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.Action1).ToString());
			button2Row.button.text = PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.Action2).ToString());
			button3Row.button.text = PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.Action3).ToString());
			button4Row.button.text = PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.Action4).ToString());
			moveRow.button.text = PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickUp).ToString()) + PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickLeft).ToString()) + PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickDown).ToString()) + PrepareButtonName(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickRight).ToString());
			return;
		}
		GameActionSet sharedActionSet = GameUserInput.sharedActionSet;
		if (sharedActionSet.ActiveDevice != InputDevice.Null)
		{
			button1Row.button.text = PrepareButtonName(sharedActionSet.GetButtonName(sharedActionSet.button1));
			button2Row.button.text = PrepareButtonName(sharedActionSet.GetButtonName(sharedActionSet.button2));
			button3Row.button.text = PrepareButtonName(sharedActionSet.GetButtonName(sharedActionSet.button3));
			button4Row.button.text = PrepareButtonName(sharedActionSet.GetButtonName(sharedActionSet.button4));
		}
		string PrepareButtonName(string text)
		{
			return SimplifyKeyboardText(GameUserInput.WrapButtonNameForText(inputType, text));
		}
	}

	private string SimplifyKeyboardText(string text)
	{
		text = text.Replace("ALPHA", "");
		return text;
	}
}
