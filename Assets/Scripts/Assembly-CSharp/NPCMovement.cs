using UnityEngine;

public class NPCMovement : PhysicsMovement
{
	public float inactiveTimeout;

	public bool assumeGrounded;

	public Renderer inactiveRenderer;

	protected DialogueInteractable dialogueInteractable;

	protected NavMeshNavigator navigator;

	private Timer faceTimer;

	private Transform faceTowards;

	private Timer inactiveTimer;

	private bool isInactive
	{
		get
		{
			if (inactiveTimer != null)
			{
				return inactiveTimer.isCompleted;
			}
			return false;
		}
	}

	protected virtual void Start()
	{
		navigator = base.gameObject.AddComponent<NavMeshNavigator>();
		navigator.onGoalReached += OnGoalReached;
		dialogueInteractable = GetComponent<DialogueInteractable>();
		if (inactiveRenderer == null)
		{
			inactiveRenderer = GetComponentInChildren<Renderer>();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (inactiveRenderer.isVisible && inactiveTimer != null)
		{
			base.body.isKinematic = (bool)dialogueInteractable && dialogueInteractable.isConversationActive && base.isGrounded;
			base.myCollider.enabled = true;
			navigator.enableMovement = true;
			Timer.Cancel(inactiveTimer);
			inactiveTimer = null;
		}
		else if (!inactiveRenderer.isVisible && inactiveTimer == null)
		{
			inactiveTimer = this.RegisterTimer(inactiveTimeout, delegate
			{
				base.body.isKinematic = true;
				base.myCollider.enabled = false;
				navigator.enableMovement = false;
			});
		}
	}

	protected override void FixedUpdate()
	{
		if (!isInactive)
		{
			base.FixedUpdate();
		}
	}

	public override bool CheckIfGrounded()
	{
		if (isInactive || assumeGrounded)
		{
			return true;
		}
		return base.CheckIfGrounded();
	}

	public override Vector3 GetDesiredMovementVector()
	{
		if (faceTowards != null || (dialogueInteractable != null && dialogueInteractable.isConversationActive))
		{
			return Vector3.zero;
		}
		return navigator.GetMovementDirection();
	}

	protected override Vector3 GetDesiredRotateDirection()
	{
		if (dialogueInteractable.isConversationActive)
		{
			return TowardPlayerRotation(base.transform);
		}
		if (faceTowards != null)
		{
			return TowardsTransformRotation(base.transform, faceTowards);
		}
		return base.GetDesiredRotateDirection();
	}

	public static Vector3 TowardPlayerRotation(Transform transform)
	{
		Player player = Singleton<ServiceLocator>.instance.Locate<LevelController>().player;
		if ((bool)player)
		{
			return TowardsTransformRotation(transform, player.transform);
		}
		return Vector3.zero;
	}

	private static Vector3 TowardsTransformRotation(Transform transform, Transform turnTorwards)
	{
		Vector3 vector = (turnTorwards.position - transform.position).SetY(0f);
		if (Vector3.Angle(vector, transform.forward) > 5f)
		{
			return vector;
		}
		return transform.forward;
	}

	protected virtual void OnGoalReached()
	{
	}

	public void PauseAndFace(Transform face, float time)
	{
		Timer.Cancel(faceTimer);
		faceTowards = face;
		faceTimer = this.RegisterTimer(time, delegate
		{
			faceTowards = null;
			faceTimer = null;
		});
	}
}
