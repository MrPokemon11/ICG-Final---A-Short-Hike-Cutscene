using System.Linq;
using InControl;

namespace QuickUnityTools.Input
{
	public static class InControlExtensions
	{
		public static BindingSource GetGamepadBinding(this PlayerAction action)
		{
			foreach (BindingSource unfilteredBinding in action.UnfilteredBindings)
			{
				if (!unfilteredBinding.BindingSourceType.IsMouseOrKeyboard())
				{
					return unfilteredBinding;
				}
			}
			return null;
		}

		public static string GetName(this UnknownDeviceBindingSource binding)
		{
			UnknownDeviceControl control = binding.Control;
			string text = "";
			if (control.IsAnalog)
			{
				if (control.SourceRange == InputRangeType.ZeroToMinusOne)
				{
					text = "-";
				}
				else if (control.SourceRange == InputRangeType.ZeroToOne)
				{
					text = "+";
				}
			}
			int num = (int)control.Control % 100;
			return text + control.Control.ToString()[0] + num;
		}

		public static string GetName(this DeviceBindingSource binding, InputDevice inputDevice)
		{
			if (inputDevice == null)
			{
				return binding.Control.ToString();
			}
			if (inputDevice.GetControl(binding.Control) == InputControl.Null)
			{
				return binding.Control.ToString();
			}
			return inputDevice.GetControl(binding.Control).Handle;
		}

		public static bool IsMouseOrKeyboard(this BindingSourceType bindingSourceType)
		{
			if (bindingSourceType != BindingSourceType.KeyBindingSource)
			{
				return bindingSourceType == BindingSourceType.MouseBindingSource;
			}
			return true;
		}

		public static void SetOrReplaceBinding(this PlayerAction action, Key key)
		{
			action.SetOrReplaceBinding(new KeyBindingSource(key));
		}

		public static void SetOrReplaceBinding(this PlayerAction action, BindingSource newBinding)
		{
			BindingSource bindingSource = action.UnfilteredBindings.FirstOrDefault((BindingSource binding) => binding.BindingSourceType == newBinding.BindingSourceType);
			if (bindingSource != null)
			{
				action.ReplaceBinding(bindingSource, newBinding);
			}
			else
			{
				action.AddBinding(newBinding);
			}
		}

		public static void AddBinding(this PlayerAction action, Key key)
		{
			action.AddBinding(new KeyBindingSource(key));
		}

		public static void AddBinding(this PlayerAction action, InputControlType input)
		{
			action.AddBinding(new DeviceBindingSource(input));
		}

		public static bool IsDPad(this InputControlType input)
		{
			if (input >= InputControlType.DPadUp)
			{
				return input <= InputControlType.DPadRight;
			}
			return false;
		}
	}
}
