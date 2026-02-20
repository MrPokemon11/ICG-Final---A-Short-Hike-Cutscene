using System;
using UnityEngine;

[Obsolete("SceneSingletons aren't really singletons. Use the ServiceLocator instead.")]
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static T instance
	{
		get
		{
			if (_instance != null && !_instance.isActiveAndEnabled)
			{
				_instance = null;
				Debug.Log("[SceneSingleton] The current instance of " + typeof(T)?.ToString() + " is inactive. Searching for an active one...");
			}
			if (_instance == null)
			{
				_instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
				if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
				{
					Debug.LogError("[SceneSingleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
					return _instance;
				}
				if (_instance == null)
				{
					Debug.LogWarning("[SceneSingleton] An instance of " + typeof(T)?.ToString() + " is needed in the scene, but none exists!");
				}
				else
				{
					Debug.Log("[SceneSingleton] Using instance already created: " + _instance.gameObject.name);
				}
			}
			return _instance;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}
}
