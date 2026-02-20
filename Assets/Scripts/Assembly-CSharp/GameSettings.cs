using System.Linq;
using QuickUnityTools.Input;
using UnityEngine;

public static class GameSettings
{
	public enum ThirdPartyLicenseInfo
	{
		Hidden = 0,
		External = 1,
		Popup = 2
	}

	private const string TARGET_FPS_PREF = "TARGET_FPS_";

	private const string FPS_PREF = "FPS_";

	private const string JITTER_PREF = "JITTER_";

	private const string SCALE_PREF = "PIXWIDTH_";

	private const string LANG_PREF = "LANG_";

	private const string AUTOSAVE_PREF = "AUTOSAVE_";

	private const string SAVESLOT_PREF = "SAVESLOT_";

	private const bool FORCE_TOUCH_SCREEN = false;

	private const bool FORCE_LOW_QUALITY = false;

	private static bool _allowConsoleCheats;

	private static bool _useCullingGroups = !useLowQualityOptimizations;

	private static bool _useBakedTerrain = true;

	private static string _language;

	private static bool _showFPS;

	private static int _targetFPS;

	private static int _saveSlot;

	private static bool _autosave;

	public static bool restrictScreenSettings
	{
		get
		{
			if (!Debug.isDebugBuild)
			{
				if (!isGameConsole)
				{
					return Application.isMobilePlatform;
				}
				return true;
			}
			return false;
		}
	}

	public static bool restrictQualitySettings
	{
		get
		{
			if (!Debug.isDebugBuild && isGameConsole)
			{
				return !allowConsoleCheats;
			}
			return false;
		}
	}

	public static bool restrictKeyboardSettings
	{
		get
		{
			if (!isGameConsole)
			{
				return Application.isMobilePlatform;
			}
			return true;
		}
	}

	public static bool restrictControllerRemapping => isGameConsole;

	public static bool isTouchScreen => Application.isMobilePlatform;

	public static bool useLowQualityOptimizations => Application.isMobilePlatform;

	public static bool prewarmMusic => isGameConsole;

	public static int maxSaveSlots => 3;

	public static bool hasSaveSlots => !isGameConsole;

	public static bool isGameConsole => Application.isConsolePlatform;

	public static bool restrictMaxPixelWidth => false;

	public static ThirdPartyLicenseInfo showThirdPartyLicenseInfo
	{
		get
		{
			if (!GameUserInput.sharedActionSet.LastInputType.IsMouseOrKeyboard())
			{
				return ThirdPartyLicenseInfo.Popup;
			}
			return ThirdPartyLicenseInfo.External;
		}
	}

	public static bool allowConsoleCheats
	{
		get
		{
			if (!_allowConsoleCheats)
			{
				return Debug.isDebugBuild;
			}
			return true;
		}
		set
		{
			if (value && !_allowConsoleCheats)
			{
				Cheats cheats = Object.FindObjectOfType<Cheats>();
				if ((bool)cheats)
				{
					cheats.RegisterConsoleReleaseCheats();
				}
			}
			_allowConsoleCheats = value;
		}
	}

	public static bool useCullingGroups
	{
		get
		{
			return _useCullingGroups;
		}
		set
		{
			_useCullingGroups = value;
			UpdateCullingGroupsAndTerrainSettings();
		}
	}

	public static bool useBakedTerrain
	{
		get
		{
			return _useBakedTerrain;
		}
		set
		{
			_useBakedTerrain = value;
			UpdateCullingGroupsAndTerrainSettings();
		}
	}

	public static string language => _language;

	public static bool showFPS
	{
		get
		{
			return _showFPS;
		}
		set
		{
			_showFPS = value;
			PlayerPrefsAdapter.SetInt("FPS_", value ? 1 : 0);
			FPSCounter fPSCounter = Singleton<ServiceLocator>.instance.Locate<FPSCounter>(allowFail: true);
			if ((bool)fPSCounter)
			{
				fPSCounter.visible = value;
			}
		}
	}

	public static int targetFPS
	{
		get
		{
			return _targetFPS;
		}
		set
		{
			_targetFPS = value;
			PlayerPrefsAdapter.SetInt("TARGET_FPS_", value);
			Application.targetFrameRate = value;
		}
	}

	public static Player.JitterFixConfiguration useJitterFix
	{
		get
		{
			return Player.jitterFixConfiguration;
		}
		set
		{
			Player.jitterFixConfiguration = value;
			PlayerPrefsAdapter.SetInt("JITTER_", (int)value);
		}
	}

	public static int pixelWidth
	{
		get
		{
			return PixelFilterAdjuster.pixelWidth;
		}
		set
		{
			PixelFilterAdjuster.pixelWidth = value;
			PlayerPrefsAdapter.SetInt("PIXWIDTH_", value);
		}
	}

	public static int saveSlot
	{
		get
		{
			return _saveSlot;
		}
		set
		{
			_saveSlot = value;
			PlayerPrefsAdapter.SetInt("SAVESLOT_", value);
			GlobalData.SetSaveSlot(value);
		}
	}

	public static bool autosave
	{
		get
		{
			return _autosave;
		}
		set
		{
			_autosave = value;
			PlayerPrefsAdapter.SetInt("AUTOSAVE_", value ? 1 : 0);
		}
	}

	public static IWaitable LoadSettingsPrefs()
	{
		useJitterFix = (Player.JitterFixConfiguration)PlayerPrefsAdapter.GetInt("JITTER_");
		showFPS = PlayerPrefsAdapter.GetInt("FPS_", Debug.isDebugBuild ? 1 : 0) == 1;
		targetFPS = PlayerPrefsAdapter.GetInt("TARGET_FPS_", -1);
		pixelWidth = PlayerPrefsAdapter.GetInt("PIXWIDTH_", 384);
		autosave = PlayerPrefsAdapter.GetInt("AUTOSAVE_", 1) == 1;
		saveSlot = PlayerPrefsAdapter.GetInt("SAVESLOT_");
		Volume.LoadVolumePrefs();
		return SetLanguage(PlayerPrefsAdapter.GetString("LANG_", I18n.GetLanguages().FirstOrDefault((I18n.Language l) => l.systemLanguage == Application.systemLanguage)?.saveName));
	}

	public static IWaitable SetLanguage(string value, float forceWaitTime = 0f)
	{
		bool flag = _language == value || (_language == null && value == SystemLanguage.English.ToString());
		_language = value;
		PlayerPrefsAdapter.SetString("LANG_", _language);
		if (!string.IsNullOrEmpty(_language) && !flag)
		{
			I18n.Language[] array = I18n.GetLanguages().ToArray();
			int num = array.IndexOf((I18n.Language l) => l.saveName == _language);
			if (num >= 0)
			{
				return I18n.SetLanguage(array[num], forceWaitTime);
			}
		}
		return FlagWaitable.FinishedWaitable();
	}

	private static void UpdateCullingGroupsAndTerrainSettings()
	{
		CullingRegion[] array = Object.FindObjectsOfType<CullingRegion>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = useCullingGroups;
		}
		CullingTerrain[] array2 = Object.FindObjectsOfType<CullingTerrain>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = useCullingGroups && !useBakedTerrain;
		}
		TerrainBaker[] array3 = Object.FindObjectsOfType<TerrainBaker>();
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].enabled = useBakedTerrain;
		}
	}
}
