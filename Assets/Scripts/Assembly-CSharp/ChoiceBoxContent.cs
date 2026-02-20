using System;
using System.Collections.Generic;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceBoxContent : MonoBehaviour, IFloatingBoxContent
{
	public GameObject choiceTemplate;

	public LinearMenu linearMenu;

	public Image arrow;

	private List<GameObject> menuItems;

	private Color textColor = Color.black;

	private Action<int> onSelect;

	public bool wasSelectionMade { get; private set; }

	public Vector2 extraFloatPadding => new Vector2(16f, 0f);

	GameObject IFloatingBoxContent.gameObject => base.gameObject;

	private void Start()
	{
		arrow.enabled = false;
		this.RegisterTimer(0.25f, delegate
		{
			arrow.enabled = true;
			linearMenu.UpdateArrowPosition();
		});
	}

	public void SetupChoices(IList<string> options, Action<int> onSelect)
	{
		Asserts.Null(menuItems);
		choiceTemplate.SetActive(value: true);
		this.onSelect = onSelect;
		menuItems = new List<GameObject>();
		foreach (string option in options)
		{
			GameObject gameObject = choiceTemplate.Clone();
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			TMP_Text component = gameObject.GetComponent<TMP_Text>();
			component.text = option;
			component.color = textColor;
			gameObject.GetComponent<BasicMenuItem>().onConfirm.AddListener(OnItemSelected);
			menuItems.Add(gameObject);
		}
		linearMenu.SetMenuObjects(menuItems);
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		linearMenu.isConfirmPressed = (FocusableUserInput input) => input.WasChooseDialoguePressed();
		choiceTemplate.SetActive(value: false);
	}

	private void OnItemSelected()
	{
		if (!wasSelectionMade)
		{
			onSelect(linearMenu.selectedIndex);
			wasSelectionMade = true;
		}
	}

	public void Configure(Transform target, TextBoxStyleProfile styleProfile)
	{
		if (styleProfile == null)
		{
			return;
		}
		foreach (GameObject menuItem in menuItems)
		{
			menuItem.GetComponent<TMP_Text>().color = textColor;
		}
		arrow.color = styleProfile.textColor;
		Shadow[] componentsInChildren = arrow.GetComponentsInChildren<Shadow>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].effectColor = styleProfile.boxColor;
		}
	}
}
