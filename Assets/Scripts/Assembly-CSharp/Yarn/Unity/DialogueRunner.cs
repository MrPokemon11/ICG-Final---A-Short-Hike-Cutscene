using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CsvHelper;
using UnityEngine;

namespace Yarn.Unity
{
	[AddComponentMenu("Scripts/Yarn Spinner/Dialogue Runner")]
	public class DialogueRunner : MonoBehaviour
	{
		public TextAsset[] sourceText;

		public LocalisedStringGroup[] stringGroups;

		public bool shouldOverrideLanguage;

		public SystemLanguage overrideLanguage = SystemLanguage.English;

		public VariableStorageBehaviour variableStorage;

		public DialogueUIBehaviour dialogueUI;

		public string startNode = "Start";

		public bool startAutomatically = true;

		public bool automaticCommands = true;

		private Dialogue _dialogue;

		internal Program proloadedProgram;

		public bool isDialogueRunning { get; private set; }

		public Dialogue dialogue
		{
			get
			{
				if (_dialogue == null)
				{
					_dialogue = new Dialogue(variableStorage);
					_dialogue.LogDebugMessage = delegate(string message)
					{
						Debug.Log(message);
					};
					_dialogue.LogErrorMessage = delegate(string message)
					{
						Debug.LogError(message);
					};
				}
				return _dialogue;
			}
			set
			{
				_dialogue = value;
			}
		}

		public string currentNodeName => dialogue.currentNode;

		private void Start()
		{
			if (dialogueUI == null)
			{
				Debug.LogError("Implementation was not set! Can't run the dialogue!");
				return;
			}
			if (variableStorage == null)
			{
				Debug.LogError("Variable storage was not set! Can't run the dialogue!");
				return;
			}
			variableStorage.ResetToDefaults();
			if (proloadedProgram != null)
			{
				dialogue.program = proloadedProgram;
			}
			else if (sourceText != null)
			{
				TextAsset[] array = sourceText;
				foreach (TextAsset textAsset in array)
				{
					dialogue.LoadString(textAsset.text, textAsset.name);
				}
			}
			if (startAutomatically)
			{
				StartDialogue();
			}
			if (stringGroups == null)
			{
				return;
			}
			LocalisedStringGroup localisedStringGroup = new List<LocalisedStringGroup>(stringGroups).Find((LocalisedStringGroup entry) => entry.language == (shouldOverrideLanguage ? overrideLanguage : Application.systemLanguage));
			if (localisedStringGroup != null)
			{
				TextAsset[] array = localisedStringGroup.stringFiles;
				foreach (TextAsset textAsset2 in array)
				{
					AddStringTable(textAsset2.text);
				}
			}
		}

		public void AddScript(string text)
		{
			dialogue.LoadString(text);
		}

		public void AddScript(TextAsset asset)
		{
			dialogue.LoadString(asset.text);
		}

		public void AddStringTable(Dictionary<string, string> stringTable)
		{
			dialogue.AddStringTable(stringTable);
		}

		public void AddStringTable(string text)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			using (StringReader reader = new StringReader(text))
			{
				using CsvReader csvReader = new CsvReader(reader);
				foreach (LocalisedLine record in csvReader.GetRecords<LocalisedLine>())
				{
					dictionary[record.LineCode] = record.LineText;
				}
			}
			AddStringTable(dictionary);
		}

		public void AddStringTable(TextAsset text)
		{
			AddStringTable(text.text);
		}

		public void ResetDialogue()
		{
			variableStorage.ResetToDefaults();
			StartDialogue();
		}

		public void StartDialogue()
		{
			StartDialogue(startNode);
		}

		public void StartDialogue(string startNode)
		{
			StopAllCoroutines();
			dialogueUI.StopAllCoroutines();
			StartCoroutine(RunDialogue(startNode));
		}

