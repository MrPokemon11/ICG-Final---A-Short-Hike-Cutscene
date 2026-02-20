using System;
using UnityEngine;

public class FishingPermitEvent : MonoBehaviour
{
	public string requiredTag = "MissingPermit";

	public string dialogueNode = "FoundFishPermit";

	public string caughtFishTag = "PermitFishCount";

	public string doneEvent = "DonePermitFishEvent";

	public int requiredFishCaught = 3;

	private void Start()
	{
		if (!Singleton<GlobalData>.instance.gameData.tags.GetBool(doneEvent))
		{
			Fish.onFishCaught = (Action<Fish>)Delegate.Combine(Fish.onFishCaught, new Action<Fish>(OnFishCaught));
		}
	}

	private void OnDestroy()
	{
		Fish.onFishCaught = (Action<Fish>)Delegate.Remove(Fish.onFishCaught, new Action<Fish>(OnFishCaught));
	}

	private void OnFishCaught(Fish fish)
	{
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		int num = tags.GetInt(caughtFishTag);
		num++;
		tags.SetInt(caughtFishTag, num);
		if (num >= requiredFishCaught && tags.GetBool(requiredTag))
		{
			tags.SetBool(requiredTag, value: false);
			Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(dialogueNode, Singleton<GameServiceLocator>.instance.levelController.player.transform);
			Singleton<GlobalData>.instance.gameData.tags.SetBool(doneEvent);
			Fish.onFishCaught = (Action<Fish>)Delegate.Remove(Fish.onFishCaught, new Action<Fish>(OnFishCaught));
		}
	}
}
