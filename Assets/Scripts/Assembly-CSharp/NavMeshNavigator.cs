using System;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshNavigator : MonoBehaviour
{
	public float successDistance = 0.7f;

	public bool enableMovement = true;

	public Range repathTimeout = new Range(0.25f, 1f);

	private Vector3[] currentPathCorners;

	private float recalculateTimer = 0.1f;

	private NavMeshPath cachedReusablePath;

	private Collider myCollider;

	public bool hasGoal => goal.HasValue;

	public bool hasValidPath
	{
		get
		{
			if (currentPath != null)
			{
				return currentPath.status != NavMeshPathStatus.PathInvalid;
			}
			return false;
		}
	}

	public Vector3 destinationNode => currentPathCorners[pathNode];

	public Vector3? goal { get; private set; }

	protected NavMeshPath currentPath { get; private set; }

	protected int pathNode { get; private set; }

	public event Action onGoalReached;

	public event Action onBeforeCalculationTimeout;

	private void Awake()
	{
		cachedReusablePath = new NavMeshPath();
		myCollider = GetComponent<Collider>();
	}

	protected virtual void Update()
	{
		if (!enableMovement)
		{
			return;
		}
		recalculateTimer -= Time.deltaTime;
		if (recalculateTimer <= 0f)
		{
			OnCalculationTimeout();
			recalculateTimer = GetRecalculationTime();
		}
		if (!hasGoal)
		{
			return;
		}
		if (hasValidPath)
		{
			if ((destinationNode - base.transform.position).SetY(0f).magnitude < successDistance)
			{
				SelectNextNode();
			}
		}
		else if ((goal.Value - base.transform.position).SetY(0f).magnitude < successDistance)
		{
			ReachGoal();
		}
	}

	protected virtual void OnCalculationTimeout()
	{
		if (this.onBeforeCalculationTimeout != null)
		{
			this.onBeforeCalculationTimeout();
		}
		if (hasGoal)
		{
			RecalculatePath();
		}
	}

	public Vector3 GetMovementDirection()
	{
		if (hasValidPath)
		{
			return (destinationNode - base.transform.position).SetY(0f).normalized;
		}
		return Vector3.zero;
	}

	public void SetGoal(Vector3 goal)
	{
		this.goal = goal;
	}

	public void SetGoalImmediately(Vector3 goal)
	{
		SetGoal(goal);
		RecalculatePath();
	}

	public void ClearGoal()
	{
		goal = null;
		currentPath = null;
		currentPathCorners = null;
	}

	private void ReachGoal()
	{
		ClearGoal();
		if (this.onGoalReached != null)
		{
			this.onGoalReached();
		}
	}

	public float GetDistanceToGoal()
	{
		if (!hasValidPath)
		{
			return 0f;
		}
		float num = (base.transform.position - destinationNode).magnitude;
		for (int i = pathNode; i < currentPathCorners.Length - 1; i++)
		{
			num += (currentPathCorners[i + 1] - currentPathCorners[i]).magnitude;
		}
		return num;
	}

	private void SelectNextNode()
	{
		pathNode++;
		if (pathNode >= currentPathCorners.Length)
		{
			ReachGoal();
		}
	}

	private void RecalculatePath()
	{
		NavMesh.CalculatePath(myCollider.bounds.center + myCollider.bounds.extents.y * Vector3.down, goal.Value, int.MaxValue, cachedReusablePath);
		currentPath = cachedReusablePath;
		currentPathCorners = currentPath.corners;
		pathNode = Mathf.Min(1, currentPathCorners.Length - 1);
	}

	private float GetRecalculationTime()
	{
		return repathTimeout.Random();
	}

	protected virtual void OnDrawGizmos()
	{
		if (hasValidPath)
		{
			for (int i = 1; i < currentPath.corners.Length; i++)
			{
				Gizmos.DrawLine(currentPath.corners[i - 1], currentPath.corners[i]);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(destinationNode, successDistance);
		}
		if (hasGoal)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(goal.Value, Vector3.one * 1.4f);
		}
	}
}
