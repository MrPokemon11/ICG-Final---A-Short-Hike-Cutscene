using System;
using System.Collections.Generic;
using System.Linq;
using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
	private struct MenuItem
	{
		public string name;

		public Action<BasicMenuItem> action;

		public MenuItem(string name, Action action)
		{
			this.name = name;
			this.action = delegate
			{
				action();
			};
		}

		public MenuItem(string name, Action<BasicMenuItem> action)
		{
			this.name = name;
			this.action = action;
		}
	}

	public const string DATA_FOLDER = "/Resources/Data/";

	public const string LICENSE_FILE = "SoftwareLicenses.txt";

	private static Resolution[] REQUIRED_RESOLUTIONS = new Resolution[5]
	{
		new Resolution
		{
			width = 384,
			height = 216
		},
		new Resolution
		{
			width = 768,
			height = 432
		},
		new Resolution
		{
			width = 1152,
			height = 648
		},
		new Resolution
		{
			width = 1280,
			height = 720
		},
		new Resolution
		{
			width = 1920,
			height = 1080
		}
	};

	private static Dictionary<FullScreenMode, Func<string>> SCREEN_MODES = new Dictionary<FullScreenMode, Func<string>>
	{
		{
			FullScreenMode.Windowed,
			() => I18n.STRINGS.windowed
		},
		{
			FullScreenMode.FullScreenWindow,
			() => I18n.STRINGS.borderlessWindowed
		},
		{
			FullScreenMode.ExclusiveFullScreen,
			() => I18n.STRINGS.trueFullscreen
		}
	};

	private static Dictionary<Volume.Channel, Func<string>> VOLUME_CHANNELS = new Dictionary<Volume.Channel, Func<string>>
	{
		{
			Volume.Channel.Master,
			() => I18n.STRINGS.master
		},
		{
			Volume.Channel.Music,
			() => I18n.STRINGS.music
		},
		{
			Volume.Channel.SoundEffects,
			() => I18n.STRINGS.soundEffects
		},
		{
			Volume.Channel.Ambience,
			() => I18n.STRINGS.ambience
		}
	};

	public static Dictionary<SystemLanguage, string> LANGUAGE_TITLE_TEXTURE = new Dictionary<SystemLanguage, string> { 
	{
		SystemLanguage.Japanese,
		"Fonts/Japanese_Option"
	} };

	private static List<float> RENDER_SCALES = new List<float> { 1f, 1.25f, 1.5f };

	private static List<float> EXTRA_RENDER_SCALES = new List<float> { 2f, 2.5f, -1f };

	public static string OPTIONS_RED_HEX;

	public Color highlightColor = Color.red;

	public GameObject dialogBoxPrefab;

	public GameObject creditsPrefab;

	public GameObject messagePrefab;

	public GameObject gamepadControlsPrefab;

	public GameObject keyboardControlsPrefab;

	public GameObject rebindPrefab;

	public GameObject achievementsMenuPrefab;

	public GameObject controllerRemapperPrefab;

	public GameObject loadingBoxPrefab;

	public GameObject thirdPartyLicensesPrefab;

	private UI ui;

	private LinearMenu optionsMenu;

	private Color defaultTextColor = Color.clear;

	private void Awake()
	{
		ui = Singleton<ServiceLocator>.instance.Locate<UI>();
	}

	private void Start()
	{
		OPTIONS_RED_HEX = highlightColor.ToHexString();
		BuildMainMenu();
	}

	private void BuildMainMenu()
	{
		List<MenuItem> list = new List<MenuItem>();
		if (GameSettings.restrictQualitySettings && GameSettings.restrictScreenSettings)
		{
			list.Add(new MenuItem(I18n.STRINGS.pixelSize, delegate(BasicMenuItem menuItem)
			{
				ShowRenderResolutionMenu(optionsMenu, menuItem, highlighted: true);
			}));
		}
		else
		{
			list.Add(new MenuItem(I18n.STRINGS.graphics, (Action)delegate
			{
				ShowGraphicsMenu();
			}));
		}
		list.Add(new MenuItem(I18n.STRINGS.audio, ShowAudioMenu));
		if (GameSettings.restrictKeyboardSettings && GameSettings.restrictControllerRemapping)
		{
			list.Add(new MenuItem(I18n.STRINGS.controls, (Action)delegate
			{
				ui.AddUI(gamepadControlsPrefab.Clone());
			}));
		}
		else
		{
			list.Add(new MenuItem(I18n.STRINGS.controls, ShowControlsMenu));
		}
		list.Add(new MenuItem(I18n.STRINGS.languages, ShowLanguagesMenu));
		if (GameSettings.hasSaveSlots)
		{
			list.Add(new MenuItem(I18n.STRINGS.saveData, ShowSaveMenu));
		}
		list.Add(new MenuItem(I18n.STRINGS.info, ShowAboutMenu));
		if (CrossPlatform.PlatformSpecificOptions(out var menuItems))
		{
			foreach (var item in menuItems)
			{
				list.Add(new MenuItem(item.Item1, item.Item2));
			}
		}
		if (GameSettings.isGameConsole && GameSettings.allowConsoleCheats)
		{
			list.Add(new MenuItem(I18n.STRINGS.cheats, ShowCheatsMenu));
		}
		optionsMenu = BuildSimpleMenu(list);
		optionsMenu.onKill += OnKillLinearMenu;
	}

	private void OnKillLinearMenu()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void RefreshMainMenu()
	{
		optionsMenu.onKill -= OnKillLinearMenu;
		optionsMenu.Kill();
		BuildMainMenu();
	}

	private LinearMenu BuildSimpleMenu(List<MenuItem> menuItems)
	{
		LinearMenu linearMenu = ui.CreateSimpleMenu(menuItems.Select((MenuItem i) => i.name).ToArray(), menuItems.Select((MenuItem i) => i.action).ToArray());
		PositionMenu(linearMenu.transform);
		return linearMenu;
	}

	private void HighlightSimpleMenuItem(LinearMenu menu, string highlighted)
	{
		if (defaultTextColor == Color.clear)
		{
			defaultTextColor = menu.GetMenuObjects().First().GetComponentInChildren<TMP_Text>()
				.color;
		}
		foreach (GameObject menuObject in menu.GetMenuObjects())
		{
			TMP_Text componentInChildren = menuObject.GetComponentInChildren<TMP_Text>();
			if ((bool)componentInChildren)
			{
				componentInChildren.color = ((componentInChildren.text == highlighted) ? highlightColor : defaultTextColor);
			}
		}
	}

	public static void PositionMenu(Transform menu)
	{
		GameUserInput component = menu.GetComponent<GameUserInput>();
		RectTransform rectTransform = menu.transform as RectTransform;
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		rectTransform.CenterWithinParent();
		RectTransform rectTransform2 = rectTransform.parent as RectTransform;
		float num = rectTransform2.rect.width * 0.25f;
		int num2 = ((component != null) ? component.GetInputStackHeight() : Singleton<FocusableUserInputManager>.instance.inputStackCount);
		num += (float)((num2 - 3) * 20);
		num = Mathf.RoundToInt(num);
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, num, rectTransform.rect.size.x);
		if (rectTransform.localPosition.x + rectTransform.rect.size.x / 2f > rectTransform2.rect.max.x)
		{
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, rectTransform.rect.size.x);
		}
	}

	public void Kill()
	{
		optionsMenu.Kill();
	}

	private string HighlightText(string text)
	{
		int num = text.IndexOf(':');
		if (num == -1)
		{
			return text;
		}
		text = text.Insert(text.Length, "</color>");
		text = text.Insert(num + 1, "<color=" + OPTIONS_RED_HEX + ">");
		return text;
	}

	private string GetVSyncText()
	{
		string text = ((ExtendedQualitySettings.vSync == 0) ? I18n.STRINGS.disabled : ((ExtendedQualitySettings.vSync == 1) ? I18n.STRINGS.enabled : I18n.STRINGS.doubleBuffered));
		return HighlightText(I18n.STRINGS.vSync + ": " + text);
	}

	private string GetShadowsText()
	{
		string text = I18n.Localize(QualitySettings.names[ExtendedQualitySettings.baseQualityLevel]);
		return HighlightText(I18n.STRINGS.shadows + ": " + ((QualitySettings.shadows == ShadowQuality.Disable) ? I18n.STRINGS.disabled : text));
	}

	private string GetEdgesText()
	{
		return HighlightText(I18n.STRINGS.edgeOutlines + ": " + ((!ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.EdgeDetection)) ? I18n.STRINGS.disabled : I18n.STRINGS.enabled));
	}

	private string GetColorCorrectionText()
	{
		return HighlightText(I18n.STRINGS.colorCorrection + ": " + ((!ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.ColorCorrection)) ? I18n.STRINGS.disabled : I18n.STRINGS.enabled));
	}

	private string GetJitterFixText()
	{
		return HighlightText(I18n.STRINGS.jitterFix + ": " + GameSettings.useJitterFix.ToString().ToLower());
	}

	private string GetTargetFPSText()
	{
		return HighlightText(I18n.STRINGS.targetFPS + ": " + ((GameSettings.targetFPS == -1) ? I18n.STRINGS.unlimited : GameSettings.targetFPS.ToString()));
	}

	private string GetRenderResolutionText()
	{
		return HighlightText(I18n.STRINGS.pixelSize + ": " + PixelFilterAdjuster.GetWidthString(PixelFilterAdjuster.pixelWidth));
	}

	private string GetSaveSlotText()
	{
		return HighlightText(I18n.STRINGS.saveSlotOption + ": " + string.Format(I18n.STRINGS.numberedSaveSlot, GameSettings.saveSlot + 1));
	}

	private string GetAutoSaveText()
	{
		return HighlightText(I18n.STRINGS.autosave + ": " + (GameSettings.autosave ? I18n.STRINGS.enabled : I18n.STRINGS.disabled));
	}

	private LinearMenu ShowGraphicsMenu()
	{
		LinearMenu menu = null;
		List<MenuItem> list = new List<MenuItem>();
		list.Add(new MenuItem(GetRenderResolutionText(), delegate(BasicMenuItem menuItem)
		{
			ShowRenderResolutionMenu(menu, menuItem, highlighted: false);
		}));
		if (!GameSettings.restrictScreenSettings)
		{
			list.Add(new MenuItem(GetResolutionText(), ShowResolutionMenu));
			list.Add(new MenuItem(GetWindowModeText(), delegate(BasicMenuItem menuItem)
			{
				ShowWindowModeMenu(menu, menuItem);
			}));
		}
		if (!GameSettings.restrictQualitySettings)
		{
			list.Add(new MenuItem(GetImageQualityText(), (Action<BasicMenuItem>)delegate
			{
				ShowImageQualityMenu(menu);
			}));
			list.Add(new MenuItem(GetShadowsText(), delegate(BasicMenuItem item)
			{
				ExtendedQualitySettings.EnsureCustomSettings();
				ExtendedQualitySettings.shadows = ((ExtendedQualitySettings.shadows == ShadowQuality.Disable) ? ShadowQuality.HardOnly : ShadowQuality.Disable);
				UI.SetGenericText(item.gameObject, GetShadowsText());
				UpdateCustomImageQualityMenus(menu);
			}));
			list.Add(new MenuItem(GetEdgesText(), delegate(BasicMenuItem item)
			{
				ExtendedQualitySettings.EnsureCustomSettings();
				ExtendedQualitySettings.imageEffects = (ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.EdgeDetection) ? (ExtendedQualitySettings.imageEffects & ~ImageEffectsSettingsController.Effect.EdgeDetection) : (ExtendedQualitySettings.imageEffects | ImageEffectsSettingsController.Effect.EdgeDetection));
				UI.SetGenericText(item.gameObject, GetEdgesText());
				UpdateCustomImageQualityMenus(menu);
			}));
			list.Add(new MenuItem(GetColorCorrectionText(), delegate(BasicMenuItem item)
			{
				ExtendedQualitySettings.EnsureCustomSettings();
				ExtendedQualitySettings.imageEffects = (ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.ColorCorrection) ? (ExtendedQualitySettings.imageEffects & ~ImageEffectsSettingsController.Effect.ColorCorrection) : (ExtendedQualitySettings.imageEffects | ImageEffectsSettingsController.Effect.ColorCorrection));
				UI.SetGenericText(item.gameObject, GetColorCorrectionText());
				UpdateCustomImageQualityMenus(menu);
			}));
			list.Add(new MenuItem(GetVSyncText(), delegate(BasicMenuItem item)
			{
				ExtendedQualitySettings.vSync = (ExtendedQualitySettings.vSync + 1) % 3;
				UI.SetGenericText(item.gameObject, GetVSyncText());
				RefreshGraphicsMenu(menu);
			}));
			if (ExtendedQualitySettings.vSync == 0)
			{
				list.Add(new MenuItem(GetTargetFPSText(), delegate(BasicMenuItem item)
				{
					GameSettings.targetFPS += 30;
					GameSettings.targetFPS = Mathf.RoundToInt((float)GameSettings.targetFPS / 30f) * 30;
					if (GameSettings.targetFPS > 60)
					{
						GameSettings.targetFPS = -1;
					}
					UI.SetGenericText(item.gameObject, GetTargetFPSText());
				}));
			}
			Func<string> showFPSText = () => HighlightText(I18n.STRINGS.displayFPS + ": " + ((!GameSettings.showFPS) ? I18n.STRINGS.disabled : I18n.STRINGS.enabled));
			list.Add(new MenuItem(showFPSText(), delegate(BasicMenuItem item)
			{
				GameSettings.showFPS = !GameSettings.showFPS;
				UI.SetGenericText(item.gameObject, showFPSText());
			}));
		}
		menu = BuildSimpleMenu(list);
		return menu;
	}

	private LinearMenu RefreshGraphicsMenu(LinearMenu graphicsMenu)
	{
		int selectedIndex = graphicsMenu.selectedIndex;
		graphicsMenu.Kill();
		graphicsMenu = ShowGraphicsMenu();
		graphicsMenu.selectedIndex = selectedIndex;
		return graphicsMenu;
	}

	private string GetImageQualityText()
	{
		return HighlightText(I18n.STRINGS.imageQuality + ": " + I18n.Localize(QualitySettings.names[QualitySettings.GetQualityLevel()]));
	}

	private void UpdateCustomImageQualityMenus(LinearMenu menu)
	{
		UpdateItemStartingWith(menu, I18n.STRINGS.imageQuality, GetImageQualityText());
		UpdateItemStartingWith(menu, I18n.STRINGS.shadows, GetShadowsText());
		PositionMenu(menu.transform);
	}

	private LinearMenu ShowImageQualityMenu(LinearMenu graphicsMenu)
	{
		LinearMenu submenu = null;
		List<MenuItem> list = new List<MenuItem>();
		for (int i = 0; i < QualitySettings.names.Length - 1; i++)
		{
			int index = i;
			list.Add(new MenuItem(I18n.Localize(QualitySettings.names[i]), (Action)delegate
			{
				QualitySettings.SetQualityLevel(index);
				ExtendedQualitySettings.SynchronizeSettingsWithQualityLevel();
				submenu.Kill();
				LinearMenu linearMenu = RefreshGraphicsMenu(graphicsMenu);
				ShowImageQualityMenu(linearMenu).selectedIndex = index;
				linearMenu.UnfocusArrowHackily();
			}));
		}
		submenu = BuildSimpleMenu(list);
		return submenu;
	}

	private string GetResolutionText()
	{
		return HighlightText(I18n.STRINGS.resolution + ": " + Screen.width + " x " + Screen.height);
	}

	private void ShowResolutionMenu(BasicMenuItem resolutionItem)
	{
		List<MenuItem> menuItems = (from r in Screen.resolutions.Concat(REQUIRED_RESOLUTIONS)
			orderby r.width
			group r by (r.width + r.height).GetHashCode() into g
			select g.First() into r
			select new MenuItem(r.width + " X " + r.height, (Action)delegate
			{
				Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
				this.RegisterTimer(0.01f, delegate
				{
					UI.SetGenericText(resolutionItem.gameObject, GetResolutionText());
				});
			})).ToList();
		BuildSimpleMenu(menuItems);
	}

	private void ShowRenderResolutionMenu(LinearMenu parentMenu, BasicMenuItem menuItem, bool highlighted)
	{
		LinearMenu menu = null;
		Action updateHighlight = delegate
		{
			if (highlighted)
			{
				HighlightSimpleMenuItem(menu, PixelFilterAdjuster.GetWidthString(PixelFilterAdjuster.pixelWidth));
			}
			else
			{
				UI.SetGenericText(menuItem.gameObject, GetRenderResolutionText());
				PositionMenu(parentMenu.transform);
			}
		};
		List<MenuItem> list = (from p in PixelFilterAdjuster.GetPixelWidths()
			select new MenuItem(p.name(), (Action)delegate
			{
				GameSettings.pixelWidth = p.width;
				updateHighlight();
			})).ToList();
		if (!GameSettings.restrictMaxPixelWidth)
		{
			list.Add(new MenuItem(PixelFilterAdjuster.GetWidthString(0), (Action)delegate
			{
				GameSettings.pixelWidth = 0;
				updateHighlight();
			}));
		}
		menu = BuildSimpleMenu(list);
		updateHighlight();
		Action killMenu = delegate
		{
			menu.Kill();
		};
		BaseResolutionHandler.onBaseResolutionChanged += killMenu;
		menu.onKill += delegate
		{
			BaseResolutionHandler.onBaseResolutionChanged -= killMenu;
		};
	}

	private void ShowResolutionWarning()
	{
		UI.SetGenericText(ui.AddUI(dialogBoxPrefab.Clone()), "<sprite name=\"Warning\" tint=1> This game was designed to\nbe viewed at the default\npixelation level.\nThese options are only included\nfor accessibility reasons.");
	}

	private string GetWindowModeText()
	{
		if (!SCREEN_MODES.ContainsKey(Screen.fullScreenMode))
		{
			return "screen mode";
		}
		return HighlightText(I18n.STRINGS.screen + ": " + SCREEN_MODES[Screen.fullScreenMode]());
	}

	private void ShowWindowModeMenu(LinearMenu parentMenu, BasicMenuItem windowItem)
	{
		List<MenuItem> list = new List<MenuItem>();
		foreach (FullScreenMode mode in SCREEN_MODES.Keys)
		{
			list.Add(new MenuItem(SCREEN_MODES[mode](), (Action)delegate
			{
				Screen.SetResolution(Screen.width, Screen.height, mode);
				this.RegisterTimer(0.01f, delegate
				{
					UI.SetGenericText(windowItem.gameObject, GetWindowModeText());
					PositionMenu(parentMenu.transform);
				});
			}));
		}
		BuildSimpleMenu(list);
	}

	private string GetVolumeText(Volume.Channel channel)
	{
		return HighlightText(VOLUME_CHANNELS[channel]() + ": " + Mathf.Round(Volume.GetVolume(channel) * 10f));
	}

	private void ShowAudioMenu()
	{
		LinearMenu linearMenu = ui.CreateSimpleMenu();
		List<GameObject> list = new List<GameObject>();
		foreach (Volume.Channel channel in VOLUME_CHANNELS.Keys)
		{
			GameObject gameObject = ui.CreateScrollMenuItem(GetVolumeText(channel), delegate
			{
			}, delegate(int scrollValue, ScrollMenuItem item)
			{
				float volume = Volume.GetVolume(channel);
				volume += (float)scrollValue * 0.1f;
				volume = Mathf.Clamp(Mathf.Round(volume * 10f) / 10f, 0f, 2f);
				Volume.SetVolume(channel, volume);
				UI.SetGenericText(item.gameObject, GetVolumeText(channel));
			});
			gameObject.transform.SetParent(linearMenu.transform, worldPositionStays: false);
			list.Add(gameObject);
		}
		linearMenu.SetMenuObjects(list);
		PositionMenu(linearMenu.transform);
	}

	private void ShowSaveMenu()
	{
		LinearMenu menu = null;
		List<MenuItem> list = new List<MenuItem>();
		list.Add(new MenuItem(GetSaveSlotText(), delegate(BasicMenuItem item)
		{
			if (SceneManager.GetActiveScene().name == "TitleScene")
			{
				ShowSaveSlotsMenu(menu, item);
			}
			else
			{
				UI.SetGenericText(ui.AddUI(dialogBoxPrefab.Clone()), I18n.STRINGS.returnToMenuToChangeSlot);
			}
		}));
		list.Add(new MenuItem(GetAutoSaveText(), delegate(BasicMenuItem item)
		{
			GameSettings.autosave = !GameSettings.autosave;
			UI.SetGenericText(item.gameObject, GetAutoSaveText());
			PositionMenu(menu.transform);
		}));
		menu = BuildSimpleMenu(list);
	}

	private void ShowSaveSlotsMenu(LinearMenu parentMenu, BasicMenuItem menuItem)
	{
		LinearMenu submenu = null;
		List<MenuItem> list = new List<MenuItem>();
		for (int i = 0; i < GameSettings.maxSaveSlots; i++)
		{
			int cachedIndex = i;
			string filenameForSaveSlot = GlobalData.GetFilenameForSaveSlot(i);
			string text = "";
			if (FileSystem.Exists(filenameForSaveSlot))
			{
				text = " <color=#AAA>(" + FileSystem.LastModified(filenameForSaveSlot).ToString(I18n.STRINGS.dateFormat) + ")</color>";
			}
			list.Add(new MenuItem(string.Format(I18n.STRINGS.numberedSaveSlot, i + 1) + text, (Action)delegate
			{
				GameSettings.saveSlot = cachedIndex;
				UI.SetGenericText(menuItem.gameObject, GetSaveSlotText());
				PositionMenu(parentMenu.transform);
				submenu.Kill();
				TitleScreen titleScreen = UnityEngine.Object.FindObjectOfType<TitleScreen>();
				if ((bool)titleScreen)
				{
					titleScreen.UpdateTitleScreenMenuItems();
				}
			}));
		}
		submenu = BuildSimpleMenu(list);
	}

	private void ShowAchievementsMenu()
	{
		CrossPlatform.LoadRemoteAchievements(loadingBoxPrefab, delegate
		{
			GameObject gameObject = achievementsMenuPrefab.Clone();
			Singleton<GameServiceLocator>.instance.ui.AddUI(gameObject);
			gameObject.GetComponent<CollectionListUI>().Setup(Achievements.all, delegate(Achievement data, CollectionListUIElement element)
			{
				AchievementData data2 = data.GetData();
				bool flag = Singleton<GameServiceLocator>.instance.achievements.HasAchievement(data);
				element.image.sprite = (flag ? data2.sprite : data2.lockedSprite);
				string text = string.Format("<color={2}>{0}</color>\n{1}", I18n.Localize(data2.title), I18n.Localize(data2.description), flag ? "#FC4" : "#863");
				text = ((!flag && data2.secret) ? "???" : text);
				element.text.text = text;
			});
		});
	}

	private void ShowLanguagesMenu()
	{
		ShowLanguagesMenu(0);
	}

	private void ShowLanguagesMenu(int startIndex)
	{
		LinearMenu menu = null;
		List<MenuItem> list = new List<MenuItem>();
		I18n.Language[] languages = I18n.GetLanguages();
		I18n.Language[] array = languages;
		foreach (I18n.Language language in array)
		{
			list.Add(new MenuItem(language.menuName, (Action)delegate
			{
				GameObject loadingBox = Singleton<GameServiceLocator>.instance.ui.AddUI(loadingBoxPrefab.Clone());
				IWaitable waitable = GameSettings.SetLanguage(language.saveName, 1f);
				WaitFor.WithCoroutine(this, delegate
				{
					UnityEngine.Object.Destroy(loadingBox);
					int selectedIndex = menu.selectedIndex;
					menu.Kill();
					RefreshMainMenu();
					ShowLanguagesMenu(selectedIndex);
					optionsMenu.selectedIndex = 3;
				}, waitable);
			}));
		}
		menu = BuildSimpleMenu(list);
		List<GameObject> menuObjects = menu.GetMenuObjects();
		for (int num = 0; num < menuObjects.Count; num++)
		{
			if (LANGUAGE_TITLE_TEXTURE.ContainsKey(languages[num].systemLanguage))
			{
				menuObjects[num].GetComponentInChildren<TMP_Text>().gameObject.SetActive(value: false);
				Image image = new GameObject().AddComponent<Image>();
				image.sprite = Resources.Load<Sprite>(LANGUAGE_TITLE_TEXTURE[languages[num].systemLanguage]);
				image.transform.SetParent(menuObjects[num].transform, worldPositionStays: false);
				image.transform.SetAsLastSibling();
			}
			else
			{
				TextTranslator componentInChildren = menuObjects[num].GetComponentInChildren<TextTranslator>();
				componentInChildren.enabled = false;
				componentInChildren.DisableCustomizedFont();
			}
		}
		menu.selectedIndex = startIndex;
	}

	private void ShowAboutMenu()
	{
		LinearMenu menu = null;
		List<MenuItem> list = new List<MenuItem>();
		list.Add(new MenuItem(CrossPlatform.GetAchievementsName(), ShowAchievementsMenu));
		list.Add(new MenuItem(I18n.STRINGS.credits, (Action)delegate
		{
			ui.AddUI(creditsPrefab.Clone());
		}));
		if (GameSettings.showThirdPartyLicenseInfo != GameSettings.ThirdPartyLicenseInfo.Hidden)
		{
			list.Add(new MenuItem(I18n.STRINGS.thirdPartyLicenses, ShowThirdPartyLicenses));
		}
		list.Add(new MenuItem(I18n.STRINGS.personalMessage, (Action)delegate
		{
			ui.AddUI(messagePrefab.Clone());
		}));
		list.Add(new MenuItem(I18n.STRINGS.version, (Action)delegate
		{
			GameObject obj = ui.AddUI(dialogBoxPrefab.Clone());
			VersionInfo versionInfo = VersionInfo.Load();
			UI.SetGenericText(obj, I18n.STRINGS.version + ": " + versionInfo.version + "\n" + I18n.STRINGS.buildNumber + ": " + versionInfo.buildNumber);
		}));
		if (PlayerPrefsAdapter.GetInt("BeatGame") != 0 || Application.isEditor)
		{
			Func<string> speedrunText = () => HighlightText(I18n.STRINGS.speedrunClock + ": " + ((!LevelController.speedrunClockActive) ? I18n.STRINGS.disabled : I18n.STRINGS.enabled));
			list.Add(new MenuItem(speedrunText(), delegate(BasicMenuItem element)
			{
				LevelController.speedrunClockActive = !LevelController.speedrunClockActive;
				UI.SetGenericText(element.gameObject, speedrunText());
				PositionMenu(menu.transform);
			}));
		}
		menu = BuildSimpleMenu(list);
	}

	private void ShowThirdPartyLicenses()
	{
		if (GameSettings.showThirdPartyLicenseInfo == GameSettings.ThirdPartyLicenseInfo.Popup)
		{
			thirdPartyLicensesPrefab.Clone();
		}
		else if (GameSettings.showThirdPartyLicenseInfo == GameSettings.ThirdPartyLicenseInfo.External)
		{
			CreateSubmenuPrompt(I18n.STRINGS.openNewWindow, I18n.STRINGS.letsDoIt, I18n.STRINGS.neverMind, delegate
			{
				Application.OpenURL((Application.isEditor ? (Application.dataPath + "/Resources/Data/") : (Application.dataPath + "/")) + "SoftwareLicenses.txt");
			}, delegate
			{
			});
		}
	}

	private void CreateSubmenuPrompt(string warningText, string[] options, Action[] actions)
	{
		LinearMenu submenu = null;
		List<MenuItem> list = new List<MenuItem>();
		for (int i = 0; i < options.Length; i++)
		{
			Action action = actions[i];
			list.Add(new MenuItem(options[i], (Action)delegate
			{
				action();
				submenu.Kill();
			}));
		}
		submenu = BuildSimpleMenu(list);
		GameObject obj = ui.CreateTextMenuItem(warningText);
		obj.transform.SetParent(submenu.transform, worldPositionStays: false);
		obj.transform.SetAsFirstSibling();
		LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
		(submenu.transform as RectTransform).CenterWithinParent();
	}

	private void CreateSubmenuPrompt(string warningText, string confirmText, string cancelText, Action confirmAction, Action cancelAction)
	{
		CreateSubmenuPrompt(warningText, new string[2] { confirmText, cancelText }, new Action[2] { confirmAction, cancelAction });
	}

	private void ShowControlsMenu()
	{
		List<MenuItem> list = new List<MenuItem>();
		list.Add(new MenuItem(I18n.STRINGS.gamepad, (Action)delegate
		{
			ui.AddUI(gamepadControlsPrefab.Clone());
		}));
		if (!GameSettings.restrictKeyboardSettings)
		{
			list.Add(new MenuItem(I18n.STRINGS.keyboard, (Action)delegate
			{
				ui.AddUI(keyboardControlsPrefab.Clone());
			}));
			list.Add(new MenuItem(I18n.STRINGS.rebindKeyboard, (Action)delegate
			{
				ui.AddUI(rebindPrefab.Clone());
			}));
			list.Add(new MenuItem(I18n.STRINGS.resetKeyboardBindings, (Action)delegate
			{
				KeyboardRemapper.ResetKeyboardMapping();
				this.RegisterTimer(0.01f, delegate
				{
					GameObject dialogue = ui.AddUI(dialogBoxPrefab.Clone());
					UI.SetGenericText(dialogue, I18n.STRINGS.keyBindingsReset);
					this.RegisterTimer(1f, delegate
					{
						if (dialogue != null)
						{
							UnityEngine.Object.Destroy(dialogue);
						}
					});
				});
			}));
		}
		if (!GameSettings.restrictControllerRemapping)
		{
			list.Add(new MenuItem(I18n.STRINGS.advancedController, (Action)delegate
			{
				controllerRemapperPrefab.Clone();
			}));
		}
		BuildSimpleMenu(list);
	}

	private void ShowCheatsMenu()
	{
		List<MenuItem> list = new List<MenuItem>();
		Cheats cheats = UnityEngine.Object.FindObjectOfType<Cheats>();
		string[] oPTION_MENU_CHEATS = Cheats.OPTION_MENU_CHEATS;
		foreach (string cheat in oPTION_MENU_CHEATS)
		{
			list.Add(new MenuItem(cheat, (Action)delegate
			{
				cheats.TriggerCheat(cheat);
			}));
		}
		BuildSimpleMenu(list);
	}

	private void UpdateItemStartingWith(LinearMenu menu, string startsWith, string text)
	{
		GameObject gameObject = menu.GetMenuObjects().FirstOrDefault(delegate(GameObject item)
		{
			TMP_Text componentInChildren = item.GetComponentInChildren<TMP_Text>();
			return componentInChildren != null && componentInChildren.text.Contains(startsWith);
		});
		if (!gameObject)
		{
			Debug.LogWarning("Could not find menu item with " + startsWith);
		}
		else
		{
			gameObject.GetComponentInChildren<TMP_Text>().text = text;
		}
	}
}
