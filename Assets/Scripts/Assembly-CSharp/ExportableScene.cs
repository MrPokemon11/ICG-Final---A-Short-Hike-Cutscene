using System;
using UnityEngine;

[Serializable]
public class ExportableScene
{
	[SerializeField]
	private UnityEngine.Object sceneReference;

	[SerializeField]
	private string sceneName = "";

	public void Validate(MonoBehaviour dirtyTarget)
	{
		string text = ((sceneReference != null) ? sceneReference.name : "");
		if (sceneName != text)
		{
			Debug.LogWarning("Click here to go to the object and fix outdated ExportableScene reference for " + dirtyTarget.gameObject.name, dirtyTarget);
		}
	}

	public string GetSceneName()
	{
		if (sceneReference != null && sceneReference.name != sceneName)
		{
			Debug.LogError("An ExportableScene reference is out of date: " + sceneName + " -> " + sceneReference.name);
		}
		return sceneName;
	}
}
