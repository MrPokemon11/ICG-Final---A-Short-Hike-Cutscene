using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextBoxContent : MonoBehaviour, IFloatingBoxContent
{
	[Header("Text Links")]
	public TextMeshProUGUI textMesh;

	public AudioSource beepSource;

	[FormerlySerializedAs("beeps")]
	public AudioClip[] defaultBeeps;

	[Header("Text Animations")]
	public bool animateSetText = true;

	public float secondsPerCharacter = 0.05f;

	public float beepFrequency = 0.05f;

	private float characterCountdown;

	private ITalkingAnimator talkingAnimator;

	private Timer beepTimer;

	private AudioClip[] currentBeepClips;

	private bool hasBeenConfigured;

	public Vector2 extraFloatPadding => Vector2.zero;

	public bool isFinishedAnimating => textMesh.maxVisibleCharacters >= textMesh.textInfo.characterCount;

	GameObject IFloatingBoxContent.gameObject => base.gameObject;

	protected virtual void Awake()
	{
		textMesh.text = "";
		currentBeepClips = defaultBeeps;
	}

	public void Configure(Transform target, TextBoxStyleProfile styleProfile)
	{
		if (talkingAnimator != null)
		{
			talkingAnimator.SetTalking(isTalking: false);
		}
		talkingAnimator = ((target == null) ? null : target.GetComponentInChildren<ITalkingAnimator>());
		if (styleProfile != null)
		{
			textMesh.color = styleProfile.textColor;
			beepSource.pitch = styleProfile.pitch;
			currentBeepClips = ((styleProfile.customBeepClips != null && styleProfile.customBeepClips.Length != 0) ? styleProfile.customBeepClips : defaultBeeps);
		}
		hasBeenConfigured = true;
	}

	private void OnDestroy()
	{
		if (talkingAnimator != null)
		{
			talkingAnimator.SetTalking(isTalking: false);
		}
	}

	public void Reset(string text)
	{
		textMesh.text = text;
		textMesh.maxVisibleCharacters = 0;
		textMesh.ForceMeshUpdate(ignoreActiveState: true);
	}

	public void SkipTextAnimation()
	{
		textMesh.maxVisibleCharacters += 10000;
	}

	protected virtual void Update()
	{
		if (!hasBeenConfigured)
		{
			return;
		}
		if (textMesh.maxVisibleCharacters < textMesh.textInfo.characterCount)
		{
			characterCountdown += Time.deltaTime;
			if (characterCountdown > secondsPerCharacter)
			{
				int num = (int)Mathf.Floor(characterCountdown / secondsPerCharacter);
				textMesh.maxVisibleCharacters += num;
				characterCountdown -= (float)num * secondsPerCharacter;
				if (beepTimer == null)
				{
					beepSource.Stop();
					beepSource.clip = currentBeepClips.PickRandom();
					beepSource.Play();
					Timer.Cancel(beepTimer);
					beepTimer = this.RegisterTimer(beepFrequency, delegate
					{
						beepTimer = null;
					});
				}
			}
		}
		if (talkingAnimator != null)
		{
			talkingAnimator.SetTalking(textMesh.maxVisibleCharacters < textMesh.textInfo.characterCount);
		}
	}
}
