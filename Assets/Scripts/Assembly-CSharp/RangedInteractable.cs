using System.Collections.Generic;
using UnityEngine;

public class RangedInteractable : MonoBehaviour, IInteractable
{
	private class RangedInteractableUpdater : MonoBehaviour
	{
		public static RangedInteractableUpdater instance;

		private const int MAX_FAST_INTERACTABLES = 4;

		private const int MAX_PER_FRAME = 8;

		private List<RangedInteractable> interactables = new List<RangedInteractable>();

		private int updateIndex;

		private RangedInteractable[] fastQueue = new RangedInteractable[4];

		private int fastQueueLength;

		private LevelController levelController;

		public static RangedInteractableUpdater Initalize()
		{
			if (instance == null)
			{
				instance = new GameObject("RangedInteractableUpdater").AddComponent<RangedInteractableUpdater>();
			}
			return instance;
		}

		private void Start()
		{
			levelController = Singleton<GameServiceLocator>.instance.levelController;
		}

		private void Update()
		{
			if (interactables.Count == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < fastQueueLength; i++)
			{
				RangedInteractable rangedInteractable = fastQueue[i];
				if (fastQueue[i].ManualUpdate())
				{
					fastQueue[num] = rangedInteractable;
					num++;
				}
			}
			for (int j = 0; j < 8; j++)
			{
				RangedInteractable rangedInteractable2 = interactables[updateIndex];
				if (rangedInteractable2.ManualUpdate() && num < 4)
				{
					bool flag = false;
					for (int k = 0; k < num; k++)
					{
						if (fastQueue[k] == rangedInteractable2)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						fastQueue[num] = rangedInteractable2;
						num++;
					}
				}
				updateIndex++;
				if (updateIndex == interactables.Count)
				{
					updateIndex = 0;
				}
			}
			fastQueueLength = num;
		}

		public void Unregister(RangedInteractable rangedInteractable)
		{
			int num = interactables.IndexOf(rangedInteractable);
			if (num == -1)
			{
				return;
			}
			interactables.RemoveAt(num);
			if (updateIndex > num)
			{
				updateIndex--;
			}
			else if (updateIndex == num && interactables.Count > 0)
			{
				updateIndex %= interactables.Count;
			}
			bool flag = false;
			for (int i = 0; i < fastQueueLength; i++)
			{
				if (fastQueue[i] == rangedInteractable)
				{
					flag = true;
				}
				else if (flag)
				{
					fastQueue[i - 1] = fastQueue[i];
				}
			}
			if (flag)
			{
				fastQueueLength--;
			}
		}

		public void Register(RangedInteractable rangedInteractable)
		{
			interactables.Add(rangedInteractable);
		}
	}

	public const int MAX_UPDATE_DELAY = 30;

	public const float VERY_FAR_SQR = 900f;

	public const float MOVING_INPUT = 0.5f;

	public const float MOVING_RANGE = 0.5f;

	public const float LOOK_AT_ANGLE = 75f;

	public float range = 8.5f;

	public bool fineTuneRange;

	public Transform lookAt;

	public int priority;

	private LevelController levelController;

	private Player player;

	private float rangeSqr;

	public bool isRegistered { get; private set; }

	Transform IInteractable.transform
	{
		get
		{
			if (!(lookAt == null))
			{
				return lookAt;
			}
			return base.transform;
		}
	}

	int IInteractable.priority => priority;

	bool IInteractable.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	private void Start()
	{
		levelController = Singleton<ServiceLocator>.instance.Locate<LevelController>();
		player = levelController.player;
		RangedInteractableUpdater.Initalize().Register(this);
		rangeSqr = range * range;
	}

	private void OnDisable()
	{
		if (levelController != null)
		{
			levelController.player.UnregisterInteractable(this);
			isRegistered = false;
		}
	}

	private void OnDestroy()
	{
		if (RangedInteractableUpdater.instance != null)
		{
			RangedInteractableUpdater.instance.Unregister(this);
		}
	}

	public bool ManualUpdate()
	{
		if (!base.isActiveAndEnabled)
		{
			return false;
		}
		float sqrMagnitude = (player.transform.position - base.transform.position).sqrMagnitude;
		bool flag = sqrMagnitude < rangeSqr;
		if (flag && fineTuneRange)
		{
			float num = range;
			if (Vector3.Angle(player.transform.forward, (base.transform.position - player.transform.position).SetY(0f)) > 75f && player.input.GetMovement().sqrMagnitude > 0.25f)
			{
				num *= 0.5f;
			}
			flag = sqrMagnitude < num * num;
		}
		if (flag && !isRegistered)
		{
			player.RegisterInteractable(this);
			isRegistered = true;
		}
		else if (!flag && isRegistered)
		{
			player.UnregisterInteractable(this);
			isRegistered = false;
		}
		if (!(sqrMagnitude < 900f))
		{
			return isRegistered;
		}
		return true;
	}

	public void Interact()
	{
		IInteractableComponent[] components = GetComponents<IInteractableComponent>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Interact();
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, range);
	}
}
