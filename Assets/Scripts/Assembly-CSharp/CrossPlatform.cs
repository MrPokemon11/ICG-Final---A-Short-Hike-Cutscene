using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using QuickUnityTools.Input;
using UnityEngine;

public static class CrossPlatform
{
	public static bool isCrossPlatformSplashScreenInitFinished = true;

	public static bool cachedDoesFileExist = false;

	private static List<string> VALID_START_BUTTONS = new List<string> { "A", "B", "X", "Y", "Cross", "Square", "Circle", "Triangle" };

	public static void OnSplashScreenStart(LoadingScene loadingScene)
	{
	}

	public static void ConfigureInputManager()
	{
		if (NativeInputDeviceManager.CheckPlatformSupport(new List<string>()))
		{
			InputManager.AddDeviceManager<NativeInputDeviceManager>();
		}
		else
		{
			InputManager.AddDeviceManager<UnityInputDeviceManager>();
		}
	}

	public static bool AllowControllerConfiguration()
	{
		return true;
	}

	public static int IsSkipLogoPressed()
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
		{
			return 10;
		}
		for (int i = 0; i < 20; i++)
		{
			if (Input.GetKeyDown("joystick button " + i))
			{
				return 1;
			}
		}
		return 0;
	}

	public static void OnTitleScreenStart(Action updateEngagementText)
	{
		InputManager.Update();
		ForceFirstDeviceAsActive(updateEngagementText);
	}

	private static void ForceFirstDeviceAsActive(Action updateEngagementText)
	{
		Debug.Log("All Devices: " + string.Join(",", InputManager.Devices.Select((InputDevice d) => d.Name)));
		InputDevice inputDevice = InputManager.Devices.FirstOrDefault((InputDevice d) => VALID_START_BUTTONS.Contains(d.Action1.Handle));
		Debug.Log("Start Device: " + inputDevice?.Name);
		if (inputDevice != null)
		{
			Asserts.WeakAssertTrue(GameUserInput.sharedActionSet != null, "The action set is null?!");
			InputManager.ForceActiveDevice(inputDevice, GameUserInput.sharedActionSet.button1);
			updateEngagementText();
		}
	}

	public static void OnTitleScreenDeviceAttached(Action updateEngagementText)
	{
		if (InputManager.Devices.Count == 1)
		{
			ForceFirstDeviceAsActive(updateEngagementText);
		}
	}

	public static FileSystem.IFileSystem CreateFileSystem()
	{
		return new StandardFileSystem();
	}

	public static PlayerPrefsAdapter.IPlayerPrefs CreatePlayerPrefs()
	{
		if (GameSettings.isGameConsole)
		{
			return new PlayerPrefsAdapter.FilePlayerPrefs();
		}
		return new PlayerPrefsAdapter.StandardPlayerPrefs();
	}

	public static string PlatformDefaultButton()
	{
		return "?";
	}

	public static IAchievementBackend CreateAchievementBackend(AchievementManager manager)
	{
		return new SteamAchievements();
	}

	public static void OnEngagementScreenActivated(TitleScreen title, bool forceUserSelect, Action<bool> onFinish)
	{
		Timer.Register(0.5f, delegate
		{
			onFinish(obj: false);
		});
	}

	public static bool PlatformSpecificOptions(out List<(string, Action)> menuItems)
	{
		menuItems = null;
		return false;
	}

	private static void ReloadXboxPlayerPrefs(TitleScreen title, Action onFinish)
	{
	}

	public static void UpdateQuitMenuItem(GameObject quitItem)
	{
		quitItem.GetComponent<IMenuItem>().enabled = !GameSettings.isGameConsole;
	}

	public static bool ReengageInsteadOfQuit()
	{
		return false;
	}

	public static void LoadRemoteAchievements(GameObject loadingBoxPrefab, Action onFinish)
	{
		onFinish();
	}

	public static bool DoesSaveExist()
	{
		return Singleton<GlobalData>.instance.DoesSaveExist();
	}

	public static string GetAchievementsName()
	{
		return I18n.STRINGS.achievements;
	}

	public static T LoadPrefsFile<T>(string filename)
	{
		return FileSystem.LoadObjectUnsafe<T>(filename);
	}

	public static void SavePrefsFile(string filename, object obj, int saveDataSize)
	{
		FileSystem.SaveObject(filename, obj, saveDataSize, null);
	}
}
