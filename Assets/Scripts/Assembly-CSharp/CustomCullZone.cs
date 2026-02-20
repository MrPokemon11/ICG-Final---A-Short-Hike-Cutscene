using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CustomCullZone : MonoBehaviour
{
	public List<CinemachineVirtualCameraBase> cullCameras = new List<CinemachineVirtualCameraBase>();

	public List<Renderer> renderers;

	private CinemachineBrain brain;

	private bool shown;

	private void Start()
	{
		shown = true;
		if (cullCameras.Count > 0)
		{
			brain = Camera.main.GetComponent<CinemachineBrain>();
			brain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
			if ((bool)cinemachineVirtualCameraBase)
			{
				OnCameraActivated(cinemachineVirtualCameraBase, null);
			}
		}
	}

	private void OnDestroy()
	{
		if ((bool)brain)
		{
			brain.m_CameraActivatedEvent.RemoveListener(OnCameraActivated);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>())
		{
			UpdateRenderers(show: false);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<Player>())
		{
			UpdateRenderers(show: true);
		}
	}

	private void OnCameraActivated(ICinemachineCamera newCam, ICinemachineCamera oldCam)
	{
		bool flag = cullCameras.Contains(newCam);
		UpdateRenderers(!flag);
	}

	private void UpdateRenderers(bool show)
	{
		if (show == shown)
		{
			return;
		}
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = show;
		}
		shown = show;
	}
}
