using UnityEngine;

public class WaterOffset : MonoBehaviour
{
	public WaterRegion water;

	public float offset;

	public Renderer cullingRenderer;

	public float activeDistanceSqr = 2500f;

	public float syncAcceleration = 1f;

	private Rigidbody body;

	private bool active = true;

	private Player player;

	private float syncVelocity;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
	}

	private void FixedUpdate()
	{
		if ((bool)cullingRenderer)
		{
			active = cullingRenderer.isVisible;
			if (active)
			{
				active = (player.transform.position - base.transform.position).sqrMagnitude < activeDistanceSqr;
			}
			body.isKinematic = !active;
		}
		if (active)
		{
			syncVelocity += syncAcceleration * Time.fixedDeltaTime;
			Vector3 position = base.transform.position;
			float target = water.GetWaterY(base.transform.position) + offset;
			position.y = Mathf.MoveTowards(position.y, target, Time.fixedDeltaTime * syncVelocity);
			base.transform.position = position;
		}
		else
		{
			syncVelocity = 0f;
		}
	}
}
