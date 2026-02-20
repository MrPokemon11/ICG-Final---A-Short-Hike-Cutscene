using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class CollectableItem : ScriptableObject
{
	public enum PickUpPrompt
	{
		Never = 0,
		OnlyOnce = 1,
		Always = 2
	}

	[Header("Apperance")]
	public string readableName;

	public string readableNamePlural;

	[Multiline]
	public string description;

	public Sprite icon;

	[Header("Item Behaviour")]
	public bool stacksInInventory = true;

	public PickUpPrompt showPrompt = PickUpPrompt.OnlyOnce;

	public bool cannotDrop;

	public bool cannotStash;

	public int priority;

	[Header("Usage Behaviour")]
	[FormerlySerializedAs("holdablePrefab")]
	public GameObject worldPrefab;

	public string yarnNodeTitle;

	public string yarnNode;

	public string saveTag => "ITEM_" + base.name;

	public bool hasShownPrompt
	{
		get
		{
			return Singleton<GlobalData>.instance.gameData.tags.GetBool("PROMPT_" + base.name);
		}
		set
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool("PROMPT_" + base.name, value);
		}
	}

	public IEnumerator PickUpRoutine(int amount = 1)
	{
		Singleton<GlobalData>.instance.gameData.AddCollected(this, amount);
		if ((showPrompt == PickUpPrompt.Always || (showPrompt == PickUpPrompt.OnlyOnce && !hasShownPrompt)) && amount > 0)
		{
			Player player = Singleton<GameServiceLocator>.instance.levelController.player;
			player.TurnToFace(Camera.main.transform);
			StackResourceSortingKey emotionKey = player.ikAnimator.ShowEmotion(Emotion.Happy);
			Action undoPose = player.ikAnimator.Pose(Pose.RaiseArms);
			hasShownPrompt = true;
			ItemPrompt prompt = Singleton<GameServiceLocator>.instance.ui.CreateItemPrompt(this);
			yield return new WaitUntil(() => prompt == null);
			emotionKey.ReleaseResource();
			undoPose();
		}
	}

	public static CollectableItem Load(string name)
	{
		return Resources.Load<CollectableItem>("Items/" + name);
	}
}
