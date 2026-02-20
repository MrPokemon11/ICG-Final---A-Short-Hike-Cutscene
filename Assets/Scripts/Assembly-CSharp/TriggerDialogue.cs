using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
	public string startNode = "Start";

	public Transform speaker;

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>())
		{
			Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(startNode, speaker);
		}
	}
}
