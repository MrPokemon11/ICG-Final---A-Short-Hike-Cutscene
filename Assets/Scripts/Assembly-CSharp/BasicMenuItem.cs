using System;
using UnityEngine;
using UnityEngine.Events;

public class BasicMenuItem : MonoBehaviour, IMenuItem
{
	public Animator animator;

	public string selectedBoolParameter = "Selected";

	public UnityEvent onConfirm;

	public Vector2 normalizedArrowPos = new Vector2(-1f, 0f);

	private bool isHighlighted;

	private bool isDisabled;

	public bool highlighted => isHighlighted;

	public bool showArrow => true;

	public Vector2 localArrowPosition
	{
		get
		{
			RectTransform rectTransform = base.transform as RectTransform;
			return rectTransform.rect.center + rectTransform.rect.size / 2f * normalizedArrowPos;
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

	public event Action<IMenuItem, bool> onSelectionChanged;

	private void OnEnable()
	{
		animator.SetBool("Enabled", base.enabled && !isDisabled);
	}

	private void OnDisable()
	{
		animator.SetBool("Enabled", base.enabled && !isDisabled);
	}

	public void Highlight()
	{
		isHighlighted = true;
		animator.SetBool(selectedBoolParameter, value: true);
		this.onSelectionChanged?.Invoke(this, arg2: true);
	}

	public void Unhighlight()
	{
		isHighlighted = false;
		animator.SetBool(selectedBoolParameter, value: false);
		this.onSelectionChanged?.Invoke(this, arg2: false);
	}

	public virtual void Confirm()
	{
		onConfirm.Invoke();
	}

	public void SetDisabledStyle(bool isDisabled)
	{
		this.isDisabled = isDisabled;
		animator.SetBool("Enabled", base.enabled && !isDisabled);
	}
}
