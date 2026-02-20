using UnityEngine;

public class FishShadow : MonoBehaviour
{
	private static int ALPHA_ID = Shader.PropertyToID("_Alpha");

	[Header("Movement")]
	public float curvePeriod;

	public float speed;

	public AnimationCurve speedCurve;

	public float rotationSpeed;

	public AnimationCurve rotationCurve;

	public float tooCloseRadius = 4f;

	[Header("Animation")]
	public SkinnedMeshRenderer skinnedMesh;

	public AnimationCurve blendCurve;

	public float blendKeyStrength = 60f;

	public float animationTime = 1f;

	public float minAnimSpeed = 0.1f;

	public float maxAnimSpeed = 1.7f;

	[Header("Life")]
	public Range lifetime = new Range(2f, 4f);

	public AnimationCurve lifeAlpha;

	private float time;

	private Timer lifeTimer;

	private Renderer myRenderer;

	private Player player;

	private bool escaping;

	private float animationPlayTime;

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		myRenderer = GetComponentInChildren<Renderer>();
		time = Random.value * curvePeriod;
		lifeTimer = this.RegisterTimer(lifetime.Random(), Kill);
		UpdateAlpha(0f);
	}

	private void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (time > curvePeriod)
		{
			time -= curvePeriod;
		}
		if (player.isSwimming && !escaping && (player.transform.position - base.transform.position).sqrMagnitude < tooCloseRadius.Sqr())
		{
			escaping = true;
		}
		float num = time / curvePeriod;
		float num2 = rotationCurve.Evaluate(num) * rotationSpeed;
		float num3 = speedCurve.Evaluate(num);
		float num4 = (escaping ? speed : (num3 * speed));
		base.transform.Rotate(Vector3.up, num2 * Time.deltaTime, Space.World);
		base.transform.position += base.transform.forward * num4 * Time.deltaTime;
		if (skinnedMesh.isVisible)
		{
			float num5 = Mathf.Lerp(minAnimSpeed, maxAnimSpeed, num3);
			animationPlayTime += Time.deltaTime * num5;
			if (animationPlayTime > animationTime)
			{
				animationPlayTime -= animationTime;
			}
			float num6 = blendCurve.Evaluate(animationPlayTime / animationTime) * blendKeyStrength;
			skinnedMesh.SetBlendShapeWeight(0, num6);
			skinnedMesh.SetBlendShapeWeight(1, blendKeyStrength - num6);
			UpdateAlpha(lifeAlpha.Evaluate(lifeTimer.GetPercentageComplete()));
		}
	}

	public void Face(Vector3 position)
	{
		base.transform.forward = (position - base.transform.position).SetY(0f).normalized;
	}

	private void UpdateAlpha(float a)
	{
		myRenderer.material.SetFloat(ALPHA_ID, a);
	}
}
