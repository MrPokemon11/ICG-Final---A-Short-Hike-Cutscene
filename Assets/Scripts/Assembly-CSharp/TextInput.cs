using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextInput : MonoBehaviour
{
	public TMP_Text prompt;

	public TMP_Text typedText;

	private int maxLength = 3;

	private Action<string> onFinish;

	private string input;

	public void Setup(string prompt, int maxLength, Action<string> onFinish)
	{
		this.prompt.text = prompt;
		this.maxLength = maxLength;
		this.onFinish = onFinish;
	}

	private void Update()
	{
		if (Input.inputString.Contains("\b") && input.Length > 0)
		{
			input = input.Remove(input.Length - 1);
		}
		else if (Input.inputString.Length > 0)
		{
			input += Regex.Replace(Input.inputString.ToUpper(), "[^A-Z0-9]", "");
			if (input.Length > maxLength)
			{
				input = input.Substring(0, maxLength);
			}
		}
		if (typedText.text != input)
		{
			typedText.text = input;
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			onFinish?.Invoke(input);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
