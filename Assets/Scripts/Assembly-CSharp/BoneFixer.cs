using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoneFixer : MonoBehaviour
{
	public GameObject target;

	private void Update()
	{
		if (target == null)
		{
			return;
		}
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		Transform[] bones = target.GetComponent<SkinnedMeshRenderer>().bones;
		foreach (Transform transform in bones)
		{
			dictionary[transform.gameObject.name] = transform;
		}
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		Transform[] bones2 = new Transform[component.bones.Length];
		for (int j = 0; j < component.bones.Length; j++)
		{
			GameObject gameObject = component.bones[j].gameObject;
			gameObject.name = gameObject.name.Replace('.', '_');
			if (!dictionary.ContainsKey(gameObject.name))
			{
				Debug.Log("Unable to map bone \"" + gameObject.name + "\" to target skeleton.");
				continue;
			}
			gameObject.transform.localPosition = dictionary[gameObject.name].localPosition;
			gameObject.transform.localRotation = dictionary[gameObject.name].localRotation;
			gameObject.transform.localScale = dictionary[gameObject.name].localScale;
		}
		component.bones = bones2;
		target = null;
	}

	private void GetAllSkinnedMeshRenderers(ref Dictionary<string, Transform> map)
	{
		SkinnedMeshRenderer[] componentsInChildren = target.GetComponentsInChildren<SkinnedMeshRenderer>();
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform[] bones = array[i].bones;
			foreach (Transform transform in bones)
			{
				if (!dictionary.ContainsKey(transform.gameObject.name))
				{
					dictionary[transform.gameObject.name] = transform;
				}
			}
		}
		map = dictionary;
	}
}
