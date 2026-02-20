using UnityEngine;

public class AnimatorOffset : MonoBehaviour
{
	public Range speed = new Range(0.8f, 1.2f);

	private void Start()
	{
		Animator component = GetComponent<Animator>();
		component.Play(component.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, Random.Range(0f, 1f));
		component.speed = speed.Random();
	}
}
