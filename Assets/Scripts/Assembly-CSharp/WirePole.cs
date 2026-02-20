using System.Collections.Generic;
using UnityEngine;

public class WirePole : MonoBehaviour
{
	public List<Vector3> localWirePositions;

	private void OnDrawGizmos()
	{
		foreach (Vector3 localWirePosition in localWirePositions)
		{
			Gizmos.DrawCube(base.transform.TransformPoint(localWirePosition), Vector3.one);
		}
	}
}
