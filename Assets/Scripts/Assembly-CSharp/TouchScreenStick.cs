using UnityEngine;

public class TouchScreenStick : MonoBehaviour
{
	public RectTransform dragArea;

	public RectTransform stick;

	public float localMaxRadius = 30f;

	public float localPullRadius = 45f;

	private Canvas canvas;

	private Vector2? prevScreenPos;

	public Vector2 value { get; private set; }

	private void Start()
	{
		canvas = GetComponentInParent<Canvas>();
	}

	private void Update()
	{
		Vector2? screenPointInsideRect = TouchScreenControls.GetScreenPointInsideRect(dragArea, canvas);
		if (screenPointInsideRect.HasValue && !prevScreenPos.HasValue)
		{
			base.transform.localPosition = screenPointInsideRect.Value;
		}
		stick.localPosition = (screenPointInsideRect.HasValue ? screenPointInsideRect.Value : ((Vector2)base.transform.localPosition));
		Vector2 vector = stick.localPosition - base.transform.localPosition;
		float magnitude = vector.magnitude;
		Vector2 vector2 = ((magnitude == 0f) ? Vector2.zero : (vector / magnitude));
		value = vector2 * Mathf.InverseLerp(0f, localMaxRadius, magnitude);
		prevScreenPos = screenPointInsideRect;
		if (magnitude > localPullRadius)
		{
			base.transform.localPosition = stick.localPosition - (Vector3)(vector2 * localPullRadius);
		}
	}
}
