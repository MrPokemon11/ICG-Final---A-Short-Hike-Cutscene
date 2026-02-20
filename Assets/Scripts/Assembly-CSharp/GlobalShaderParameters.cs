using UnityEngine;

[ExecuteInEditMode]
public class GlobalShaderParameters : MonoBehaviour
{
	public static float shaderTime;

	private static float mouseCountdown;

	private static Vector3 lastMousePosition;

	[Range(0f, 1f)]
	public float maxDarkness = 0.25f;

	[Range(0f, 1f)]
	public float lightCutoff = 0.2f;

	public Vector4 globalWind = new Vector4(0f, 0f, 1f, 0.5f);

	public float premultiplyAlpha
	{
		set
		{
			Shader.SetGlobalFloat("_PremultiplyAlpha", value);
		}
	}

	private void Awake()
	{
		premultiplyAlpha = 0f;
	}

	private void Start()
	{
		Shader.SetGlobalFloat("_MaxDarkness", maxDarkness);
		Shader.SetGlobalFloat("_LightCutoff", lightCutoff);
		Shader.SetGlobalVector("_Wind", globalWind);
	}

	private void Update()
	{
		shaderTime = Shader.GetGlobalVector("_Time").x;
		if (Application.isPlaying)
		{
			mouseCountdown -= Time.deltaTime;
			if (Input.mousePosition != lastMousePosition)
			{
				mouseCountdown = 0.5f;
			}
			Cursor.visible = mouseCountdown > 0f;
			lastMousePosition = Input.mousePosition;
		}
	}
}
