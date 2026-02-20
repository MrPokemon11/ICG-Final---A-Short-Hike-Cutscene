using System.Linq;
using InControl;

namespace QuickUnityTools.Input
{
	public class GameActionSet : PlayerActionSet
	{
		public static GameActionSet EMPTY = new GameActionSet();

		public PlayerAction button1;

		public PlayerAction button2;

		public PlayerAction button3;

		public PlayerAction button4;

		public PlayerAction menuButton;

		public PlayerAction leftBumper;

		public PlayerAction rightBumper;

		public PlayerAction moveLeft;

		public PlayerAction moveRight;

		public PlayerAction moveUp;

		public PlayerAction moveDown;

		public PlayerAction lookLeft;

		public PlayerAction lookRight;

		public PlayerAction lookUp;

		public PlayerAction lookDown;

		public PlayerTwoAxisAction leftStick;

		public PlayerTwoAxisAction rightStick;

		public string[] customButtonHandles = new string[4];

		public static GameActionSet LoadOrCreate()
		{
			GameActionSetSettings gameActionSetSettings = GameActionSetSettings.Load();
			GameActionSet gameActionSet = new GameActionSet();
			if (gameActionSetSettings != null)
			{
				gameActionSet.Load(gameActionSetSettings.savedBindings);
				CleanNonGamepadBindings(gameActionSet);
				if (gameActionSetSettings.customButtonHandles != null)
				{
					gameActionSet.customButtonHandles = gameActionSetSettings.customButtonHandles;
				}
			}
			else
			{
				gameActionSet.AddNormalGamepadBindings();
			}
			gameActionSet.UpdateKeyboardBindingsFromPrefs();
			return gameActionSet;
		}

		private GameActionSet()
		{
			CreateActions();
		}

		private void CreateActions()
		{
			button1 = CreatePlayerAction(InputControlType.Action1.ToString());
			button2 = CreatePlayerAction(InputControlType.Action2.ToString());
			button3 = CreatePlayerAction(InputControlType.Action3.ToString());
			button4 = CreatePlayerAction(InputControlType.Action4.ToString());
			menuButton = CreatePlayerAction(InputControlType.Command.ToString());
			moveLeft = CreatePlayerAction(InputControlType.LeftStickLeft.ToString());
			moveRight = CreatePlayerAction(InputControlType.LeftStickRight.ToString());
			moveUp = CreatePlayerAction(InputControlType.LeftStickUp.ToString());
			moveDown = CreatePlayerAction(InputControlType.LeftStickDown.ToString());
			lookLeft = CreatePlayerAction(InputControlType.RightStickLeft.ToString());
			lookRight = CreatePlayerAction(InputControlType.RightStickRight.ToString());
			lookUp = CreatePlayerAction(InputControlType.RightStickUp.ToString());
			lookDown = CreatePlayerAction(InputControlType.RightStickDown.ToString());
			leftBumper = CreatePlayerAction(InputControlType.LeftBumper.ToString());
			rightBumper = CreatePlayerAction(InputControlType.RightBumper.ToString());
			leftStick = CreateTwoAxisPlayerAction(moveLeft, moveRight, moveDown, moveUp);
			rightStick = CreateTwoAxisPlayerAction(lookLeft, lookRight, lookDown, lookUp);
		}

		private void AddNormalGamepadBindings()
		{
			button1.AddBinding(InputControlType.Action1);
			button2.AddBinding(InputControlType.Action2);
			button3.AddBinding(InputControlType.Action3);
			button4.AddBinding(InputControlType.Action4);
			leftBumper.AddBinding(InputControlType.LeftBumper);
			rightBumper.AddBinding(InputControlType.RightBumper);
			menuButton.AddBinding(InputControlType.Command);
			moveUp.AddBinding(InputControlType.LeftStickUp);
			moveDown.AddBinding(InputControlType.LeftStickDown);
			moveLeft.AddBinding(InputControlType.LeftStickLeft);
			moveRight.AddBinding(InputControlType.LeftStickRight);
			moveUp.AddBinding(InputControlType.DPadUp);
			moveDown.AddBinding(InputControlType.DPadDown);
			moveLeft.AddBinding(InputControlType.DPadLeft);
			moveRight.AddBinding(InputControlType.DPadRight);
			lookUp.AddBinding(InputControlType.RightStickUp);
			lookDown.AddBinding(InputControlType.RightStickDown);
			lookLeft.AddBinding(InputControlType.RightStickLeft);
			lookRight.AddBinding(InputControlType.RightStickRight);
		}

