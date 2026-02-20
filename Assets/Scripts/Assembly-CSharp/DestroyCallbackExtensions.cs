using System;
using UnityEngine;

public static class DestroyCallbackExtensions
{
	private class DestroyCallback : MonoBehaviour
	{
		private event Action onDestroy;

		public void RegisterOnDestroyCallback(Action destroyEvent)
		{
			onDestroy += destroyEvent;
		}

		public void UnregisterOnDestroyCallback(Action destroyEvent)
		{
			onDestroy -= destroyEvent;
		}

		private void OnDestroy()
		{
			if (this.onDestroy != null)
			{
				this.onDestroy();
			}
		}
	}

	public static void RegisterOnDestroyCallback(this GameObject obj, Action callback)
	{
		DestroyCallback destroyCallback = obj.GetComponent<DestroyCallback>();
		if (destroyCallback == null)
		{
			destroyCallback = obj.AddComponent<DestroyCallback>();
		}
		destroyCallback.RegisterOnDestroyCallback(callback);
	}

	public static void UnregisterOnDestroyCallback(this GameObject obj, Action callback)
	{
		DestroyCallback component = obj.GetComponent<DestroyCallback>();
		if (!(component == null))
		{
			component.UnregisterOnDestroyCallback(callback);
		}
	}
}
