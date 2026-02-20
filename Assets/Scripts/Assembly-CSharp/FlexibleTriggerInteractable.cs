using System.Collections;
using System.Linq;
using UnityEngine;

public class FlexibleTriggerInteractable : MonoBehaviour, IInteractable
{
	public Transform lookAt;

	public int priority;

	public float maximumMovementInput = 0.5f;

	public float lookAtPointOverrideDistance = 1f;

	private LevelController levelController;

	private BoxCollider boxCollider;

	private bool playerInside;

	private Coroutine registrationUpdater;

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
		boxCollider = GetComponent<BoxCollider>();
	}

	public void Interact()
	{
		IInteractableComponent[] components = GetComponents<IInteractableComponent>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Interact();
		}
	}

	public bool IsPlayerInside()
	{
		if (boxCollider != null)
		{
			return Physics.OverlapBox(base.transform.TransformPoint(boxCollider.center), Vector3.Scale(base.transform.lossyScale, boxCollider.size / 2f), boxCollider.transform.rotation, 512).Any((Collider obj) => obj.GetComponent<Player>());
		}
		return false;
	}

	private void OnEnable()
	{
		if (IsPlayerInside())
		{
			PlayerEnter();
		}
	}

	private void OnDisable()
	{
		PlayerLeave();
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>() && base.enabled)
		{
			PlayerEnter();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<Player>() && base.enabled)
		{
			PlayerLeave();
		}
	}

	public void PlayerEnter()
	{
		if (!playerInside)
		{
			playerInside = true;
			registrationUpdater = StartCoroutine(UpdateRegisteredState());
		}
	}

	public void PlayerLeave()
	{
		if (playerInside)
		{
			playerInside = false;
			if (levelController != null)
			{
				levelController.player.UnregisterInteractable(this);
			}
			StopCoroutine(registrationUpdater);
		}
	}

	public IEnumerator UpdateRegisteredState()
	{
		bool isRegistered = false;
		while (true)
		{
			bool flag = levelController.player.input.GetMovement().sqrMagnitude < maximumMovementInput.Sqr() || (levelController.player.transform.position - lookAt.transform.position).sqrMagnitude < lookAtPointOverrideDistance.Sqr();
			if (flag && !isRegistered)
			{
				levelController.player.RegisterInteractable(this);
				isRegistered = true;
			}
			else if (!flag && isRegistered)
			{
				levelController.player.UnregisterInteractable(this);
				isRegistered = false;
			}
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		if (lookAt != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(lookAt.transform.position, lookAtPointOverrideDistance);
		}
	}
}
