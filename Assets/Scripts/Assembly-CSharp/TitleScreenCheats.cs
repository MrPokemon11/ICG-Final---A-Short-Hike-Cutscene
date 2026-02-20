public class TitleScreenCheats : CheatCodeDetector
{
	private void Start()
	{
		RegisterCheat("convertplz", delegate
		{
			I18nProcessor.ShowMenu();
		});
	}
}
