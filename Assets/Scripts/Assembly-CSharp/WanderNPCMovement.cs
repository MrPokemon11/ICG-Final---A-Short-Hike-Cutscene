using UnityEngine;

public class WanderNPCMovement : NPCMovement
{
	[Header("NPC Settings")]
	public BoxCollider wanderRegion;

	public Range waitAtDestination = new Range(5f, 20f);

	protected override void Start()
	{
		base.Start();
		PickNextWanderSpot();
	}

	private void PickNextWanderSpot()
	{
		Vector3 b = new Vector3(Random.value, Random.value, Random.value);
		b -= Vector3.one * 0.5f;
		Vector3 position = wanderRegion.center + Vector3.Scale(wanderRegion.size, b);
		Vector3 goal = wanderRegion.transform.TransformPoint(position);
		navigator.SetGoal(goal);
	}

	protected override void OnGoalReached()
	{
		this.RegisterTimer(waitAtDestination.Random(), delegate
		{
			PickNextWanderSpot();
		});
	}
}
