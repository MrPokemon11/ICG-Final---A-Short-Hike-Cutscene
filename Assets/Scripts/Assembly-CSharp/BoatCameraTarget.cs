using UnityEngine;

public class BoatCameraTarget : MonoBehaviour
{
	private BoatScripting scripting;

	private Transform cameraTarget;

	private Transform boat;

	private Timer killTimer;

	private Vector3 velocity = Vector3.zero;

	private Transform checkpoint;

	private Transform nextCheckpoint;

	public BoatCameraTarget Setup(BoatScripting scripting)
	{
		boat = scripting.boat.transform;
		cameraTarget = scripting.cameraTarget;
		this.scripting = scripting;
		cameraTarget.parent = null;
		return this;
	}

	private void OnDestroy()
	{
		if (cameraTarget != null)
		{
			cameraTarget.parent = boat;
			cameraTarget.localPosition = Vector3.zero;
		}
	}

	private void Update()
	{
		Vector3 position = boat.transform.position;
		if (checkpoint != null)
		{
			Vector3 vector = checkpoint.position - boat.position;
			float magnitude = vector.magnitude;
			vector = vector / magnitude * Mathf.Min(magnitude * scripting.boatCameraCheckpointPull, scripting.boatCameraMaxPull);
			position += vector + scripting.boat.body.linearVelocity.SetY(0f) * scripting.boatCameraVelocityFactor;
			if (nextCheckpoint != null && magnitude < scripting.boatCameraNextCheckpointDistance)
			{
				position += (nextCheckpoint.position - boat.position).normalized * (scripting.boatCameraNextCheckpointDistance - magnitude) * scripting.boatCameraNextCheckpointFactor;
			}
		}
		cameraTarget.position = Vector3.SmoothDamp(cameraTarget.position, position, ref velocity, scripting.boatCameraSmoothTime * ((checkpoint == null) ? 0.5f : 1f));
	}

	public void ResetOffset()
	{
		velocity = Vector3.zero;
		cameraTarget.position = boat.transform.position;
	}

	public void Revive()
	{
		Timer.Cancel(killTimer);
	}

	public void Kill()
	{
		checkpoint = null;
		killTimer = this.RegisterTimer(5f, delegate
		{
			Object.Destroy(this);
		});
	}

	internal void SetCheckpoint(Transform checkpoint, Transform nextCheckpoint)
	{
		this.checkpoint = checkpoint;
		this.nextCheckpoint = nextCheckpoint;
	}
}
