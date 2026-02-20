using System;

namespace QuickUnityTools.Input
{
	[Serializable]
	public class GameActionSetSettings
	{
		public static string SAVE_FILE = "GameActionSet.whatsup";

		public string savedBindings;

		public string[] customButtonHandles;

		public static void Save(GameActionSet actionSet)
		{
			GameActionSetSettings gameActionSetSettings = new GameActionSetSettings();
			gameActionSetSettings.savedBindings = actionSet.Save();
			gameActionSetSettings.customButtonHandles = actionSet.customButtonHandles;
			CrossPlatform.SavePrefsFile(SAVE_FILE, gameActionSetSettings, 20000);
		}

		public static GameActionSetSettings Load()
		{
			return CrossPlatform.LoadPrefsFile<GameActionSetSettings>(SAVE_FILE);
		}
	}
}
