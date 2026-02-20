using UnityEngine;
using UnityEngine.UI;

public class TouchScreenButton : MonoBehaviour
{
	private Canvas canvas;

	private Image image;

	private float originalAlpha;

	public bool value { get; private set; }

	public string buttonName => GetComponentInChildren<Text>().text;

	private void Start()
	{
		canvas = GetComponentInParent<Canvas>();
		image = GetComponentInChildren<Image>();
		originalAlpha = image.color.a;
	}

	private void Update()
	{
		value = TouchScreenControls.GetScreenPointInsideRect(base.transform as RectTransform, canvas).HasValue;
		image.color = image.color.SetA(value ? 1f : originalAlpha);
	}
}
