using UnityEngine;

public interface IWhackable
{
	int priority { get; }

	void Whack(GameObject heldObject);
}
