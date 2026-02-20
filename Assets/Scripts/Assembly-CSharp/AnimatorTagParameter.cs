using UnityEngine;

public class AnimatorTagParameter : MonoBehaviour
{
	public Animator animator;

	public string boolParameter;

	public string dataTag;

	private void Start()
	{
		Singleton<GlobalData>.instance.gameData.tags.WatchBool(dataTag, OnTagChanged);
		OnTagChanged(Singleton<GlobalData>.instance.gameData.tags.GetBool(dataTag));
	}

	private void OnTagChanged(bool tagValue)
	{
		animator.SetBool(boolParameter, tagValue);
	}
}
