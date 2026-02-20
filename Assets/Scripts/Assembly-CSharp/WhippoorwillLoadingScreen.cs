using System.Collections.Generic;
using UnityEngine;

public class WhippoorwillLoadingScreen : MonoBehaviour
{
	public List<Animator> logoAnimators;

	public List<Animator> optionalUnityAnimators;

	private int logoIndex;

	private Timer logoTimer;

	public bool isFinished { get; private set; }

	private void Awake()
	{
		if (DisplayUnityLogo())
		{
			logoAnimators.InsertRange(logoAnimators.Count - 1, optionalUnityAnimators);
		}
	}

	private void Start()
	{
		foreach (Animator logoAnimator in logoAnimators)
		{
			logoAnimator.gameObject.SetActive(value: false);
		}
		AdvanceLogo();
	}

	public void AdvanceLogo()
	{
		if (!isFinished)
		{
			if (logoTimer != null)
			{
				Timer.Cancel(logoTimer);
				OnLogoFinished();
			}
			else
			{
				logoAnimators[logoIndex].gameObject.SetActive(value: true);
				logoTimer = this.RegisterTimer(logoAnimators[logoIndex].GetCurrentAnimatorClipInfo(0)[0].clip.length, OnLogoFinished);
			}
		}
	}

	private void OnLogoFinished()
	{
		logoTimer = null;
		logoAnimators[logoIndex].gameObject.SetActive(value: false);
		logoIndex++;
		if (logoIndex >= logoAnimators.Count)
		{
			isFinished = true;
		}
		else
		{
			AdvanceLogo();
		}
	}

	private bool DisplayUnityLogo()
	{
		return false;
	}
}
