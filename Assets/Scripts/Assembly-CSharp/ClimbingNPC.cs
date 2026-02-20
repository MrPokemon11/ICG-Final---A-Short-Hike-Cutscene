using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbingNPC : MonoBehaviour
{
	private static int CLIMBING_ID = Animator.StringToHash("Climbing");

	private static int Y_SPEED_ID = Animator.StringToHash("YSpeed");

	public Transform pathParent;

	public bool reversePath;

	public float moveSpeed;

	public Animator characterAnimator;

	public float talkCooldown = 5f;

	public Renderer myRenderer;

	private Transform[] destinations;

	private DialogueInteractable dialogueInteractable;

	private ITalkingAnimator talkingAnimator;

	private int destinationIndex;

	private Vector3? goal;

	private float talkCooldownTimer;

	protected void Start()
	{
		dialogueInteractable = GetComponent<DialogueInteractable>();
		talkingAnimator = GetComponentInChildren<ITalkingAnimator>();
		IEnumerable<Transform> enumerable = pathParent.GetChildren();
		if (reversePath)
		{
			int num = enumerable.Count();
			enumerable = enumerable.Concat(pathParent.GetChildren().Take(num - 1).Skip(1)
				.Reverse());
		}
		destinations = enumerable.ToArray();
		int num2 = destinations.MinValueIndex((Transform t) => (base.transform.position - t.position).sqrMagnitude);
		destinationIndex = num2;
		goal = destinations[destinationIndex].position;
	}

	private void Update()
	{
		if (!myRenderer.isVisible)
		{
			return;
		}
		bool flag = (dialogueInteractable != null && dialogueInteractable.isConversationActive) || (talkingAnimator != null && talkingAnimator.isTalking);
		if (flag)
		{
			talkCooldownTimer = talkCooldown;
		}
		else if (talkCooldown > 0f)
		{
			talkCooldownTimer -= Time.deltaTime;
		}
		bool flag2 = goal.HasValue && !flag && talkCooldownTimer <= 0f;
		characterAnimator.SetBool(CLIMBING_ID, flag2);
		if (flag2)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, goal.Value, Time.deltaTime * moveSpeed);
			characterAnimator.SetFloat(Y_SPEED_ID, goal.Value.y - base.transform.position.y);
			if (base.transform.position == goal.Value)
			{
				OnGoalReached();
			}
		}
	}

	protected void OnGoalReached()
	{
		goal = null;
		PathNode component = destinations[destinationIndex].GetComponent<PathNode>();
		if ((bool)component)
		{
			this.RegisterTimer(component.waitTime, SetNextGoal);
		}
		else
		{
			SetNextGoal();
		}
	}

	private void SetNextGoal()
	{
		destinationIndex = (destinationIndex + 1) % destinations.Length;
		goal = destinations[destinationIndex].position;
	}
}
