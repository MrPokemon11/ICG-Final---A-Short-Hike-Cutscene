public class GameServiceLocator : AbstractServiceLocator<GameServiceLocator>
{
	public DialogueController dialogue => LocateServiceInActiveScene<DialogueController>();

	public TransitionAnimation transitionAnimation => LocateServiceInActiveScene<TransitionAnimation>();

	public LevelController levelController => LocateServiceInActiveScene<LevelController>();

	public AtmosphereController atmosphereController => LocateServiceInActiveScene<AtmosphereController>();

	public UI ui => LocateServiceInActiveScene<UI>();

	public LevelUI levelUI => LocateServiceInActiveScene<LevelUI>();

	public CullingRegionManager cullingManager => LocateServiceInActiveSceneWithoutErrors<CullingRegionManager>();

	public AchievementManager achievements => LocateOrCreateServiceInActiveScene<AchievementManager>();
}
