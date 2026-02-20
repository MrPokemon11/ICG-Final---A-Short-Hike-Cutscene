using UnityEngine;

public class NavigatorMovement : PhysicsMovement
{
	private NavMeshNavigator navigator;

	private void Start()
	{
		navigator = base.gameObject.AddComponent<NavMeshNavigator>();
	}

	public void SetGoal(Vector3 goal)
	{
		navigator.SetGoal(goal);
	}

	public override Vector3 GetDesiredMovementVector()
	{
		return navigator.GetMovementDirection();
	}
}
