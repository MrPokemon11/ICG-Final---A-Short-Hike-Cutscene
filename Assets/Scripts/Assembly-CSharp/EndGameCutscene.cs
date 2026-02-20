using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class EndGameCutscene : MonoBehaviour, IInteractableComponent
{
	public string confirmEndTag = "CanEndGame";

	public PlayableDirector timeline;

	public string endGameNode;

	public AudioMixerSnapshot defaultSnapshot;

	public AudioMixerSnapshot quietSnapshot;

	public TMP_Text versionString;

	private GameUserInput inputLock;

	public bool isRunning { get; private set; }

	bool IInteractableComponent.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public void Interact()
	{
		inputLock = GameUserInput.CreateInput(base.gameObject);
		Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(endGameNode, Singleton<GameServiceLocator>.instance.levelController.player.transform).onConversationFinish += OnConversationFinish;
		versionString.text = (LevelController.speedrunClockActive ? VersionInfo.Load().version : "");
	}

	private void OnConversationFinish()
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(confirmEndTag))
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool("WonGameNiceJob");
			Singleton<GlobalData>.instance.gameData.tags.SetBool(confirmEndTag, value: false);
			Singleton<GameServiceLocator>.instance.levelController.gameRunning = false;
			PlayerPrefsAdapter.SetInt("BeatGame", 1);
			timeline.Play();
			this.RegisterTimer((float)timeline.duration, OnCutsceneFinished);
			quietSnapshot.TransitionTo(2f);
			this.RegisterTimer(2f, delegate
			{
				Singleton<MusicManager>.instance.UnregisterAll();
				Singleton<MusicManager>.instance.TrimRetiredActiveMusicSets(0.01f);
			});
			isRunning = true;
		}
		else
		{
			Object.Destroy(inputLock);
		}
	}

	private void OnCutsceneFinished()
	{
		defaultSnapshot.TransitionTo(0.05f);
		LevelController.SaveAndShowCredits();
	}
}
