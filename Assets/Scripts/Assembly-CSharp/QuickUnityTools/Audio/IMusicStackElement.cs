namespace QuickUnityTools.Audio
{
	public interface IMusicStackElement
	{
		MusicStack.IMusicAsset GetMusicAsset();

		MusicStack.Transition GetTakeControlTransition();

		MusicStack.Transition GetReleaseControlTransition();

		float GetDesiredVolume();

		MusicStackPriorty GetPriority();
	}
}
