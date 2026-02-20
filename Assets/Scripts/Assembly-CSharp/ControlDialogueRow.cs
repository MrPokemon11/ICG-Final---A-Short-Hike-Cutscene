using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlDialogueRow : MonoBehaviour
{
	public TMP_Text button;

	public TMP_Text actions;

	public void Awake()
	{
		if (GameSettings.language != SystemLanguage.English.ToString())
		{
			GetComponent<LayoutElement>().preferredWidth = 361f;
		}
	}
}
