using UnityEngine;

[ExecuteInEditMode]
public class TopoCameraScript : MonoBehaviour
{
	private Camera myCamera;

	private void Start()
	{
		myCamera = GetComponent<Camera>();
	}

	public void Update()
	{
		Matrix4x4 worldToCameraMatrix = myCamera.worldToCameraMatrix;
		Matrix4x4 value = GL.GetGPUProjectionMatrix(myCamera.projectionMatrix, renderIntoTexture: true) * worldToCameraMatrix;
		Shader.SetGlobalMatrix("_CustomShadowMapMatrix", value);
	}
}
