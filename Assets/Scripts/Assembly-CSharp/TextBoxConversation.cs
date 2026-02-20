using System;
using System.Collections;
using System.Collections.Generic;
using QuickUnityTools.Input;
using UnityEngine;

public class TextBoxConversation : IConversation
{
	private GameUserInput conversationInput;

	private FloatingBox floatingBox;

	private bool alive = true;

	private Dictionary<string, GameObject> objectCache = new Dictionary<string, GameObject>();

	public bool isAlive => alive;

	public Transform currentSpeaker { get; set; }

	public Transform originalSpeaker { get; private set; }

	public Dictionary<GameObject, StackResourceSortingKey> emotionKeys { get; private set; }

	public Dictionary<GameObject, Action> poseActions { get; private set; }

	public ScriptedMusic scriptedMusic { get; set; }

	public event Action onConversationFinish;

	public TextBoxConversation(Transform speaker)
	{
		originalSpeaker = speaker;
		currentSpeaker = speaker;
		conversationInput = GameUserInput.CreateInput(new GameObject("Conversation Focus Holder"));
		emotionKeys = new Dictionary<GameObject, StackResourceSortingKey>();
		poseActions = new Dictionary<GameObject, Action>();
	}

	public IEnumerator ShowLine(string line)
	{
		if (!alive)
		{
			Debug.LogError("Sending dialogue to a dead conversation!");
			yield break;
		}
		ConfigureFloatingBox();
		TextBoxContent textContent = Singleton<ServiceLocator>.instance.Locate<UI>().CreateTextBoxContent(line);
		floatingBox.SetContent(textContent);
		while (!textContent.isFinishedAnimating)
		{
			if (IsAdvanceDialoguePressed())
			{
				textContent.SkipTextAnimation();
			}
			yield return null;
		}
		yield return new WaitUntil(IsAdvanceDialoguePressed);
	}

	public IEnumerator ShowOptions(IList<string> options, Action<int> onChoose)
	{
		if (!alive)
		{
			Debug.LogError("Sending choices to a dead conversation!");
			yield break;
		}
		ConfigureFloatingBox();
		ChoiceBoxContent choices = Singleton<GameServiceLocator>.instance.ui.CreateChoiceBoxContent(options, onChoose);
		floatingBox.SetContent(choices);
		yield return new WaitUntil(() => choices.wasSelectionMade);
	}

	private void ConfigureFloatingBox()
	{
		if (!(floatingBox == null) && !(floatingBox.target != currentSpeaker))
		{
			return;
		}
		float num = 0f;
		if (floatingBox != null)
		{
			if (floatingBox.target != null && currentSpeaker != null)
			{
				Vector3 position = floatingBox.target.transform.position;
				Vector3 position2 = currentSpeaker.transform.position;
				RectTransform toTransform = floatingBox.rectTransform.parent as RectTransform;
				Vector2 vector = QuickUnityExtensions.WorldToRectTransform(position, toTransform);
				num = Mathf.Sign(QuickUnityExtensions.WorldToRectTransform(position2, toTransform).x - vector.x);
			}
			floatingBox.Kill();
		}
		floatingBox = Singleton<ServiceLocator>.instance.Locate<UI>().CreateFloatingBox();
		floatingBox.target = currentSpeaker;
		floatingBox.desiredPositionNormalizedXOffset = num * 0.35f;
	}

	public void Hide()
	{
		if (floatingBox != null)
		{
			floatingBox.Kill();
			floatingBox = null;
		}
	}

	public bool IsAdvanceDialoguePressed()
	{
		return conversationInput.WasAdvanceDialoguePressed();
	}

	public void Kill()
	{
		if (!alive)
		{
			return;
		}
		alive = false;
		if (floatingBox != null)
		{
			floatingBox.Kill();
		}
		foreach (StackResourceSortingKey value in emotionKeys.Values)
		{
			value.ReleaseResource();
		}
		foreach (Action value2 in poseActions.Values)
		{
			value2();
		}
		if (scriptedMusic != null)
		{
			scriptedMusic.Stop();
		}
		this.onConversationFinish?.Invoke();
		UnityEngine.Object.Destroy(conversationInput.gameObject);
	}

	public GameObject LookUpObject(string name)
	{
		if (objectCache.ContainsKey(name) && objectCache[name] != null)
		{
			return objectCache[name];
		}
		GameObject gameObject = GameObject.Find(name);
		objectCache[name] = gameObject;
		return gameObject;
	}
}
