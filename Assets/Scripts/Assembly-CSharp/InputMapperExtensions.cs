using System;
using System.Collections.Generic;
using QuickUnityTools.Input;
using UnityEngine;

public static class InputMapperExtensions
{
	public enum Button
	{
		ConfirmButton = 0,
		JumpButton = 1,
		UseItemButton = 2,
		InteractButton = 3,
		RunButton = 4,
		MenuButton = 5
	}

	private static bool _swapInteractButtons;

	private const float textSkipCooldown = 1f / 60f;

	private static float lastTextSkipTime = float.MinValue;

	private static Dictionary<Button, Func<FocusableUserInput, ButtonState>> buttonMap = new Dictionary<Button, Func<FocusableUserInput, ButtonState>>
	{
		{
			Button.ConfirmButton,
			GetConfirmButton
		},
		{
			Button.JumpButton,
			GetJumpButton
		},
		{
			Button.UseItemButton,
			GetUseItemButton
		},
		{
			Button.InteractButton,
			GetInteractButton
		},
		{
			Button.RunButton,
			GetRunButton
		},
		{
			Button.MenuButton,
			GetMenuFaceButton
		}
	};

	public static bool swapInteractButtons
	{
		get
		{
			return _swapInteractButtons;
		}
		set
		{
			_swapInteractButtons = value;
			InputMapperExtensions.onInteractButtonsSwapped?.Invoke();
		}
	}

	public static event Action onInteractButtonsSwapped;

	public static ButtonState GetActionButton(this FocusableUserInput input, Button button)
	{
		return buttonMap[button](input);
	}

	public static ButtonState GetConfirmButton(this FocusableUserInput input)
	{
		return input.button1;
	}

	public static ButtonState GetJumpButton(this FocusableUserInput input)
	{
		return input.button1;
	}

	public static ButtonState GetCancelButton(this FocusableUserInput input)
	{
		return input.button2;
	}

	public static ButtonState GetInteractButton(this FocusableUserInput input)
	{
		if (!swapInteractButtons)
		{
			return input.button1;
		}
		return input.button2;
	}

	public static ButtonState GetUseItemButton(this FocusableUserInput input)
	{
		return input.button2;
	}

	public static ButtonState GetRunButton(this FocusableUserInput input)
	{
		return input.button3;
	}

	public static ButtonState GetMenuFaceButton(this FocusableUserInput input)
	{
		return input.button4;
	}

	public static bool WasOpenMenuPressed(this FocusableUserInput input)
	{
		if (!input.GetMenuFaceButton().ConsumePress())
		{
			return input.menuButton.ConsumePress();
		}
		return true;
	}

	public static bool WasDismissPressed(this FocusableUserInput input)
	{
		if (!input.GetConfirmButton().ConsumePress() && !input.GetCancelButton().ConsumePress())
		{
			return input.GetInteractButton().ConsumePress();
		}
		return true;
	}

	public static bool WasAdvanceDialoguePressed(this FocusableUserInput input)
	{
		if (lastTextSkipTime + 1f / 60f > Time.realtimeSinceStartup)
		{
			return false;
		}
		int num;
		if (!input.GetConfirmButton().ConsumePress() && !input.GetCancelButton().ConsumePress() && !input.GetInteractButton().ConsumePress())
		{
			if (!LevelController.speedrunClockActive)
			{
				num = 0;
				goto IL_0094;
			}
			if (!input.rightBumper.isPressed)
			{
				num = (Input.GetKey(KeyCode.Backspace) ? 1 : 0);
				if (num == 0)
				{
					goto IL_0094;
				}
			}
			else
			{
				num = 1;
			}
		}
		else
		{
			num = 1;
		}
		lastTextSkipTime = Mathf.Min(Time.realtimeSinceStartup, Mathf.Max(lastTextSkipTime + 1f / 60f, Time.realtimeSinceStartup - 1f / 60f));
		goto IL_0094;
		IL_0094:
		return (byte)num != 0;
	}

	public static bool WasChooseDialoguePressed(this FocusableUserInput input)
	{
		if (!input.GetConfirmButton().ConsumePress() && !input.GetCancelButton().ConsumePress() && !input.GetInteractButton().ConsumePress())
		{
			if (LevelController.speedrunClockActive)
			{
				if (!input.rightBumper.wasPressed)
				{
					return Input.GetKeyDown(KeyCode.Backspace);
				}
				return true;
			}
			return false;
		}
		return true;
	}
}
