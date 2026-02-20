using UnityEngine;

public class BucketUIController : MonoBehaviour
{
	public Animator heavyBucketIcon;

	public GameObject featherUI;

	public AudioClip errorSound;

	private IHeavyItem heavyItem;

	private Player player;

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		player.onHeldItemChanged += OnHeldItemChanged;
		player.onAttemptHeavyAction += OnHeavyActionAttempted;
	}

	private void Update()
	{
		bool flag = heavyItem != null && heavyItem.isHeavy;
		heavyBucketIcon.gameObject.SetActive(flag);
		featherUI.SetActive(!flag && !player.isMounted);
	}

	private void OnHeldItemChanged(Holdable obj)
	{
		heavyItem = ((obj == null) ? null : obj.GetComponent<IHeavyItem>());
	}

	private void OnHeavyActionAttempted()
	{
		heavyBucketIcon.SetTrigger("Error");
		errorSound.Play();
	}
}
