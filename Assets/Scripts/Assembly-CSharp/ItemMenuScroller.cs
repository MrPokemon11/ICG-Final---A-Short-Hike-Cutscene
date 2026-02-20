using UnityEngine;

public class ItemMenuScroller : MonoBehaviour
{
	public float borderPercent = 0.2f;

	public float scrollSpeed = 100f;

	private LinearMenu menu;

	private Canvas canvas;

	private RectTransform myRect;

	private void Start()
	{
		myRect = base.transform as RectTransform;
		menu = GetComponent<LinearMenu>();
		canvas = GetComponentInParent<Canvas>();
	}

	private void Update()
	{
		RectTransform rectTransform = menu.GetMenuObjects()[menu.selectedIndex].transform as RectTransform;
		Camera worldCamera = canvas.worldCamera;
		Vector3 vector = worldCamera.WorldToViewportPoint(rectTransform.transform.position);
		float num = (float)worldCamera.pixelWidth / (float)worldCamera.pixelHeight / ((float)Screen.width / (float)Screen.height);
		float num2 = (vector.x - 0.5f) * num + 0.5f;
		if (num2 < borderPercent)
		{
			myRect.anchoredPosition += Vector2.right * Time.deltaTime * scrollSpeed;
		}
		else if (num2 > 1f - borderPercent)
		{
			myRect.anchoredPosition -= Vector2.right * Time.deltaTime * scrollSpeed;
		}
	}
}
