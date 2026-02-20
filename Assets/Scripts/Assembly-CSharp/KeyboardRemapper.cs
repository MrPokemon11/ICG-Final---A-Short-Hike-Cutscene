using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;

public class KeyboardRemapper : MonoBehaviour
{
	private static KeyCode[] ALL_KEYS;

	public TMP_Text text;

	private void Start()
	{
		SetupKeyList();
		StartCoroutine(DetectKeys());
	}

	private void SetupKeyList()
	{
		if (ALL_KEYS == null)
		{
			ALL_KEYS = ((IEnumerable<KeyCode>)Enum.GetValues(typeof(KeyCode))).Where((KeyCode k) => k < KeyCode.JoystickButton0).ToArray();
		}
	}

	private IEnumerator DetectKeys()
	{
		KeyCode[] pickedKeyCodes = new KeyCode[KeyboardInputDeviceMapping.CONTROL_COUNT];
		for (int i = 0; i < KeyboardInputDeviceMapping.CONTROL_COUNT; i++)
		{
			text.text = string.Format(I18n.STRINGS.pressTheKey, KeyboardInputDeviceMapping.GetReadableControlName(i));
			KeyCode foundKeyCode = (KeyCode)(-1);
			while (foundKeyCode == (KeyCode)(-1))
			{
				yield return new WaitUntil(() => Input.anyKeyDown);
				KeyCode[] aLL_KEYS = ALL_KEYS;
				foreach (KeyCode keyCode in aLL_KEYS)
				{
					if (Input.GetKeyDown(keyCode) && !pickedKeyCodes.Contains(keyCode))
					{
						foundKeyCode = keyCode;
						break;
					}
				}
			}
			text.text = string.Format(I18n.STRINGS.youPickedKey, KeyCodeToString(foundKeyCode));
			yield return new WaitForSeconds(0.5f);
			pickedKeyCodes[i] = foundKeyCode;
		}
		ApplyKeyboardMapping(new KeyboardInputDeviceMapping(pickedKeyCodes));
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public string KeyCodeToString(KeyCode code)
	{
		switch (code)
		{
		case KeyCode.UpArrow:
		case KeyCode.DownArrow:
		case KeyCode.RightArrow:
		case KeyCode.LeftArrow:
			return "<sprite tint=1 name=\"" + code.ToString() + "\">";
		case KeyCode.Alpha0:
		case KeyCode.Alpha1:
		case KeyCode.Alpha2:
		case KeyCode.Alpha3:
		case KeyCode.Alpha4:
		case KeyCode.Alpha5:
		case KeyCode.Alpha6:
		case KeyCode.Alpha7:
		case KeyCode.Alpha8:
		case KeyCode.Alpha9:
			return code.ToString().Replace("Alpha", "");
		default:
			return code.ToString();
		}
	}

	public static void ApplyKeyboardMapping(KeyboardInputDeviceMapping keyboardMapping)
	{
		keyboardMapping.SaveToPlayerPrefs();
		GameUserInput.sharedActionSet.UpdateKeyboardBindingsFromPrefs();
	}

	public static void ResetKeyboardMapping()
	{
		ApplyKeyboardMapping(KeyboardInputDeviceMapping.GetDefaultMapping());
	}
}
