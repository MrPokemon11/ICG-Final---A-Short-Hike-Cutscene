using System;
using QuickUnityTools.Input;
using UnityEngine;

public class Vehicle : MonoBehaviour, IInteractableComponent
{
	[Header("Vehicle Settings")]
	public GameUserInput input;

	public Transform mountPosition;

	public GameObject vehicleCamera;

	public Transform[] savePositions;

	public Rigidbody body { get; private set; }

	public bool mounted { get; private set; }

	protected Player player { get; private set; }

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

	public event Action<bool> onMounted;

	protected virtual void Start()
	{
		body = GetComponent<Rigidbody>();
		player = Singleton<GameServiceLocator>.instance.levelController.player;
	}

	public virtual void Interact()
	{
		Enter();
	}

	protected virtual void Enter()
	{
		mounted = true;
		input.enabled = true;
		player.body.position = mountPosition.position;
		player.transform.parent = base.transform;
		player.body.isKinematic = true;
		player.mountedVehicle = this;
		if ((bool)vehicleCamera)
		{
			vehicleCamera.SetActive(value: true);
		}
		this.onMounted?.Invoke(obj: true);
	}

	protected virtual void Exit()
	{
		mounted = false;
		input.enabled = false;
		player.body.isKinematic = false;
		player.transform.parent = null;
		player.mountedVehicle = null;
		if ((bool)vehicleCamera)
		{
			vehicleCamera.SetActive(value: false);
		}
		this.onMounted?.Invoke(obj: false);
	}

	protected virtual void Update()
	{
	}

	public Vector3? GetSavePosition()
	{
		Transform transform = savePositions.MinValue((Transform pos) => (pos.position - base.transform.position).sqrMagnitude);
		if (!(transform != null))
		{
			return null;
		}
		return transform.position;
	}
}
