using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConversation
{
	Transform currentSpeaker { get; set; }

	Transform originalSpeaker { get; }

	bool isAlive { get; }

	Dictionary<GameObject, StackResourceSortingKey> emotionKeys { get; }

	Dictionary<GameObject, Action> poseActions { get; }

	ScriptedMusic scriptedMusic { get; set; }

	event Action onConversationFinish;

	IEnumerator ShowLine(string line);

	void Kill();

	void Hide();

	IEnumerator ShowOptions(IList<string> options, Action<int> onChoose);

	GameObject LookUpObject(string name);
}
