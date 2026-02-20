using UnityEngine;

public class VolleyballOpponent : DummyPlayer
{
	[Header("Volleyball Settings")]
	public VolleyballGameController controller;

	public Range idleTime = new Range(0.25f, 1f);

	public BoxCollider wanderArea;

	public AudioClip swipeSound;

	public ParticleSystem smackParticles;

	private float idleCountdown;

	private DialogueInteractable dialogue;

	public Vector3? walkTo { get; set; }

	protected override void Awake()
	{
		base.Awake();
		idleCountdown = idleTime.Random();
		dialogue = GetComponent<DialogueInteractable>();
	}

	protected override void Update()
	{
		base.Update();
		UpdateIdlePose(!controller.gameStarted, Pose.StretchDance);
		if (!controller.gameStarted)
		{
			walkTo = null;
		}
		if (!walkTo.HasValue && controller.playerShouldCatch && controller.gameStarted)
		{
			idleCountdown -= Time.deltaTime;
			if (idleCountdown <= 0f)
			{
				walkTo = wanderArea.RandomWithin();
				idleCountdown = idleTime.Random();
			}
		}
		if (walkTo.HasValue && (base.transform.position - walkTo.Value).SetY(0f).sqrMagnitude < 1f)
		{
			walkTo = null;
		}
		base.animator.lookAt = ((controller.ball != null) ? controller.ball.transform : null);
	}

	protected override void FixedUpdate()
	{
		base.body.isKinematic = !myRenderer.isVisible;
		if (myRenderer.isVisible)
		{
			base.FixedUpdate();
		}
	}

	public override Vector3 GetDesiredMovementVector()
	{
		if (walkTo.HasValue)
		{
			return (walkTo.Value - base.transform.position).SetY(0f).normalized;
		}
		return Vector3.zero;
	}

	protected override Vector3 GetDesiredRotateDirection()
	{
		if (dialogue.isConversationActive)
		{
			return NPCMovement.TowardPlayerRotation(base.transform);
		}
		if (controller.ball != null)
		{
			Vector3 desiredMovementVector = GetDesiredMovementVector();
			return (((controller.ball.transform.position - base.transform.position).SetY(0f).normalized + desiredMovementVector) / 2f).normalized;
		}
		return base.GetDesiredRotateDirection();
	}

	public void SwingArms()
	{
		base.animator.SwipeArms();
		swipeSound.Play();
		this.RegisterTimer(0.21f, delegate
		{
			smackParticles.Play();
			if ((bool)controller && (bool)controller.ball)
			{
				controller.ball.WhackNoise();
			}
		});
	}
}
