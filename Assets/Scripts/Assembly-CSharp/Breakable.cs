using System.Collections;
using System.Linq;
using UnityEngine;

public class Breakable : MonoBehaviour, IWhackable
{
	public const string SAVE_TAG = "Broke_";

	public const int DEBRIS_LAYER = 17;

	public int priority;

	public int hits = 1;

	public CollectableItem[] breakItems;

	public float explosionForce = 100f;

	public float explosionRadius = 5f;

	public AudioClip hitSound;

	public AudioClip breakSound;

	private GameObjectID id;

	private BreakablePiece[] pieces;

	public bool broken { get; private set; }

	int IWhackable.priority => priority;

	private void Awake()
	{
		pieces = GetComponentsInChildren<BreakablePiece>();
		id = GetComponent<GameObjectID>();
		if ((bool)id && id.GetBoolForID("Broke_"))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Whack(GameObject heldObject)
	{
		if (!broken && breakItems.Contains(heldObject.GetComponent<Holdable>().associatedItem))
		{
			hits--;
			if (hits == 0)
			{
				Break(heldObject);
			}
			else
			{
				hitSound.Play().pitch = 0.9f + Random.value * 0.2f;
			}
		}
	}

	private void Break(GameObject heldObject)
	{
		broken = true;
		Camera.main.GetComponent<CameraShake>().Shake(CameraShake.Intensity.Moderate);
		breakSound.Play();
		if ((bool)id)
		{
			id.SaveBoolForID("Broke_", value: true);
		}
		Vector3 normalized = (base.transform.position - heldObject.transform.position).normalized;
		BreakablePiece[] array = pieces;
		foreach (BreakablePiece breakablePiece in array)
		{
			breakablePiece.gameObject.layer = 17;
			Rigidbody component = breakablePiece.GetComponent<Rigidbody>();
			component.isKinematic = false;
			component.AddExplosionForce(explosionForce, base.transform.position + normalized * (explosionRadius / 2f), explosionRadius);
			if (breakablePiece.shrinkTime > 0f)
			{
				StartCoroutine(ShrinkPiece(breakablePiece));
			}
		}
	}

	private IEnumerator ShrinkPiece(BreakablePiece piece)
	{
		Vector3 originalScale = piece.transform.localScale;
		float time = piece.shrinkTime;
		while (time > 0f)
		{
			piece.transform.localScale = originalScale * (time / piece.shrinkTime);
			time -= Time.deltaTime;
			yield return null;
		}
		Object.Destroy(piece.gameObject);
	}
}
