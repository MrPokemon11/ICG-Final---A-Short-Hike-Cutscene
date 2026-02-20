using QuickUnityTools.Input;
using TMPro;
using UnityEngine;

public class CreditsBox : MonoBehaviour
{
	public TMP_Text text;

	[TextArea(8, 16)]
	public string[] creditText;

	private FocusableUserInput input;

	private int index;

	private void Start()
	{
		input = GetComponent<FocusableUserInput>();
		SetAndIncrementText();
	}

	private void SetAndIncrementText()
	{
		if (index < creditText.Length)
		{
			TextTranslator component = text.GetComponent<TextTranslator>();
			component.originalText = creditText[index];
			component.UpdateTranslation();
			index++;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (input.WasDismissPressed())
		{
			SetAndIncrementText();
		}
	}
}
