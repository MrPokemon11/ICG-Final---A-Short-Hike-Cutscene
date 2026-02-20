using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectionListUI : MonoBehaviour
{
	public GameObject elementPrefab;

	public Transform elementParent;

	public float edgeOffset = 8f;

	private LinearMenu menu;

	private List<GameObject> elements = new List<GameObject>();

	public void Awake()
	{
		menu = GetComponent<LinearMenu>();
		menu.onSelectionChanged += OnSelectionChanged;
	}

	public void Setup<T>(IEnumerable<T> data, Action<T, CollectionListUIElement> setupElement)
	{
		GameObject[] array = elements.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
		elements.Clear();
		foreach (T datum in data)
		{
			GameObject gameObject = elementPrefab.Clone();
			gameObject.transform.SetParent(elementParent, worldPositionStays: false);
			setupElement(datum, gameObject.GetComponent<CollectionListUIElement>());
			elements.Add(gameObject);
		}
		menu.SetMenuObjects(elements);
	}

	public CollectionListUIElement GetElement(int i)
	{
		return elements[i].GetComponent<CollectionListUIElement>();
	}

	public void RemoveElement(GameObject element)
	{
		if (elements.Contains(element))
		{
			elements.Remove(element);
			UnityEngine.Object.Destroy(element);
			menu.SetMenuObjects(elements);
			if (elements.Count == 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnSelectionChanged()
	{
		menu.AttemptCanvasRebuild();
		List<GameObject> menuObjects = menu.GetMenuObjects();
		if (menuObjects.Count != 0)
		{
			RectTransform rectTransform = menuObjects[menu.selectedIndex].transform as RectTransform;
			RectTransform rectTransform2 = elementParent.parent as RectTransform;
			float num = elementParent.transform.localPosition.y + rectTransform.localPosition.y;
			if (num + rectTransform.rect.min.y < rectTransform2.rect.min.y)
			{
				float offset = rectTransform2.rect.min.y + edgeOffset - (num + rectTransform.rect.min.y);
				ScrollWindow(offset);
			}
			else if (num + rectTransform.rect.max.y > rectTransform2.rect.max.y)
			{
				float offset2 = rectTransform2.rect.max.y - edgeOffset - (num + rectTransform.rect.max.y);
				ScrollWindow(offset2);
			}
		}
	}

	private void ScrollWindow(float offset)
	{
		elementParent.transform.localPosition += Vector3.up * offset;
		menu.UpdateArrowPosition();
	}
}
