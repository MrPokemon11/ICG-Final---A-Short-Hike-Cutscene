using QuickUnityTools.Audio;
using UnityEngine;

public class PlaySoundEvent : MonoBehaviour
{
	public AudioClip sound;

	public void PlaySound()
	{
		Singleton<SoundPlayer>.instance.Play(sound);
	}
}
