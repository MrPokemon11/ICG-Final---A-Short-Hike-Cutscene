using System.Collections;
using Cinemachine;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TeleportPoint : MonoBehaviour, IInteractableComponent
{
	[Header("Links")]
	public TeleportPoint linkedPoint;

	public Breakable[] blockades;

	public CinemachineVirtualCamera zoomCamera;

	public GameObject blockedDialogPrefab;

	public Transform landpointPoint;

	[Header("Scene Transitions")]
	public int doorId;

	public ExportableScene toScene;

	public int toSceneDoor;

	[Header("Audio")]
	public AudioClip fadeOutSound;

	public AudioMixerSnapshot quietAudio;

	public AudioMixerSnapshot normalAudio;

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
		if ((bool)zoomCamera)
		{
			zoomCamera.enabled = false;
		}
		if (landpointPoint == null)
		{
			landpointPoint = base.transform;
		}
	}

	public bool IsBlocked()
	{
		Breakable[] array = blockades;
		foreach (Breakable breakable in array)
		{
			if (breakable != null && breakable.gameObject.activeInHierarchy && !breakable.broken)
			{
				return true;
			}
		}
		return false;
	}

	public void Interact()
	{
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		GameUserInput input = GameUserInput.CreateInput(base.gameObject);
		TeleportPoint endPoint = ((linkedPoint == null || linkedPoint.IsBlocked()) ? this : linkedPoint);
		if ((bool)quietAudio)
		{
			quietAudio.TransitionTo(1.5f);
		}
		if ((bool)fadeOutSound)
		{
			fadeOutSound.Play();
		}
		if (zoomCamera != null)
		{
			zoomCamera.enabled = true;
		}
		AsyncOperation asyncScene = null;
		if (!string.IsNullOrEmpty(toScene.GetSceneName()))
		{
			LevelController.loadDoor = toSceneDoor;
			asyncScene = SceneManager.LoadSceneAsync(toScene.GetSceneName());
			asyncScene.allowSceneActivation = false;
		}
		TransitionAnimation transition = Singleton<GameServiceLocator>.instance.transitionAnimation;
		transition.Begin(delegate
		{
			if (asyncScene != null)
			{
				transition.Pause();
				asyncScene.allowSceneActivation = true;
			}
			else
			{
				if (this == endPoint)
				{
					StartCoroutine(ShowPrompt());
				}
				player.body.position = endPoint.landpointPoint.position;
				if (endPoint.zoomCamera != null)
				{
					endPoint.zoomCamera.enabled = true;
				}
				if (zoomCamera != null)
				{
					zoomCamera.enabled = false;
				}
			}
		}, delegate
		{
			normalAudio?.TransitionTo(0.75f);
			if (endPoint.zoomCamera != null)
			{
				endPoint.zoomCamera.enabled = false;
			}
			Object.Destroy(input);
		});
	}

	private IEnumerator ShowPrompt()
	{
		TransitionAnimation transition = Singleton<GameServiceLocator>.instance.transitionAnimation;
		transition.Pause();
		yield return new WaitForSeconds(1f);
		GameObject clone = blockedDialogPrefab.Clone();
		transition.AddUI(clone.transform);
		yield return new WaitUntil(() => clone == null);
		transition.Unpause();
	}
}
