using System;
using System.Collections.Generic;
using UnityEngine;

public class CullingRegionManager : ServiceMonoBehaviour
{
	public const int MAX_SPHERES = 100;

	private CullingGroup cullingGroup;

	private List<CullingRegionSphere> cullingSpheres = new List<CullingRegionSphere>();

	private BoundingSphere[] boundingSpheres = new BoundingSphere[100];

	private int boundingSpheresCount;

	private void InitalizeCullingGroup()
	{
		if (cullingGroup == null)
		{
			cullingGroup = new CullingGroup();
			cullingGroup.targetCamera = Camera.main;
			CullingGroup obj = cullingGroup;
			obj.onStateChanged = (CullingGroup.StateChanged)Delegate.Combine(obj.onStateChanged, new CullingGroup.StateChanged(OnCullingGroupChange));
			cullingGroup.SetBoundingSpheres(boundingSpheres);
			cullingGroup.SetBoundingSphereCount(boundingSpheresCount);
			cullingGroup.SetDistanceReferencePoint(Camera.main.transform);
			cullingGroup.SetBoundingDistances(new float[3] { 250f, 500f, 1000f });
		}
	}

	private void OnDestroy()
	{
		if (cullingGroup != null)
		{
			cullingGroup.Dispose();
			cullingGroup = null;
		}
	}

	public void RegisterSpheres(CullingRegionSphere[] spheres)
	{
		foreach (CullingRegionSphere sphere in spheres)
		{
			RegisterSphere(sphere);
		}
	}

	private void RegisterSphere(CullingRegionSphere sphere)
	{
		InitalizeCullingGroup();
		if (boundingSpheresCount >= boundingSpheres.Length)
		{
			Debug.LogError("Could not add culling region since there are too many!");
			return;
		}
		boundingSpheres[boundingSpheresCount] = new BoundingSphere(sphere.position, sphere.radius);
		boundingSpheresCount++;
		cullingSpheres.Add(sphere);
		cullingGroup.SetBoundingSphereCount(boundingSpheresCount);
	}

	public void UnregisterSpheres(CullingRegionSphere[] spheres)
	{
		foreach (CullingRegionSphere sphere in spheres)
		{
			UnregisterSphere(sphere);
		}
	}

	private void UnregisterSphere(CullingRegionSphere sphere)
	{
		int num = cullingSpheres.IndexOf((CullingRegionSphere b) => b == sphere);
		if (num != -1)
		{
			cullingSpheres.RemoveAt(num);
			for (int num2 = num; num2 < boundingSpheresCount - 1; num2++)
			{
				boundingSpheres[num2] = boundingSpheres[num2 + 1];
			}
			boundingSpheresCount--;
			cullingGroup.SetBoundingSphereCount(boundingSpheresCount);
		}
	}

	private void OnCullingGroupChange(CullingGroupEvent evt)
	{
		CullingRegionSphere cullingRegionSphere = cullingSpheres[evt.index];
		cullingRegionSphere.viewDistance = evt.currentDistance;
		if (evt.hasBecomeVisible)
		{
			cullingRegionSphere.isVisible = true;
		}
		if (evt.hasBecomeInvisible)
		{
			cullingRegionSphere.isVisible = false;
		}
		cullingRegionSphere.AlertListeners();
	}
}
