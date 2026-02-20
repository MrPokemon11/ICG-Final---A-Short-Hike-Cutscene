using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class DialogueController : ServiceMonoBehaviour
{
	public class PreloadLoaderLogger : LoaderLogger
	{
		public Yarn.Logger LogDebugMessage => delegate(string message)
		{
			Debug.Log(message);
		};

		public Yarn.Logger LogErrorMessage => delegate(string message)
		{
			Debug.LogError(message);
		};
	}

	private class YarnDialogueForwarder : DialogueUIBehaviour
	{
		public IConversation conversation;

		public override IEnumerator RunLine(Line line)
		{
			string text = TextReplacer.ReplaceVariables(I18n.UpdateTextGender(line.text, line.id));
			if (!string.IsNullOrWhiteSpace(text))
			{
				yield return conversation.ShowLine(text);
			}
		}

		public override IEnumerator RunCommand(Command command)
		{
			string[] array = command.text.Trim().Split(' ');
			YarnScripting<IConversation>.YarnCommandHandler command2 = YarnScripting<IConversation>.GetCommand(array[0]);
			if (command2 != null)
			{
				yield return command2(conversation, array.Skip(1).ToArray());
			}
			else
			{
				Debug.LogError("No command found for: " + command.text);
			}
		}

		public override IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser)
		{
			List<string> options = optionsCollection.options.Select((Line o) => TextReplacer.ReplaceVariables(I18n.UpdateTextGender(o.text, o.id))).ToList();
			yield return conversation.ShowOptions(options, delegate(int index)
			{
				optionChooser(index);
			});
		}

		public override IEnumerator DialogueComplete()
		{
			conversation.Kill();
			yield break;
		}
	}

	private static Program preloadedDialogue;

	private static Dictionary<string, string> preloadLanguageTable;

	public GameObject dialogueRunnerPrefab;

	private DialogueRunner runner;

	private YarnDialogueForwarder yarnDialogueReciever;

	private TextBoxConversation currentConversation;

	public static Library CreateDummyLibrary()
	{
		Library library = new Library();
		library.ImportLibrary(new Dialogue.StandardLibrary());
		library.RegisterFunction("visited", -1, (Value[] p) => false);
		library.RegisterFunction("visitCount", -1, (Value[] p) => 0);
		YarnScripting<IConversation>.Initalize(typeof(YarnCommands), typeof(YarnFunctions));
		foreach (YarnScripting<IConversation>.YarnFunction allFunction in YarnScripting<IConversation>.GetAllFunctions())
		{
			library.RegisterFunction(allFunction.name, allFunction.parameters, delegate
			{
			});
		}
		return library;
	}

	public static void PreloadDialogue(string text, string filename, Library library, bool showTokens = false, bool showParseTree = false, string onlyConsiderNode = null)
	{
		Loader loader = new Loader(new PreloadLoaderLogger());
		NodeFormat format = Dialogue.DetermineNodeFormat(text);
		preloadedDialogue = loader.Load(text, library, filename, null, showTokens, showParseTree, onlyConsiderNode, format);
	}

	public void Start()
	{
		YarnScripting<IConversation>.Initalize(typeof(YarnCommands), typeof(YarnFunctions));
		runner = Object.FindObjectOfType<DialogueRunner>();
		if (!runner)
		{
			GameObject gameObject = dialogueRunnerPrefab.Clone();
			Object.DontDestroyOnLoad(gameObject);
			runner = gameObject.GetComponent<DialogueRunner>();
			if (preloadedDialogue != null)
			{
				runner.proloadedProgram = preloadedDialogue;
				preloadedDialogue = null;
			}
			YarnToGlobalDataAdapter variableStorage = gameObject.AddComponent<YarnToGlobalDataAdapter>();
			runner.variableStorage = variableStorage;
			foreach (YarnScripting<IConversation>.YarnFunction function in YarnScripting<IConversation>.GetAllFunctions())
			{
				runner.dialogue.library.RegisterFunction(function.name, function.parameters, delegate(Value[] parameters)
				{
					DialogueController dialogueController = Singleton<ServiceLocator>.instance.Locate<DialogueController>();
					return function.handler(dialogueController.currentConversation, parameters);
				});
			}
			if (preloadLanguageTable != null)
			{
				runner.AddStringTable(preloadLanguageTable);
				preloadLanguageTable = null;
			}
		}
		yarnDialogueReciever = base.gameObject.AddComponent<YarnDialogueForwarder>();
		runner.dialogueUI = yarnDialogueReciever;
	}

	public static void LoadLanguageFile(Dictionary<string, string> text)
	{
		DialogueController dialogueController = Singleton<ServiceLocator>.instance.Locate<DialogueController>();
		if (dialogueController == null || dialogueController.runner == null)
		{
			preloadLanguageTable = text;
		}
		else
		{
			dialogueController.runner.AddStringTable(text);
		}
	}

	public IConversation StartConversation(string startNode, Transform speaker)
	{
		if (string.IsNullOrEmpty(startNode))
		{
			Debug.LogWarning("Trying to start an empty node!");
			return null;
		}
		if (currentConversation != null)
		{
			currentConversation.Kill();
		}
		currentConversation = new TextBoxConversation(speaker);
		yarnDialogueReciever.conversation = currentConversation;
		runner.StartDialogue(startNode);
		return currentConversation;
	}
}
