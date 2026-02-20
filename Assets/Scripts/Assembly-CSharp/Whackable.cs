using System;
using UnityEngine;

public class Whackable : MonoBehaviour, IWhackable
{
	public float whackForce = 20f;

	public AudioClip whackSound;

	public Range whackPitch = new Range(0.8f, 1.2f);

	public AudioSource whackSource;

	public Animator whackAnimator;

	public string whackAnimatorTrigger = "Whack";

	public bool optimizeAnimator;

	private Timer animatorTimer;

	public int priority => 0;

	public event Action<GameObject> onWhack;

	private void Awake()
	{
		if (optimizeAnimator && (bool)whackAnimator)
		{
			whackAnimator.keepAnimatorStateOnDisable = true;
			whackAnimator.enabled = false;
		}
	}

	public void Whack(GameObject heldObject)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		if ((bool)component && (bool)player)
		{
			component.AddForce((player.transform.forward + Vector3.up * 0.5f).normalized * whackForce, ForceMode.Impulse);
		}
		if ((bool)whackAnimator)
		{
			if (optimizeAnimator)
			{
				EnableAnimatorTemporarily();
			}
			whackAnimator.SetTrigger(whackAnimatorTrigger);
		}
		WhackNoise();
		this.onWhack?.Invoke(heldObject);
	}

	private void EnableAnimatorTemporarily()
	{
		whackAnimator.enabled = true;
		Timer.Cancel(animatorTimer);
		animatorTimer = this.RegisterTimer(3f, delegate
		{
			whackAnimator.enabled = false;
		});
	}

	public void WhackNoise()
	{
		if ((bool)whackSound)
		{
			AudioSource audioSource = whackSource;
			if (!audioSource)
			{
				audioSource = whackSound.Play();
			}
			else
			{
				audioSource.clip = whackSound;
				audioSource.Play();
			}
			audioSource.pitch = whackPitch.Random();
		}
	}
}
