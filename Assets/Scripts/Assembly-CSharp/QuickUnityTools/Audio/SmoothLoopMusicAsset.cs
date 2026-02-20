namespace QuickUnityTools.Audio
{
	public class SmoothLoopMusicAsset : MusicStack.IMusicAsset
	{
		private SmoothLoopAudioClip clip;

		public SmoothLoopMusicAsset(SmoothLoopAudioClip clip)
		{
			this.clip = clip;
		}

		public MusicStack.IMusicPlayer CreatePlayer()
		{
			return new SmoothLoopMusicPlayer(clip);
		}

		public int GetMusicID()
		{
			return clip.GetInstanceID();
		}
	}
}
