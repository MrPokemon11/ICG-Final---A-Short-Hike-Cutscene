using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class I18n
{
	public enum Gender
	{
		Masculine = 0,
		Feminine = 1,
		Other = 2
	}

	public class Language
	{
		public string menuName;

		public string saveName;

		public SystemLanguage systemLanguage;

		public Func<string> loadFile;

		public Language(SystemLanguage language, string menuName)
		{
			this.menuName = menuName;
			systemLanguage = language;
			saveName = language.ToString();
			loadFile = delegate
			{
				string text = "Languages/LANG_" + language.ToString() + ".yarn_lines";
				Debug.Log("Loading resource... " + text);
				return Resources.Load<TextAsset>(text).text;
			};
		}

		public Language(string languageName, string filepath)
		{
			menuName = languageName.ToLower();
			saveName = languageName;
			if (Enum.TryParse<SystemLanguage>(languageName, out var result))
			{
				systemLanguage = result;
			}
			else
			{
				systemLanguage = SystemLanguage.Unknown;
			}
			loadFile = () => File.ReadAllText(filepath);
		}
	}

	public const string TAG_REGEX = "##([A-Za-z0-9]+)$";

	public const string DETECT_GENDER_TAG = "^\\[\\[([MFO])\\]\\]";

	public const string GENDER_SWAP_TAG = "^\\[\\[\\*([MFO])\\]\\]";

	public const string LANGUAGE_FILE_PREFX = "LANG_";

	public const string LANGUAGE_FILE_SUFFIX = ".yarn_lines.csv";

	public const string LANGUAGE_RESOURCE_SUFFIX = ".yarn_lines";

	public const string EDITOR_DATA_FOLDER = "/BuildData/";

	public const string FONTS_RESOURCE_FOLDER = "Fonts/";

	public const string DEFUALT_FONT = "Pixellari";

	public const string TMP_SUFFIX = "_TMP";

	private static Dictionary<SystemLanguage, string> BUILT_IN_LANGUAGES = new Dictionary<SystemLanguage, string>
	{
		{
			SystemLanguage.English,
			"english"
		},
		{
			SystemLanguage.Spanish,
			"español"
		},
		{
			SystemLanguage.French,
			"français"
		},
		{
			SystemLanguage.Portuguese,
			"português"
		},
		{
			SystemLanguage.Japanese,
			"japanese"
		}
	};

	private static Dictionary<Gender, string> GENDER_TO_SYMBOL = new Dictionary<Gender, string>
	{
		{
			Gender.Masculine,
			"M"
		},
		{
			Gender.Feminine,
			"F"
		},
		{
			Gender.Other,
			"O"
		}
	};

	private static Dictionary<string, Gender> SYMBOL_TO_GENDER = new Dictionary<string, Gender>
	{
		{
			"M",
			Gender.Masculine
		},
		{
			"F",
			Gender.Feminine
		},
		{
			"O",
			Gender.Other
		}
	};

	public static I18nStrings STRINGS = new I18nStrings();

	public static int englishFontSize = 16;

	public static TMP_FontAsset englishFontTMP = Resources.Load<TMP_FontAsset>("Fonts/Pixellari_TMP");

	public static Font englishFontUnity = Resources.Load<Font>("Fonts/Pixellari");

	public static bool customFont = false;

	public static int customFontSpacing = 0;

	public static Vector4 customMargins = Vector4.zero;

	public static TMP_FontAsset customFontTMP;

	public static Font customFontUnity;

	public static int customFontSize;

	private static Dictionary<string, string> languageTable;

	private static Language[] languages;

	public static Gender currentGender { get; private set; }

	public static event Action onLanguageChanged;

	public static Language[] GetLanguages()
	{
		if (languages == null)
		{
			languages = FindLanguages().ToArray();
		}
		return languages;
	}

	private static IEnumerable<Language> FindLanguages()
	{
		foreach (KeyValuePair<SystemLanguage, string> bUILT_IN_LANGUAGE in BUILT_IN_LANGUAGES)
		{
			yield return new Language(bUILT_IN_LANGUAGE.Key, bUILT_IN_LANGUAGE.Value);
		}
		if (GameSettings.isGameConsole || Application.isMobilePlatform)
		{
			yield break;
		}
		string path = (Application.isEditor ? (Application.dataPath + "/BuildData/") : (Application.dataPath + "/"));
		string[] files = Directory.GetFiles(path);
		foreach (string text in files)
		{
			Match match = Regex.Match(Path.GetFileName(text), "^LANG_([A-Za-z0-9]+)\\.yarn_lines\\.csv$");
			if (match.Success)
			{
				yield return new Language(match.Groups[1].Value, text);
			}
		}
	}

	public static string RemoveLocalizationTag(string text)
	{
		Match match = Regex.Match(text, "##([A-Za-z0-9]+)$");
		if (match.Success)
		{
			return text.Substring(0, match.Index).TrimEnd();
		}
		return text;
	}

	public static string Localize(string text)
	{
		Match match = Regex.Match(text, "##([A-Za-z0-9]+)$");
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			if (languageTable != null && languageTable.TryGetValue(value, out var value2))
			{
				return HandleGenderLocalization(value2, value);
			}
			return text.Substring(0, match.Index).TrimEnd();
		}
		if (languageTable != null && languageTable.TryGetValue(text, out var value3))
		{
			return HandleGenderLocalization(value3, text);
		}
		return text;
	}

	private static string HandleGenderLocalization(string text, string id)
	{
		if (text.StartsWith("[["))
		{
			Match match = Regex.Match(text, "^\\[\\[([MFO])\\]\\]");
			if (match.Success)
			{
				Gender gender = SYMBOL_TO_GENDER[match.Groups[1].Value];
				if (currentGender != gender)
				{
					currentGender = gender;
				}
				return Regex.Replace(text, "^\\[\\[([MFO])\\]\\]", "");
			}
			return text;
		}
		return UpdateTextGender(text, id);
	}

	public static string UpdateTextGender(string text, string id)
	{
		if (currentGender != Gender.Masculine)
		{
			string key = $"[[{GENDER_TO_SYMBOL[currentGender]}]]{id}";
			if (languageTable.ContainsKey(key))
			{
				return languageTable[key];
			}
		}
		return text;
	}

	public static IWaitable SetLanguage(Language language, float forceWaitTime = 0f)
	{
		Dictionary<string, string> dictionary = LoadStringTable(language.loadFile());
		if (dictionary == null)
		{
			return null;
		}
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			if (item.Key.StartsWith("line:"))
			{
				dictionary2.Add(item.Key, item.Value);
				continue;
			}
			I18nStrings.UpdateString(item.Key, item.Value);
			dictionary3.Add(item.Key, item.Value);
		}
		DialogueController.LoadLanguageFile(dictionary2);
		languageTable = dictionary3;
		currentGender = Gender.Masculine;
		bool flag = false;
		string key = null;
		int _customFontSize = -1;
		int _customFontSpacing = -1;
		Vector4 _customMargins = default(Vector4);
		if (dictionary3.ContainsKey("FONT"))
		{
			string[] array = dictionary3["FONT"].Split(':');
			if (array.Length >= 2 && int.TryParse(array[1], out var result))
			{
				flag = true;
				_customFontSize = result;
				key = array[0];
				if (array.Length >= 3 && int.TryParse(array[2], out var result2))
				{
					_customFontSpacing = result2;
				}
				if (array.Length >= 4)
				{
					string[] array2 = array[3].Split('*');
					for (int i = 0; i < 4; i++)
					{
						if (i < array2.Length && int.TryParse(array2[i], out var result3))
						{
							_customMargins[i] = result3;
						}
					}
				}
			}
		}
		FlagWaitable waitable = new FlagWaitable();
		if (flag)
		{
			AsyncOperationHandle<IList<UnityEngine.Object>> async = Addressables.LoadAssetsAsync<UnityEngine.Object>(key, null);
			IWaitable[] waitables = MinimumTimeWaitable(forceWaitTime, new AsyncOperationHandleWaitable(async));
			WaitFor.WithCoroutine(Singleton<ServiceLocator>.instance, delegate
			{
				if (async.Status == AsyncOperationStatus.Succeeded)
				{
					customFontUnity = async.Result.FirstOrDefault((UnityEngine.Object e) => e is Font) as Font;
					customFontTMP = async.Result.FirstOrDefault((UnityEngine.Object e) => e is TMP_FontAsset) as TMP_FontAsset;
					customFontSize = _customFontSize;
					customFontSpacing = _customFontSpacing;
					customMargins = _customMargins;
					customFont = true;
					I18n.onLanguageChanged?.Invoke();
					waitable.Finish();
				}
			}, waitables);
		}
		else
		{
			customFont = false;
			I18n.onLanguageChanged?.Invoke();
			waitable.Finish();
		}
		return waitable;
	}

	private static IWaitable[] MinimumTimeWaitable(float time, IWaitable waitable)
	{
		if (!(time > 0f))
		{
			return new IWaitable[1] { waitable };
		}
		return new IWaitable[2]
		{
			waitable,
			new TimerWaitable(Timer.Register(time, delegate
			{
			}))
		};
	}

	public static Dictionary<string, string> LoadStringTable(string text)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		try
		{
			
			CsvHelper.Configuration.Configuration configuration = new CsvHelper.Configuration.Configuration()
			{
				CultureInfo = CultureInfo.InvariantCulture
			};
			using StringReader reader = new StringReader(text);
			//using CsvReader csvReader = new CsvReader(reader, configuration);
			using CsvReader csvReader = new CsvReader(reader, true);
			csvReader.ReadHeader();
			while (csvReader.Read())
			{
				dictionary[csvReader.GetField("LineCode")] = csvReader.GetField("LineText");
			}
			return dictionary;
		}
		catch (CsvHelper.MissingFieldException exception)
		{
			ShowException(exception, STRINGS.headerIsMissing);
			return null;
		}
		catch (Exception exception2)
		{
			ShowException(exception2, STRINGS.fileReadError);
			return null;
		}
	}

	private static void ShowException(Exception exception, string dialogue)
	{
		Debug.LogException(exception);
		UI uI = Singleton<ServiceLocator>.instance.Locate<UI>(allowFail: true);
		if ((bool)uI)
		{
			uI.CreateSimpleDialogue(dialogue);
		}
	}
}
