using UnityEngine;

public abstract class ServiceMonoBehaviour : MonoBehaviour
{
	protected virtual void OnEnable()
	{
		if (base.gameObject.scene.FindComponentsOfTypeInScene(GetType()).AtLeast(2))
		{
			Debug.LogError("[ServiceMonoBehaviour] " + GetType().Name + ": Multiple instances of this service in this scene.");
		}
	}
}
