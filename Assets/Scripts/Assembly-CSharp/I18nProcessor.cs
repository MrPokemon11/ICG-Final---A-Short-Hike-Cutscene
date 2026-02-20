using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using UnityEngine;
using Yarn;

public class I18nProcessor
{
	public const string CHOICE_TEXT_REGEX = "\\[\\[(.+)\\|.+\\]\\]";

	public const string CHOICE_QUICK_TEXT_REGEX = "->(.+)";

	public const string TAG_REGEX = "##(line:.+)";

	public const string NODE_TITLE_REGEX = "^title: ([A-Za-z0-9]+)";

	public static void ShowMenu()
	{
		UI ui = Singleton<ServiceLocator>.instance.Locate<UI>();
		LinearMenu menu = null;
		menu = ui.CreateSimpleMenu(new string[2] { "convert fan translation to new format", "nevermind" }, new Action[2]
		{
			delegate
			{
				TextAsset textAsset = Resources.Load<TextAsset>("Languages/MountainQuest_Cleaned_Tagged.yarn");
				string path = Application.dataPath + "/Translation.yarn.txt";
				if (!File.Exists(path))
				{
					ui.CreateSimpleDialogue(string.Format("Place tranlsation in:\n{0}\nRename it to:\n{1}", "A Short Hike_Data", Path.GetFileName(path)));
				}
				else
				{
					List<(string, string, string)> entries = BuildLanguageFileFromFanTranslation(File.ReadAllText(path), textAsset.text);
					AppendOrOverwriteTagFile(Application.dataPath + "/_LANG_Custom.yarn_lines.csv", entries, Application.persistentDataPath + "/Translation_Converted.csv");
					ui.CreateSimpleDialogue("Finished!\nOverwrote _LANG_Custom.yarn_lines.csv");
				}
			},
			delegate
			{
				menu.Kill();
			}
		});
		menu.transform.localPosition = Vector3.zero;
	}

	public static void AppendOrOverwriteTagFile(string filepath, List<(string, string)> entries, string outputPath = null)
	{
		AppendOrOverwriteTagFile(filepath, entries.Select(((string, string) e) => ((string, string, string))(e.Item1, e.Item2, null)).ToList(), outputPath);
	}

	public static void AppendOrOverwriteTagFile(string filepath, List<(string, string, string)> entries, string outputPath = null)
	{
		AppendOrOverwriteTagFile(filepath, entries.Select(delegate((string, string, string) e)
		{
			LocalisedLineWithMetadata localisedLineWithMetadata = new LocalisedLineWithMetadata();
			(localisedLineWithMetadata.LineCode, localisedLineWithMetadata.LineText, localisedLineWithMetadata.Comment) = e;
			return localisedLineWithMetadata;
		}).ToList(), outputPath);
	}

