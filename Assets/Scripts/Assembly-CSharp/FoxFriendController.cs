using System.Collections;
using UnityEngine;

public class FoxFriendController : MonoBehaviour
{
	public PlayerReplay playerReplay;

	public RangedInteractable interactable;

	public string climbedTag = "FoxClimbedToTop";

	public GameObject cameraProp;

	public GameObject cutsceneCamera;

	public GameObject photoCamera;

	public Transform playerWalkTo;

	private void Start()
	{
		cameraProp.SetActive(value: false);
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(climbedTag))
		{
			base.transform.position = playerReplay.data.lastFrame.position;
			base.transform.rotation = playerReplay.data.lastFrame.rotation;
		}
	}

	public void StartClimbing()
	{
		interactable.enabled = false;
		playerReplay.Play(-1f);
		playerReplay.onStop += OnFinishClimbing;
		Singleton<GlobalData>.instance.gameData.tags.SetBool(climbedTag);
	}

	public IEnumerator WalkPlayer()
	{
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		player.WalkTo(playerWalkTo.position);
		yield return new WaitUntil(() => !player.walkTo.HasValue);
		player.TurnToFace(base.transform.position);
	}

	public void CameraShift1()
	{
		photoCamera.SetActive(value: false);
		cutsceneCamera.SetActive(value: true);
	}

	public void CameraShift2()
	{
		photoCamera.SetActive(value: true);
		cutsceneCamera.SetActive(value: false);
	}

	public void CameraShift3()
	{
		photoCamera.SetActive(value: false);
		cutsceneCamera.SetActive(value: false);
	}

	public void ShowPropCamera()
	{
		cameraProp.SetActive(value: true);
	}

	public void HidePropCamera()
	{
		cameraProp.SetActive(value: false);
	}

	private void OnFinishClimbing()
	{
		playerReplay.onStop -= OnFinishClimbing;
		interactable.enabled = true;
	}
}
