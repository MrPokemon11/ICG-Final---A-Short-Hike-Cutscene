using System;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractMenu : MonoBehaviour, IKillable
{
	public FocusableUserInput userInput;

	public RectTransform menuArrow;

	public AudioClip moveSound;

	public AudioClip confirmSound;

	private bool prevIsFocused;

	private bool updateArrowLater;

	public Func<FocusableUserInput, bool> isConfirmPressed { get; set; }

	public abstract IMenuItem indexedMenuItem { get; }

	protected virtual bool isFocused => userInput.hasFocus;

	GameObject IKillable.gameObject => base.gameObject;

	public event Action onKill;

	protected virtual void Awake()
	{
		isConfirmPressed = (FocusableUserInput input) => input.GetConfirmButton().ConsumePress();
		if (moveSound == null)
		{
			moveSound = Resources.Load<AudioClip>("MenuMove");
		}
	}

	protected virtual void OnEnable()
	{
		UpdateIndexSelectionApperance(null, indexedMenuItem);
	}

	public void Kill()
	{
		this.onKill?.Invoke();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void UnfocusArrowHackily()
	{
		UpdateIndexSelectionApperance(null, null);
	}

	protected virtual void UpdateIndexSelectionApperance(IMenuItem previous, IMenuItem next)
	{
		if (!isFocused)
		{
			previous?.Unhighlight();
			next?.Unhighlight();
			if (menuArrow != null)
			{
				menuArrow.gameObject.SetActive(value: false);
			}
			return;
		}
		previous?.Unhighlight();
		next?.Highlight();
		if ((bool)menuArrow)
		{
			menuArrow.gameObject.SetActive(value: false);
			updateArrowLater = true;
		}
	}

	public void UpdateArrowPosition()
	{
		if ((bool)menuArrow && isFocused && indexedMenuItem != null && indexedMenuItem.showArrow)
		{
			menuArrow.gameObject.SetActive(value: true);
			RectTransform componentInChildren = indexedMenuItem.gameObject.GetComponentInChildren<RectTransform>();
			Vector2 localArrowPosition = indexedMenuItem.localArrowPosition;
			Vector3 localPosition = componentInChildren.TransformPointTo(localArrowPosition, menuArrow.parent as RectTransform);
			menuArrow.localPosition = localPosition;
			menuArrow.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(localArrowPosition.y, localArrowPosition.x) * 57.29578f + 90f);
		}
	}

	protected virtual void Update()
	{
		if (isFocused && isConfirmPressed(userInput) && indexedMenuItem != null)
		{
			indexedMenuItem.Confirm();
			if (confirmSound != null)
			{
				confirmSound.Play();
			}
		}
	}

	private void LateUpdate()
	{
		if (isFocused != prevIsFocused)
		{
			UpdateIndexSelectionApperance(null, indexedMenuItem);
		}
		if (updateArrowLater)
		{
			AttemptCanvasRebuild();
			UpdateArrowPosition();
			updateArrowLater = false;
		}
		prevIsFocused = isFocused;
	}

	public void AttemptCanvasRebuild()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		HorizontalOrVerticalLayoutGroup componentInChildren = GetComponentInChildren<HorizontalOrVerticalLayoutGroup>();
		if ((bool)componentInChildren)
		{
			componentInChildren.CalculateLayoutInputHorizontal();
			componentInChildren.CalculateLayoutInputVertical();
			componentInChildren.SetLayoutHorizontal();
			componentInChildren.SetLayoutVertical();
		}
	}
}
