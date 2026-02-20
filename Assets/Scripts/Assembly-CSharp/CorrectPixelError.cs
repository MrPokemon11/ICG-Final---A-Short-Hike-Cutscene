using UnityEngine;
using UnityEngine.EventSystems;

public class CorrectPixelError : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		RectTransform rectTransform = base.transform as RectTransform;
		if (rectTransform.pivot.x == 0.5f)
		{
			rectTransform.sizeDelta = rectTransform.sizeDelta.SetX(Mathf.Ceil(rectTransform.sizeDelta.x / 2f) * 2f);
		}
		if (rectTransform.pivot.y == 0.5f)
		{
			rectTransform.sizeDelta = rectTransform.sizeDelta.SetY(Mathf.Ceil(rectTransform.sizeDelta.y / 2f) * 2f);
		}
	}
}
