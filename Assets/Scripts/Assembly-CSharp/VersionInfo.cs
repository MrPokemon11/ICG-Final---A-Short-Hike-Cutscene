using UnityEngine;

[CreateAssetMenu]
public class VersionInfo : ScriptableObject
{
	public string date = "";

	public int dateBuild;

	public int bigVersion = 1;

	public int middleVersion = 1;

	public int smallVersion = 1;

	public string version => bigVersion + "." + middleVersion + "." + smallVersion;

	public string buildNumber => date + "." + dateBuild;

	public static VersionInfo Load()
	{
		return Resources.Load<VersionInfo>("VersionInfo");
	}
}
