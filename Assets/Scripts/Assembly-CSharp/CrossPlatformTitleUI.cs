using UnityEngine;

public class CrossPlatformTitleUI : MonoBehaviour
{
	public GameObject gamerTagPrefab;

	private void Awake()
	{
		if (gamerTagPrefab != null)
		{
			gamerTagPrefab.Clone();
		}
	}
}
