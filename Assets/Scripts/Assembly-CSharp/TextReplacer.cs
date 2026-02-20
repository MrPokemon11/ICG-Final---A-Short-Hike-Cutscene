using System;
using System.Text.RegularExpressions;
using QuickUnityTools.Input;
using UnityEngine;

public class TextReplacer : MonoBehaviour
{
	public const int CHARACTERS_IN_LINE = 25;

	private static string ReplaceCapturedVariable(string capture)
	{
		if (capture.StartsWith("Item_"))
		{
			string text = capture.Remove(0, 5);
			return Singleton<GlobalData>.instance.gameData.GetCollected(CollectableItem.Load(text)).ToString();
		}
		foreach (InputMapperExtensions.Button value in Enum.GetValues(typeof(InputMapperExtensions.Button)))
		{
			if (capture == value.ToString())
			{
				GameUserInput obj = Singleton<FocusableUserInputManager>.instance.inputWithFocus as GameUserInput;
				return obj.WrapButtonNameForText(obj.GetActionButton(value).name);
			}
		}
		if (capture == "Time")
		{
			try
			{
				return DateTime.Now.ToString("h:mm");
			}
			catch (TimeZoneNotFoundException)
			{
				return "10:00";
			}
		}
		if (Singleton<GlobalData>.instance.gameData.tags.HasFloat(capture))
		{
			return Singleton<GlobalData>.instance.gameData.tags.GetFloat(capture).ToString();
		}
		if (Singleton<GlobalData>.instance.gameData.tags.HasString(capture))
		{
			return Singleton<GlobalData>.instance.gameData.tags.GetString(capture);
		}
		foreach (Button value2 in Enum.GetValues(typeof(Button)))
		{
			Debug.LogWarning("TextReplacer: This button is deprecated! (" + capture + ")");
			if (capture == value2.ToString())
			{
				GameUserInput obj2 = Singleton<FocusableUserInputManager>.instance.inputWithFocus as GameUserInput;
				return obj2.WrapButtonNameForText(obj2.GetButton(value2).name);
			}
		}
		return capture;
	}

	public static string ReplaceVariables(string text, bool autoLineBreak = true)
	{
		text = text.Replace("\\n", "\n");
		text = Regex.Replace(text, "\\{\\{(.+?)\\}\\}", (Match match) => ReplaceCapturedVariable(match.Groups[1].Value));
		return text;
	}
}
