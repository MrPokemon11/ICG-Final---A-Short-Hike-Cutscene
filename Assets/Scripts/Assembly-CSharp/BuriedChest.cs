using UnityEngine;

public class BuriedChest : MonoBehaviour, IBuried
{
	private const string TAG_PREFIX = "Unearthed_";

	public GameObject chest;

	public GameObject mesh;

	public ParticleSystem particles;

	public float chestSpawnOffset = 0.727f;

	public AudioClip poofSound;

	public AudioClip spawnSound;

	private bool unearthed;

	private void Start()
	{
		GameObjectID component = GetComponent<GameObjectID>();
		unearthed = (bool)component && component.GetBoolForID("Unearthed_");
		chest.SetActive(unearthed);
		if (unearthed)
		{
			GetComponent<Collider>().enabled = false;
			if (mesh != null)
			{
				mesh.SetActive(value: false);
			}
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
			float duration = 0f;
			if ((bool)particles)
			{
				poofSound.Play();
				particles.transform.position = position;
				particles.Play();
				duration = 0.4f;
			}
			this.RegisterTimer(duration, delegate
			{
				spawnSound.Play();
				chest.transform.position = position + Vector3.up * chestSpawnOffset;
				chest.SetActive(value: true);
				chest.GetComponentInChildren<Chest>().UnearthAnimation();
			});
			GetComponent<Collider>().enabled = false;
			GetComponent<GameObjectID>().SaveBoolForID("Unearthed_", value: true);
			unearthed = true;
		}
	}
}
