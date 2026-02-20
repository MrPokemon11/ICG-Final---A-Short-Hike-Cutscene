using System.Collections;
using TMPro;
using UnityEngine;

public class FPSCounter : ServiceMonoBehaviour
{
	public TMP_Text text;

	public float frequency = 0.5f;

	private bool _visible;

	private WaitForSeconds waitCommand;

	public int fps { get; protected set; }

	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			_visible = value;
			text.enabled = value;
		}
	}

	private void Awake()
	{
		waitCommand = new WaitForSeconds(frequency);
	}

	protected override void OnEnable()
	{
		visible = GameSettings.showFPS;
		StartCoroutine(FPS());
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return waitCommand;
			float num = Time.realtimeSinceStartup - lastTime;
			int num2 = Time.frameCount - lastFrameCount;
			fps = Mathf.RoundToInt((float)num2 / num);
			if (visible)
			{
				text.text = $"{fps} fps";
			}
		}
	}
}
