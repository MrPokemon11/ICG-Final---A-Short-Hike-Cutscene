using UnityEngine;

public class BouncyCollider : MonoBehaviour
{
	public float bounceSpeed = 20f;

	public float trailTime;

	public Collider bouncyPart;

	public AudioClip soundEffect;

	public Animator animator;

	private void OnCollisionEnter(Collision collision)
	{
		if (bouncyPart != null && collision.contacts[0].thisCollider != bouncyPart)
		{
			return;
		}
		Player component = collision.collider.GetComponent<Player>();
		if ((bool)component)
		{
			soundEffect.Play();
			animator.SetTrigger("Bounce");
			Rigidbody component2 = component.GetComponent<Rigidbody>();
			component2.linearVelocity = component2.linearVelocity.SetY(bounceSpeed);
			if (trailTime > 0f)
			{
				component.ShowTrail(trailTime);
			}
		}
	}
}
