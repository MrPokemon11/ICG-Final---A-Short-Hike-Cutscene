using UnityEngine;

public class BreakablePiece : MonoBehaviour, IWhackable
{
	public float shrinkTime;

	private Breakable breakable;

	public int priority => breakable.priority;

	private void Awake()
	{
		breakable = GetComponentInParent<Breakable>();
	}

	public void Whack(GameObject heldObject)
	{
		breakable.Whack(heldObject);
	}
}
