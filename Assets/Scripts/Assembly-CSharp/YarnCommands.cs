using System;
using System.Collections;
using System.Globalization;
using QuickUnityTools.Audio;
using UnityEngine;

public static class YarnCommands
{
	[YarnCommand]
	public static IEnumerator SetSpeaker(IConversation conversation, string[] args)
	{
		if (args.Length == 0)
		{
			Debug.LogError("A SetSpeaker target required!");
			yield break;
		}
		GameObject character = GetCharacter(conversation, args[0]);
		if (character != null)
		{
			conversation.currentSpeaker = character.transform;
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator SetEmotion(IConversation conversation, string[] args)
	{
		Emotion result;
		if (args.Length == 0)
		{
			Debug.LogError("A SetSpeaker target required!");
		}
		else if (Enum.TryParse<Emotion>(args[0], ignoreCase: true, out result))
		{
			GameObject gameObject = ((args.Length > 1) ? GetCharacter(conversation, args[1]) : conversation.currentSpeaker.gameObject);
			if (conversation.emotionKeys.ContainsKey(gameObject))
			{
				conversation.emotionKeys[gameObject].ReleaseResource();
			}
			IEmotionAnimator componentInChildren = gameObject.GetComponentInChildren<IEmotionAnimator>();
			conversation.emotionKeys[gameObject] = componentInChildren.ShowEmotion(result);
			yield return null;
		}
	}

	[YarnCommand]
	public static IEnumerator ClearEmotion(IConversation conversation, string[] args)
	{
		GameObject key = ((args.Length != 0) ? GetCharacter(conversation, args[0]) : conversation.currentSpeaker.gameObject);
		if (conversation.emotionKeys.ContainsKey(key))
		{
			conversation.emotionKeys[key].ReleaseResource();
			conversation.emotionKeys.Remove(key);
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator SetPose(IConversation conversation, string[] args)
	{
		Pose result;
		if (args.Length == 0)
		{
			Debug.LogError("A SetSpeaker target required!");
		}
		else if (Enum.TryParse<Pose>(args[0], ignoreCase: true, out result))
		{
			GameObject gameObject = ((args.Length > 1) ? GetCharacter(conversation, args[1]) : conversation.currentSpeaker.gameObject);
			if (conversation.poseActions.ContainsKey(gameObject))
			{
				conversation.poseActions[gameObject]?.Invoke();
			}
			IPoseAnimator componentInChildren = gameObject.GetComponentInChildren<IPoseAnimator>();
			conversation.poseActions[gameObject] = componentInChildren.Pose(result);
		}
		yield break;
	}

	[YarnCommand]
	public static IEnumerator ClearPose(IConversation conversation, string[] args)
	{
		GameObject key = ((args.Length != 0) ? GetCharacter(conversation, args[0]) : conversation.currentSpeaker.gameObject);
		if (conversation.poseActions.ContainsKey(key))
		{
			conversation.poseActions[key]?.Invoke();
			conversation.poseActions.Remove(key);
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator SetStyle(IConversation conversation, string[] args)
	{
		if (args.Length == 0)
		{
			Debug.LogError("A profile is requred!");
			yield break;
		}
		GameObject target = new GameObject("ProfileHolder");
		target.transform.position = conversation.currentSpeaker.position;
		TextBoxSpeaker textBoxSpeaker = target.AddComponent<TextBoxSpeaker>();
		textBoxSpeaker.textBoxStyle = TextBoxStyleProfile.Load(args[0]);
		conversation.onConversationFinish += delegate
		{
			UnityEngine.Object.Destroy(target);
		};
		conversation.currentSpeaker = textBoxSpeaker.transform;
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator LookAt(IConversation conversation, string[] args)
	{
		if (args.Length == 0)
		{
			Debug.LogError("A look at target required!");
		}
		GameObject character = GetCharacter(conversation, args[0]);
		GameObject gameObject = ((args.Length > 1) ? GetCharacter(conversation, args[1]) : conversation.currentSpeaker.gameObject);
		if ((bool)character && (bool)gameObject)
		{
			ICanLook componentInChildren = gameObject.GetComponentInChildren<ICanLook>();
			if (componentInChildren != null)
			{
				componentInChildren.lookAt = character.transform;
				yield break;
			}
		}
		Debug.LogError("There was an error or the look at is not supported yet.");
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator ClearLookAt(IConversation conversation, string[] args)
	{
		GameObject gameObject = ((args.Length != 0) ? GetCharacter(conversation, args[0]) : conversation.currentSpeaker.gameObject);
		if ((bool)gameObject)
		{
			ICanLook componentInChildren = gameObject.GetComponentInChildren<ICanLook>();
			if (componentInChildren != null)
			{
				componentInChildren.lookAt = null;
				yield break;
			}
		}
		Debug.LogError("There was an error or the look at is not supported yet.");
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator Face(IConversation conversation, string[] args)
	{
		if (args.Length == 0)
		{
			Debug.LogError("A Face target required!");
		}
		GameObject character = GetCharacter(conversation, args[0]);
		GameObject gameObject = ((args.Length > 1) ? GetCharacter(conversation, args[1]) : conversation.currentSpeaker.gameObject);
		if ((bool)character && (bool)gameObject)
		{
			ICanFace component = gameObject.GetComponent<ICanFace>();
			if (component != null)
			{
				component.TurnToFace(character.transform);
				yield break;
			}
		}
		Debug.LogError("There was an error or the facer is not supported yet.");
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator FaceDefault(IConversation conversation, string[] args)
	{
		GameObject gameObject = ((args.Length != 0) ? GetCharacter(conversation, args[0]) : conversation.currentSpeaker.gameObject);
		if ((bool)gameObject)
		{
			ICanFace component = gameObject.GetComponent<ICanFace>();
			if (component != null)
			{
				component.FaceDefault();
				yield break;
			}
		}
		Debug.LogError("There was an error or the facer is not supported yet.");
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator ShowCollection(IConversation context, string[] args)
	{
		CollectableItem item = CollectableItem.Load(args[0]);
		StatusBarUI statusBar = Singleton<GameServiceLocator>.instance.levelUI.statusBar;
		if (statusBar != null)
		{
			statusBar.ShowCollection(item);
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator HideCollection(IConversation context, string[] args)
	{
		CollectableItem item = CollectableItem.Load(args[0]);
		StatusBarUI statusBar = Singleton<GameServiceLocator>.instance.levelUI.statusBar;
		if (statusBar != null)
		{
			statusBar.HideCollection(item);
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator GiveAchievement(IConversation context, string[] args)
	{
		if (Enum.TryParse<Achievement>(args[0], ignoreCase: true, out var result))
		{
			Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(result);
			yield return null;
		}
	}

	[YarnCommand]
	public static IEnumerator FillItem(IConversation context, string[] args)
	{
		CollectableItem collectableItem = CollectableItem.Load(args[0]);
		int num = ((args.Length <= 1) ? 1 : int.Parse(args[1], CultureInfo.InvariantCulture));
		num -= Singleton<GlobalData>.instance.gameData.GetCollected(collectableItem);
		if (args.Length < 3 || bool.Parse(args[2]))
		{
			yield return collectableItem.PickUpRoutine(num);
		}
		else
		{
			Singleton<GlobalData>.instance.gameData.AddCollected(collectableItem, num);
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator GiveItem(IConversation context, string[] args)
	{
		CollectableItem collectableItem = CollectableItem.Load(args[0]);
		int result = 1;
		if (args.Length > 1 && !int.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			result = (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat(args[1]);
		}
		if (args.Length < 3 || bool.Parse(args[2]))
		{
			yield return collectableItem.PickUpRoutine(result);
		}
		else
		{
			Singleton<GlobalData>.instance.gameData.AddCollected(collectableItem, result);
		}
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator PlaySound(IConversation context, string[] args)
	{
		Singleton<SoundPlayer>.instance.Play(Resources.Load<AudioClip>(args[0]));
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator Wait(IConversation context, string[] args)
	{
		float seconds = ((args.Length != 0) ? float.Parse(args[0], CultureInfo.InvariantCulture) : 1f);
		yield return new WaitForSeconds(seconds);
	}

	[YarnCommand]
	public static IEnumerator Hide(IConversation context, string[] args)
	{
		context.Hide();
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator PlayMusic(IConversation context, string[] args)
	{
		if (context.scriptedMusic != null)
		{
			context.scriptedMusic.Stop();
		}
		context.scriptedMusic = new ScriptedMusic(MusicSet.Load(args[0]));
		context.scriptedMusic.Play();
		yield return null;
	}

	[YarnCommand]
	public static IEnumerator SendMessage(IConversation context, string[] args)
	{
		if (args.Length < 2)
		{
			Debug.LogError("Not enough arguments to send message!");
			yield break;
		}
		GameObject character = GetCharacter(context, args[0]);
		if (!character)
		{
			Debug.LogError("Could not find object: " + args[0]);
		}
		else
		{
			character.SendMessage(args[1]);
		}
	}

	[YarnCommand]
	public static IEnumerator SendMessageCoroutine(IConversation context, string[] args)
	{
		if (args.Length < 2)
		{
			Debug.LogError("Not enough arguments to send message!");
			yield break;
		}
		GameObject character = GetCharacter(context, args[0]);
		if (!character)
		{
			Debug.LogError("Could not find object: " + args[0]);
			yield break;
		}
		MonoBehaviour[] components = character.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if (monoBehaviour.GetType().GetMethod(args[1]) != null)
			{
				yield return monoBehaviour.StartCoroutine(args[1]);
			}
		}
	}

	private static GameObject GetCharacter(IConversation conversation, string name)
	{
		if (!(name == "Player"))
		{
			if (name == "Original")
			{
				return conversation.originalSpeaker.gameObject;
			}
			GameObject gameObject = conversation.LookUpObject(name);
			if ((bool)gameObject)
			{
				return gameObject;
			}
			Debug.LogError("Don't know who is speaker: " + name);
			return null;
		}
		Player player = Singleton<ServiceLocator>.instance.Locate<LevelController>().player;
		if (!(player != null))
		{
			return null;
		}
		return player.gameObject;
	}
}
