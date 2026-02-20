using UnityEngine;

public class InteractableComponentTransfer : MonoBehaviour, IInteractableComponent
{
	public GameObject transferTo;

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

	public void Interact()
	{
		transferTo.GetComponent<IInteractableComponent>().Interact();
	}
}
