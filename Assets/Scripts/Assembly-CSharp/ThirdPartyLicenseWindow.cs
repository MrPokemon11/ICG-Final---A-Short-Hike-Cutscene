using QuickUnityTools.Input;
using TMPro;
using UnityEngine;

public class ThirdPartyLicenseWindow : MonoBehaviour
{
	public TextAsset text;

	public RectTransform scroller;

	public Range scrollRange;

	public float scrollSpeed;

	public GameUserInput input;

	private void Start()
	{
		TMP_Text componentInChildren = scroller.GetComponentInChildren<TMP_Text>();
		componentInChildren.text = ProcessLegalText(text.text);
		componentInChildren.ForceMeshUpdate();
		scrollRange.max = componentInChildren.textBounds.size.y;
	}

	private void Update()
	{
		if (input.WasDismissPressed())
		{
			Object.Destroy(base.gameObject);
		}
		Vector2 anchoredPosition = scroller.anchoredPosition;
		anchoredPosition.y += input.leftStick.vector.y * scrollSpeed * Time.deltaTime;
		anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, scrollRange.min, scrollRange.max);
		scroller.anchoredPosition = anchoredPosition;
	}

	private string ProcessLegalText(string text)
	{
		return text;
	}
}
