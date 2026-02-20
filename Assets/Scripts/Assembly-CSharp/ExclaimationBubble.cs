using System;
using InControl;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;

public class ExclaimationBubble : MonoBehaviour
{
	public static bool USE_CUSTOM_ICONS = true;

	private static int SHOW_ID = Animator.StringToHash("Show");

	public Transform target;

	public float lerpSpeed = 10f;

	public Animator animator;

	public TMP_Text text;

	public GameObject icon;

	private string buttonName;

	private void Start()
	{
		base.transform.localPosition = GetDesinationPosition();
		UpdateBubbleIcon();
		GameUserInput.sharedActionSet.OnLastInputTypeChanged += OnInputTypeChanged;
		InputManager.OnDeviceAttached += OnDeviceChanged;
		InputManager.OnActiveDeviceChanged += OnDeviceChanged;
		InputMapperExtensions.onInteractButtonsSwapped += UpdateBubbleIcon;
		AdvancedControllerConfigurationMenu.onIconsUpdated = (Action)Delegate.Combine(AdvancedControllerConfigurationMenu.onIconsUpdated, new Action(UpdateBubbleIcon));
	}

	private void OnDestroy()
	{
		GameUserInput.sharedActionSet.OnLastInputTypeChanged -= OnInputTypeChanged;
		InputManager.OnDeviceAttached -= OnDeviceChanged;
		InputManager.OnActiveDeviceChanged -= OnDeviceChanged;
		InputMapperExtensions.onInteractButtonsSwapped -= UpdateBubbleIcon;
		AdvancedControllerConfigurationMenu.onIconsUpdated = (Action)Delegate.Remove(AdvancedControllerConfigurationMenu.onIconsUpdated, new Action(UpdateBubbleIcon));
	}

	private void OnInputTypeChanged(BindingSourceType inputType)
	{
		UpdateBubbleIcon();
	}

	private void OnDeviceChanged(InputDevice obj)
	{
		UpdateBubbleIcon();
	}

	private void UpdateBubbleIcon()
	{
		GameUserInput gameUserInput = Singleton<FocusableUserInputManager>.instance.inputWithFocus as GameUserInput;
		text.enabled = true;
		text.text = gameUserInput.WrapButtonNameForText(gameUserInput.GetInteractButton().name);
		text.ForceMeshUpdate();
		bool flag = text.GetParsedText().Length > 1 || !USE_CUSTOM_ICONS;
		icon.gameObject.SetActive(flag);
		text.enabled = !flag;
		bool flag2 = text.text.Contains("sprite");
		RectTransform obj = text.transform as RectTransform;
		obj.anchoredPosition = obj.anchoredPosition.SetX((!flag2) ? 1 : 0);
	}

	private void Update()
	{
		Vector2 desinationPosition = GetDesinationPosition();
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, desinationPosition, Time.deltaTime * lerpSpeed);
	}

	private Vector2 GetDesinationPosition()
	{
		RectTransform toTransform = base.transform.parent as RectTransform;
		return QuickUnityExtensions.WorldToRectTransform(target.transform.position, toTransform);
	}

	public void Show(bool show)
	{
		animator.SetBool(SHOW_ID, show);
	}
}
