using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Music/Music Layer")]
public class MusicLayer : ScriptableObject
{
	public enum StartTime
	{
		StartOnLoop = 0,
		StartImmediately = 1
	}

	public AssetReference clipReference;

	public float normalVolume = 1f;

	public float fadeInTime;

	public float fadeOutTime;

	public StartTime startTime = StartTime.StartImmediately;

	private AsyncOperationHandle<AudioClip> loadingHandle;

	public AudioClip loadedClip { get; private set; }

	private void OnDisable()
	{
	}

	public void StartLoadingClip()
	{
		if (!loadingHandle.IsValid())
		{
			loadingHandle = clipReference.LoadAssetAsync<AudioClip>();
			loadingHandle.Completed += delegate(AsyncOperationHandle<AudioClip> operation)
			{
				loadedClip = operation.Result;
			};
		}
	}
}
