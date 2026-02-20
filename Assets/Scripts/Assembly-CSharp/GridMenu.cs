using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMenu : AbstractMenu
{
	[Serializable]
	public struct Row
	{
		public List<GameObject> items;
	}

	public Row[] startMenuItemGrid = new Row[2];

	private Vector2Int _selectedIndex;

	private List<List<GameObject>> menuItemGrid;

	public override IMenuItem indexedMenuItem => indexedGameObject?.GetComponent<IMenuItem>();

	private GameObject indexedGameObject
	{
		get
		{
			if (menuItemGrid.Count == 0 || menuItemGrid[selectedIndex.y].Count == 0)
			{
				return null;
			}
			return menuItemGrid[selectedIndex.y][selectedIndex.x];
		}
	}

	private Vector2Int selectedIndex
	{
		get
		{
			return _selectedIndex;
		}
		set
		{
			IMenuItem previous = indexedMenuItem;
			_selectedIndex = value;
			UpdateIndexSelectionApperance(previous, indexedMenuItem);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		menuItemGrid = startMenuItemGrid.Select((Row row) => row.items).ToList();
	}

	public void SetMenuItem(int x, int y, GameObject newItem)
	{
		GameObject gameObject = menuItemGrid[y][x];
		IMenuItem previous = indexedMenuItem;
		menuItemGrid[y][x] = newItem;
		if (gameObject == gameObject.gameObject)
		{
			UpdateIndexSelectionApperance(previous, newItem?.GetComponent<IMenuItem>());
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!isFocused || menuItemGrid.Count == 0 || menuItemGrid[selectedIndex.y].Count == 0)
		{
			return;
		}
		EnsureValidIndex();
		Vector2Int[] mAIN_DIRECTIONS = Vector2Int.MAIN_DIRECTIONS;
		for (int i = 0; i < mAIN_DIRECTIONS.Length; i++)
		{
			Vector2Int vector2Int = mAIN_DIRECTIONS[i];
			if (userInput.leftStick.WasDirectionTapped(vector2Int.ToV2()))
			{
				Vector2Int vector2Int2 = new Vector2Int((selectedIndex.x - vector2Int.x).Mod(menuItemGrid[selectedIndex.y].Count), (selectedIndex.y - vector2Int.y).Mod(menuItemGrid.Count));
				selectedIndex = vector2Int2;
				EnsureValidIndex();
				moveSound.Play();
				break;
			}
		}
	}

	public void EnsureValidIndex()
	{
		if (IsValidItem(indexedGameObject))
		{
			return;
		}
		List<GameObject> list = menuItemGrid[selectedIndex.y];
		int x = selectedIndex.x;
		for (int i = 1; i <= Math.Max(x, list.Count - x - 1); i++)
		{
			if (x + i < list.Count && IsValidItem(list[x + i]))
			{
				selectedIndex = selectedIndex.SetX(x + 1);
				break;
			}
			if (x - i >= 0 && IsValidItem(list[x - i]))
			{
				selectedIndex = selectedIndex.SetX(x - 1);
				break;
			}
		}
	}

	public bool IsValidItem(GameObject item)
	{
		if (item != null)
		{
			return item.GetComponent<IMenuItem>()?.enabled ?? false;
		}
		return false;
	}
}
