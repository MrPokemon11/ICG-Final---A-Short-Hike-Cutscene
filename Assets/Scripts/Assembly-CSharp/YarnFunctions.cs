using System;
using System.Linq;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn;

public static class YarnFunctions
{
	[YarnFunction]
	public static bool IsUsingJoystick(IConversation context)
	{
		return !GameUserInput.sharedActionSet.LastInputType.IsMouseOrKeyboard();
	}

	[YarnFunction]
	public static string CurrentPlatform(IConversation context)
	{
		return "Default";
	}

	[YarnFunction]
	public static bool HasItem(IConversation context, Value itemName, Value amount)
	{
		CollectableItem item = CollectableItem.Load(itemName.AsString);
		return Singleton<GlobalData>.instance.gameData.GetCollected(item) >= (int)amount.AsNumber;
	}

	[YarnFunction]
	public static bool IsVisible(IConversation context, Value objectName)
	{
		GameObject gameObject = context.LookUpObject(objectName.AsString);
		if (!gameObject)
		{
			return false;
		}
		return Camera.main.IsPointInView(gameObject.transform.position);
	}

	[YarnFunction]
	public static bool IsEquipped(IConversation context, Value itemName)
	{
		CollectableItem collectableItem = CollectableItem.Load(itemName.AsString);
		Holdable heldItem = Singleton<GameServiceLocator>.instance.levelController.player.heldItem;
		if (heldItem != null)
		{
			return heldItem.associatedItem == collectableItem;
		}
		return false;
	}

	[YarnFunction]
	public static int GetHour(IConversation context)
	{
		try
		{
			return DateTime.Now.Hour;
		}
		catch (TimeZoneNotFoundException)
		{
			return 10;
		}
	}

	[YarnFunction]
	public static string GetSceneName(IConversation context)
	{
		return SceneManager.GetActiveScene().name;
	}

	[YarnFunction]
	public static bool HasSoldAllFish(IConversation context)
	{
		return FishSpecies.LoadAll().All((FishSpecies f) => Singleton<GlobalData>.instance.gameData.tags.GetBool(FishBuyer.GetFishSoldTag(f, rare: false)) || Singleton<GlobalData>.instance.gameData.tags.GetBool(FishBuyer.GetFishSoldTag(f, rare: true)));
	}

	[YarnFunction]
	public static int GetItemCount(IConversation context, Value itemName)
	{
		CollectableItem item = CollectableItem.Load(itemName.AsString);
		return Singleton<GlobalData>.instance.gameData.GetCollected(item);
	}

	[YarnFunction]
	public static float GetSpeakerY(IConversation context)
	{
		if (context == null)
		{
			Debug.LogError("The context is missing!?");
			return 0f;
		}
		if (context.currentSpeaker == null)
		{
			Debug.LogWarning("Called GetSpeakerY() when the speaker was null!");
			return 0f;
		}
		return context.currentSpeaker.position.y;
	}

	[YarnFunction]
	public static int GetNPCSpeakerNextNode(IConversation context)
	{
		if (context == null)
		{
			Debug.LogError("The context is missing!?");
			return 0;
		}
		if (context.currentSpeaker == null)
		{
			Debug.LogWarning("Called GetNPCSpeakerNextNode() when the speaker was null!");
			return 0;
		}
		PathNPCMovement component = context.currentSpeaker.GetComponent<PathNPCMovement>();
		if (!component)
		{
			Debug.LogWarning("Called GetNPCSpeakerNextNode() when the speaker is not an NPC!");
			return 0;
		}
		return component.nextNode;
	}
}
