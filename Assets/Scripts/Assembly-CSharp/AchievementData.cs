using UnityEngine;

[CreateAssetMenu]
public class AchievementData : ScriptableObject
{
	public Sprite sprite;

	public Sprite lockedSprite;

	public string title;

	[TextArea]
	public string description;

	public bool secret;
}
