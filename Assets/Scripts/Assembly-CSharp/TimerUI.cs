using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
	public TMP_Text text;

	private LevelUI ui;

	private bool begun;

	private char[] timeString = new char["0:00.0".Length];

	private Animator animator;

	public float time { get; set; }

	private void Awake()
	{
		ui = Singleton<GameServiceLocator>.instance.levelUI;
		animator = GetComponent<Animator>();
		SecondsToCharArray(time, timeString);
		text.SetCharArray(timeString);
	}

	public void Flash()
	{
		animator.SetTrigger("Flash");
	}

	private void Update()
	{
		if (begun)
		{
			time += Time.deltaTime;
		}
		text.enabled = !ui.pauseMenuOpen;
		if (text.enabled && begun)
		{
			SecondsToCharArray(time, timeString);
			text.SetCharArray(timeString);
		}
	}

	private void SecondsToCharArray(float timeInSeconds, char[] array)
	{
		int num = (int)(timeInSeconds / 60f);
		array[0] = (char)(48 + num % 10);
		array[1] = ':';
		int num2 = (int)(timeInSeconds - (float)(num * 60));
		array[2] = (char)(48 + num2 / 10);
		array[3] = (char)(48 + num2 % 10);
		array[4] = '.';
		int num3 = (int)(timeInSeconds % 1f * 1000f);
		array[5] = (char)(48 + num3 / 100);
	}

	public void Begin()
	{
		time = 0f;
		begun = true;
	}

	public void Stop()
	{
		begun = false;
	}

	public string GetTimeString()
	{
		SecondsToCharArray(time, timeString);
		return new string(timeString);
	}
}
