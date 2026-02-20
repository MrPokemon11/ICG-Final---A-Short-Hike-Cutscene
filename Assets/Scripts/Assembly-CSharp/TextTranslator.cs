using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTranslator : MonoBehaviour
{
	public bool fontOnly;

	public Material forceTMPMaterial;

	public bool replaceTags;

	public bool protectFontIfUnchanged;

	private TMP_Text tmpText;

	private Text unityText;

	private bool customizedFont;

	private Vector4 originalMargin;

	private LayoutGroup parentLayoutGroup;

	public string originalText { get; set; }

	private string text
	{
		get
		{
			if ((bool)tmpText)
			{
				return tmpText.text;
			}
			return unityText.text;
		}
		set
		{
			if ((bool)tmpText)
			{
				tmpText.text = value;
			}
			else
			{
				unityText.text = value;
			}
		}
	}

	private void Awake()
	{
		tmpText = GetComponent<TMP_Text>();
		unityText = GetComponent<Text>();
		originalText = text;
		if ((bool)base.transform.parent)
		{
			parentLayoutGroup = base.transform.parent.GetComponent<LayoutGroup>();
		}
		if ((bool)tmpText)
		{
			originalMargin = tmpText.margin;
		}
		else if ((bool)unityText && (bool)parentLayoutGroup)
		{
			originalMargin = OffsetToV4(parentLayoutGroup.padding);
		}
	}

	private void OnEnable()
	{
		I18n.onLanguageChanged += UpdateTranslation;
		UpdateTranslation();
	}

	public void UpdateTranslation()
	{
		if (!fontOnly)
		{
			if (replaceTags)
			{
				text = TextReplacer.ReplaceVariables(I18n.Localize(originalText));
			}
			else
			{
				text = I18n.Localize(originalText);
			}
		}
		bool flag = protectFontIfUnchanged && string.Compare(text, I18n.RemoveLocalizationTag(originalText), CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) == 0;
		bool flag2 = I18n.customFont && !flag;
		if (flag2 && (!customizedFont || IsFontOutdated()))
		{
			EnableCustomizedFont();
		}
		else if (customizedFont && !flag2)
		{
			DisableCustomizedFont();
		}
	}

	private bool IsFontOutdated()
	{
		if ((bool)unityText)
		{
			return unityText.font != I18n.customFontUnity;
		}
		if ((bool)tmpText)
		{
			return tmpText.font != I18n.customFontTMP;
		}
		return false;
	}

	private void EnableCustomizedFont()
	{
		customizedFont = true;
		if ((bool)tmpText)
		{
			tmpText.font = I18n.customFontTMP;
			tmpText.fontSize = I18n.customFontSize;
			tmpText.lineSpacing = I18n.customFontSpacing;
			tmpText.margin = I18n.customMargins;
			UpdateTMPMaterial();
		}
		else
		{
			unityText.font = I18n.customFontUnity;
			unityText.fontSize = I18n.customFontSize;
			if ((bool)parentLayoutGroup)
			{
				parentLayoutGroup.padding = V4ToOffset(originalMargin + I18n.customMargins);
			}
		}
	}

	public void DisableCustomizedFont()
	{
		customizedFont = false;
		if ((bool)tmpText)
		{
			tmpText.font = I18n.englishFontTMP;
			tmpText.fontSize = I18n.englishFontSize;
			tmpText.lineSpacing = 0f;
			tmpText.margin = originalMargin;
			UpdateTMPMaterial();
		}
		else
		{
			unityText.font = I18n.englishFontUnity;
			unityText.fontSize = I18n.englishFontSize;
			if ((bool)parentLayoutGroup)
			{
				parentLayoutGroup.padding = V4ToOffset(originalMargin);
			}
		}
	}

	private void UpdateTMPMaterial()
	{
		if ((bool)forceTMPMaterial)
		{
			Material material = new Material(forceTMPMaterial);
			material.mainTexture = tmpText.fontSharedMaterial.mainTexture;
			tmpText.fontSharedMaterial = material;
		}
	}

	private void OnDisable()
	{
		I18n.onLanguageChanged -= UpdateTranslation;
	}

	private Vector4 OffsetToV4(RectOffset offset)
	{
		return new Vector4(offset.left, offset.top, offset.right, offset.bottom);
	}

	private RectOffset V4ToOffset(Vector4 vector)
	{
		return new RectOffset((int)vector[0], (int)vector[2], (int)vector[1], (int)vector[3]);
	}
}
