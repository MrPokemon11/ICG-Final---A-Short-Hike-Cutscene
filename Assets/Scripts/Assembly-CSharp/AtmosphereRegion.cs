using UnityEngine;

public class AtmosphereRegion : MonoBehaviour
{
	public Atmosphere atmosphere;

	public int priority = 100;

	public bool debugColors;

	private Collider myCollider;

	private bool isPlayerInside;

	private StackResourceSortingKey insideKey;

	private Bounds cachedBounds;

	private AtmosphereController atmosphereController;

	private Player player;

	private void Start()
	{
		myCollider = GetComponent<Collider>();
		cachedBounds = myCollider.bounds;
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		atmosphereController = Singleton<GameServiceLocator>.instance.atmosphereController;
	}

	private void Update()
	{
		if (!Application.isPlaying || !(player != null))
		{
			return;
		}
		bool num = isPlayerInside;
		isPlayerInside = ShouldActivateRegion(player);
		ShouldActivateRegion(player);
		if (num != isPlayerInside)
		{
			if (isPlayerInside)
			{
				Asserts.Null(insideKey);
				insideKey = atmosphereController.AddToAtmosphereStack(priority, atmosphere);
			}
			else
			{
				insideKey.ReleaseResource();
				insideKey = null;
			}
		}
	}

	private bool ShouldActivateRegion(Player player)
	{
		return myCollider.IsPointWithin(player.transform.position, cachedBounds);
	}
}
