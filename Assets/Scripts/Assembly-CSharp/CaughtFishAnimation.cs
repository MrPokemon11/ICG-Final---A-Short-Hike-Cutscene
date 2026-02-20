using System.Collections;
using UnityEngine;

public class CaughtFishAnimation : MonoBehaviour
{
	public float arcHeight = 5f;

	public float totalTime = 1f;

	public float totalRotation = 720f;

	public AnimationCurve arcCurve = AnimationCurve.Constant(0f, 1f, 0f);

	public AnimationCurve timeCurve = AnimationCurve.Constant(0f, 1f, 0f);

	public AnimationCurve rotationCurve = AnimationCurve.Constant(0f, 1f, 0f);

	public AnimationCurve scaleCurve = AnimationCurve.Constant(0f, 1f, 1f);

	public Vector3 scaleComponentFactor = Vector3.one;

	public ParticleSystem waterParticles;

	public AudioClip splashSound;

	public AudioClip catchSound;

	public bool debugAnimation;

	private Fish _fish;

	private Player player;

	private Vector3 startPosition;

	private float startTime;

	private Vector3 startScale;

	public Fish fish
	{
		get
		{
			return _fish;
		}
		set
		{
			_fish = value;
			GetComponentInChildren<SpriteRenderer>().sprite = value.GetSprite();
		}
	}

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		startPosition = base.transform.position;
		startTime = Time.time;
		startScale = base.transform.localScale;
		splashSound.Play();
	}

	private void Update()
	{
		float num = (Time.time - startTime) / totalTime;
		if (num > 1f)
		{
			if (!debugAnimation)
			{
				waterParticles.Stop();
				waterParticles.transform.parent = null;
				catchSound.Play();
				Object.Destroy(base.gameObject);
				if (fish != null)
				{
					player.StartCoroutine(GiveFish(player, fish));
				}
				return;
			}
			startTime = Time.time;
			num = 0f;
		}
		num = Mathf.Clamp01(num);
		num = timeCurve.Evaluate(num);
		Vector3 position = Vector3.Lerp(startPosition, player.transform.position, num);
		position += Vector3.up * arcHeight * arcCurve.Evaluate(num);
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - base.transform.position) * Quaternion.AngleAxis(totalRotation * rotationCurve.Evaluate(num), Vector3.forward);
		Vector3 b = Vector3.Scale(Vector3.one * scaleCurve.Evaluate(num), scaleComponentFactor) + Vector3.one - scaleComponentFactor;
		base.transform.localScale = Vector3.Scale(startScale, b);
	}

	private static IEnumerator GiveFish(Player player, Fish fish)
	{
		yield return new WaitUntil(() => player.input.hasFocus);
		yield return fish.CatchFishRoutine();
	}
}
