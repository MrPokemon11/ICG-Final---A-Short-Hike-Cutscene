using System;
using QuickUnityTools.Input;
using UnityEngine;

public class ScrollMenuItem : MonoBehaviour
{
	public Vector2 scrollDirection = Vector2.right;

	public Action<int> onScroll;

	private GameUserInput input;

	private IMenuItem menuItem;

	private void Start()
	{
		input = GetComponentInParent<GameUserInput>();
		menuItem = GetComponent<IMenuItem>();
	}

	private void Update()
	{
		if (menuItem.highlighted)
		{
			if (input.leftStick.WasDirectionTapped(scrollDirection))
			{
				onScroll?.Invoke(1);
			}
			if (input.leftStick.WasDirectionTapped(-scrollDirection))
			{
				onScroll?.Invoke(-1);
			}
		}
	}
}
