using System;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractableComponent
{
	private class VisibilityListener : MonoBehaviour
	{
		public Chest parent;

		private void OnBecameVisible()
		{
			parent.EnableAnimatorTemporarily(forceUpdate: true, 0.5f);
		}
	}

	public const string USED_PREFIX = "Opened_";

	public Animator animator;

	public ParticleSystem puff;

	public AudioClip openSound;

	public float launchSpeed = 10f;

	public GameObject[] prefabsInside;

	public float spawnOffset = 1f;

	private bool _opened;

	private Timer disableSoon;

	private bool opened
	{
		get
		{
			return _opened;
		}
		set
		{
			_opened = value;
			EnableAnimatorTemporarily(forceUpdate: false, 0.5f);
			animator.SetBool("Open", value);
			GetComponent<IInteractable>().enabled = !opened;
		}
	}

	bool IInteractableComponent.enabled
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

	private void Awake()
	{
		animator.keepAnimatorStateOnDisable = true;
		GetComponentInChildren<Renderer>().gameObject.AddComponent<VisibilityListener>().parent = this;
	}

	public void Start()
	{
		GameObjectID component = GetComponent<GameObjectID>();
		opened = (bool)component && component.GetBoolForID("Opened_");
		EnableAnimatorTemporarily(forceUpdate: false, 0.5f);
	}

	public void Interact()
	{
		if (!opened)
		{
			openSound.Play();
			EnableAnimatorTemporarily();
			animator.SetTrigger("OpenAnimation");
			opened = true;
			this.RegisterTimer(0.4f, puff.Play);
			this.RegisterTimer(0.8f, delegate
			{
				SpawnRewards(prefabsInside, base.transform.position + Vector3.up * spawnOffset, OnCollectedTreasure, launchSpeed);
			});
		}
	}

	public void UnearthAnimation()
	{
		EnableAnimatorTemporarily();
		animator.SetTrigger("Unearth");
	}

	private void EnableAnimatorTemporarily(bool forceUpdate = false, float time = 3f)
	{
		animator.enabled = true;
		animator.cullingMode = ((!forceUpdate) ? AnimatorCullingMode.CullCompletely : AnimatorCullingMode.AlwaysAnimate);
		Timer.Cancel(disableSoon);
		disableSoon = this.RegisterTimer(time, delegate
		{
			animator.enabled = false;
		});
	}

	private void OnCollectedTreasure()
	{
		GameObjectID component = GetComponent<GameObjectID>();
		if ((bool)component)
		{
			component.SaveBoolForID("Opened_", value: true);
		}
	}

	public static void SpawnRewards(GameObject[] rewardPrefabs, Vector3 spawnPosition, Action onAnyCollected, float launchSpeed = 20f, float autoCollectTime = 1f)
	{
		foreach (GameObject obj in rewardPrefabs)
		{
			GameObject gameObject = obj.CloneAt(spawnPosition);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			ICollectable collectable = gameObject.GetComponent<ICollectable>();
			if (component != null)
			{
				component.linearVelocity = Vector3.up * launchSpeed * (0.8f + UnityEngine.Random.value * 0.4f);
				component.AddTorque(UnityEngine.Random.insideUnitSphere * 720f);
				if (collectable != null)
				{
					gameObject.AddComponent<MagnetRigidbody>();
				}
			}
			if (collectable != null)
			{
				Action onCollect = null;
				onCollect = delegate
				{
					onAnyCollected?.Invoke();
					collectable.onCollect -= onCollect;
				};
				collectable.onCollect += onCollect;
			}
			Singleton<GameServiceLocator>.instance.RegisterTimer(autoCollectTime, delegate
			{
				if (collectable as MonoBehaviour != null)
				{
					collectable.Collect();
				}
			});
		}
	}
}
