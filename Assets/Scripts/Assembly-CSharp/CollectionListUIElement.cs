using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectionListUIElement : MonoBehaviour
{
	public Image image;

	public TMP_Text text;

	private BasicMenuItem menuItem;

	public event UnityAction onConfirm
	{
		add
		{
			menuItem.onConfirm.AddListener(value);
		}
		remove
		{
			menuItem.onConfirm.RemoveListener(value);
		}
	}

	private void Awake()
	{
		menuItem = GetComponent<BasicMenuItem>();
	}

	public void Setup(Sprite sprite, string info)
	{
		text.text = info;
		image.sprite = sprite;
		image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.y * sprite.rect.width / sprite.rect.height, image.rectTransform.sizeDelta.y);
	}

	public void PositionSimpleMenuAbove(GameObject menu)
	{
		RectTransform rectTransform = base.transform as RectTransform;
		Vector3 localPosition = rectTransform.TransformPointTo(rectTransform.rect.center + Vector2.left * rectTransform.rect.size.x / 2f, menu.transform.parent as RectTransform);
		menu.transform.localPosition = localPosition;
		RectTransform rectTransform2 = menu.transform as RectTransform;
		RectTransform rectTransform3 = menu.transform.parent as RectTransform;
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform2);
		if (rectTransform2.rect.min.x + rectTransform2.localPosition.x < rectTransform3.rect.min.x)
		{
			rectTransform2.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5f, rectTransform2.rect.width);
		}
	}

	public void ClearConfirmActions()
	{
		menuItem.onConfirm.RemoveAllListeners();
	}
}
