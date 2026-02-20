using System.Collections;
using UnityEngine;

public class PlayMusicOnUpdraft : MonoBehaviour
{
	public MusicSet music;

	public int musicPriority = 100;

	public string playedBeforeTag = "UpdraftMusicPlayedBefore";

	public float requiredHeightToReplay;

	private ScriptedMusic playingMusic;

	private bool insideUpdraft;

	private void Start()
	{
		Updraft component = GetComponent<Updraft>();
		component.onRegister += OnUpdraftEnter;
		component.onUnregister += OnUpdraftLeave;
	}

	private void OnDestroy()
	{
		if (playingMusic != null)
		{
			playingMusic.Stop();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * requiredHeightToReplay, 1f);
	}

	private void OnUpdraftEnter()
	{
		if ((playingMusic == null || !playingMusic.isPlaying) && Singleton<MusicManager>.instance.currentMusicPriority <= musicPriority)
		{
			if (!Singleton<GlobalData>.instance.gameData.tags.GetBool(playedBeforeTag))
			{
				BeginMusic();
			}
			else
			{
				StartCoroutine(BeginMusicLater());
			}
		}
	}

	private void OnUpdraftLeave()
	{
		insideUpdraft = false;
	}

	private IEnumerator BeginMusicLater()
	{
		insideUpdraft = true;
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		while (insideUpdraft && player.transform.position.y < base.transform.position.y + requiredHeightToReplay)
		{
			yield return null;
		}
		if (insideUpdraft)
		{
			BeginMusic();
		}
	}

	private void BeginMusic()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetBool(playedBeforeTag);
		playingMusic = new ScriptedMusic(music, musicPriority);
		playingMusic.PlayOnce(music.fadeOutTime);
	}
}
