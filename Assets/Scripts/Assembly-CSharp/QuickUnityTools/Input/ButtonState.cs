namespace QuickUnityTools.Input
{
	public struct ButtonState
	{
		public Button button;

		public bool isPressed;

		public bool wasPressed;

		public string name;

		public ButtonState(Button button, string name)
		{
			this.button = button;
			this.name = name;
			wasPressed = false;
			isPressed = false;
		}

		public ButtonState(Button button, bool isPressed, bool wasPressed, string name)
		{
			this.button = button;
			this.isPressed = isPressed;
			this.wasPressed = wasPressed;
			this.name = name;
		}

		public bool ConsumePress()
		{
			if (wasPressed)
			{
				return Singleton<FocusableUserInputManager>.instance.ConsumePressForFrame(button);
			}
			return false;
		}
	}
}
