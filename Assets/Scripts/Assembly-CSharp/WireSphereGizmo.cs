using UnityEngine;

public class WireSphereGizmo : MonoBehaviour
{
	public Color color = Color.red;

	public float radius = 1f;

	private void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
