using TMPro;
using UnityEngine;

public class GameCoreGamerTagDisplay : MonoBehaviour
{
	public TMP_Text text;

	public GameObject container;

	private void Awake()
	{
		container.SetActive(value: false);
	}
}
