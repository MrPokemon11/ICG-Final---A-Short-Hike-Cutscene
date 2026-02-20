using System.Collections;
using TMPro;
using UnityEngine;

internal class SavingDialogue : MonoBehaviour
{
	public TMP_Text text;

	public bool noString;

	private void Start()
	{
		text.text = (noString ? "" : I18n.STRINGS.saving) + "...";
		StartCoroutine(IncrementSaveEllipsis());
	}

	private IEnumerator IncrementSaveEllipsis()
	{
		int dots = 3;
		while (true)
		{
			text.maxVisibleCharacters = text.text.Length - (3 - dots);
			yield return new WaitForSecondsRealtime(0.3f);
			dots++;
			if (dots > 3)
			{
				dots = 1;
			}
		}
	}
}
