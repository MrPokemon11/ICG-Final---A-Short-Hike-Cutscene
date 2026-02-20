public class SimpleMusicController : IMusicLayerController
{
	private MusicLayer layer;

	public MusicLayer musicLayer => layer;

	public float volume { get; set; } = 1f;

	public SimpleMusicController(MusicLayer layer)
	{
		this.layer = layer;
	}
}
