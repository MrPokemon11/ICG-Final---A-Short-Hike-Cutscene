using UnityEngine;

public class CutsceneEnabler : MonoBehaviour
{
	public MonoBehaviour behaviour;

	public void Enable()
	{
		behaviour.enabled = true;
	}

	public void Disable()
	{
		behaviour.enabled = false;
	}
}
