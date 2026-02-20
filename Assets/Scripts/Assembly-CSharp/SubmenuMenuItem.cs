using System;
using UnityEngine;

public class SubmenuMenuItem : LinearMenu, IMenuItem
{
	private bool isHighlighted;

	public bool showArrow => false;

	public Vector2 localArrowPosition => Vector2.zero;

	public bool highlighted => isHighlighted;

	protected override bool isFocused
	{
		get
		{
			if (base.isFocused)
			{
				return isHighlighted;
			}
			return false;
		}
	}

	bool IMenuItem.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	GameObject IMenuItem.gameObject => base.gameObject;

	public new event Action<IMenuItem, bool> onSelectionChanged;

	public void Confirm()
	{
		indexedMenuItem?.Confirm();
	}

	public void Unhighlight()
	{
		isHighlighted = false;
		this.onSelectionChanged?.Invoke(this, arg2: false);
	}

	public void Highlight()
	{
		isHighlighted = true;
		this.onSelectionChanged?.Invoke(this, arg2: true);
	}
}
