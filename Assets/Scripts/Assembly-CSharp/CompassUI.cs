using UnityEngine;

public class CompassUI : MonoBehaviour
{
	public RectTransform compassArrow;

	public RectTransform northSymbol;

	public RectTransform northPoint;

	private LevelController levelController;

	private RectTransform rect;

	private void Start()
	{
		levelController = Singleton<GameServiceLocator>.instance.levelController;
		rect = base.transform as RectTransform;
		Singleton<GlobalData>.instance.gameData.tags.WatchBool("ShowCompass", OnShowCompassChange);
		OnShowCompassChange(Singleton<GlobalData>.instance.gameData.tags.GetBool("ShowCompass"));
	}

	private void OnShowCompassChange(bool shown)
	{
		base.gameObject.SetActive(shown);
	}

	private void Update()
	{
		Vector2 vector = Camera.main.WorldToScreenPoint(levelController.player.transform.position);
		Vector2 vector2 = (Vector2)Camera.main.WorldToScreenPoint(levelController.player.transform.position + Vector3.forward) - vector;
		float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - 90f;
		compassArrow.localEulerAngles = new Vector3(0f, 0f, z);
		northSymbol.transform.position = northPoint.position;
		northSymbol.localPosition = northSymbol.localPosition.SetZ(0f);
	}
}
