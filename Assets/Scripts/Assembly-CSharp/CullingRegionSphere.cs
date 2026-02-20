using System;
using UnityEngine;

public class CullingRegionSphere
{
	public Vector3 position;

	public float radius;

	public int viewDistance;

	public bool isVisible;

	public event Action<CullingRegionSphere> onStateChanged;

	public CullingRegionSphere(Vector3 position, float radius)
	{
		this.position = position;
		this.radius = radius;
	}

	public void AlertListeners()
	{
		this.onStateChanged(this);
	}
}
