using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerCameraMovement : MonoBehaviour
{
	protected static List<GameObject> EMPTY_LIST = new List<GameObject>();

	public float minDistanceBetweenPlayers = 3f;

	public float maxDistanceBetweenPlayers = 20f;

	public float minCameraSize = 5f;

	public float maxCameraSize = 10f;

	public float minDistanceFromWorld = 20f;

	public float maxDistanceFromWorld = 50f;

	public float cameraDriftLerp = 1f;

	public float acceleration = 12f;

	public Vector3 cameraOffset;

	public List<GameObject> additionalCameraTargets;

	public Func<Vector3, Vector3> limitCameraMovement;

	protected Camera myCamera;

	protected Transform lockedTransform;

	protected bool isLocked;

	public float speed { get; set; }

	protected virtual void Awake()
	{
		myCamera = GetComponent<Camera>();
	}

	protected virtual void FixedUpdate()
	{
		float orthoSize = myCamera.orthographicSize;
		Vector3 vector = base.transform.position;
		if (GetGameObjectsToTrack().Count() > 0 || additionalCameraTargets.Count > 0)
		{
			vector = CalculatePlayerFocusCameraSettings(out orthoSize);
		}
		if (limitCameraMovement != null)
		{
			vector = limitCameraMovement(vector);
		}
		if (isLocked)
		{
			vector = lockedTransform.position;
		}
		Vector3 vector2 = (vector - base.transform.position) * cameraDriftLerp;
		if (vector2.sqrMagnitude > speed.Sqr())
		{
			float magnitude = vector2.magnitude;
			speed = Mathf.MoveTowards(speed, magnitude, acceleration * Time.fixedDeltaTime);
			vector2 = vector2 * speed / magnitude;
		}
		else
		{
			speed = vector2.magnitude;
		}
		base.transform.position += vector2 * Time.fixedDeltaTime;
		myCamera.orthographicSize += (orthoSize - myCamera.orthographicSize) * cameraDriftLerp * Time.fixedDeltaTime;
	}

	public virtual IEnumerable<GameObject> GetGameObjectsToTrack()
	{
		return EMPTY_LIST;
	}

	public Vector3 CalculateDesiredPosition()
	{
		float orthoSize;
		return CalculatePlayerFocusCameraSettings(out orthoSize);
	}

	public virtual void LockTransform(Transform lockedTransform, bool snap = false)
	{
		isLocked = true;
		this.lockedTransform = lockedTransform;
		if (snap)
		{
			base.transform.position = lockedTransform.transform.position;
			base.transform.rotation = lockedTransform.transform.rotation;
		}
	}

	public virtual void UnlockTransform()
	{
		isLocked = false;
	}

	private Vector3 CalculatePlayerFocusCameraSettings(out float orthoSize)
	{
		IEnumerable<GameObject> source = GetGameObjectsToTrack().Concat(additionalCameraTargets);
		if (source.Count() == 0)
		{
			orthoSize = GetComponent<Camera>().orthographicSize;
			return base.transform.position;
		}
		Vector3 vector = new Vector3(source.Min((GameObject p) => p.transform.position.x), source.Min((GameObject p) => p.transform.position.y), source.Min((GameObject p) => p.transform.position.z));
		Vector3 vector2 = new Vector3(source.Max((GameObject p) => p.transform.position.x), source.Max((GameObject p) => p.transform.position.y), source.Max((GameObject p) => p.transform.position.z));
		float t = Mathf.InverseLerp(value: (vector - vector2).magnitude, a: minDistanceBetweenPlayers, b: maxDistanceBetweenPlayers);
		float num = Mathf.Lerp(minDistanceFromWorld, maxDistanceFromWorld, t);
		orthoSize = Mathf.Lerp(minCameraSize, maxCameraSize, t);
		return (vector + vector2) / 2f - base.transform.forward * num + cameraOffset;
	}
}
