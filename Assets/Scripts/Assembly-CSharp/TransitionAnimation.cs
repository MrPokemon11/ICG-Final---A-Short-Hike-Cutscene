using System;
using UnityEngine;

public class TransitionAnimation : ServiceMonoBehaviour
{
	public Animator animator;

	public GameObject canvas;

	private Action onFadeOut;

	private Action onFadeIn;

	private bool fadeInOnStart;

	public bool isTransitioning => canvas.activeSelf;

	private void Start()
	{
		canvas.SetActive(value: false);
		if (fadeInOnStart)
		{
			canvas.SetActive(value: true);
			animator.Play("Transition", 0, 0.5f);
		}
	}

	public void Begin(Action onFadeOut, Action onFadeIn)
	{
		this.onFadeOut = onFadeOut;
		this.onFadeIn = onFadeIn;
		canvas.SetActive(value: true);
		animator.SetTrigger("Begin");
	}

	public void FadeInOnStart()
	{
		fadeInOnStart = true;
	}

	public void OnFadeOut()
	{
		onFadeOut?.Invoke();
	}

	public void OnFadeIn()
	{
		onFadeIn?.Invoke();
	}

	public void OnTransitionFinished()
	{
		canvas.SetActive(value: false);
	}

	public void Pause()
	{
		animator.speed = 0f;
	}

	public void Unpause()
	{
		animator.speed = 1f;
	}

	public void AddUI(Transform ui)
	{
		ui.SetParent(canvas.transform, worldPositionStays: false);
	}
}
