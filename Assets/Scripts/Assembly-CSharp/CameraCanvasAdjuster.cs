using UnityEngine;

public class CameraCanvasAdjuster : MonoBehaviour
{
	private Canvas canvas;

	private void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	private void LateUpdate()
	{
		canvas.planeDistance = canvas.worldCamera.nearClipPlane + 1f;
	}
}
