using UnityEngine;
using UnityEngine.UI;

public class BoatGoalArrowUI : MonoBehaviour
{
	public Transform destination;

	public Vector2 destinationOffset;

	public float arrowToDestinationPercent = 0.75f;

	public Range canvasDistance = new Range(50f, 100f);

	public float lerpSpeed = 20f;

	private Image image;

	private LevelUI ui;

	private Player player;

	private void Awake()
	{
		ui = Singleton<GameServiceLocator>.instance.levelUI;
		image = GetComponentInChildren<Image>();
		player = Singleton<GameServiceLocator>.instance.levelController.player;
	}

	private void Start()
	{
		RectTransform toTransform = base.transform.parent as RectTransform;
		base.transform.localPosition = QuickUnityExtensions.WorldToRectTransform(player.transform.position, toTransform);
		image.color = Color.clear;
	}

	private void Update()
	{
		if ((bool)destination)
		{
			Vector2 vector = RaceGoalUI.PositionUIOnDestination(base.transform, destination.position, destinationOffset);
			RectTransform toTransform = base.transform.parent as RectTransform;
			Vector2 vector2 = QuickUnityExtensions.WorldToRectTransform(player.transform.position, toTransform);
			Vector2 b = Vector2.Lerp(vector2, vector, arrowToDestinationPercent);
			base.transform.localPosition = Vector2.Lerp(base.transform.localPosition, b, Time.deltaTime * lerpSpeed);
			Vector3 vector3 = vector - vector2;
			image.transform.localEulerAngles = new Vector3(0f, 0f, -90f + 57.29578f * Mathf.Atan2(vector3.y, vector3.x));
			image.color = new Color(1f, 1f, 1f, canvasDistance.InverseLerp(vector3.magnitude));
		}
	}
}
