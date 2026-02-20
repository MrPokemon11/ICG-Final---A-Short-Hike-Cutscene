using UnityEngine;

[CreateAssetMenu]
public class FishSpecies : ScriptableObject
{
	private static FishSpecies[] ALL;

	[Header("Display")]
	public string readableName;

	public string typeName = "fish";

	public Sprite customIcon;

	public Sprite sprite;

	public Sprite rareSprite;

	public string rarePrefix = "Albino";

	public Color rareHighlightColor = Color.white;

	[TextArea]
	public string journalInfo;

	[Header("Size")]
	public Range size = new Range(28f, 33f);

	public float rareLikelihood = 1f;

	[Header("Economy")]
	public int price = 10;

	[Header("Fish Behaviour")]
	public float encounterTime = 10f;

	public float nibbleCheckPeriod = 0.25f;

	public float nibbleChance = 0.0625f;

	public int minRequiredNibbles = 2;

	public int maxRequiredNibbles = 8;

	public static FishSpecies Load(string name)
	{
		FishSpecies fishSpecies = Resources.Load<FishSpecies>("Fish/" + name);
		if (!fishSpecies)
		{
			return Resources.Load<FishSpecies>("Fish/CarpFish");
		}
		return fishSpecies;
	}

	public string GetRarePrefix()
	{
		return $"<color={rareHighlightColor.ToHexString()}>{I18n.Localize(rarePrefix)}</color>";
	}

	public static FishSpecies[] LoadAll()
	{
		if (ALL == null)
		{
			ALL = Resources.LoadAll<FishSpecies>("Fish/");
		}
		return ALL;
	}
}
