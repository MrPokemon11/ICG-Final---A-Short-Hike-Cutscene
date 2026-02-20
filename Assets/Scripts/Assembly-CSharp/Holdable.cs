using System;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : MonoBehaviour, IInteractableComponent, IActionableItem
{
	public enum UseAction
	{
		Swing = 0,
		Dig = 1,
		Bucket = 2
	}

	public UseAction useAction;

	public CollectableItem associatedItem;

	public bool canUseWhileJumping;

	private Collider[] _colliders;

	public bool cannotDrop => associatedItem.cannotDrop;

	public bool cannotStash => associatedItem.cannotStash;

	public Player anchoredTo { get; private set; }

	private Collider[] colliders
	{
		get
		{
			if (_colliders == null)
			{
				_colliders = GetComponentsInChildren<Collider>();
			}
			return _colliders;
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

	public event Action onReleased;

	public event Action onPickedUp;

	public void Interact()
	{
		Singleton<GameServiceLocator>.instance.levelController.player.PickUp(this);
		StatusBarUI statusBar = Singleton<GameServiceLocator>.instance.levelUI.statusBar;
		int collected = Singleton<GlobalData>.instance.gameData.GetCollected(associatedItem);
		statusBar.ShowStatusBox(associatedItem, collected + 1).HideAndKill(statusBar.statusBoxTime);
	}

	private void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (anchoredTo != null)
		{
			base.transform.position = anchoredTo.handTransform.position;
			base.transform.rotation = anchoredTo.handTransform.rotation;
		}
	}

	public void ParentToPlayer(Player player)
	{
		if (this == null)
		{
			Debug.LogError("Trying to parent a destroyed object!", this);
			return;
		}
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<RangedInteractable>().enabled = false;
		anchoredTo = player;
		UpdatePosition();
		this.onPickedUp?.Invoke();
	}

	public void ReleaseFromPlayer()
	{
		if (anchoredTo == null)
		{
			Debug.LogWarning("Cannot release from player since it is not held.");
			return;
		}
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<RangedInteractable>().enabled = true;
		anchoredTo = null;
		this.onReleased?.Invoke();
	}

	public List<ItemAction> GetMenuActions(bool held)
	{
		List<ItemAction> list = new List<ItemAction>();
		if (!associatedItem)
		{
			return list;
		}
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		if (!held)
		{
			list.Add(new ItemAction(I18n.STRINGS.equip, delegate
			{
				EquipFromInventory(player, associatedItem);
				return true;
			}));
		}
		else
		{
			if (!associatedItem.cannotDrop)
			{
				list.Add(new ItemAction(I18n.STRINGS.drop, delegate
				{
					player.DropItem();
					return true;
				}));
			}
			list.Add(new ItemAction(I18n.STRINGS.stash, delegate
			{
				player.StashHeldItem();
				return false;
			}));
		}
		return list;
	}

	public static void EquipFromInventory(Player player, CollectableItem item)
	{
		Singleton<GlobalData>.instance.gameData.AddCollected(item, -1, equipAction: true);
		GameObject gameObject = item.worldPrefab.Clone();
		player.PickUp(gameObject.GetComponent<Holdable>());
	}
}
