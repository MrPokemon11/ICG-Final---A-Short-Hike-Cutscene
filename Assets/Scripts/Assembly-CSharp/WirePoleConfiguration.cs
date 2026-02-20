using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WirePoleConfiguration : MonoBehaviour
{
	public GameObject wirePrefab;

	public Transform wirePoleParent;

	public void Start()
	{
		List<Transform> list = wirePoleParent.GetChildren().ToList();
		wirePrefab.gameObject.SetActive(value: false);
		for (int i = 0; i < list.Count - 1; i++)
		{
			WirePole component = list[i].GetComponent<WirePole>();
			WirePole component2 = list[i + 1].GetComponent<WirePole>();
			for (int j = 0; j < component.localWirePositions.Count; j++)
			{
				Vector3 position = component.transform.TransformPoint(component.localWirePositions[j]);
				Vector3 endPosition = component2.transform.TransformPoint(component2.localWirePositions[j]);
				GameObject obj = wirePrefab.CloneAt(position);
				obj.GetComponent<HangingWireRenderer>().endPosition = endPosition;
				obj.SetActive(value: true);
			}
		}
		wirePrefab.gameObject.SetActive(value: true);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		List<Transform> list = wirePoleParent.GetChildren().ToList();
		for (int i = 0; i < list.Count - 1; i++)
		{
			Gizmos.DrawLine(list[i].position, list[i + 1].position);
		}
	}
}
