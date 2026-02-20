using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : ServiceMonoBehaviour
{
	public Transform uiParent;

	public GameObject exclaimationBubblePrefab;

	public GameObject floatingBoxPrefab;

	public GameObject textBoxContentPrefab;

	public GameObject choiceContentPrefab;

	public GameObject itemPrompt;

	public GameObject simpleMenuPrefab;

	public GameObject simpleMenuItemPrefab;

	public GameObject scrollMenuItemPrefab;

	public GameObject textMenuItemPrefab;

	public GameObject fishCatchPrefab;

	public GameObject simpleDialoguePrefab;

	public GameObject loadingPrefab;

	public static float widthPerHeightRatio => Mathf.Min(1.7777778f, (float)Screen.width / (float)Screen.height);

	public LinearMenu CreateUndismissableSimpleMenu(string[] options, Action[] events)
	{
		LinearMenu linearMenu = CreateSimpleMenu(options, events);
		UnityEngine.Object.Destroy(linearMenu.GetComponent<KillOnBackButton>());
		return linearMenu;
	}

	public LinearMenu CreateSimpleMenu()
	{
		return CreateSimpleMenu(new string[0], new Action[0]);
	}

	public LinearMenu CreateSimpleMenu(string[] options, Action[] events)
	{
		return CreateSimpleMenu(options, ((IEnumerable<Action>)events).Select((Func<Action, Action<BasicMenuItem>>)((Action wrappedEvent) => delegate
		{
			wrappedEvent();
		})).ToArray());
	}

	public LinearMenu CreateSimpleMenu(string[] options, Action<BasicMenuItem>[] events)
	{
		GameObject gameObject = simpleMenuPrefab.Clone();
		AddUI(gameObject);
		LinearMenu component = gameObject.GetComponent<LinearMenu>();
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < options.Length; i++)
		{
			GameObject gameObject2 = simpleMenuItemPrefab.Clone();
			SetGenericText(gameObject2, options[i]);
			BasicMenuItem item = gameObject2.GetComponentInChildren<BasicMenuItem>();
			Action<BasicMenuItem> cachedEvent = events[i];
			item.onConfirm.AddListener(delegate
			{
				cachedEvent(item);
			});
			gameObject2.transform.SetParent(component.transform, worldPositionStays: false);
			list.Add(gameObject2);
		}
		component.SetMenuObjects(list);
		return component;
	}

	public GameObject CreateTextMenuItem(string text)
	{
		GameObject result = textMenuItemPrefab.Clone();
		SetGenericText(result, text);
		return result;
	}

	public GameObject CreateScrollMenuItem(string text, Action<BasicMenuItem> onConfirm, Action<int, ScrollMenuItem> onScroll)
	{
		GameObject gameObject = scrollMenuItemPrefab.Clone();
		SetGenericText(gameObject, text);
		BasicMenuItem menuItem = gameObject.GetComponent<BasicMenuItem>();
		menuItem.onConfirm.AddListener(delegate
		{
			onConfirm(menuItem);
		});
		ScrollMenuItem scroll = gameObject.GetComponent<ScrollMenuItem>();
		ScrollMenuItem scrollMenuItem = scroll;
		scrollMenuItem.onScroll = (Action<int>)Delegate.Combine(scrollMenuItem.onScroll, (Action<int>)delegate(int scrollValue)
		{
			onScroll(scrollValue, scroll);
		});
		return gameObject;
	}

	public static void SetGenericText(GameObject gameObject, string text)
	{
		TMP_Text componentInChildren = gameObject.GetComponentInChildren<TMP_Text>();
		if ((bool)componentInChildren)
		{
			componentInChildren.text = text;
			return;
		}
		Text componentInChildren2 = gameObject.GetComponentInChildren<Text>();
		if ((bool)componentInChildren2)
		{
			componentInChildren2.text = text;
		}
	}

	public ExclaimationBubble AddExclaimationBubble()
	{
		GameObject gameObject = exclaimationBubblePrefab.Clone();
		AddUI(gameObject);
		return gameObject.GetComponent<ExclaimationBubble>();
	}

	public ChoiceBoxContent CreateChoiceBoxContent(IList<string> options, Action<int> onSelect)
	{
		GameObject gameObject = choiceContentPrefab.Clone();
		AddUI(gameObject);
		gameObject.GetComponent<ChoiceBoxContent>().SetupChoices(options, onSelect);
		return gameObject.GetComponent<ChoiceBoxContent>();
	}

	public FloatingBox CreateFloatingBox()
	{
		GameObject gameObject = floatingBoxPrefab.Clone();
		AddUI(gameObject, addToStatusBar: false);
		return gameObject.GetComponent<FloatingBox>();
	}

	public TextBoxContent CreateTextBoxContent(string text)
	{
		GameObject gameObject = textBoxContentPrefab.Clone();
		AddUI(gameObject);
		gameObject.GetComponent<TextBoxContent>().Reset(text);
		return gameObject.GetComponent<TextBoxContent>();
	}

	public GameObject CreateSimpleDialogue(string text)
	{
		GameObject result = AddUI(simpleDialoguePrefab.Clone());
		SetGenericText(result, text);
		return result;
	}

	public GameObject CreateLoadingPrompt()
	{
		return AddUI(loadingPrefab.Clone());
	}

	public FishCollectPrompt CreateFishCatchPrompt(Fish collectable)
	{
		GameObject gameObject = fishCatchPrefab.Clone();
		AddUI(gameObject);
		FishCollectPrompt component = gameObject.GetComponent<FishCollectPrompt>();
		component.Setup(collectable);
		return component;
	}

	public ItemPrompt CreateItemPrompt(CollectableItem collectable)
	{
		GameObject gameObject = itemPrompt.Clone();
		AddUI(gameObject);
		ItemPrompt component = gameObject.GetComponent<ItemPrompt>();
		component.Setup(collectable);
		return component;
	}

	public GameObject AddUI(GameObject gameObject, bool addToStatusBar = true)
	{
		Transform transform = (addToStatusBar ? uiParent : base.transform);
		if (transform == null)
		{
			transform = base.transform;
		}
		gameObject.transform.SetParent(transform, worldPositionStays: false);
		return gameObject;
	}

	public void ForceVisibleIfNecessary()
	{
		EndGameCutscene endGameCutscene = UnityEngine.Object.FindObjectOfType<EndGameCutscene>();
		if (endGameCutscene != null && endGameCutscene.isRunning)
		{
			GetComponent<Canvas>().sortingOrder = 1000;
		}
	}

	internal void CreateSimpleDialogue(object resetControllerBindings, string v)
	{
		throw new NotImplementedException();
	}
}
