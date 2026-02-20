using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameObjectID))]
public class SaveRegion : MonoBehaviour
{
	public const string SAVE_REGION_TAG = "SaveRegion";

	public Transform[] spawnPoints;

	private GameObjectID _id;

	public GameObjectID id
	{
		get
		{
			if (_id == null)
			{
				_id = GetComponent<GameObjectID>();
			}
			return _id;
		}
	}

	private void Awake()
	{
		if (spawnPoints.Length == 0)
		{
			spawnPoints = new Transform[1] { base.transform.Find("SavePoint") };
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>())
		{
			Singleton<GlobalData>.instance.gameData.tags.SetString("SaveRegion", id.id);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Transform[] array = spawnPoints;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawCube(array[i].position, Vector3.one * 2f);
		}
	}

	public Vector3 GetNearestLoadPosition(Vector3? loadPoint = null)
	{
		if (!loadPoint.HasValue)
		{
			return spawnPoints[0].position;
		}
		return spawnPoints.MinValue((Transform p) => (p.position - loadPoint.Value).sqrMagnitude).position;
	}

	public static void ClearStoredSaveRegion()
	{
		Singleton<GlobalData>.instance.gameData.tags.SetString("SaveRegion", "");
	}

	public static SaveRegion GetStoredSaveRegion()
	{
		string savedRegion = Singleton<GlobalData>.instance.gameData.tags.GetString("SaveRegion");
		if (!string.IsNullOrEmpty(savedRegion))
		{
			SaveRegion saveRegion = Object.FindObjectsOfType<SaveRegion>().FirstOrDefault((SaveRegion r) => r.id.id == savedRegion);
			if ((bool)saveRegion)
			{
				return saveRegion;
			}
		}
		return null;
	}
}
