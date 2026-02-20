using System;
using InControl;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
	public const float ENGAGEMENT_SCREEN_PAUSE = 0.5f;

	public static int DEMO_MODE = -1;

	public static bool fadeInAudio;

	public string titleScreenIntroStart = "TitleScreenIntroStart";

	public float loadTickFrequency = 0.5f;

	public GridMenu menu;

	public GameObject newGameItem;

	public GameObject continueItem;

	public GameObject quitItem;

	public GameObject optionsMenuPrefab;

	public TMP_Text pressToStart;

	public TMP_Text loadingText;

	public TMP_Text demoText;

	public Transform introSpeaker;

	public GameObject transitionAnimation;

	public AudioClip driveAwayClip;

	public AudioMixerSnapshot quietSnapshot;

	public AudioMixerSnapshot defaultSnapshot;

	public GameObject canvasCover;

	private DialogueController dialogue;

	private IAsyncOperationBundle loadingOperation;

	private GameUserInput input;

	private float loadTimer;

	private IConversation introConversastion;

	private bool startedTransition;

	private GameSetup gameSetupController;

	private float? pressStartTime;

	public event Action onMenuItemsUpdated;

	private void Start()
	{
		UpdateTitleScreenMenuItems();
		dialogue = Singleton<ServiceLocator>.instance.Locate<DialogueController>();
		GameObject gameObject = new GameObject("InputHolder");
		gameObject.gameObject.SetActive(value: false);
		input = GameUserInput.CreateInput(gameObject);
		input.priority = -10;
		gameObject.gameObject.SetActive(value: true);
		TextTranslator component = pressToStart.GetComponent<TextTranslator>();
		component.UpdateTranslation();
		pressToStart.gameObject.SetActive(value: true);
		menu.gameObject.SetActive(value: false);
		if (fadeInAudio)
		{
			fadeInAudio = false;
			quietSnapshot.TransitionTo(0f);
			this.RegisterTimer(0.25f, delegate
			{
				defaultSnapshot.TransitionTo(0.5f);
			});
		}
		gameSetupController = UnityEngine.Object.FindObjectOfType<GameSetup>();
		canvasCover.gameObject.SetActive(value: true);
		CrossPlatform.OnTitleScreenStart(component.UpdateTranslation);
		InputManager.OnDeviceAttached += OnDeviceAttached;
	}

	private void OnDestroy()
	{
		InputManager.OnDeviceAttached -= OnDeviceAttached;
	}

	private void OnDeviceAttached(InputDevice obj)
	{
		CrossPlatform.OnTitleScreenDeviceAttached(pressToStart.GetComponent<TextTranslator>().UpdateTranslation);
	}

	public void UpdateTitleScreenMenuItems()
	{
		if (!CrossPlatform.DoesSaveExist())
		{
			newGameItem.transform.SetAsFirstSibling();
			menu.SetMenuItem(0, 0, newGameItem);
			menu.SetMenuItem(1, 0, continueItem);
			continueItem.GetComponent<IMenuItem>().enabled = false;
			continueItem.GetComponent<BasicMenuItem>().SetDisabledStyle(isDisabled: true);
		}
		else
		{
			continueItem.transform.SetAsFirstSibling();
			menu.SetMenuItem(1, 0, newGameItem);
			menu.SetMenuItem(0, 0, continueItem);
			continueItem.GetComponent<IMenuItem>().enabled = true;
			continueItem.GetComponent<BasicMenuItem>().SetDisabledStyle(isDisabled: false);
			newGameItem.GetComponent<IMenuItem>().Unhighlight();
		}
		UpdateQuitMenuItem();
		this.onMenuItemsUpdated?.Invoke();
	}

	private void Update()
	{
		if (canvasCover.activeSelf && Time.timeSinceLevelLoad > 0.7f && gameSetupController.isSetupFinished)
		{
			canvasCover.SetActive(value: false);
		}
		if (pressToStart.gameObject.activeSelf && input.GetConfirmButton().ConsumePress())
		{
			pressToStart.gameObject.SetActive(value: false);
			OnEngagementScreenActivated(forceUserSelect: false);
		}
		if (loadingOperation != null && loadingOperation.progress >= 0.9f && (introConversastion == null || !introConversastion.isAlive))
		{
			PlayTransitionAnimation();
		}
		UpdateLoadingIndicator();
	}

	private void UpdateLoadingIndicator()
	{
		bool flag = loadingOperation != null && loadingOperation.progress < 0.9f;
		bool flag2 = pressStartTime.HasValue && Time.time > pressStartTime + 0.5f + 0.01f;
		loadingText.enabled = flag || flag2;
		if (loadingText.enabled)
		{
			loadTimer += Time.deltaTime;
			if (loadTimer > loadTickFrequency)
			{
				loadTimer = 0f;
				loadingText.text += ".";
				if (loadingText.text.Length > 3)
				{
					loadingText.text = "";
				}
			}
		}
		else
		{
			loadingText.text = "";
		}
	}

	private void OnEngagementScreenActivated(bool forceUserSelect)
	{
		pressStartTime = Time.time;
		CrossPlatform.OnEngagementScreenActivated(this, forceUserSelect, delegate(bool updateMenu)
		{
			pressStartTime = null;
			if (updateMenu)
			{
				UpdateTitleScreenMenuItems();
			}
			menu.gameObject.SetActive(value: true);
		});
	}

	private void PlayTransitionAnimation()
	{
		if (!startedTransition)
		{
			startedTransition = true;
			transitionAnimation.SetActive(value: true);
			UnityEngine.Object.DontDestroyOnLoad(driveAwayClip.Play().gameObject);
			quietSnapshot.TransitionTo(1f);
			this.RegisterTimer(1.4f, delegate
			{
				loadingOperation.allowSceneActivation = true;
			});
		}
	}

	public void StartNewGame()
	{
		if (CrossPlatform.DoesSaveExist())
		{
			UI uI = Singleton<ServiceLocator>.instance.Locate<UI>();
			LinearMenu submenu = null;
			submenu = uI.CreateSimpleMenu(new string[2]
			{
				I18n.STRINGS.continueText,
				I18n.STRINGS.dontContinue
			}, new Action[2]
			{
				delegate
				{
					submenu.Kill();
					BeginLoadingNewGame();
				},
				delegate
				{
					submenu.Kill();
				}
			});
			GameObject obj = uI.CreateTextMenuItem(I18n.STRINGS.overwriteSaveFile);
			obj.transform.SetParent(submenu.transform, worldPositionStays: false);
			obj.transform.SetAsFirstSibling();
			LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
			(submenu.transform as RectTransform).CenterWithinParent();
		}
		else
		{
			BeginLoadingNewGame();
		}
	}

	private void BeginLoadingNewGame()
	{
		LoadGameScene(Singleton<GlobalData>.instance.NewGameAsync());
		introConversastion = dialogue.StartConversation(titleScreenIntroStart, introSpeaker);
	}

	public void ContinueGame()
	{
		LoadGameScene(Singleton<GlobalData>.instance.LoadGameAsync(OnLoadFileFail));
	}

	private void LoadGameScene(IAsyncOperationBundle loading)
	{
		loadingOperation = loading;
		menu.gameObject.SetActive(value: false);
		loadingOperation.allowSceneActivation = false;
	}

	private void OnLoadFileFail()
	{
		loadingOperation = null;
		menu.gameObject.SetActive(value: true);
	}

	private void UpdateQuitMenuItem()
	{
		CrossPlatform.UpdateQuitMenuItem(quitItem);
	}

	public void Quit()
	{
		if (CrossPlatform.ReengageInsteadOfQuit())
		{
			RestartTitleScreenEngagement();
		}
		else if (DEMO_MODE == -1)
		{
			PlayerPrefsAdapter.Save();
			Application.Quit();
		}
	}

	public void RestartTitleScreenEngagement()
	{
		PlayerPrefsAdapter.Save();
		menu.gameObject.SetActive(value: false);
		OnEngagementScreenActivated(forceUserSelect: true);
	}

	public void ShowOptions()
	{
		optionsMenuPrefab.Clone();
	}
}
