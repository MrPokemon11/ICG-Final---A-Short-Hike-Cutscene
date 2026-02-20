using UnityEngine;

public class BuriedCollectable : MonoBehaviour, IBuried
{
	public const string USED_PREFIX = "Unearthed_";

	public float spawnOffset = 1f;

	public float launchSpeed = 10f;

	public GameObject[] buriedPrefabs;

	public GameObject mesh;

	public ParticleSystem particles;

	private bool unearthed;

	public void Start()
	{
		GameObjectID component = GetComponent<GameObjectID>();
		if ((bool)component && component.GetBoolForID("Unearthed_"))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Unearth(Vector3 position)
	{
		if (!unearthed)
		{
			if (mesh != null)
			{
				mesh.SetActive(value: false);
			}
			if ((bool)particles)
			{
				particles.transform.parent = null;
				particles.Play();
			}
			unearthed = true;
			Chest.SpawnRewards(buriedPrefabs, position + Vector3.up, OnAnyCollected, launchSpeed);
		}
	}

	private void OnAnyCollected()
	{
		if (this != null)
		{
			GameObjectID component = GetComponent<GameObjectID>();
			if ((bool)component)
			{
				component.SaveBoolForID("Unearthed_", value: true);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
