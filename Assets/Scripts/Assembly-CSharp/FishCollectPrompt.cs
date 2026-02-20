using QuickUnityTools.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectPrompt : MonoBehaviour
{
	public Image icon;

	public Image fishPicture;

	public TMP_Text fishTypeName;

	public TMP_Text fishTitle;

	public TMP_Text fishInfo;

	public TextTranslator beforeName;

	public TextTranslator afterName;

	public Image newPill;

	public Image newRecordPill;

	private FocusableUserInput input;

	private void Start()
	{
		input = GameUserInput.CreateInput(base.gameObject);
	}

	private void Update()
	{
		if (input.WasDismissPressed())
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Setup(Fish fish)
	{
		fishTitle.text = fish.GetTitleWithSize();
		fishInfo.text = string.Format("{0} cm", fish.size.ToString("0.0"));
		fishTypeName.text = I18n.Localize(fish.species.typeName);
		beforeName.UpdateTranslation();
		afterName.UpdateTranslation();
		newPill.enabled = false;
		newRecordPill.enabled = false;
		if (Singleton<GlobalData>.instance.gameData.inventory.GetCatchCount(fish.species) == 1)
		{
			newPill.enabled = true;
		}
		else if (Singleton<GlobalData>.instance.gameData.inventory.GetBiggestFishRecord(fish.species, fish.rare) == fish)
		{
			newRecordPill.enabled = true;
		}
		FishItemActions.SetupFishSprite(fish, fishPicture);
		if ((bool)fish.species.customIcon)
		{
			icon.sprite = fish.species.customIcon;
		}
	}
}
