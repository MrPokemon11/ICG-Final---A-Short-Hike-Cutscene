using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerCollidersTracker<T> : MonoBehaviour
{
	protected List<Collider> collidersInside;

	public int objectsInsideCount => collidersInside.Count;

	protected virtual void Start()
	{
		collidersInside = new List<Collider>();
	}

	protected virtual void Update()
	{
		for (int num = collidersInside.Count - 1; num >= 0; num--)
		{
			Collider collider = collidersInside[num];
			if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy)
			{
				MarkColliderRemoved(collider);
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<T>() == null)
		{
			return;
		}
		if (!collidersInside.Contains(collider))
		{
			collidersInside.Add(collider);
			if (collidersInside.Count == 1)
			{
				OnAtLeastOneCollider();
			}
		}
		OnColliderEnter(collider);
	}

	private void OnTriggerExit(Collider collider)
	{
		MarkColliderRemoved(collider);
	}

	private void MarkColliderRemoved(Collider collider)
	{
		if (collidersInside.Contains(collider))
		{
			collidersInside.Remove(collider);
			OnColliderExit(collider);
			if (collidersInside.Count == 0)
			{
				OnNoColliders();
			}
		}
	}

	protected virtual void OnAtLeastOneCollider()
	{
	}

	protected virtual void OnNoColliders()
	{
	}

	protected virtual void OnColliderEnter(Collider collider)
	{
	}

	protected virtual void OnColliderExit(Collider collider)
	{
	}
}
