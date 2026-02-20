using System;
using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractableComponent
{
	public const float TOO_CLOSE_DISTANCE_MIN = 2f;

	public const float TOO_CLOSE_DISTANCE = 5f;

	public const float TOO_CLOSE_FORCE = 25f;

	public string startNode = "Start";

	public bool stepBack = true;

	private IConversation conversation;

	public bool isConversationActive
	{
		get
		{
			if (conversation != null)
			{
				return conversation.isAlive;
			}
			return false;
		}
	}

	public IConversation currentConversation
	{
		get
		{
			if (!isConversationActive)
			{
				return null;
			}
			return conversation;
		}
	}

	bool IInteractableComponent.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public event Action<IConversation> onConversationStart;

	public void Interact()
	{
		Player player = Singleton<ServiceLocator>.instance.Locate<LevelController>().player;
		if (player != null)
		{
			player.TurnToFace(base.transform);
			Vector3 vector = (player.transform.position - base.transform.position).SetY(0f);
			if (vector.sqrMagnitude < 5f.Sqr() && stepBack)
			{
				float magnitude = vector.magnitude;
				float num = Mathf.Lerp(25f, 0f, Mathf.InverseLerp(2f, 5f, magnitude));
				player.body.AddForce(vector / magnitude * num, ForceMode.Impulse);
			}
		}
		conversation = Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(startNode, base.transform);
		this.onConversationStart?.Invoke(conversation);
	}
}
