using UnityEngine;
using UnityEngine.UI;

public class RaceGoalUI : MonoBehaviour
{
	public Transform destination;

	public Vector2 offset;

	public bool disappearOnScreen = true;

	private Image image;

	private LevelUI ui;

	private void Awake()
	{
		ui = Singleton<GameServiceLocator>.instance.levelUI;
		image = GetComponentInChildren<Image>();
	}

	private void Update()
	{
		if ((bool)destination)
		{
			bool flag = Camera.main.IsPointInView(destination.position);
			image.enabled = !(disappearOnScreen && flag) && !ui.pauseMenuOpen;
			if (image.enabled)
			{
				base.transform.localPosition = PositionUIOnDestination(base.transform, destination.position, offset);
			}
		}
	}

	public static Vector2 PositionUIOnDestination(Transform uiTransform, Vector3 destination, Vector2 offset)
	{
		RectTransform obj = uiTransform as RectTransform;
		RectTransform rectTransform = uiTransform.parent as RectTransform;
		Vector2 result = QuickUnityExtensions.WorldToRectTransform(destination, rectTransform) + offset;
		Vector2 vector = rectTransform.rect.size / 2f;
		Vector2 vector2 = obj.rect.size / 2f;
		if (!(result.x + vector2.x > vector.x) && !(result.x - vector2.x < 0f - vector.x) && !(result.y + vector2.y > vector.y) && !(result.y - vector2.y < 0f - vector.y))
		{
			return result;
		}
		float f = Mathf.Atan2(result.y, result.x);
		float a = (vector.y - vector2.y) / Mathf.Abs(Mathf.Sin(f));
		float b = (vector.x - vector2.x) / Mathf.Abs(Mathf.Cos(f));
		float num = Mathf.Min(a, b);
		return result.normalized * num;
	}
}
