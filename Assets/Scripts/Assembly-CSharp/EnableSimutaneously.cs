using UnityEngine;

public class EnableSimutaneously : MonoBehaviour
{
	public GameObject other;

	private void OnEnable()
	{
		other.SetActive(value: true);
	}

	private void OnDisable()
	{
		if (other != null)
		{
			other.SetActive(value: false);
		}
	}
}
