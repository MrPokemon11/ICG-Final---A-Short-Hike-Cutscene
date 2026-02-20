using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
	public PlayableDirector timeline;

	private void Start()
	{
		timeline.stopped += OnStopped;
	}

	private void OnStopped(PlayableDirector obj)
	{
		Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.FinishedGame);
		PlayerPrefsAdapter.Save();
		SceneManager.LoadScene("TitleScene");
	}
}
