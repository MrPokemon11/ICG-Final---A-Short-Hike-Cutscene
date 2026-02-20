using UnityEngine;

public class AuntDynamicDialogue : MonoBehaviour
{
	public static bool TALK_FOREVER_CHEAT;

	public string talkPointsTag = "$TalkPoints";

	public string transitionTag = "Transition";

	private void Start()
	{
		Singleton<GlobalData>.instance.gameData.tags.WatchFloat(talkPointsTag, OnTalkPointsChanged);
	}

	private void OnDestroy()
	{
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.gameData.tags.UnwatchFloat(talkPointsTag, OnTalkPointsChanged);
		}
	}

	private void OnTalkPointsChanged(float number)
	{
		string value = ((int)number % 4) switch
		{
			0 => "", 
			1 => I18n.STRINGS.also, 
			2 => I18n.STRINGS.ohTransition, 
			3 => I18n.STRINGS.whatElse, 
			_ => "", 
		};
		Singleton<GlobalData>.instance.gameData.tags.SetString(transitionTag, value);
		if (TALK_FOREVER_CHEAT && number == 3f)
		{
			Singleton<GlobalData>.instance.gameData.tags.SetFloat(talkPointsTag, 0f);
		}
	}
}
