using UnityEngine;

public class MobileOnly : MonoBehaviour
{
	private void Awake()
	{
		if (!Application.isMobilePlatform && Application.platform != RuntimePlatform.WebGLPlayer)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
