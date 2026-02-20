using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	private static bool applicationIsQuitting = false;

	public static T instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T)?.ToString() + "' already destroyed on application quit. Won't create again - returning null.");
				return null;
			}
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
					if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
						return _instance;
					}
					if (_instance == null)
					{
						ResourceSingletonAttribute resourceSingletonAttribute = null;
						object[] customAttributes = typeof(T).GetCustomAttributes(inherit: false);
						for (int i = 0; i < customAttributes.Length; i++)
						{
							resourceSingletonAttribute = ((Attribute)customAttributes[i]) as ResourceSingletonAttribute;
							if (resourceSingletonAttribute != null)
							{
								break;
							}
						}
						if (resourceSingletonAttribute != null)
						{
							GameObject gameObject = Resources.Load<GameObject>(resourceSingletonAttribute.resourceFilePath);
							if (gameObject == null)
							{
								Debug.LogError("The Resource Singleton " + typeof(T)?.ToString() + " was not found in any resources folder!");
								return null;
							}
							GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
							gameObject2.name = "(Resource Global Singleton) " + gameObject2.name;
							_instance = gameObject2.GetComponent<T>();
							if (_instance == null)
							{
								Debug.LogError("A prefab was loaded for the singleton, but the component was not on it!");
							}
							else
							{
								UnityEngine.Object.DontDestroyOnLoad(gameObject2);
								Debug.Log("[Singleton] An instance of " + typeof(T)?.ToString() + " is needed in the scene, so '" + gameObject2?.ToString() + "' was loaded as a prefab with DontDestroyOnLoad.");
							}
						}
						else
						{
							GameObject gameObject3 = new GameObject();
							_instance = gameObject3.AddComponent<T>();
							gameObject3.name = "(Global Singleton) " + typeof(T).ToString();
							UnityEngine.Object.DontDestroyOnLoad(gameObject3);
							Debug.Log("[Singleton] An instance of " + typeof(T)?.ToString() + " is needed in the scene, so '" + gameObject3?.ToString() + "' was created with DontDestroyOnLoad.");
						}
					}
					else
					{
						Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
					}
				}
				return _instance;
			}
		}
	}

	public void OnDestroy()
	{
		applicationIsQuitting = true;
	}
}
