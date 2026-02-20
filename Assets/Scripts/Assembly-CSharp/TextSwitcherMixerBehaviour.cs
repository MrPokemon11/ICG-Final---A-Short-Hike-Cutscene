using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class TextSwitcherMixerBehaviour : PlayableBehaviour
{
	private Color m_DefaultColor;

	private float m_DefaultFontSize;

	private string m_DefaultText;

	private TMP_Text m_TrackBinding;

	private bool m_FirstFrameHappened;

	private string currentOriginalText;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		m_TrackBinding = playerData as TMP_Text;
		if (m_TrackBinding == null)
		{
			return;
		}
		if (!m_FirstFrameHappened)
		{
			m_DefaultColor = m_TrackBinding.color;
			m_DefaultFontSize = m_TrackBinding.fontSize;
			m_DefaultText = m_TrackBinding.text;
			m_FirstFrameHappened = true;
		}
		int inputCount = playable.GetInputCount();
		Color clear = Color.clear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = 0;
		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);
			TextSwitcherBehaviour behaviour = ((ScriptPlayable<TextSwitcherBehaviour>)playable.GetInput(i)).GetBehaviour();
			clear += behaviour.color * inputWeight;
			num += (float)behaviour.fontSize * inputWeight;
			num2 += inputWeight;
			if (inputWeight > num3)
			{
				UpdateText(behaviour.text);
				num3 = inputWeight;
			}
			if (!Mathf.Approximately(inputWeight, 0f))
			{
				num4++;
			}
		}
		m_TrackBinding.color = m_DefaultColor;
		if (num4 != 1 && 1f - num2 > num3)
		{
			UpdateText(m_DefaultText);
		}
	}

	private void UpdateText(string text)
	{
		if (currentOriginalText != text)
		{
			TextTranslator component = m_TrackBinding.GetComponent<TextTranslator>();
			if ((bool)component)
			{
				component.originalText = text;
			}
			m_TrackBinding.text = I18n.Localize(text);
			if ((bool)component && Application.isPlaying)
			{
				component.UpdateTranslation();
			}
			currentOriginalText = text;
		}
	}

	public override void OnPlayableDestroy(Playable playable)
	{
		m_FirstFrameHappened = false;
		if (!(m_TrackBinding == null))
		{
			m_TrackBinding.color = m_DefaultColor;
			m_TrackBinding.text = m_DefaultText;
		}
	}
}