		public void UpdateKeyboardBindingsFromPrefs()
		{
			CleanNonGamepadBindings(this);
			KeyboardInputDeviceMapping keyboardInputDeviceMapping = KeyboardInputDeviceMapping.LoadFromPlayerPrefs();
			button1.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.Action1)));
			button2.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.Action2)));
			button3.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.Action3)));
			button4.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.Action4)));
			moveUp.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickUp)));
			moveDown.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickDown)));
			moveLeft.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickLeft)));
			moveRight.AddBinding(KeyCodeToInControl.Convert(keyboardInputDeviceMapping.GetKey(InputControlType.LeftStickRight)));
			moveUp.AddBinding(Key.UpArrow);
			moveDown.AddBinding(Key.DownArrow);
			moveLeft.AddBinding(Key.LeftArrow);
			moveRight.AddBinding(Key.RightArrow);
			button1.AddBinding(Key.Return);
			menuButton.AddBinding(Key.Escape);
			lookUp.AddBinding(new MouseBindingSource(Mouse.PositiveY));
			lookDown.AddBinding(new MouseBindingSource(Mouse.NegativeY));
			lookLeft.AddBinding(new MouseBindingSource(Mouse.NegativeX));
			lookRight.AddBinding(new MouseBindingSource(Mouse.PositiveX));
		}

		public string GetButtonName(PlayerAction action)
		{
			BindingSourceType bindingSourceType = LastInputType;
			if (bindingSourceType == BindingSourceType.None && InputManager.IsSetup)
			{
				bindingSourceType = ((InputManager.Devices.Count > 0) ? BindingSourceType.DeviceBindingSource : BindingSourceType.KeyBindingSource);
			}
			if (!bindingSourceType.IsMouseOrKeyboard())
			{
				string customHandle = GetCustomHandle(action);
				if (customHandle != null)
				{
					return customHandle;
				}
			}
			foreach (BindingSource binding in action.Bindings)
			{
				if (binding.BindingSourceType.IsMouseOrKeyboard() == bindingSourceType.IsMouseOrKeyboard())
				{
					return binding.Name;
				}
			}
			if (action.Bindings.Count > 0)
			{
				return action.Bindings[0].Name;
			}
			return "?";
		}

		public string GetCustomHandle(PlayerAction action)
		{
			if (action == button1)
			{
				return customButtonHandles[0];
			}
			if (action == button2)
			{
				return customButtonHandles[1];
			}
			if (action == button3)
			{
				return customButtonHandles[2];
			}
			if (action == button4)
			{
				return customButtonHandles[3];
			}
			return null;
		}

		public string GetCustomHandle(InputControlType action)
		{
			int num = (int)(action - 19);
			if (num >= 0 && num <= 3)
			{
				return customButtonHandles[num];
			}
			return null;
		}

		public void SetCustomHandle(InputControlType action, string value)
		{
			int num = (int)(action - 19);
			if (num >= 0 && num <= 3)
			{
				customButtonHandles[num] = value;
			}
		}

		public void ResetGamepadBindings()
		{
			Reset();
			AddNormalGamepadBindings();
			UpdateKeyboardBindingsFromPrefs();
		}

		private static void CleanNonGamepadBindings(GameActionSet savedSet)
		{
			foreach (PlayerAction action in savedSet.Actions)
			{
				foreach (BindingSource item in action.Bindings.ToList())
				{
					if (item.BindingSourceType.IsMouseOrKeyboard())
					{
						action.HardRemoveBinding(item);
					}
				}
			}
		}
	}
}
