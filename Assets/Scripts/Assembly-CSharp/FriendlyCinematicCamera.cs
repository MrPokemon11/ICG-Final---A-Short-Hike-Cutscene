using System;
using System.Collections.Generic;
using Cinemachine;
using InControl;
using TMPro;
using UnityEngine;

public class FriendlyCinematicCamera : MonoBehaviour
{
	public enum CameraMode
	{
		ThirdPerson = 0,
		Freelook = 1
	}

	public enum CameraSetting
	{
		Damping = 0,
		Fog = 1,
		PanSpeed = 2,
		PanForwardSpeed = 3,
		RotateSpeed = 4
	}

	public class CameraSettingController
	{
		public Func<string> GetValue;

		public Action<int> Change;

		public bool holdToChange;
	}

	[Header("Settings")]
	public float rotateSpeed = 45f;

	public float zoomSpeed = 50f;

	public float panSpeed = 50f;

	public float panForwardSpeed = 100f;

	public float autoRotateSpeed = 5f;

	public float autoZoomSpeed = 5f;

	public float autoPanSpeed = 2.5f;

	[Header("Links")]
	public TMP_Text guiText;

	public Canvas guiCanvas;

	private CinemachineVirtualCamera vCam;

	private CinemachineFramingTransposer framing;

	private InputDevice input;

	private Cheats cheats;

	private CameraMode mode;

	private CameraSetting cameraSetting;

	private float autoZoom;

	private float autoRotate;

	private Vector2 autoPan;

	private Dictionary<CameraSetting, CameraSettingController> cameraSettingControllers = new Dictionary<CameraSetting, CameraSettingController>();

	private float savedDistance;

	private Quaternion savedRotation;

	private Vector3? savedPosition;

	private void Start()
	{
		vCam = GetComponent<CinemachineVirtualCamera>();
		framing = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
		cameraSettingControllers.Add(CameraSetting.Damping, new CameraSettingController
		{
			GetValue = () => framing.m_XDamping.ToString(),
			Change = delegate(int value)
			{
				SetDamping(framing.m_XDamping + 0.5f * (float)value);
			}
		});
		cameraSettingControllers.Add(CameraSetting.Fog, new CameraSettingController
		{
			GetValue = () => RenderSettings.fogStartDistance.ToString(),
			Change = delegate(int value)
			{
				RenderSettings.fogStartDistance += value;
				RenderSettings.fogEndDistance += value;
			},
			holdToChange = true
		});
		cameraSettingControllers.Add(CameraSetting.PanSpeed, new CameraSettingController
		{
			GetValue = () => panSpeed.ToString(),
			Change = delegate(int value)
			{
				panSpeed += 10f * (float)value;
			}
		});
		cameraSettingControllers.Add(CameraSetting.PanForwardSpeed, new CameraSettingController
		{
			GetValue = () => panForwardSpeed.ToString(),
			Change = delegate(int value)
			{
				panForwardSpeed += 10f * (float)value;
			}
		});
		cameraSettingControllers.Add(CameraSetting.RotateSpeed, new CameraSettingController
		{
			GetValue = () => rotateSpeed.ToString(),
			Change = delegate(int value)
			{
				rotateSpeed += 10f * (float)value;
			}
		});
	}

	private void OnEnable()
	{
		cheats = UnityEngine.Object.FindObjectOfType<Cheats>();
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (framing == null)
		{
			return;
		}
		input = InputManager.ActiveDevice;
		if (!ModifierButton())
		{
			int num = (input.LeftBumper.WasPressed ? (-1) : 0);
			mode = (CameraMode)((int)(mode + num)).Mod(2);
			if ((bool)input.RightTrigger)
			{
				autoRotate += (float)((input.DPad.Right.WasPressed ? (-1) : 0) + (input.DPad.Left.WasPressed ? 1 : 0)) * autoRotateSpeed;
				autoZoom += (float)((input.DPad.Down.WasPressed ? 1 : 0) + (input.DPad.Up.WasPressed ? (-1) : 0)) * autoZoomSpeed;
				if ((bool)input.LeftStickButton)
				{
					ResetAutoCamera();
				}
			}
			else if ((bool)input.RightBumper && mode == CameraMode.Freelook)
			{
				autoPan.x += (float)(-((input.DPad.Right.WasPressed ? (-1) : 0) + (input.DPad.Left.WasPressed ? 1 : 0))) * autoPanSpeed;
				autoPan.y += (float)(-((input.DPad.Down.WasPressed ? 1 : 0) + (input.DPad.Up.WasPressed ? (-1) : 0))) * autoPanSpeed;
				if ((bool)input.LeftStickButton)
				{
					ResetAutoCamera();
				}
			}
			else
			{
				cameraSetting += (input.DPad.Right.WasPressed ? (-1) : 0) + (input.DPad.Left.WasPressed ? 1 : 0);
				cameraSetting = (CameraSetting)((int)cameraSetting).Mod(Enum.GetValues(typeof(CameraSetting)).Length);
				CameraSettingController cameraSettingController = cameraSettingControllers[cameraSetting];
				if (input.DPad.Down.WasPressed || ((bool)input.DPad.Down && cameraSettingController.holdToChange))
				{
					cameraSettingController.Change(-1);
				}
				else if (input.DPad.Up.WasPressed || ((bool)input.DPad.Up && cameraSettingController.holdToChange))
				{
					cameraSettingController.Change(1);
				}
			}
		}
		else if ((bool)cheats)
		{
			if (input.RightStickButton.WasPressed)
			{
				autoRotate = 0f;
				autoZoom = 0f;
				cheats.TriggerCheat("returnplz");
				if (savedPosition.HasValue)
				{
					framing.m_CameraDistance = savedDistance;
					base.transform.position = savedPosition.Value;
					base.transform.rotation = savedRotation;
				}
			}
			if (input.LeftStickButton.WasPressed)
			{
				savedDistance = framing.m_CameraDistance;
				savedPosition = base.transform.position;
				savedRotation = base.transform.rotation;
				cheats.TriggerCheat("setposplz");
			}
			if (ModifierButton() && input.DPadUp.WasPressed)
			{
				cheats.TriggerCheat("toggleuiplz");
			}
			if (ModifierButton() && input.DPadDown.WasPressed)
			{
				guiCanvas.gameObject.SetActive(!guiCanvas.gameObject.activeSelf);
			}
		}
		UpdateCameraMovement();
		if (guiCanvas.gameObject.activeSelf)
		{
			guiText.text = GetUIString();
		}
	}

