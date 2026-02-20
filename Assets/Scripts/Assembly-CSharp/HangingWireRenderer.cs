using UnityEngine;

public class HangingWireRenderer : MonoBehaviour
{
	public const float VERY_FAR_SQR = 160000f;

	[Header("Basic Properties")]
	public Material material;

	public Material lineRendererMaterial;

	public Transform wireEnd;

	public int nodes = 6;

	public float droopHeight = 10f;

	public bool gpuInstanced = true;

	[Header("Wind Properties")]
	public float windWave1Freq = 1f;

	public float windWave1Amp = 1f;

	public float windWave2Freq = 2f;

	public float windWave2Amp = 1f;

	public float windWavePhaseScale = 0.15f;

	private Vector3 _endPosition;

	private IHangingWireInstance implementation;

	public Vector3 endPosition
	{
		get
		{
			if (!(wireEnd != null))
			{
				return _endPosition;
			}
			return wireEnd.position;
		}
		set
		{
			if (wireEnd != null)
			{
				wireEnd.position = value;
			}
			else
			{
				_endPosition = value;
			}
			if (implementation != null)
			{
				implementation.UpdateEndPoint();
			}
		}
	}

	public void Awake()
	{
		PixelFilterAdjuster.onPixelWidthChanged += OnPixelWidthModified;
	}

	private void OnEnable()
	{
		if ((float)PixelFilterAdjuster.realPixelWidth > 960f)
		{
			implementation = new LineRendererHangingWireInstance(this);
		}
		else if (gpuInstanced)
		{
			implementation = new InstancedHangingWireInstance(this);
		}
		else
		{
			implementation = new MeshHangingWireInstance(this);
		}
		implementation.Enable();
	}

	private void OnDisable()
	{
		implementation.Disable();
		implementation = null;
	}

	private void OnDestroy()
	{
		PixelFilterAdjuster.onPixelWidthChanged -= OnPixelWidthModified;
	}

	private void OnPixelWidthModified()
	{
		OnDisable();
		OnEnable();
	}

	public static void UpdatePoints(Vector3[] points, HangingWireRenderer obj)
	{
		Vector3 b = obj.endPosition;
		for (int i = 0; i < points.Length; i++)
		{
			float num = (float)i / (float)(points.Length - 1);
			Vector3 vector = Vector3.Lerp(obj.transform.position, b, num);
			float droop = GetDroop(num);
			float num2 = Mathf.Sin(Time.time * obj.windWave1Freq + vector.x * obj.windWavePhaseScale) * obj.windWave1Amp + Mathf.Sin(Time.time * obj.windWave2Freq + vector.z * obj.windWavePhaseScale) * obj.windWave2Amp;
			points[i] = vector + new Vector3(0f, (0f - droop) * obj.droopHeight, droop * num2);
		}
	}

	public static float GetDroop(float x)
	{
		return 4f * x - 4f * x * x;
	}
}
