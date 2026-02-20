using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class KeyboardInputDeviceMapping
{
	private static readonly InputControlType[] INDEX_TO_CONTROL = new InputControlType[8]
	{
		InputControlType.LeftStickUp,
		InputControlType.LeftStickDown,
		InputControlType.LeftStickLeft,
		InputControlType.LeftStickRight,
		InputControlType.Action1,
		InputControlType.Action2,
		InputControlType.Action3,
		InputControlType.Action4
	};

	private static readonly Dictionary<InputControlType, KeyCode> DEFAULT_KEYS = new Dictionary<InputControlType, KeyCode>
	{
		{
			InputControlType.LeftStickUp,
			KeyCode.UpArrow
		},
		{
			InputControlType.LeftStickDown,
			KeyCode.DownArrow
		},
		{
			InputControlType.LeftStickLeft,
			KeyCode.LeftArrow
		},
		{
			InputControlType.LeftStickRight,
			KeyCode.RightArrow
		},
		{
			InputControlType.Action1,
			KeyCode.Z
		},
		{
			InputControlType.Action2,
			KeyCode.X
		},
		{
			InputControlType.Action3,
			KeyCode.C
		},
		{
			InputControlType.Action4,
			KeyCode.Space
		}
	};

	private static readonly Dictionary<InputControlType, Func<string>> CONTROL_READABLE_NAMES = new Dictionary<InputControlType, Func<string>>
	{
		{
			InputControlType.LeftStickUp,
			() => I18n.STRINGS.UP
		},
		{
			InputControlType.LeftStickDown,
			() => I18n.STRINGS.DOWN
		},
		{
			InputControlType.LeftStickLeft,
			() => I18n.STRINGS.LEFT
		},
		{
			InputControlType.LeftStickRight,
			() => I18n.STRINGS.RIGHT
		},
		{
			InputControlType.Action1,
			() => I18n.STRINGS.JUMP
		},
		{
			InputControlType.Action2,
			() => I18n.STRINGS.USE
		},
		{
			InputControlType.Action3,
			() => I18n.STRINGS.RUN
		},
		{
			InputControlType.Action4,
			() => I18n.STRINGS.MENU
		}
	};

	private Dictionary<InputControlType, KeyCode> _map = new Dictionary<InputControlType, KeyCode>();

	public static int CONTROL_COUNT => INDEX_TO_CONTROL.Length;

	public IEnumerable<KeyValuePair<InputControlType, KeyCode>> map => _map;

	public static string GetReadableControlName(int controlIndex)
	{
		return CONTROL_READABLE_NAMES[INDEX_TO_CONTROL[controlIndex]]();
	}

	public static KeyboardInputDeviceMapping GetDefaultMapping()
	{
		KeyboardInputDeviceMapping keyboardInputDeviceMapping = new KeyboardInputDeviceMapping();
		for (int i = 0; i < CONTROL_COUNT; i++)
		{
			keyboardInputDeviceMapping.SetKey(i, DEFAULT_KEYS[INDEX_TO_CONTROL[i]]);
		}
		return keyboardInputDeviceMapping;
	}

	public static KeyboardInputDeviceMapping LoadFromPlayerPrefs()
	{
		KeyboardInputDeviceMapping keyboardInputDeviceMapping = new KeyboardInputDeviceMapping();
		for (int i = 0; i < CONTROL_COUNT; i++)
		{
			InputControlType key = INDEX_TO_CONTROL[i];
			int num = PlayerPrefsAdapter.GetInt("KEY_" + i, -1);
			if (num == -1)
			{
				num = (int)DEFAULT_KEYS[key];
			}
			keyboardInputDeviceMapping.SetKey(i, (KeyCode)num);
		}
		return keyboardInputDeviceMapping;
	}

	private KeyboardInputDeviceMapping()
	{
	}

	public KeyboardInputDeviceMapping(KeyCode[] keys)
	{
		for (int i = 0; i < CONTROL_READABLE_NAMES.Count; i++)
		{
			SetKey(i, keys[i]);
		}
	}

	public KeyCode GetKey(InputControlType control)
	{
		return _map[control];
	}

	private KeyCode GetKey(int controlIndex)
	{
		return _map[INDEX_TO_CONTROL[controlIndex]];
	}

	private void SetKey(int controlIndex, KeyCode key)
	{
		_map[INDEX_TO_CONTROL[controlIndex]] = key;
	}

	public void SaveToPlayerPrefs()
	{
		for (int i = 0; i < CONTROL_COUNT; i++)
		{
			PlayerPrefsAdapter.SetInt("KEY_" + i, (int)GetKey(i));
		}
	}
}
