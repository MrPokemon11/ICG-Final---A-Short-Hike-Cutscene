using UnityEngine;

public class FaceTowardsCamera : MonoBehaviour
{
	public Vector3 rotationFromCamera = Vector3.zero;

	private MeshRenderer myRenderer;

	private Quaternion rotation;

	private void Awake()
	{
		myRenderer = GetComponentInChildren<MeshRenderer>();
		rotation = Quaternion.Euler(rotationFromCamera);
	}

	private void Update()
	{
		if (myRenderer.isVisible)
		{
			base.transform.forward = rotation * -Camera.main.transform.forward.SetY(0f);
		}
	}
}
