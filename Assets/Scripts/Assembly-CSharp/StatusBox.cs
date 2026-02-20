using UnityEngine;
using UnityEngine.UI;

public class StatusBox : MonoBehaviour
{
	public Image icon;

	public Text text;

	private Timer hideTimer;

	private Timer killTimer;

	public RectTransform rect => base.transform as RectTransform;

	public void Setup(Sprite icon, string text)
	{
		this.icon.sprite = icon;
		this.text.text = text;
	}

	public void Show()
	{
		Timer.Cancel(killTimer);
		Timer.Cancel(hideTimer);
		Animator component = GetComponent<Animator>();
		if (component != null)
		{
			component.SetBool("Hidden", value: false);
		}
	}

	public void HideAndKill(float time = 0.01f)
	{
		if (this == null)
		{
			return;
		}
		Timer.Cancel(killTimer);
		Timer.Cancel(hideTimer);
		hideTimer = this.RegisterTimer(time, delegate
		{
			if (this != null)
			{
				Animator component = GetComponent<Animator>();
				if (component != null)
				{
					component.SetBool("Hidden", value: true);
				}
				Timer.Cancel(killTimer);
				killTimer = this.RegisterTimer(0.5f, delegate
				{
					if (base.gameObject != null)
					{
						Object.Destroy(base.gameObject);
					}
				});
			}
		});
	}
}
