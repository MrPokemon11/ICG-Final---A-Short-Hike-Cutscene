using UnityEngine;

public class TriggerInteractable : MonoBehaviour, IInteractable
{
	public Transform lookAt;

	public int priority;

	private LevelController levelController;

	Transform IInteractable.transform
	{
		get
		{
			if (!(lookAt == null))
			{
				return lookAt;
			}
			return base.transform;
		}
	}

	int IInteractable.priority => priority;

	bool IInteractable.enabled
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

	private void Start()
	{
		levelController = Singleton<ServiceLocator>.instance.Locate<LevelController>();
	}

	private void OnDisable()
	{
		if (levelController != null)
		{
			levelController.player.UnregisterInteractable(this);
		}
	}

	public void Interact()
	{
		IInteractableComponent[] components = GetComponents<IInteractableComponent>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Interact();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Player component = other.GetComponent<Player>();
		if ((bool)component)
		{
			component.RegisterInteractable(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Player component = other.GetComponent<Player>();
		if ((bool)component)
		{
			component.UnregisterInteractable(this);
		}
	}
}
