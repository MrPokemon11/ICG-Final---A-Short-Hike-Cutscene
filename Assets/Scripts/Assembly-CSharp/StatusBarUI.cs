using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
	public GameObject statusBoxPrefab;

	public float statusBoxTime = 3f;

	public float margins = 2f;

	private Dictionary<CollectableItem, StatusBox> statusBoxes = new Dictionary<CollectableItem, StatusBox>();

	private int prevScreenWidth;

	private void Start()
	{
		Singleton<GlobalData>.instance.gameData.WatchAllCollections(OnCollectionChanged);
		Singleton<TimerServiceLocator>.instance.timerManager.onTimerError += OnTimerError;
		ResizeUIRegion();
	}

	private void OnTimerError()
	{
		ShowStatusBox(CollectableItem.Load("Toast"), -1).HideAndKill(statusBoxTime);
	}

	private void OnDestroy()
	{
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.gameData.UnwatchAllCollections(OnCollectionChanged);
		}
	}

	private void ResizeUIRegion()
	{
		RectTransform obj = base.transform as RectTransform;
		RectTransform rectTransform = base.transform.parent as RectTransform;
		obj.sizeDelta = obj.sizeDelta.SetX(rectTransform.rect.height * UI.widthPerHeightRatio - rectTransform.rect.width);
	}

	private void Update()
	{
		if (prevScreenWidth != Screen.width)
		{
			ResizeUIRegion();
		}
		prevScreenWidth = Screen.width;
	}

	private void OnCollectionChanged(CollectableItem item, int amount, bool equipAction)
	{
		if (!equipAction)
		{
			ShowStatusBox(item, amount).HideAndKill(statusBoxTime);
		}
	}

	public StatusBox ShowStatusBox(CollectableItem item, int amount)
	{
		if (statusBoxes.ContainsKey(item) && statusBoxes[item] != null)
		{
			StatusBox statusBox = statusBoxes[item];
			statusBox.Show();
			statusBox.Setup(item.icon, amount.ToString());
			return statusBox;
		}
		GameObject obj = statusBoxPrefab.Clone();
		obj.transform.SetParent(base.transform, worldPositionStays: false);
		StatusBox component = obj.GetComponent<StatusBox>();
		component.Setup(item.icon, amount.ToString());
		float? num = statusBoxes.Values.Where((StatusBox b) => b != null).Min((Func<StatusBox, float?>)((StatusBox b) => b.rect.anchoredPosition.y - b.rect.sizeDelta.y));
		float num2 = (num.HasValue ? num.Value : 0f);
		num2 -= margins;
		component.rect.anchoredPosition = new Vector2(margins, num2);
		statusBoxes[item] = component;
		return component;
	}

	public StatusBox ShowCollection(CollectableItem item)
	{
		return ShowStatusBox(item, Singleton<GlobalData>.instance.gameData.GetCollected(item));
	}

	public void HideCollection(CollectableItem item)
	{
		if (statusBoxes.ContainsKey(item))
		{
			statusBoxes[item].HideAndKill();
		}
	}

	public static void SetText(MaskableGraphic graphic, string text)
	{
		Text text2 = graphic as Text;
		if ((bool)text2 && text2.text != text)
		{
			text2.text = text;
			return;
		}
		TMP_Text tMP_Text = graphic as TMP_Text;
		if ((bool)tMP_Text && tMP_Text.text != text)
		{
			tMP_Text.text = text;
		}
	}
}