	private void ResetAutoCamera()
	{
		framing.m_CameraDistance = 80f;
		autoRotate = 0f;
		autoZoom = 0f;
		autoPan = Vector2.zero;
	}

	private string GetUIString()
	{
		string text = "";
		if (!ModifierButton())
		{
			text += Wrap($"LB - Change Mode ({mode.ToString()})\n", input.LeftBumper);
		}
		text += Wrap("LT - Modifier\n", ModifierButton());
		if (ModifierButton())
		{
			if (mode == CameraMode.Freelook)
			{
				text += Wrap("RT - Forward\n", input.RightTrigger);
				text += Wrap("RB - Backward\n", input.RightBumper);
				text += Wrap("RS - Pan Camera\n", input.RightStick);
			}
			else if (mode == CameraMode.ThirdPerson)
			{
				text += Wrap("RS - Zoom Camera\n", input.RightStick);
			}
		}
		else
		{
			text += Wrap("RS - Rotate Camera\n", input.RightStick);
		}
		if (!ModifierButton())
		{
			text += Wrap("RT - Auto Rotate\n", input.RightTrigger);
			if (!input.RightTrigger && mode == CameraMode.Freelook)
			{
				text += Wrap("RB - Auto Pan\n", input.RightBumper);
			}
			if ((bool)input.RightTrigger)
			{
				text += Wrap($"DPL DPR - Auto Rotate ({autoRotate})\n", (float)input.DPadX != 0f);
				text += Wrap($"DPU DPD - Auto Zoom ({autoZoom})\n", (float)input.DPadY != 0f);
				return text + Wrap($"LSB - Reset\n", input.LeftStickButton);
			}
			if ((bool)input.RightBumper && mode == CameraMode.Freelook)
			{
				text += Wrap($"DPAD - Auto Pan ({autoPan.x},{autoPan.y})\n", input.DPad);
				return text + Wrap($"LSB - Reset\n", input.LeftStickButton);
			}
			text += Wrap($"DPL DPR - Setting ({cameraSetting.ToString()})\n", (float)input.DPadX != 0f);
			return text + Wrap($"DPU DPD - Change Setting ({cameraSettingControllers[cameraSetting].GetValue().ToString()})\n", (float)input.DPadY != 0f);
		}
		text += Wrap($"LSB - Save Position\n", input.LeftStickButton);
		text += Wrap($"RSB - Restore Position\n", input.RightStickButton);
		text += Wrap($"DPU - Toggle Game UI\n", input.DPadUp);
		return text + Wrap($"DPD - Toggle Camera UI\n", input.DPadDown);
	}

	public bool ModifierButton()
	{
		if ((bool)input.Action2 && (bool)input.Action3)
		{
			return true;
		}
		return input.LeftTrigger;
	}

	private string Wrap(string text, bool highlight)
	{
		if (!highlight)
		{
			return text;
		}
		return "<color=green>" + text + "</color>";
	}

	private void UpdateCameraMovement()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 vector = input.RightStick;
		if (input.Action3.IsPressed)
		{
			vector = Vector3.zero;
		}
		switch (mode)
		{
		case CameraMode.Freelook:
			if (ModifierButton())
			{
				zero.x = vector.x * panSpeed;
				zero.y = vector.y * panSpeed;
				zero.z += Mathf.Abs((float)input.RightTrigger * panForwardSpeed);
				zero.z += 0f - Mathf.Abs((float)input.RightBumper * panForwardSpeed);
			}
			else
			{
				zero2.x = (0f - vector.y) * rotateSpeed;
				zero2.y = vector.x * rotateSpeed;
			}
			zero.x += autoPan.x;
			zero.y += autoPan.y;
			break;
		case CameraMode.ThirdPerson:
			if (ModifierButton())
			{
				zero.z = (0f - vector.y) * zoomSpeed;
				break;
			}
			zero2.x = vector.y * rotateSpeed;
			zero2.y = vector.x * rotateSpeed;
			break;
		}
		framing.enabled = mode == CameraMode.ThirdPerson;
		base.transform.Rotate(Vector3.up, (autoRotate + zero2.y) * Time.deltaTime, Space.World);
		base.transform.Rotate(Vector3.right, zero2.x * Time.deltaTime, Space.Self);
		switch (mode)
		{
		case CameraMode.Freelook:
			base.transform.position += base.transform.right * zero.x * Time.deltaTime;
			base.transform.position += base.transform.up * zero.y * Time.deltaTime;
			base.transform.position += base.transform.forward * (zero.z + autoZoom) * Time.deltaTime;
			break;
		case CameraMode.ThirdPerson:
			framing.m_CameraDistance += (zero.z + autoZoom) * Time.deltaTime;
			break;
		}
	}

	private void SetDamping(float value)
	{
		value = Mathf.Clamp(value, 0f, 2f);
		framing.m_XDamping = value;
		framing.m_YDamping = value;
		framing.m_ZDamping = value;
	}
}
