using System.Threading.Tasks;
using QuickUnityTools.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn;

public class LoadingScene : MonoBehaviour
{
	private static bool SKIP_INDIVIDUAL_LOGOS = false;

	private static int IMPATIENT_PRESSES = 3;

	public TextAsset preloadDialogue;

	public WhippoorwillLoadingScreen logos;

	private Task dialogueLoadTask;

	private AsyncOperation asyncScene;

	private bool skipLogos;

	private int impatientPresses;

	private void Start()
	{
		asyncScene = SceneManager.LoadSceneAsync(1);
		asyncScene.allowSceneActivation = false;
		TitleScreen.fadeInAudio = true;
		string text = preloadDialogue.text;
		string name = preloadDialogue.name;
		dialogueLoadTask = Task.Run(delegate
		{
			Library library = DialogueController.CreateDummyLibrary();
			DialogueController.PreloadDialogue(text, name, library);
		});
		Singleton<SoundPlayer>.instance.ToString();
		CrossPlatform.OnSplashScreenStart(this);
	}

	private void Update()
	{
		if (asyncScene.progress >= 0.9f && dialogueLoadTask.IsCompleted && CrossPlatform.isCrossPlatformSplashScreenInitFinished && (logos.isFinished || skipLogos))
		{
			asyncScene.allowSceneActivation = true;
		}
		int num = CrossPlatform.IsSkipLogoPressed();
		impatientPresses += num;
		if (num > 0 && impatientPresses >= IMPATIENT_PRESSES)
		{
			if (SKIP_INDIVIDUAL_LOGOS)
			{
				logos.AdvanceLogo();
			}
			else
			{
				skipLogos = true;
			}
		}
	}
}
