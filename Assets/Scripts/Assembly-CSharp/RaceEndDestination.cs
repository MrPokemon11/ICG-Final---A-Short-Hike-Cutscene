using UnityEngine;

public class RaceEndDestination : MonoBehaviour
{
	public float radius = 3f;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
