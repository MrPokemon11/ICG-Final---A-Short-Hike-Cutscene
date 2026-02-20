using System.Text;
using System.Text.RegularExpressions;
using InControl;
using UnityEngine;

namespace QuickUnityTools.Input
{
	public class GameUserInput : FocusableUserInput
	{
		public static bool FORCE_ARROW_KEYS;

		public static GameActionSet sharedActionSet;

		protected override void Awake()
		{
			base.Awake();
			if (sharedActionSet == null)
			{
				sharedActionSet = GameActionSet.EMPTY;
			}
		}

		private void Start()
		{
			if (sharedActionSet == null || sharedActionSet == GameActionSet.EMPTY)
			{
				if (!InputManager.IsSetup)
				{
					Object.FindObjectOfType<GameSetup>().Start();
				}
				Asserts.WeakAssertTrue(InputManager.IsSetup, "InControl is not set up somehow!");
				sharedActionSet = GameActionSet.LoadOrCreate();
			}
		}

		protected override ButtonState GetRawButton(Button button)
		{
			return button switch
			{
				Button.Button1 => ButtonFromInputControl(button, sharedActionSet.button1), 
				Button.Button2 => ButtonFromInputControl(button, sharedActionSet.button2), 
				Button.Button3 => ButtonFromInputControl(button, sharedActionSet.button3), 
				Button.Button4 => ButtonFromInputControl(button, sharedActionSet.button4), 
				Button.LeftBumper => ButtonFromInputControl(button, sharedActionSet.leftBumper), 
				Button.RightBumper => ButtonFromInputControl(button, sharedActionSet.rightBumper), 
				Button.MenuButton => ButtonFromInputControl(button, sharedActionSet.menuButton), 
				_ => default(ButtonState), 
			};
		}

		protected override StickState GetRawStick(Stick stick)
		{
			switch (stick)
			{
			case Stick.LeftStick:
			{
				Vector2 stick2 = sharedActionSet.leftStick.Vector;
				float sqrMagnitude = stick2.sqrMagnitude;
				if (sqrMagnitude > 1.0001f)
				{
					stick2 /= Mathf.Sqrt(sqrMagnitude);
				}
				if (FORCE_ARROW_KEYS)
				{
					OverrideStickInputWithKeyboard(ref stick2);
				}
				return new StickState(stick2, prevStick[(int)stick]);
			}
			case Stick.RightStick:
				return StickFromInputControl(stick, sharedActionSet.rightStick);
			default:
				return default(StickState);
			}
		}

		private StickState StickFromInputControl(Stick stick, TwoAxisInputControl stickControl)
		{
			return new StickState(stickControl.Vector, prevStick[(int)stick]);
		}

		private ButtonState ButtonFromInputControl(Button button, PlayerAction control)
		{
			return new ButtonState(button, control.IsPressed, control.WasPressed, sharedActionSet.GetButtonName(control));
		}

		private void OverrideStickInputWithKeyboard(ref Vector2 stick)
		{
			if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
			{
				stick = Vector2.up;
			}
			else if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
			{
				stick = Vector2.down;
			}
			else if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
			{
				stick = Vector2.left;
			}
			else if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
			{
				stick = Vector2.right;
			}
		}

		public string WrapButtonNameForText(string name)
		{
			return WrapButtonNameForText(sharedActionSet.LastInputType, name);
		}

		public static string WrapButtonNameForText(BindingSourceType inputType, string name)
		{
			switch (name)
			{
			case "UpArrow":
			case "DownArrow":
			case "LeftArrow":
			case "RightArrow":
				return "<sprite tint=1 name=\"" + name + "\">";
			default:
				if (inputType.IsMouseOrKeyboard())
				{
					return "<b>" + name.ToUpper() + "</b>";
				}
				switch (name)
				{
				case "A":
				case "B":
				case "Y":
				case "Square":
				case "Triangle":
				case "Cross":
				case "Circle":
					return "<sprite tint=1 name=\"" + name + "Button\">";
				case "X":
					return "<sprite tint=1 name=\"CrossButton\">";
				case "Command":
					return "START";
				default:
				{
					if (name.StartsWith("Action") && name.Length > 6)
					{
						return "B" + name[6];
					}
					if (HasDirectionSuffix(name, out var nameAcronym))
					{
						return nameAcronym;
					}
					if (name.StartsWith("Left") || name.StartsWith("Right"))
					{
						return AcronymName(name);
					}
					if (name == "NullInputControl")
					{
						return WrapButtonNameForText(inputType, CrossPlatform.PlatformDefaultButton());
					}
					return name;
				}
				}
			}
		}

		private static bool HasDirectionSuffix(string name, out string nameAcronym)
		{
			nameAcronym = null;
			Match match = Regex.Match(name, "(.+)(Left|Right|Up|Down)");
			if (!match.Success)
			{
				return false;
			}
			nameAcronym = AcronymName(match.Groups[1].Value) + "-" + I18n.Localize(match.Groups[2].Value);
			return true;
		}

		private static string AcronymName(string handle)
		{
			string pattern = "[A-Z0-9]";
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Match item in Regex.Matches(handle, pattern))
			{
				stringBuilder.Append(item.Value);
			}
			return stringBuilder.ToString();
		}

		public static GameUserInput CreateInput(GameObject owner)
		{
			return owner.AddComponent<GameUserInput>();
		}

		public static GameObject CreateInputGameObjectWithPriority(int priority)
		{
			GameObject obj = new GameObject();
			obj.SetActive(value: false);
			obj.AddComponent<GameUserInput>().priority = priority;
			obj.gameObject.SetActive(value: true);
			return obj;
		}
	}
}
