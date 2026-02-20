using InControl;

public class TouchScreenInputDevice : InputDevice
{
	private TouchScreenControls controls;

	public TouchScreenInputDevice(TouchScreenControls controls)
		: base("Touch Screen")
	{
		this.controls = controls;
		base.Meta = "Touch Screen Device";
		base.SortOrder = int.MaxValue;
		AddControl(InputControlType.LeftStickX, "LeftStickX");
		AddControl(InputControlType.LeftStickY, "LeftStickY");
		for (int i = 0; i < controls.buttonMappings.Count; i++)
		{
			AddControl(controls.buttonMappings[i], controls.buttons[i].buttonName);
		}
	}

	public override void Update(ulong updateTick, float deltaTime)
	{
	}
}
