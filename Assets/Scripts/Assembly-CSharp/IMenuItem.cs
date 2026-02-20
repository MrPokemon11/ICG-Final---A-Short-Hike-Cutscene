using System;
using UnityEngine;

public interface IMenuItem
{
	bool enabled { get; set; }

	GameObject gameObject { get; }

	bool showArrow { get; }

	Vector2 localArrowPosition { get; }

	bool highlighted { get; }

	event Action<IMenuItem, bool> onSelectionChanged;

	void Highlight();

	void Unhighlight();

	void Confirm();
}
