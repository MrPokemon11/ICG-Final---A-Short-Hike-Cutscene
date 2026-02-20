using UnityEngine;
using UnityEngine.UI;

public class FeatherUI : MonoBehaviour
{
	private static int SHAKE_ID = Animator.StringToHash("Shake");

	private static int SHAKING_ID = Animator.StringToHash("Shaking");

	public Image goldenFeatherImage;

	public Image iceFeatherImage;

	public Image darkFeatherImage;

	public AnimationCurve fadeCurve;

	public AnimationCurve shakeCurve;

	public AnimationCurve iceFeatherColorCurve;

	public float shakeTime;

	public RectTransform container;

	public Color deactivatedColor;

	private float fill;

	private bool frozen;

	private bool disabled;

	private bool shaking;

	private bool shake;

	private float shakeAnimationTime;

	private void Awake()
	{
		shakeAnimationTime = shakeTime;
	}

	public void SetFill(float fill)
	{
		this.fill = fill;
		goldenFeatherImage.color = goldenFeatherImage.color.SetA(fadeCurve.Evaluate(fill));
		darkFeatherImage.color = Color.white;
		if (disabled)
		{
			goldenFeatherImage.color = goldenFeatherImage.color.SetA(0f);
			darkFeatherImage.color = deactivatedColor;
		}
		bool num = iceFeatherImage.enabled;
		iceFeatherImage.enabled = fill == 0f && frozen && !disabled;
		if (num != iceFeatherImage.enabled)
		{
			Shake();
		}
	}

	public void ManualUpdate()
	{
		if (shakeAnimationTime < shakeTime)
		{
			shakeAnimationTime += Time.deltaTime;
			float time = shakeAnimationTime / shakeTime;
			float x = shakeCurve.Evaluate(time);
			container.anchoredPosition = container.anchoredPosition.SetX(x);
			if (iceFeatherImage.enabled)
			{
				iceFeatherImage.color = Color.Lerp(Color.black, Color.white, iceFeatherColorCurve.Evaluate(time));
			}
		}
		else if (shake || shaking)
		{
			shake = false;
			shakeAnimationTime = 0f;
		}
	}

	public void Shake()
	{
		shake = true;
	}

	public void Shaking(bool value)
	{
		shaking = value;
	}

	public void SetFrozen(bool frozen)
	{
		this.frozen = frozen;
	}

	public void SetDisabled(bool disabled)
	{
		this.disabled = disabled;
	}
}
