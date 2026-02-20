using UnityEngine;

namespace QuickUnityTools.Input
{
	public abstract class FocusableUserInput : MonoBehaviour
	{
		private static int STICK_TYPES = 2;

		private static int uniqueConsecutiveInputIds = int.MinValue;

		public int priority;

		public bool obeysPriority = true;

		protected Vector2[] prevStick = new Vector2[2];

		private FocusableUserInputManager inputManager;

		public ButtonState button1 => GetButton(Button.Button1);

		public ButtonState button2 => GetButton(Button.Button2);

		public ButtonState button3 => GetButton(Button.Button3);

		public ButtonState button4 => GetButton(Button.Button4);

		public ButtonState leftBumper => GetButton(Button.LeftBumper);

		public ButtonState rightBumper => GetButton(Button.RightBumper);

		public ButtonState menuButton => GetButton(Button.MenuButton);

		public StickState leftStick => GetStick(Stick.LeftStick);

		public StickState rightStick => GetStick(Stick.RightStick);

		public bool hasFocus
		{
			get
			{
				if (obeysPriority)
				{
					return inputManager.inputWithFocus == this;
				}
				return true;
			}
		}

		public int registerTime { get; private set; }

		protected virtual void Awake()
		{
			inputManager = Singleton<FocusableUserInputManager>.instance;
			CachePreviousStickState();
		}

		protected virtual void OnEnable()
		{
			registerTime = uniqueConsecutiveInputIds;
			uniqueConsecutiveInputIds++;
			inputManager.RegisterInputReceiver(this);
		}

		protected virtual void OnDisable()
		{
			if (inputManager != null)
			{
				inputManager.UnregisterInputReceiver(this);
			}
		}

		public void ForcePriorityUpdate(int priority)
		{
			if (inputManager != null)
			{
				bool num = inputManager.UnregisterInputReceiver(this);
				this.priority = priority;
				if (num)
				{
					inputManager.RegisterInputReceiver(this);
				}
			}
		}

		protected virtual void LateUpdate()
		{
			CachePreviousStickState();
		}

		private void CachePreviousStickState()
		{
			for (int i = 0; i < STICK_TYPES; i++)
			{
				prevStick[i] = GetStick((Stick)i).vector;
			}
		}

		public ButtonState GetButton(Button button)
		{
			ButtonState rawButton = GetRawButton(button);
			if (!hasFocus)
			{
				return new ButtonState(button, rawButton.name);
			}
			return rawButton;
		}

		public StickState GetStick(Stick stick)
		{
			if (!hasFocus)
			{
				return default(StickState);
			}
			return GetRawStick(stick);
		}

		public int GetInputStackHeight()
		{
			return inputManager.inputStackCount - inputManager.sortedUserInputs.IndexOf(this);
		}

		protected abstract ButtonState GetRawButton(Button button);

		protected abstract StickState GetRawStick(Stick stick);
	}
}
