using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public float destroyTime = 2f;

	private void Start()
	{
		this.RegisterTimer(destroyTime, delegate
		{
			Object.Destroy(base.gameObject);
		});
	}
}
