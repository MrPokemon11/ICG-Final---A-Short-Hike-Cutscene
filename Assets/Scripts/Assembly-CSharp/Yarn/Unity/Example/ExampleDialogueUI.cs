using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Yarn.Unity.Example
{
	public class ExampleDialogueUI : DialogueUIBehaviour
	{
		public GameObject dialogueContainer;

		public Text lineText;

		public GameObject continuePrompt;

		private OptionChooser SetSelectedOption;

		[Tooltip("How quickly to show the text, in seconds per character")]
		public float textSpeed = 0.025f;

		public List<Button> optionButtons;

		public RectTransform gameControlsContainer;

		private void Awake()
		{
			if (dialogueContainer != null)
			{
				dialogueContainer.SetActive(value: false);
			}
			lineText.gameObject.SetActive(value: false);
			foreach (Button optionButton in optionButtons)
			{
				optionButton.gameObject.SetActive(value: false);
			}
			if (continuePrompt != null)
			{
				continuePrompt.SetActive(value: false);
			}
		}

		public override IEnumerator RunLine(Line line)
		{
			lineText.gameObject.SetActive(value: true);
			if (textSpeed > 0f)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = line.text;
				foreach (char value in text)
				{
					stringBuilder.Append(value);
					lineText.text = stringBuilder.ToString();
					yield return new WaitForSeconds(textSpeed);
				}
			}
			else
			{
				lineText.text = line.text;
			}
			if (continuePrompt != null)
			{
				continuePrompt.SetActive(value: true);
			}
			while (!Input.anyKeyDown)
			{
				yield return null;
			}
			lineText.gameObject.SetActive(value: false);
			if (continuePrompt != null)
			{
				continuePrompt.SetActive(value: false);
			}
		}

		public override IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser)
		{
			if (optionsCollection.options.Count > optionButtons.Count)
			{
				Debug.LogWarning("There are more options to present than there arebuttons to present them in. This will cause problems.");
			}
			int num = 0;
			foreach (Line option in optionsCollection.options)
			{
				optionButtons[num].gameObject.SetActive(value: true);
				optionButtons[num].GetComponentInChildren<Text>().text = option.text;
				num++;
			}
			SetSelectedOption = optionChooser;
			while (SetSelectedOption != null)
			{
				yield return null;
			}
			foreach (Button optionButton in optionButtons)
			{
				optionButton.gameObject.SetActive(value: false);
			}
		}

		public void SetOption(int selectedOption)
		{
			SetSelectedOption(selectedOption);
			SetSelectedOption = null;
		}

		public override IEnumerator RunCommand(Command command)
		{
			Debug.Log("Command: " + command.text);
			yield break;
		}

		public override IEnumerator DialogueStarted()
		{
			Debug.Log("Dialogue starting!");
			if (dialogueContainer != null)
			{
				dialogueContainer.SetActive(value: true);
			}
			if (gameControlsContainer != null)
			{
				gameControlsContainer.gameObject.SetActive(value: false);
			}
			yield break;
		}

		public override IEnumerator DialogueComplete()
		{
			Debug.Log("Complete!");
			if (dialogueContainer != null)
			{
				dialogueContainer.SetActive(value: false);
			}
			if (gameControlsContainer != null)
			{
				gameControlsContainer.gameObject.SetActive(value: true);
			}
			yield break;
		}
	}
}
