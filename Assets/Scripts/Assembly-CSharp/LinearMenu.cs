using System;
using System.Collections.Generic;
using UnityEngine;

public class LinearMenu : AbstractMenu
{
	public Vector2 menuDirection = Vector2.down;

	public bool enableFastScroll;

	public float fastScrollActivateTime = 0.5f;

	public float fastScrollStepTime = 0.1f;

	[SerializeField]
	private List<GameObject> menuGameObjects;

	private int _selectedIndex;

	private float fastScrollHeldTime;

	private float prevFastScrollDirection;

	public int selectedIndex
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
			this.onSelectionChanged?.Invoke();
		}
	}

	public override IMenuItem indexedMenuItem => GetMenuItem(selectedIndex);

	public event Action onSelectionChanged;

	public List<GameObject> GetMenuObjects()
	{
		return menuGameObjects;
	}

	public void SetMenuObjects(List<GameObject> menuObjs)
	{
		menuGameObjects = menuObjs;
		selectedIndex = ((menuGameObjects.Count != 0) ? (selectedIndex % menuGameObjects.Count) : 0);
	}

	protected IMenuItem GetMenuItem(int index)
	{
		if (index >= menuGameObjects.Count)
		{
			return null;
		}
		return menuGameObjects[index].GetComponent<IMenuItem>();
	}

	protected override void Update()
	{
		base.Update();
		if (isFocused && menuGameObjects.Count > 0)
		{
			float f = Vector2.Dot(menuDirection, userInput.leftStick.vector);
			int num = ((Mathf.Abs(f) > 0.5f) ? ((int)Mathf.Sign(f)) : 0);
			if ((float)num == prevFastScrollDirection && num != 0)
			{
				fastScrollHeldTime += Time.deltaTime;
			}
			else
			{
				fastScrollHeldTime = 0f;
			}
			prevFastScrollDirection = num;
			int num2 = 0;
			if (fastScrollHeldTime > fastScrollActivateTime)
			{
				num2 = num;
				fastScrollHeldTime = fastScrollActivateTime - fastScrollStepTime;
			}
			if (userInput.leftStick.WasDirectionTapped(menuDirection) || num2 == 1)
			{
				selectedIndex = (selectedIndex + 1).Mod(menuGameObjects.Count);
				moveSound.Play();
			}
			else if (userInput.leftStick.WasDirectionTapped(-menuDirection) || num2 == -1)
			{
				selectedIndex = (selectedIndex - 1).Mod(menuGameObjects.Count);
				moveSound.Play();
			}
		}
	}
}
