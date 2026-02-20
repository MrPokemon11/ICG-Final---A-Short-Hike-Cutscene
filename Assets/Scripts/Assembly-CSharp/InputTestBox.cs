using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputTestBox : MonoBehaviour
{
	public class ControlElement
	{
		private InputControlType controlType;

		private GameObject gameObject;

		private Image fill;

		private bool analog;

		public float lastChangeTime { get; private set; }

		public ControlElement(GameObject gameObject, InputControlType controlType)
		{
			this.controlType = controlType;
			this.gameObject = gameObject;
			fill = gameObject.transform.Find("Fill").GetComponent<Image>();
			analog = false;
			string text = Regex.Replace(controlType.ToString(), "[a-z]+", "");
			if (text.Length > 3)
			{
				text = text.Substring(0, 3);
			}
			UI.SetGenericText(gameObject, text);
		}

		public void Destroy()
		{
			Object.Destroy(gameObject);
		}

		public void Update()
		{
			InputControl control = InputManager.ActiveDevice.GetControl(controlType);
			if (control.Value != 0f && control.Value != 1f)
			{
				analog = true;
			}
			fill.fillAmount = ((!analog) ? control.Value : (control.Value / 2f + 0.5f));
			if (control.HasChanged)
			{
				lastChangeTime = Time.time;
			}
		}
	}

	public static HashSet<InputControlType> STANDARD_CONTROLS;

	public static HashSet<InputControlType> RAW_CONTROLS;

	public GameObject elementTemplate;

	public Transform container;

	public TMP_Text deviceNameText;

	private Dictionary<InputControlType, ControlElement> activeControls = new Dictionary<InputControlType, ControlElement>();

	static InputTestBox()
	{
		STANDARD_CONTROLS = new HashSet<InputControlType>();
		RAW_CONTROLS = new HashSet<InputControlType>();
		STANDARD_CONTROLS.UnionWith(from i in Enumerable.Range(15, 15)
			select (InputControlType)i);
		STANDARD_CONTROLS.UnionWith(from i in Enumerable.Range(300, 8)
			select (InputControlType)i);
		STANDARD_CONTROLS.Add(InputControlType.LeftStickButton);
		STANDARD_CONTROLS.Add(InputControlType.RightStickButton);
		RAW_CONTROLS.UnionWith(from i in Enumerable.Range(400, 19)
			select (InputControlType)i);
		RAW_CONTROLS.UnionWith(from i in Enumerable.Range(500, 29)
			select (InputControlType)i);
	}

	private void Start()
	{
		elementTemplate.SetActive(value: false);
	}

	private void Update()
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		deviceNameText.text = ((activeDevice == null) ? "???" : activeDevice.Name);
		foreach (InputControl control in activeDevice.Controls)
		{
			if (control != null && control.HasChanged && IsControlValidForDevice(control.Target, activeDevice))
			{
				TryAddingControlDisplay(control.Target);
			}
		}
		foreach (ControlElement value in activeControls.Values)
		{
			value.Update();
		}
		if (Input.GetKeyDown(KeyCode.Escape) || ((bool)activeDevice.Action1 && activeDevice.CommandWasPressed))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private bool IsControlValidForDevice(InputControlType target, InputDevice activeDevice)
	{
		if (activeDevice.DeviceClass == InputDeviceClass.Unknown)
		{
			return RAW_CONTROLS.Contains(target);
		}
		return STANDARD_CONTROLS.Contains(target);
	}

	private void TryAddingControlDisplay(InputControlType controlType)
	{
		if (activeControls.ContainsKey(controlType))
		{
			return;
		}
		if (activeControls.Count > 24)
		{
			KeyValuePair<InputControlType, ControlElement> keyValuePair = activeControls.MinValue((KeyValuePair<InputControlType, ControlElement> element) => element.Value.lastChangeTime);
			activeControls.Remove(keyValuePair.Key);
			keyValuePair.Value.Destroy();
		}
		GameObject obj = elementTemplate.Clone();
		obj.transform.SetParent(container, worldPositionStays: false);
		obj.SetActive(value: true);
		ControlElement value = new ControlElement(obj, controlType);
		activeControls.Add(controlType, value);
		Transform[] array = (from c in container.GetChildren()
			orderby c.GetComponentInChildren<TMP_Text>().text
			select c).ToArray();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			array[num].SetAsFirstSibling();
		}
	}
}