	public static void AppendOrOverwriteTagFile(string filepath, List<LocalisedLineWithMetadata> entries, string outputPath = null)
	{
		List<LocalisedLineWithMetadata> recordsFromCSV = GetRecordsFromCSV(filepath);
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int i = 0; i < recordsFromCSV.Count; i++)
		{
			dictionary.Add(recordsFromCSV[i].LineCode, i);
		}
		foreach (LocalisedLineWithMetadata entry in entries)
		{
			if (dictionary.ContainsKey(entry.LineCode))
			{
				if (!string.IsNullOrEmpty(entry.LineText))
				{
					recordsFromCSV[dictionary[entry.LineCode]].LineText = entry.LineText;
				}
				if (!string.IsNullOrEmpty(entry.Comment))
				{
					recordsFromCSV[dictionary[entry.LineCode]].Comment = entry.Comment;
				}
				if (!string.IsNullOrEmpty(entry.OriginalText))
				{
					recordsFromCSV[dictionary[entry.LineCode]].OriginalText = entry.OriginalText;
				}
				if (!string.IsNullOrEmpty(entry.Speaker))
				{
					recordsFromCSV[dictionary[entry.LineCode]].Speaker = entry.Speaker;
				}
				if (!string.IsNullOrEmpty(entry.StoryNode))
				{
					recordsFromCSV[dictionary[entry.LineCode]].StoryNode = entry.StoryNode;
				}
			}
			else
			{
				recordsFromCSV.Add(entry);
			}
		}
		if (outputPath == null)
		{
			outputPath = filepath;
		}
		Debug.Log("Saving to " + outputPath);
		using StreamWriter writer = new StreamWriter(outputPath);
		using CsvWriter csvWriter = new CsvWriter(writer);
		csvWriter.Configuration.Delimiter = ",";
		csvWriter.Configuration.TrimFields = true;
		csvWriter.WriteHeader<LocalisedLineWithMetadata>();
		csvWriter.WriteRecords(recordsFromCSV);
	}

	public static List<LocalisedLineWithMetadata> GetRecordsFromCSV(string filepath)
	{
		List<LocalisedLineWithMetadata> list = new List<LocalisedLineWithMetadata>();
		if (File.Exists(filepath))
		{
			using StringReader reader = new StringReader(File.ReadAllText(filepath));
			using CsvReader csvReader = new CsvReader(reader);
			csvReader.ReadHeader();
			while (csvReader.Read())
			{
				LocalisedLineWithMetadata localisedLineWithMetadata = new LocalisedLineWithMetadata();
				localisedLineWithMetadata.LineCode = csvReader.GetField("LineCode");
				localisedLineWithMetadata.LineText = csvReader.GetField("LineText");
				if (csvReader.TryGetField("Comment", out string field))
				{
					localisedLineWithMetadata.Comment = field;
				}
				if (csvReader.TryGetField("OriginalText", out field))
				{
					localisedLineWithMetadata.OriginalText = field;
				}
				if (csvReader.TryGetField("Speaker", out field))
				{
					localisedLineWithMetadata.Speaker = field;
				}
				if (csvReader.TryGetField("StoryNode", out field))
				{
					localisedLineWithMetadata.StoryNode = field;
				}
				list.Add(localisedLineWithMetadata);
			}
		}
		return list;
	}

	public static void ProcessChoiceMetadata(List<LocalisedLineWithMetadata> records, bool overwriteOriginalText = true)
	{
		foreach (LocalisedLineWithMetadata record in records)
		{
			if (record.OriginalText.StartsWith("->") || record.OriginalText.StartsWith("[["))
			{
				record.Speaker = "Player";
				record.Comment = "Dialogue choice.";
			}
			if (overwriteOriginalText)
			{
				record.OriginalText = record.LineText;
			}
		}
	}

	public static List<LocalisedLineWithMetadata> CompileMetadataForTags(string englishText)
	{
		List<LocalisedLineWithMetadata> list = new List<LocalisedLineWithMetadata>();
		string pattern = "<<SetSpeaker ([A-Za-z0-9]+)>>";
		string[] array = Regex.Split(englishText, "^title: ([A-Za-z0-9]+)", RegexOptions.Multiline);
		for (int i = 1; i < array.Length; i += 2)
		{
			string storyNode = array[i];
			string[] array2 = array[i + 1].Split('\n');
			bool flag = false;
			string speaker = "Original";
			for (int j = 0; j < array2.Length; j++)
			{
				Match match = Regex.Match(array2[j], pattern);
				if (match.Success)
				{
					speaker = match.Groups[1].Value;
				}
				else if (flag && !ShouldSkipLine(array2[j]))
				{
					Match match2 = Regex.Match(array2[j], "##(line:.+)");
					string originalText = Regex.Replace(array2[j].Trim(), "##(line:.+)", "");
					LocalisedLineWithMetadata localisedLineWithMetadata = new LocalisedLineWithMetadata
					{
						LineCode = match2.Groups[1].Value.Trim(),
						OriginalText = originalText
					};
					localisedLineWithMetadata.StoryNode = storyNode;
					localisedLineWithMetadata.Speaker = speaker;
					list.Add(localisedLineWithMetadata);
				}
				else if (array2[j].Trim() == "---")
				{
					flag = true;
				}
			}
		}
		return list;
	}

	private static bool ShouldSkipLine(string line)
	{
		line = line.Trim();
		if (line.StartsWith("<<") || line.StartsWith("===") || line == "")
		{
			return true;
		}
		if (line.StartsWith("[[") && !Regex.Match(line, "\\[\\[(.+)\\|.+\\]\\]").Success)
		{
			return true;
		}
		return false;
	}

	public static List<(string, string, string)> BuildLanguageFileFromFanTranslation(string fanText, string englishText)
	{
		Dictionary<string, List<(string, string)>> dictionary = new Dictionary<string, List<(string, string)>>();
		string[] array = Regex.Split(englishText, "^title: ([A-Za-z0-9]+)", RegexOptions.Multiline);
		for (int i = 1; i < array.Length; i += 2)
		{
			string key = array[i];
			dictionary[key] = new List<(string, string)>();
			string[] array2 = array[i + 1].Split('\n');
			bool flag = false;
			for (int j = 0; j < array2.Length; j++)
			{
				string text = array2[j].Trim();
				if (flag && !ShouldSkipLine(text))
				{
					Match match = Regex.Match(text, "##(line:.+)");
					string item = Regex.Replace(text.Trim(), "##(line:.+)", "");
					dictionary[key].Add((match.Groups[1].Value, item));
				}
				else if (text == "---")
				{
					flag = true;
				}
			}
		}
		Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
		array = Regex.Split(fanText, "^title: ([A-Za-z0-9]+)", RegexOptions.Multiline);
		for (int k = 1; k < array.Length; k += 2)
		{
			string key2 = array[k];
			dictionary2[key2] = new List<string>();
			string[] array3 = array[k + 1].Split('\n');
			bool flag2 = false;
			for (int l = 0; l < array3.Length; l++)
			{
				string text2 = array3[l].Trim();
				if (flag2 && !ShouldSkipLine(text2))
				{
					text2 = TrimChoiceNotation(text2);
					dictionary2[key2].Add(text2);
				}
				else if (text2 == "---")
				{
					flag2 = true;
				}
			}
		}
		Dictionary<string, (string, string)> dictionary3 = new Dictionary<string, (string, string)>();
		foreach (KeyValuePair<string, List<(string, string)>> item2 in dictionary)
		{
			if (!dictionary2.ContainsKey(item2.Key))
			{
				Debug.LogError("FAN NODE DOES NOT HAVE " + item2.Key);
				continue;
			}
			List<string> list = dictionary2[item2.Key];
			for (int m = 0; m < item2.Value.Count; m++)
			{
				if (m >= list.Count)
				{
					Debug.LogError("FAN LINES HAVE NO TRANSLATION FOR NODE " + item2.Key + " ON LINE " + m);
					break;
				}
				if (!dictionary3.ContainsKey(item2.Value[m].Item1))
				{
					dictionary3.Add(item2.Value[m].Item1, (list[m], item2.Value[m].Item2));
				}
			}
		}
		return dictionary3.Select((KeyValuePair<string, (string, string)> pair) => (Key: pair.Key, pair.Value.Item1, pair.Value.Item2)).ToList();
	}

	public static string TrimChoiceNotation(string line)
	{
		Match match = Regex.Match(line, "\\[\\[(.+)\\|.+\\]\\]");
		if (match.Success)
		{
			line = match.Groups[1].Value.Trim();
		}
		else
		{
			match = Regex.Match(line, "->(.+)");
			if (match.Success)
			{
				line = match.Groups[1].Value.Trim();
			}
		}
		return line;
	}

	private static int CountWords(string input)
	{
		char[] separator = new char[3] { ' ', '\r', '\n' };
		return input.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
	}

	public static void CompileFriendlyWorksheet()
	{
		List<LocalisedLine> list;
		using (StringReader reader = new StringReader(I18n.GetLanguages().First((I18n.Language l) => l.systemLanguage == SystemLanguage.English).loadFile()))
		{
			using CsvReader csvReader = new CsvReader(reader);
			list = csvReader.GetRecords<LocalisedLine>().ToList();
		}
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		foreach (LocalisedLine item in list)
		{
			string key = item.LineText.Trim();
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new List<string>());
			}
			dictionary[key].Add(item.LineCode);
		}
		Debug.Log("total words: " + list.Aggregate(0, (int sum, LocalisedLine next) => sum + CountWords(next.LineText)));
		Debug.Log("unique lines: " + dictionary.Count);
		Debug.Log("unique phrases: " + dictionary.Aggregate(0, (int sum, KeyValuePair<string, List<string>> next) => sum + CountWords(next.Key)));
		foreach (KeyValuePair<string, List<string>> item2 in dictionary)
		{
			if (item2.Value.Count > 1)
			{
				Debug.Log(item2.Key + " " + item2.Value.Count);
			}
		}
	}
}
