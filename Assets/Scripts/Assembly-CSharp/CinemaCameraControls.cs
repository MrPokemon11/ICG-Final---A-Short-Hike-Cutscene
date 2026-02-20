using Cinemachine;
using InControl;
using UnityEngine;

public class CinemaCameraControls : MonoBehaviour
{
	public Vector2 rotateSpeed = Vector2.one * 45f;

	public float zoomSpeed = 10f;

	public float defaultZoom = 95f;

	public float autoRotateSpeed = 5f;

	public float autoZoomSpeed = 5f;

	public float showcaseAngleSpeed = 0.1f;

	public float showcaseAngleFrequency = 3f;

	private CinemachineVirtualCamera vCam;

	private CinemachineFramingTransposer framing;

	private float panSpeed = 50f;

	private Vector3 autoPan;

	private Cheats cheats;

	private float savedDistance;

	private Quaternion savedRotation;

	private Vector3? savedPosition;

	private bool keyboardToggleFreeze;

	private Vector3? showcaseRotationCenter;

	private void Start()
	{
		vCam = GetComponent<CinemachineVirtualCamera>();
		framing = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
	}

	private void OnEnable()
	{
		cheats = Object.FindObjectOfType<Cheats>();
	}

	private void Update()
	{
		if (framing == null)
		{
			return;
		}
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (!keyboardToggleFreeze || Input.GetMouseButton(2))
		{
			base.transform.Rotate(Vector3.up, activeDevice.RightStick.X * rotateSpeed.x * Time.deltaTime, Space.World);
			base.transform.Rotate(Vector3.right, (activeDevice.LeftTrigger ? 0f : activeDevice.RightStick.Y) * rotateSpeed.y * Time.deltaTime, Space.Self);
		}
		int num = (Input.GetKey(KeyCode.Period) ? 1 : 0) + (Input.GetKey(KeyCode.Comma) ? (-1) : 0);
		if (num != 0)
		{
			RenderSettings.fogStartDistance += num;
			RenderSettings.fogEndDistance += num;
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			keyboardToggleFreeze = !keyboardToggleFreeze;
			showcaseRotationCenter = null;
		}
		panSpeed += (Input.GetKey(KeyCode.Alpha9) ? 1 : 0) + (Input.GetKey(KeyCode.Alpha8) ? (-1) : 0);
		Vector3 vector = Vector3.zero;
		if (!Input.GetKey(KeyCode.LeftControl))
		{
			vector = new Vector3((Input.GetKey(KeyCode.J) ? (-1) : 0) + (Input.GetKey(KeyCode.L) ? 1 : 0), (Input.GetKey(KeyCode.I) ? 1 : 0) + (Input.GetKey(KeyCode.K) ? (-1) : 0), Input.mouseScrollDelta.y) * panSpeed;
		}
		else
		{
			autoPan += new Vector3((Input.GetKeyDown(KeyCode.J) ? (-1) : 0) + (Input.GetKeyDown(KeyCode.L) ? 1 : 0), (Input.GetKeyDown(KeyCode.I) ? 1 : 0) + (Input.GetKeyDown(KeyCode.K) ? (-1) : 0), Input.mouseScrollDelta.y) * panSpeed / 5f;
		}
		vector += autoPan;
		vector *= Time.deltaTime;
		base.transform.position += base.transform.right * vector.x;
		base.transform.position += base.transform.up * vector.y;
		base.transform.position += base.transform.forward * vector.z;
		framing.enabled = !keyboardToggleFreeze;
		if (keyboardToggleFreeze)
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				showcaseRotationCenter = Singleton<GameServiceLocator>.instance.levelController.player.transform.position;
			}
			if (showcaseRotationCenter.HasValue)
			{
				base.transform.RotateAround(showcaseRotationCenter.Value, Vector3.up, Mathf.Sin(Time.time * showcaseAngleFrequency) * showcaseAngleSpeed);
			}
		}
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			cheats.TriggerCheat("screenshotplz");
		}
		if (!cheats)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F10))
		{
			framing.enabled = true;
			cheats.TriggerCheat("returnplz");
			if (savedPosition.HasValue)
			{
				framing.m_CameraDistance = savedDistance;
				base.transform.position = savedPosition.Value;
				base.transform.rotation = savedRotation;
			}
		}
		if (Input.GetKeyDown(KeyCode.F12))
		{
			savedDistance = framing.m_CameraDistance;
			savedPosition = base.transform.position;
			savedRotation = base.transform.rotation;
			cheats.TriggerCheat("setposplz");
		}
		if (Input.GetKeyDown(KeyCode.F11))
		{
			cheats.TriggerCheat("toggleuiplz");
		}
	}
}
