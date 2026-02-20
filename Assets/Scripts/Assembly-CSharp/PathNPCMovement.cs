using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathNPCMovement : NPCMovement
{
	[Header("NPC Settings")]
	public Transform pathParent;

	public bool reversePath = true;

	public bool immediatelyChooseNextNode = true;

	private Transform[] destinations;

	private int destinationIndex;

	public int nextNode => destinationIndex;

	protected override void Start()
	{
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
		base.Start();
		navigator.SetGoal(destinations[destinationIndex].position);
	}

	protected override void OnGoalReached()
	{
		base.OnGoalReached();
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
		Vector3 position = destinations[destinationIndex].position;
		if (immediatelyChooseNextNode)
		{
			navigator.SetGoalImmediately(position);
		}
		else
		{
			navigator.SetGoal(position);
		}
	}
}
