using System.Collections;
using Cinemachine;
using QuickUnityTools.Audio;
using UnityEngine;

public class MountainTopCutscene : MonoBehaviour
{
	public string saveTag = "MountainTopCutscene";

	public string startNode;

	public GameObject cellphonePrefab;

	public GameObject cellphoneCameraAnimator;

	public CinemachineVirtualCamera cellphoneCamera;

	public CinemachineVirtualCamera lakeCamera;

	public GameObject sitCamera;

	public GameObject bubbleEffect;

	public GameObject updraft;

	public AudioClip rumble;

	public AudioSource wind;

	public AudioSource bubblesSound;

	public GameObject moon;

	private AudioSource rumbleSource;

	private Coroutine fadeCoroutine;

	private void Awake()
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(saveTag))
		{
			ActivateHotSpring();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<Player>() && !Singleton<GlobalData>.instance.gameData.tags.GetBool(saveTag))
		{
			StartCutscene();
		}
	}

	private void StartCutscene()
	{
		Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: true);
		Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(startNode, base.transform).onConversationFinish += OnFinish;
		Singleton<GlobalData>.instance.gameData.tags.SetBool(saveTag);
	}

	private void OnFinish()
	{
		Singleton<GameServiceLocator>.instance.achievements.EnsureAchievement(Achievement.ReachedSummit);
	}

	public void ActivateHotSpring()
	{
		bubbleEffect.SetActive(value: true);
		updraft.SetActive(value: true);
		wind.gameObject.SetActive(value: true);
		bubblesSound.gameObject.SetActive(value: true);
	}

	public void TakeOutCellphone()
	{
		GameObject gameObject = cellphonePrefab.Clone();
		gameObject.name = cellphonePrefab.name;
		Singleton<GameServiceLocator>.instance.levelController.player.PickUp(gameObject.GetComponent<Holdable>(), animate: false);
	}

	public void StartRumble()
	{
		rumbleSource = Singleton<SoundPlayer>.instance.PlayLooped(rumble, Vector3.zero);
		rumbleSource.volume = 0f;
		fadeCoroutine = StartCoroutine(FadeRumble(1f, 1f));
	}

	public IEnumerator FadeRumble(float length, float toValue)
	{
		CinemachineBasicMultiChannelPerlin noiseA = cellphoneCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		CinemachineBasicMultiChannelPerlin noiseB = lakeCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		float startTime = Time.time;
		float startValue = rumbleSource.volume;
		while (Time.time < startTime + length)
		{
			float num = (Time.time - startTime) / length;
			rumbleSource.volume = Mathf.Lerp(startValue, toValue, num);
			noiseA.m_AmplitudeGain = num;
			noiseB.m_AmplitudeGain = num;
			yield return null;
		}
		rumbleSource.volume = toValue;
		noiseA.m_AmplitudeGain = toValue;
		noiseB.m_AmplitudeGain = toValue;
	}

	public void StartBubbles()
	{
		bubbleEffect.SetActive(value: true);
		wind.gameObject.SetActive(value: true);
		float volume = wind.volume;
		wind.volume = 0f;
		StartCoroutine(FadeAudio(wind, 2.5f, volume));
		bubblesSound.gameObject.SetActive(value: true);
		volume = bubblesSound.volume;
		bubblesSound.volume = 0f;
		StartCoroutine(FadeAudio(bubblesSound, 2.5f, volume));
		Singleton<GameServiceLocator>.instance.levelController.player.TurnToFace(updraft.transform);
	}

	public void FinishRumble()
	{
		StopCoroutine(fadeCoroutine);
		fadeCoroutine = StartCoroutine(FadeRumble(2f, 0f));
		this.RegisterTimer(2.5f, delegate
		{
			Object.Destroy(rumbleSource);
		});
	}

	public void ShowLakeCamera()
	{
		lakeCamera.gameObject.SetActive(value: true);
	}

	public void HideLakeCamera()
	{
		lakeCamera.gameObject.SetActive(value: false);
	}

	public void PutAwayCellphone()
	{
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		Holdable heldItem = player.heldItem;
		player.DropItem(playSound: false);
		Object.Destroy(heldItem.gameObject);
	}

	public void HideCellCamera()
	{
		cellphoneCamera.gameObject.SetActive(value: false);
		Singleton<GameServiceLocator>.instance.levelUI.HideUI(hidden: false);
		this.RegisterTimer(3.5f, delegate
		{
			moon.SetActive(value: false);
		});
	}

	public void ShowSitCamera()
	{
		moon.SetActive(value: true);
		sitCamera.SetActive(value: true);
	}

	public void ShowCellCamera()
	{
		sitCamera.SetActive(value: false);
		cellphoneCameraAnimator.SetActive(value: true);
		cellphoneCamera.gameObject.SetActive(value: true);
	}

	public static IEnumerator FadeAudio(AudioSource source, float length, float toValue)
	{
		float startTime = Time.time;
		float startValue = source.volume;
		while (Time.time < startTime + length)
		{
			float t = (Time.time - startTime) / length;
			source.volume = Mathf.Lerp(startValue, toValue, t);
			yield return null;
		}
		source.volume = toValue;
	}
}