		private IEnumerator RunDialogue(string startNode = "Start")
		{
			isDialogueRunning = true;
			yield return StartCoroutine(dialogueUI.DialogueStarted());
			foreach (Dialogue.RunnerResult item in dialogue.Run(startNode))
			{
				if (item is Dialogue.LineResult)
				{
					Dialogue.LineResult lineResult = item as Dialogue.LineResult;
					yield return StartCoroutine(dialogueUI.RunLine(lineResult.line));
				}
				else if (item is Dialogue.OptionSetResult)
				{
					Dialogue.OptionSetResult optionSetResult = item as Dialogue.OptionSetResult;
					yield return StartCoroutine(dialogueUI.RunOptions(optionSetResult.options, optionSetResult.setSelectedOptionDelegate));
				}
				else if (item is Dialogue.CommandResult)
				{
					Dialogue.CommandResult commandResult = item as Dialogue.CommandResult;
					if (!DispatchCommand(commandResult.command.text))
					{
						yield return StartCoroutine(dialogueUI.RunCommand(commandResult.command));
					}
				}
				else if (item is Dialogue.NodeCompleteResult)
				{
					Dialogue.NodeCompleteResult nodeCompleteResult = item as Dialogue.NodeCompleteResult;
					yield return StartCoroutine(dialogueUI.NodeComplete(nodeCompleteResult.nextNode));
				}
			}
			yield return StartCoroutine(dialogueUI.DialogueComplete());
			isDialogueRunning = false;
		}

		public void Clear()
		{
			if (isDialogueRunning)
			{
				throw new InvalidOperationException("You cannot clear the dialogue system while a dialogue is running.");
			}
			dialogue.UnloadAll();
		}

		public void Stop()
		{
			isDialogueRunning = false;
			dialogue.Stop();
		}

		public bool NodeExists(string nodeName)
		{
			return dialogue.NodeExists(nodeName);
		}

		public bool DispatchCommand(string command)
		{
			string[] array = command.Split(' ');
			if (array.Length < 2)
			{
				return false;
			}
			string text = array[0];
			_ = array[1];
			GameObject gameObject = null;
			if (gameObject == null)
			{
				return false;
			}
			int num = 0;
			List<string[]> list = new List<string[]>();
			List<string> list2;
			if (array.Length > 2)
			{
				list2 = new List<string>(array);
				list2.RemoveRange(0, 2);
			}
			else
			{
				list2 = new List<string>();
			}
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in components)
			{
				MethodInfo[] methods = monoBehaviour.GetType().GetMethods();
				foreach (MethodInfo methodInfo in methods)
				{
					YarnCommandAttribute[] array2 = (YarnCommandAttribute[])methodInfo.GetCustomAttributes(typeof(YarnCommandAttribute), inherit: true);
					for (int k = 0; k < array2.Length; k++)
					{
						if (!(array2[k].commandString == text))
						{
							continue;
						}
						ParameterInfo[] parameters = methodInfo.GetParameters();
						bool flag = false;
						if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(string[])))
						{
							string[][] array3 = new string[1][] { list2.ToArray() };
							object[] parameters2 = array3;
							methodInfo.Invoke(monoBehaviour, parameters2);
							num++;
							flag = true;
						}
						else if (parameters.Length == list2.Count)
						{
							flag = true;
							ParameterInfo[] array4 = parameters;
							for (int l = 0; l < array4.Length; l++)
							{
								if (!array4[l].ParameterType.IsAssignableFrom(typeof(string)))
								{
									Debug.LogErrorFormat(gameObject, "Method \"{0}\" wants to respond to Yarn command \"{1}\", but not all of its parameters are strings!", methodInfo.Name, text);
									flag = false;
									break;
								}
							}
							if (flag)
							{
								object[] parameters2 = list2.ToArray();
								methodInfo.Invoke(monoBehaviour, parameters2);
								num++;
							}
						}
						if (!flag)
						{
							list.Add(new string[4]
							{
								methodInfo.Name,
								text,
								parameters.Length.ToString(),
								list2.Count.ToString()
							});
						}
					}
				}
			}
			if (num > 1)
			{
				Debug.LogWarningFormat(gameObject, "The command \"{0}\" found {1} targets. You should only have one - check your scripts.", command, num);
			}
			else if (num == 0)
			{
				foreach (string[] item in list)
				{
					Debug.LogErrorFormat(gameObject, "Method \"{0}\" wants to respond to Yarn command \"{1}\", but it has a different number of parameters ({2}) to those provided ({3}), or is not a string array!", item[0], item[1], item[2], item[3]);
				}
			}
			return num > 0;
		}
	}
}
