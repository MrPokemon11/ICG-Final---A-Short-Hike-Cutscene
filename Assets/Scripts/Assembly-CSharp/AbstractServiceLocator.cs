using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractServiceLocator<TMyType> : Singleton<TMyType> where TMyType : MonoBehaviour
{
	private static bool applicationIsQuitting;

	protected Dictionary<Type, ServiceMonoBehaviour> cachedSceneServices = new Dictionary<Type, ServiceMonoBehaviour>();

	protected virtual void Awake()
	{
		SceneManager.activeSceneChanged += OnActiveSceneChange;
	}

	protected virtual void OnApplicationQuit()
	{
		applicationIsQuitting = true;
	}

	private void OnActiveSceneChange(Scene oldScene, Scene newScene)
	{
		Log("Clearing the cached scene services.");
		cachedSceneServices.Clear();
	}

	protected TService LocateServiceInActiveScene<TService>(bool allowFail = false) where TService : ServiceMonoBehaviour
	{
		TService val = LocateServiceInActiveSceneWithoutErrors<TService>();
		if (val == null && !allowFail)
		{
			LogError(typeof(TService), "Service is not present in this scene!");
		}
		return val;
	}

	protected TService LocateServiceInActiveSceneWithoutErrors<TService>() where TService : ServiceMonoBehaviour
	{
		Type typeFromHandle = typeof(TService);
		if (cachedSceneServices.ContainsKey(typeFromHandle))
		{
			TService val = (TService)cachedSceneServices[typeFromHandle];
			if (val != null)
			{
				return val;
			}
			Log(typeFromHandle, "The cached service was null! Hopefully it was destroyed in a scene transition?");
		}
		TService val2 = SceneManager.GetActiveScene().FindComponentsOfTypeInScene<TService>().FirstOrDefault();
		if (val2 == null)
		{
			return null;
		}
		Log(typeFromHandle, "Located service and caching it.");
		cachedSceneServices[typeFromHandle] = val2;
		return val2;
	}

	protected TService LocateOrCreateServiceInActiveScene<TService>() where TService : ServiceMonoBehaviour
	{
		TService val = LocateServiceInActiveSceneWithoutErrors<TService>();
		if (val != null)
		{
			return val;
		}
		if (applicationIsQuitting)
		{
			return null;
		}
		Type typeFromHandle = typeof(TService);
		Log(typeFromHandle, "Created a new service in the scene.");
		val = new GameObject("(Created Service) " + typeFromHandle.Name).AddComponent<TService>();
		cachedSceneServices[typeFromHandle] = val;
		return val;
	}

	private void Log(string message)
	{
		Log(null, message);
	}

	private void Log(Type service, string message)
	{
		string text = ((service != null) ? (service.Name + ": ") : "");
		Debug.Log("[" + GetType().Name + "] " + text + message);
	}

	private void LogError(Type service, string warning)
	{
		string text = ((service != null) ? (service.Name + ": ") : "");
		Debug.LogError("[" + GetType().Name + "] " + text + warning);
	}
}
