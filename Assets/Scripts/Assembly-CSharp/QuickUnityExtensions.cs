using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QuickUnityTools.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QuickUnityExtensions
{
	public static GameObject CloneAt(this GameObject obj, Vector3 position)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(obj);
		gameObject.transform.position = position;
		return gameObject;
	}

	public static GameObject Clone(this GameObject obj)
	{
		return UnityEngine.Object.Instantiate(obj);
	}

	public static GameObject CloneInactive(this GameObject obj)
	{
		obj.gameObject.SetActive(value: false);
		GameObject result = UnityEngine.Object.Instantiate(obj);
		obj.gameObject.SetActive(value: true);
		return result;
	}

	public static string ToHexString(this Color color)
	{
		return ((Color32)color).ToHexString();
	}

	public static string ToHexString(this Color32 color)
	{
		return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
	}

	public static Color HexStringToColor(string hex)
	{
		if (hex[0] == '#')
		{
			hex = hex.Substring(1);
		}
		try
		{
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, byte.MaxValue);
		}
		catch (FormatException)
		{
			return Color.black;
		}
	}

	public static Color SetA(this Color color, float a)
	{
		return new Color(color.r, color.g, color.b, a);
	}

	public static Vector3 WorldMousePosition(this Camera camera, float? distanceFromCamera = null)
	{
		if (!distanceFromCamera.HasValue)
		{
			distanceFromCamera = 0f;
		}
		return camera.ScreenToWorldPoint(Input.mousePosition.SetZ(distanceFromCamera.Value));
	}

	public static Vector3 RandomWithin(this Bounds bounds)
	{
		Vector3 vector = new Vector3(bounds.size.x * UnityEngine.Random.value, bounds.size.y * UnityEngine.Random.value, bounds.size.z * UnityEngine.Random.value);
		return bounds.min + vector;
	}

	public static Vector3 RandomWithin(this BoxCollider box)
	{
		Vector3 position = box.center + Vector3.Scale(box.size, new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) - Vector3.one * 0.5f);
		return box.transform.TransformPoint(position);
	}

	public static Vector3 RandomWithin(this SphereCollider sphere)
	{
		Vector3 position = sphere.center + sphere.radius * UnityEngine.Random.insideUnitSphere;
		return sphere.transform.TransformPoint(position);
	}

	public static Rect SplitHorizontal(this Rect rect, int number, int index, float spacing = 5f)
	{
		float num = (rect.width - spacing * (float)Math.Max(0, number - 1)) / (float)number;
		return new Rect(rect.x + (num + spacing) * (float)index, rect.y, num, rect.height);
	}

	public static Rect SplitHorizontal(this Rect rect, float splitPercent, bool first, float spacing = 5f)
	{
		return new Rect(rect.x + (first ? 0f : ((rect.width - spacing) * splitPercent + spacing)), rect.y, (rect.width - spacing) * (first ? splitPercent : (1f - splitPercent)), rect.height);
	}

	public static Rect SplitVertical(this Rect rect, int number, int index, float spacing = 5f)
	{
		float num = (rect.height - spacing * (float)Math.Max(0, number - 1)) / (float)number;
		return new Rect(rect.x, rect.y + (num + spacing) * (float)index, rect.width, num);
	}

	public static string CamelCaseToSpaces(this string text)
	{
		return Regex.Replace(text, "(\\B[A-Z])", " $1");
	}

	public static IEnumerable<Transform> GetChildren(this Transform transform)
	{
		foreach (Transform item in transform)
		{
			yield return item;
		}
	}

	public static IEnumerable<Transform> GetChildrenRecursive(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			yield return child;
			foreach (Transform item in child.GetChildrenRecursive())
			{
				yield return item;
			}
		}
	}

	public static void DestroyChildren(this Transform transform)
	{
		foreach (Transform item in transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	public static Vector3 TransformPointTo(this RectTransform from, Vector3 point, RectTransform to)
	{
		return to.InverseTransformPoint(from.TransformPoint(point));
	}

	public static Vector2 WorldToRectTransform(Vector3 worldPosition, RectTransform toTransform)
	{
		Vector3 vector = Camera.main.WorldToViewportPoint(worldPosition);
		Vector2 vector2 = new Vector2(vector.x, vector.y) * Mathf.Sign(vector.z);
		RectTransform component = toTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		Vector2 vector3 = Vector2.Scale(component.sizeDelta, vector2 - Vector2.one * 0.5f);
		return component.TransformPointTo(vector3, toTransform);
	}

	public static void CenterWithinParent(this RectTransform rect)
	{
		Vector2 vector = ((rect.parent as RectTransform).rect.size - rect.rect.size) / 2f;
		rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, vector.x, rect.rect.size.x);
		rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, vector.y, rect.rect.size.y);
	}

	public static GameObject GetGameObject(this ResourceRequest request)
	{
		return request.asset as GameObject;
	}

	public static IEnumerable<T> FindComponentsOfTypeInScene<T>(this Scene scene) where T : Component
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			T[] componentsInChildren = gameObject.GetComponentsInChildren<T>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				yield return componentsInChildren[j];
			}
		}
	}

	public static IEnumerable<Component> FindComponentsOfTypeInScene(this Scene scene, Type type)
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(type);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				yield return componentsInChildren[j];
			}
		}
	}

	public static bool IsPointWithin(this Collider collider, Vector3 point, Bounds? bounds = null)
	{
		Bounds bounds2 = (bounds.HasValue ? bounds.Value : collider.bounds);
		if (!bounds2.Contains(point))
		{
			return false;
		}
		Vector3 direction = bounds2.center - point;
		RaycastHit hitInfo;
		return !collider.Raycast(new Ray(point, direction), out hitInfo, direction.magnitude);
	}

	public static bool IsPointInView(this Camera camera, Vector3 point)
	{
		Vector3 vector = camera.WorldToViewportPoint(point);
		if (vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f)
		{
			return vector.z > 0f;
		}
		return false;
	}

	public static bool IsPointInView(this Camera camera, Vector3 point, float offscreenAllowance)
	{
		Vector3 vector = camera.WorldToViewportPoint(point);
		if (vector.x > 0f - offscreenAllowance && vector.x < 1f + offscreenAllowance && vector.y > 0f - offscreenAllowance && vector.y < 1f + offscreenAllowance)
		{
			return vector.z > 0f;
		}
		return false;
	}

	public static IEnumerable<T> Yield<T>(this T item)
	{
		yield return item;
	}

	public static AudioSource Play(this AudioClip clip)
	{
		return Singleton<SoundPlayer>.instance.Play(clip);
	}

	public static bool IncludesLayer(this LayerMask mask, int layer)
	{
		return (int)mask == ((int)mask | (1 << layer));
	}

	public static IEnumerator WrapAsEnumerator(this YieldInstruction yieldInstruction)
	{
		yield return yieldInstruction;
	}

	public static void WaitForWithCoroutine(this Task task, MonoBehaviour coroutineOwner, Action onFinish)
	{
		coroutineOwner.StartCoroutine(WaitForTask(task, onFinish));
	}

	private static IEnumerator WaitForTask(Task task, Action onFinish)
	{
		while (!task.IsCompleted)
		{
			yield return null;
		}
		onFinish();
	}

	public static void Emission(this ParticleSystem system, bool enabled)
	{
		ParticleSystem.EmissionModule emission = system.emission;
		emission.enabled = enabled;
	}

	public static void ToWorldSpaceCapsule(this CapsuleCollider capsule, Vector3 worldPosition, out Vector3 point0, out Vector3 point1, out float radius)
	{
		Vector3 vector = worldPosition + capsule.center;
		radius = 0f;
		float num = 0f;
		Vector3 vector2 = capsule.transform.lossyScale.Abs();
		Vector3 vector3 = Vector3.zero;
		switch (capsule.direction)
		{
		case 0:
			radius = Mathf.Max(vector2.y, vector2.z) * capsule.radius;
			num = vector2.x * capsule.height;
			vector3 = capsule.transform.TransformDirection(Vector3.right);
			break;
		case 1:
			radius = Mathf.Max(vector2.x, vector2.z) * capsule.radius;
			num = vector2.y * capsule.height;
			vector3 = capsule.transform.TransformDirection(Vector3.up);
			break;
		case 2:
			radius = Mathf.Max(vector2.x, vector2.y) * capsule.radius;
			num = vector2.z * capsule.height;
			vector3 = capsule.transform.TransformDirection(Vector3.forward);
			break;
		}
		if (num < radius * 2f)
		{
			vector3 = Vector3.zero;
		}
		point0 = vector + vector3 * (num * 0.5f - radius);
		point1 = vector - vector3 * (num * 0.5f - radius);
	}

	public static void IndexedForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
	{
		int num = 0;
		foreach (T item in collection)
		{
			action(item, num++);
		}
	}

	public static void BufferedForEach<T>(this IEnumerable<T> collection, Func<T, bool> condition, Action<T> performIf)
	{
		LinkedList<T> linkedList = new LinkedList<T>();
		foreach (T item in collection)
		{
			if (condition(item))
			{
				linkedList.AddFirst(item);
			}
		}
		foreach (T item2 in linkedList)
		{
			performIf(item2);
		}
	}

	public static T MinValue<T>(this IEnumerable<T> collection, Func<T, float> heuristic)
	{
		T result = default(T);
		float num = float.PositiveInfinity;
		foreach (T item in collection)
		{
			float num2 = heuristic(item);
			if (num2 < num)
			{
				result = item;
				num = num2;
			}
		}
		return result;
	}

	public static T MaxValue<T>(this IEnumerable<T> collection, Func<T, float> heuristic)
	{
		T result = default(T);
		float num = float.NegativeInfinity;
		foreach (T item in collection)
		{
			float num2 = heuristic(item);
			if (num2 > num)
			{
				result = item;
				num = num2;
			}
		}
		return result;
	}

	public static int MinValueIndex<T>(this IEnumerable<T> collection, Func<T, float> heuristic)
	{
		int result = 0;
		float num = float.PositiveInfinity;
		int num2 = 0;
		foreach (T item in collection)
		{
			float num3 = heuristic(item);
			if (num3 < num)
			{
				result = num2;
				num = num3;
			}
			num2++;
		}
		return result;
	}

	public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		int num = 0;
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int LastIndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		int num = source.Reverse().IndexOf(predicate);
		if (num == -1)
		{
			return -1;
		}
		return source.Count() - 1 - num;
	}

	public static bool AtLeast<T>(this IEnumerable<T> collection, int count, Func<T, bool> predicate = null)
	{
		if (predicate == null)
		{
			predicate = (T item) => true;
		}
		int num = 0;
		foreach (T item in collection)
		{
			if (predicate(item))
			{
				num++;
				if (num == count)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static IList<T> Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
		return list;
	}

	public static void Resize<T>(this List<T> list, int size, T element = default(T))
	{
		int count = list.Count;
		if (size < count)
		{
			list.RemoveRange(size, count - size);
		}
		else if (size > count)
		{
			if (size > list.Capacity)
			{
				list.Capacity = size;
			}
			list.AddRange(Enumerable.Repeat(element, size - count));
		}
	}

	public static T PickRandom<T>(this IList<T> list)
	{
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T PickRandom<T>(this IEnumerable<T> list)
	{
		int num = list.Count();
		if (num == 0)
		{
			return default(T);
		}
		return list.ElementAt(UnityEngine.Random.Range(0, num));
	}

	public static T PickRandomWithWeights<T>(this List<T> list, IList<float> weights)
	{
		float maxInclusive = weights.Sum();
		float num = UnityEngine.Random.Range(0f, maxInclusive);
		int i = 0;
		for (float num2 = 0f; i < weights.Count && num2 + weights[i] < num; i++)
		{
			num2 += weights[i];
		}
		if (i >= weights.Count)
		{
			Debug.LogWarning("Something went wrong...");
			return list[0];
		}
		return list[i];
	}

	public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
	{
		if (!dictionary.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public static T[] Flatten2DArray<T>(this T[,] array)
	{
		T[] array2 = new T[array.GetLength(0) * array.GetLength(1)];
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				array2[i * array.GetLength(1) + j] = array[i, j];
			}
		}
		return array2;
	}

	public static T[,] Unflatten2DArray<T>(this T[] array, int xLength, int yLength)
	{
		T[,] array2 = new T[xLength, yLength];
		for (int i = 0; i < xLength; i++)
		{
			for (int j = 0; j < yLength; j++)
			{
				array2[i, j] = array[i * yLength + j];
			}
		}
		return array2;
	}

	public static T[] Flatten3DArray<T>(this T[,,] array)
	{
		T[] array2 = new T[array.GetLength(0) * array.GetLength(1) * array.GetLength(2)];
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				for (int k = 0; k < array.GetLength(2); k++)
				{
					array2[i * array.GetLength(1) * array.GetLength(2) + j * array.GetLength(2) + k] = array[i, j, k];
				}
			}
		}
		return array2;
	}

	public static T[,,] Unflatten3DArray<T>(this T[] array, int xLength, int yLength, int zLength)
	{
		T[,,] array2 = new T[xLength, yLength, zLength];
		for (int i = 0; i < xLength; i++)
		{
			for (int j = 0; j < yLength; j++)
			{
				for (int k = 0; k < zLength; k++)
				{
					array2[i, j, k] = array[i * yLength * zLength + j * zLength + k];
				}
			}
		}
		return array2;
	}

	public static void Populate<T>(this T[] arr, T value)
	{
		for (int i = 0; i < arr.Length; i++)
		{
			arr[i] = value;
		}
	}
}
