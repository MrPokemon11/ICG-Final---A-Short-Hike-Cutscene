using UnityEngine;

[ExecuteInEditMode]
public class CameraFollowPlane : MonoBehaviour
{
	public float minDistance = 100f;

	public float maxDistance = 200f;

	public float minSize = 20f;

	public float maxSize = 60f;

	public bool rotate;

	private Camera mainCamera;

	private void Awake()
	{
		mainCamera = Camera.main;
	}

	private void LateUpdate()
	{
		Ray ray = mainCamera.ViewportPointToRay(Vector3.one * 0.5f);
		Plane plane = new Plane(Vector3.up, base.transform.position);
		float enter = 0f;
		if (plane.Raycast(ray, out enter) && enter < mainCamera.farClipPlane)
		{
			float y = base.transform.position.y;
			Vector3 point = ray.GetPoint(enter);
			base.transform.position = point.SetY(y);
			float t = Mathf.InverseLerp(minDistance, maxDistance, enter);
			base.transform.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, t);
			if (rotate)
			{
				base.transform.forward = -Camera.main.transform.forward.SetY(0f);
			}
		}
	}
}
