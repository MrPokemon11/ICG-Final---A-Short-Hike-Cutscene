using System.Collections;
using UnityEngine;

public class ClimbDetector : MonoBehaviour
{
	public Transform speaker;

	public string climbNode;

	public string otherNode;

	public float requiredClimbTime = 0.3f;

	private void OnTriggerEnter(Collider other)
	{
		Player component = other.GetComponent<Player>();
		if ((bool)component)
		{
			StartCoroutine(TriggerOnceGrounded(component));
		}
	}

	private IEnumerator TriggerOnceGrounded(Player player)
	{
		float timeClimbing = 0f;
		while (!player.isGrounded)
		{
			if (player.isClimbing)
			{
				timeClimbing += Time.deltaTime;
			}
			yield return null;
		}
		if (GetComponent<Collider>().bounds.Contains(player.transform.position))
		{
			DialogueController dialogue = Singleton<GameServiceLocator>.instance.dialogue;
			if (timeClimbing > requiredClimbTime)
			{
				dialogue.StartConversation(climbNode, speaker);
			}
			else
			{
				dialogue.StartConversation(otherNode, speaker);
			}
		}
	}
}
