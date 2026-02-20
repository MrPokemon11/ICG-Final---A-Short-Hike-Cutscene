using UnityEngine;

public class Volleyball : MonoBehaviour
{
	public LayerMask groundLayers;

	public CollectableItem pickaxeItem;

	public AudioClip popNoise;

	private Whackable whackable;

	private float originalWhackForce;

	public VolleyballGameController controller { get; set; }

	public Rigidbody body { get; private set; }

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		whackable = GetComponent<Whackable>();
		whackable.onWhack += OnWhacked;
		originalWhackForce = whackable.whackForce;
		whackable.whackForce = 0f;
	}

	public void Orphan()
	{
		controller = null;
		whackable.whackForce = originalWhackForce;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(controller == null) && (bool)other.GetComponent<WaterRegion>())
		{
			controller.OnBallHitsGround();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(controller == null) && groundLayers.IncludesLayer(collision.gameObject.layer))
		{
			controller.OnBallHitsGround();
		}
	}

	private void OnWhacked(GameObject heldObject)
	{
		Holdable component = heldObject.GetComponent<Holdable>();
		if ((bool)component && component.associatedItem == pickaxeItem)
		{
			Pop();
		}
		else if (controller != null)
		{
			controller.OnBallWhackedByPlayer();
		}
	}

	private void Pop()
	{
		popNoise.Play();
		if (controller != null)
		{
			controller.PopBall();
		}
		Object.Destroy(base.gameObject);
	}

	public void Kill()
	{
		GetComponentInChildren<Animator>().SetTrigger("Kill");
		this.RegisterTimer(0.5f, delegate
		{
			Object.Destroy(base.gameObject);
		});
	}

	public void WhackNoise()
	{
		whackable.WhackNoise();
	}
}
