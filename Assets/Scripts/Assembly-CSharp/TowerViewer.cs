using Cinemachine;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.UI;

public class TowerViewer : MonoBehaviour, IInteractableComponent
{
	public CinemachineVirtualCamera virtualCamera;

	public GameObject overlayCanvas;

	public float rotateSpeed = 5f;

	public Vector2 minOffsetAngle = new Vector2(-20f, -20f);

	public Vector2 maxOffsetAngle = new Vector2(30f, 30f);

	public float fovLerpSpeed = 25f;

	public float zoomFOV = 20f;

	public float zoomNearPlane = 7.5f;

	public float fogStartZoomRatio = 0.75f;

	public float fogEndZoomRatio = 1.25f;

	public AudioClip startSound;

	public AudioClip endSound;

	public Text displayText;

	private GameUserInput input;

	private Quaternion originalRotation;

	private Vector2 offsetAngle;

	private float originalFOV;

	private float originalFogStart;

	private float originalFogEnd;

	private float originalNearPlane;

	private GameObject oceanWater;

	private bool active;

	bool IInteractableComponent.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	private void Start()
	{
		originalRotation = virtualCamera.transform.rotation;
		originalFOV = virtualCamera.m_Lens.FieldOfView;
		originalNearPlane = virtualCamera.m_Lens.NearClipPlane;
		virtualCamera.enabled = false;
		overlayCanvas.SetActive(value: false);
	}

	public void Interact()
	{
		active = true;
		virtualCamera.enabled = true;
		input = GameUserInput.CreateInput(base.gameObject);
		overlayCanvas.SetActive(value: true);
		originalFogStart = RenderSettings.fogStartDistance;
		originalFogEnd = RenderSettings.fogEndDistance;
		Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: true);
		startSound.Play();
		UpdateVisorText();
		if (oceanWater == null)
		{
			oceanWater = GameObject.FindWithTag("OceanWater");
		}
		_ = oceanWater != null;
	}

	private void Disable()
	{
		active = false;
		Object.Destroy(input);
		virtualCamera.enabled = false;
		overlayCanvas.SetActive(value: false);
		RenderSettings.fogEndDistance = originalFogEnd;
		RenderSettings.fogStartDistance = originalFogStart;
		Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: false);
		endSound.Play();
		_ = oceanWater != null;
	}

	private void Update()
	{
		if (active)
		{
			Vector2 vector = input.leftStick.vector;
			offsetAngle += vector * Time.deltaTime * rotateSpeed;
			offsetAngle = new Vector2(Mathf.Clamp(offsetAngle.x, minOffsetAngle.x, maxOffsetAngle.x), Mathf.Clamp(offsetAngle.y, minOffsetAngle.y, maxOffsetAngle.y));
			virtualCamera.transform.rotation = Quaternion.AngleAxis(offsetAngle.x, Vector3.up) * originalRotation * Quaternion.AngleAxis(0f - offsetAngle.y, Vector3.right);
			bool isPressed = input.GetConfirmButton().isPressed;
			float b = (isPressed ? zoomFOV : originalFOV);
			virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, b, Time.deltaTime * fovLerpSpeed);
			RenderSettings.fogStartDistance = Mathf.Lerp(b: isPressed ? Mathf.Lerp(originalFogStart, originalFogEnd, fogStartZoomRatio) : originalFogStart, a: RenderSettings.fogStartDistance, t: Time.deltaTime * fovLerpSpeed);
			RenderSettings.fogEndDistance = Mathf.Lerp(b: isPressed ? Mathf.LerpUnclamped(originalFogStart, originalFogEnd, fogEndZoomRatio) : originalFogEnd, a: RenderSettings.fogEndDistance, t: Time.deltaTime * fovLerpSpeed);
			float b2 = (isPressed ? zoomNearPlane : originalNearPlane);
			virtualCamera.m_Lens.NearClipPlane = Mathf.Lerp(virtualCamera.m_Lens.NearClipPlane, b2, Time.deltaTime * fovLerpSpeed);
			if (vector != Vector2.zero)
			{
				UpdateVisorText();
			}
			if (input.GetCancelButton().ConsumePress() || Input.GetKey(KeyCode.Escape))
			{
				Disable();
			}
		}
	}

	private void UpdateVisorText()
	{
		displayText.text = offsetAngle.x.ToString("0") + "," + offsetAngle.y.ToString("0");
	}
}
