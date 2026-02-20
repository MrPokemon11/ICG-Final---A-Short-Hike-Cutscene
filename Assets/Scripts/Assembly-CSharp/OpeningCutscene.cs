using System.Collections;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class OpeningCutscene : Cutscene
{
	public PlayableDirector timeline;

	public AudioClip doorCloseClip;

	public AudioMixerSnapshot defaultSnapshot;

	public AudioMixerSnapshot quietSnapshot;

	public Transform startingPosition;

	public CollectableItem startingItem;

	protected override IEnumerator CutsceneRoutine()
	{
		LevelController level = Singleton<GameServiceLocator>.instance.levelController;
		level.gameRunning = false;
		quietSnapshot.TransitionTo(0f);
		Player player = level.player;
		player.body.position = startingPosition.position;
		player.body.rotation = Quaternion.LookRotation(startingPosition.forward);
		Singleton<GlobalData>.instance.gameData.AddCollected(startingItem, 1);
		quietSnapshot.TransitionTo(0f);
		timeline.Play();
		yield return null;
		GameUserInput inputLock = GameUserInput.CreateInput(base.gameObject);
		yield return new WaitForSeconds((float)timeline.duration);
		defaultSnapshot.TransitionTo(0f);
		doorCloseClip.Play();
		level.gameRunning = true;
		Object.Destroy(inputLock);
	}
}
