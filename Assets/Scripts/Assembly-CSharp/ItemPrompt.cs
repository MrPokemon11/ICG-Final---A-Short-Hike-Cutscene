using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.UI;

public class ItemPrompt : MonoBehaviour
{
	public Image icon;

	public Text itemName;

	public TextTranslator beforeName;

	public TextTranslator afterName;

	private FocusableUserInput input;

	private CollectableItem item;

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

	public void Setup(CollectableItem item)
	{
		this.item = item;
		icon.sprite = item.icon;
		itemName.text = string.Format(I18n.STRINGS.itemNameHighlight, I18n.Localize(item.readableName));
		beforeName.UpdateTranslation();
		afterName.UpdateTranslation();
	}
}
