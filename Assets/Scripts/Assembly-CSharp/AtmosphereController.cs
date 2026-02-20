using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class AtmosphereController : ServiceMonoBehaviour
{
	public float lerpDuration = 5f;

	public Atmosphere defaultAtmosphere;

	public Light mainLight;

	public ParticleSystem wind;

	public ParticleSystem snow;

	public ParticleSystem windySnow;

	public ParticleSystem rain;

	public Animator sunsetGlow;

	public NorthernLightsController northernLights;

	private Atmosphere prevAtmosphere;

	private Atmosphere currentAtmosphere;

	private Atmosphere desiredAtmosphere;

	private float lerpTime;

	private EdgeDetection edges;

	private ColorCorrectionCurves curves;

	private SortedList<StackResourceSortingKey, Atmosphere> atmosphereStack = new SortedList<StackResourceSortingKey, Atmosphere>();

	private IList<Atmosphere> atmosphereList;

	private void Awake()
	{
		atmosphereList = atmosphereStack.Values;
	}

	private void Start()
	{
		edges = Camera.main.GetComponent<EdgeDetection>();
		curves = Camera.main.GetComponent<ColorCorrectionCurves>();
		prevAtmosphere = defaultAtmosphere;
		desiredAtmosphere = defaultAtmosphere;
		currentAtmosphere = defaultAtmosphere;
		lerpTime = lerpDuration;
		SetAtmosphere(currentAtmosphere);
	}

	public StackResourceSortingKey AddToAtmosphereStack(int priority, Atmosphere atmosphere)
	{
		StackResourceSortingKey stackResourceSortingKey = new StackResourceSortingKey(priority, RemoveFromAtmosphereStack);
		atmosphereStack.Add(stackResourceSortingKey, atmosphere);
		UpdateAtmosphere();
		return stackResourceSortingKey;
	}

	private void RemoveFromAtmosphereStack(StackResourceSortingKey key)
	{
		atmosphereStack.Remove(key);
		UpdateAtmosphere();
	}

	private void UpdateAtmosphere()
	{
		SetAtmosphere((atmosphereList.Count > 0) ? new Atmosphere?(atmosphereList[0]) : ((Atmosphere?)null));
	}

	private void SetAtmosphere(Atmosphere? atmosphere)
	{
		Atmosphere atmosphere2 = desiredAtmosphere;
		prevAtmosphere = currentAtmosphere;
		lerpTime = 0f;
		if (atmosphere.HasValue)
		{
			desiredAtmosphere = atmosphere.Value;
		}
		else
		{
			desiredAtmosphere = defaultAtmosphere;
		}
		if (desiredAtmosphere.effects.HasFlag(Atmosphere.Effects.Snow))
		{
			snow.Play();
		}
		else
		{
			snow.Stop();
		}
		if (desiredAtmosphere.effects.HasFlag(Atmosphere.Effects.Wind))
		{
			wind.Play();
		}
		else
		{
			wind.Stop();
		}
		if (desiredAtmosphere.effects.HasFlag(Atmosphere.Effects.Rain))
		{
			rain.Play();
		}
		else
		{
			rain.Stop();
		}
		if (desiredAtmosphere.effects.HasFlag(Atmosphere.Effects.WindySnow))
		{
			windySnow.Play();
		}
		else
		{
			windySnow.Stop();
		}
		if (sunsetGlow != null)
		{
			sunsetGlow.SetBool("Enabled", desiredAtmosphere.effects.HasFlag(Atmosphere.Effects.SunsetGlow));
		}
		if (northernLights != null)
		{
			northernLights.enableLights = desiredAtmosphere.effects.HasFlag(Atmosphere.Effects.NorthernLights);
		}
		if ((bool)atmosphere2.ambientSounds)
		{
			atmosphere2.ambientSounds.gameObject.SetActive(value: false);
		}
		if ((bool)desiredAtmosphere.ambientSounds)
		{
			desiredAtmosphere.ambientSounds.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (lerpTime < lerpDuration)
		{
			lerpTime += Time.deltaTime;
			float t = lerpTime / lerpDuration;
			currentAtmosphere.shadowColor = Color.Lerp(prevAtmosphere.shadowColor, desiredAtmosphere.shadowColor, t);
			currentAtmosphere.edgeColor = Color.Lerp(prevAtmosphere.edgeColor, desiredAtmosphere.edgeColor, t);
			currentAtmosphere.fogColor = Color.Lerp(prevAtmosphere.fogColor, desiredAtmosphere.fogColor, t);
			currentAtmosphere.lightColor = Color.Lerp(prevAtmosphere.lightColor, desiredAtmosphere.lightColor, t);
			currentAtmosphere.saturation = Mathf.Lerp(prevAtmosphere.saturation, desiredAtmosphere.saturation, t);
			currentAtmosphere.cameraColor = Color.Lerp(prevAtmosphere.cameraColor, desiredAtmosphere.cameraColor, t);
			ApplyAtmoshphereColors(currentAtmosphere);
		}
	}

	public void ApplyAtmoshphereColors(Atmosphere atmosphere)
	{
		RenderSettings.fogColor = atmosphere.fogColor;
		RenderSettings.ambientLight = atmosphere.shadowColor;
		mainLight.color = atmosphere.lightColor - atmosphere.shadowColor;
		Camera.main.backgroundColor = atmosphere.cameraColor;
		edges.edgeColor = atmosphere.edgeColor;
		curves.saturation = atmosphere.saturation;
	}
}
