using UnityEngine;

public class CollisionSound : MonoBehaviour
{
	public float minImpulse = 0.5f;

	public float maxImpulse = 2f;

	public AudioClip sound;

	public Range pitch;

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.impulse.sqrMagnitude > minImpulse.Sqr())
		{
			AudioSource audioSource = sound.Play();
			audioSource.pitch = pitch.Random();
			audioSource.volume = Mathf.InverseLerp(minImpulse, maxImpulse, collision.impulse.magnitude);
		}
	}
}
