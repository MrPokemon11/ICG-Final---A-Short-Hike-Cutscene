using UnityEngine;

public interface IInteractable
{
	int priority { get; }

	bool enabled { get; set; }

	Transform transform { get; }

	void Interact();
}
