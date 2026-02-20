using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
	[Serializable]
	public class ShakeConfiguration
	{
		[Tooltip("The maximum distance the camera can move from its normal position while shaking.")]
		public float maxShakeDistance = 0.3f;

		[Tooltip("The time (in seconds) it takes for the camera to stop shaking.")]
		public float shakeTime = 1f;

		[Tooltip("The strength of the camera shake over the course of its cooldown time.")]
		public AnimationCurve shakeDistanceOverTime = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	}

	public enum Intensity
	{
		Tiny = 0,
		Gentle = 1,
		Moderate = 2,
		Heavy = 3,
		Extreme = 4
	}

	public ShakeConfiguration tinyShake;

	public ShakeConfiguration gentleShake;

	public ShakeConfiguration moderateShake;

	public ShakeConfiguration heavyShake;

	public ShakeConfiguration extremeShake;

	private Dictionary<Intensity, ShakeConfiguration> shakeConfigs;

	private Intensity lastIntensity;

	private ShakeConfiguration shakeConfig;

	private float shakeCooldown;

	private Vector3 lastShakeVector;

	public bool isShaking => shakeCooldown > 0.05f;

	private void Awake()
	{
		shakeConfigs = new Dictionary<Intensity, ShakeConfiguration>();
		shakeConfigs[Intensity.Tiny] = tinyShake;
		shakeConfigs[Intensity.Gentle] = gentleShake;
		shakeConfigs[Intensity.Moderate] = moderateShake;
		shakeConfigs[Intensity.Heavy] = heavyShake;
		shakeConfigs[Intensity.Extreme] = extremeShake;
	}

	public void Shake(Intensity intensity)
	{
		if (!isShaking || intensity > lastIntensity)
		{
			lastIntensity = intensity;
			shakeConfig = shakeConfigs[intensity];
			shakeCooldown = shakeConfig.shakeTime;
		}
	}

	public static void ShakeAllCameras(Intensity intensity)
	{
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			CameraShake component = allCameras[i].GetComponent<CameraShake>();
			if (component != null)
			{
				component.Shake(intensity);
			}
		}
	}

	private void Update()
	{
		if (isShaking)
		{
			shakeCooldown = Mathf.MoveTowards(shakeCooldown, 0f, Time.unscaledDeltaTime);
		}
		if (!Input.GetKey(KeyCode.R) || !Input.GetKey(KeyCode.T) || !Input.GetKey(KeyCode.Y))
		{
			return;
		}
		for (KeyCode keyCode = KeyCode.Alpha1; keyCode <= KeyCode.Alpha5; keyCode++)
		{
			if (Input.GetKeyDown(keyCode))
			{
				Shake((Intensity)(keyCode - 49));
			}
		}
	}

	private void OnPreRender()
	{
		if (isShaking)
		{
			float time = shakeCooldown / shakeConfig.shakeTime;
			lastShakeVector = UnityEngine.Random.insideUnitSphere * shakeConfig.shakeDistanceOverTime.Evaluate(time) * shakeConfig.maxShakeDistance;
			base.transform.position += lastShakeVector;
		}
	}

	private void OnPostRender()
	{
		if (isShaking)
		{
			base.transform.position -= lastShakeVector;
		}
	}
}
