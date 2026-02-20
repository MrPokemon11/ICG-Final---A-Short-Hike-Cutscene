using UnityEngine;
using UnityEngine.UI;

public class FloatingBox : MonoBehaviour
{
	[Header("Links")]
	public RectTransform triangle;

	public Animator animator;

	public RectTransform scaledContainer;

	[Header("Motion and Positioning")]
	public bool restrictToScreen = true;

	public float trianglePadding = 10f;

	public float triangleOffset = 5f;

	public float characterPadding = 15f;

	public float boxLerpSpeed = 1f;

	public float triangleLerpSpeed = 30f;

	public float spawnOffsetDistance = 10f;

	public bool animateContentSwitching = true;

	private Transform targetTransform;

	private RectTransform parentRect;

	private Vector2 targetPos;

	private IFloatingBoxContent currentContent;

	private IFloatingBoxContent upcomingContent;

	private Timer animationTimer;

	public float desiredPositionNormalizedXOffset { get; set; }

	public RectTransform rectTransform { get; private set; }

	public TextBoxStyleProfile styleProfile { get; private set; }

	public Transform target
	{
		get
		{
			return targetTransform;
		}
		set
		{
			SetTarget(value);
		}
	}

	private bool isAnimating
	{
		get
		{
			if (animationTimer != null)
			{
				return !animationTimer.IsDone();
			}
			return false;
		}
	}

	protected virtual void Awake()
	{
		rectTransform = base.transform as RectTransform;
	}

	private void Start()
	{
		parentRect = base.transform.parent as RectTransform;
		if (desiredPositionNormalizedXOffset != 0f)
		{
			scaledContainer.pivot = rectTransform.pivot.SetX((Mathf.Sign(desiredPositionNormalizedXOffset * -1f) + 1f) / 2f);
			rectTransform.pivot = rectTransform.pivot.SetX(scaledContainer.pivot.x);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		UpdateTargetCanvasPosition();
		rectTransform.localPosition = GetDesiredTextBoxPosition();
		triangle.localPosition = GetDesiredTrianglePosition();
		Vector2 vector = rectTransform.TransformPointTo(rectTransform.rect.center, parentRect).SetZ(0f);
		vector = new Vector2(Mathf.Sign(vector.x), Mathf.Sign(vector.y)).normalized;
		vector *= spawnOffsetDistance;
		rectTransform.localPosition += (Vector3)vector;
		LimitTextBoxPosition();
		LimitTrianglePosition();
	}

	protected virtual void SetTarget(Transform target)
	{
		targetTransform = target;
		TextBoxSpeaker textBoxSpeaker = ((target == null) ? null : target.GetComponent<TextBoxSpeaker>());
		if ((bool)textBoxSpeaker && textBoxSpeaker.textBoxStyle != null)
		{
			ApplyStyle(textBoxSpeaker.textBoxStyle);
		}
		if (currentContent != null)
		{
			currentContent.Configure(target, styleProfile);
		}
	}

	private void ApplyStyle(TextBoxStyleProfile styleProfile)
	{
		this.styleProfile = styleProfile;
		triangle.GetComponent<Image>().color = styleProfile.boxColor;
		scaledContainer.GetComponent<Image>().color = styleProfile.boxColor;
	}

	public virtual void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (upcomingContent != null)
		{
			Object.Destroy(upcomingContent.gameObject);
		}
	}

	public void SetContent(IFloatingBoxContent content)
	{
		if (upcomingContent != null)
		{
			Object.Destroy(upcomingContent.gameObject);
		}
		upcomingContent = content;
		if (!animateContentSwitching || currentContent == null)
		{
			UpdateContentDuringAnimationMidpoint();
			return;
		}
		animator.SetTrigger("Switch");
		animationTimer = this.RegisterTimer(1f / 6f, delegate
		{
			animationTimer = null;
		});
	}

	public void UpdateContentDuringAnimationMidpoint()
	{
		if (currentContent != null)
		{
			Object.Destroy(currentContent.gameObject);
		}
		currentContent = upcomingContent;
		upcomingContent = null;
		if (currentContent != null)
		{
			currentContent.gameObject.transform.SetParent(scaledContainer.transform, worldPositionStays: false);
			currentContent.Configure(target, styleProfile);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	protected virtual void Update()
	{
		UpdateTargetCanvasPosition();
	}

	private void LateUpdate()
	{
		if (targetTransform != null)
		{
			Vector2 desiredTextBoxPosition = GetDesiredTextBoxPosition();
			rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, desiredTextBoxPosition, Time.deltaTime * boxLerpSpeed);
		}
		LimitTextBoxPosition();
		rectTransform.localPosition = rectTransform.localPosition.Round();
		Vector3 desired = GetDesiredTrianglePosition();
		MoveTriangleTowardsDesiredPosition(desired);
		LimitTrianglePosition();
	}

	private void UpdateTargetCanvasPosition()
	{
		if (!targetTransform)
		{
			targetPos = Vector2.zero;
		}
		else
		{
			targetPos = QuickUnityExtensions.WorldToRectTransform(targetTransform.transform.position, parentRect);
		}
	}

	private void LimitTextBoxPosition()
	{
		Vector2 vector = new Vector2(float.MinValue, float.MinValue);
		Vector2 vector2 = new Vector2(float.MaxValue, float.MaxValue);
		vector.x = Mathf.Max(vector.x, targetPos.x - rectTransform.rect.width + trianglePadding);
		vector2.x = Mathf.Min(vector2.x, targetPos.x + rectTransform.rect.width - trianglePadding);
		vector.y = Mathf.Max(vector.y, targetPos.y + characterPadding);
		vector += rectTransform.rect.size * rectTransform.pivot;
		vector2 += -rectTransform.rect.size * (Vector2.one - rectTransform.pivot);
		Vector2 vector3 = rectTransform.localPosition;
		vector3.x = Mathf.Clamp(vector3.x, vector.x, vector2.x);
		vector3.y = Mathf.Clamp(vector3.y, vector.y, vector2.y);
		if (restrictToScreen)
		{
			vector = parentRect.rect.min;
			vector2 = parentRect.rect.max;
			float num = parentRect.rect.height * UI.widthPerHeightRatio;
			Vector2 vector4 = ((currentContent == null) ? Vector2.zero : currentContent.extraFloatPadding);
			vector.x = parentRect.rect.center.x - num / 2f + vector4.x;
			vector2.x = parentRect.rect.center.x + num / 2f - vector4.x;
			vector.y += vector4.y;
			vector2.y -= vector4.y;
			vector += rectTransform.rect.size * rectTransform.pivot;
			vector2 += -rectTransform.rect.size * (Vector2.one - rectTransform.pivot);
			vector3.x = Mathf.Clamp(vector3.x, vector.x, vector2.x);
			vector3.y = Mathf.Clamp(vector3.y, vector.y, vector2.y);
		}
		rectTransform.localPosition = vector3;
	}

	private Vector2 GetDesiredTextBoxPosition()
	{
		return targetPos + Vector2.up * (characterPadding + rectTransform.rect.height * rectTransform.pivot.y) + Vector2.right * desiredPositionNormalizedXOffset * rectTransform.rect.width / 2f;
	}

	private void LimitTrianglePosition()
	{
		RectTransform rectTransform = triangle.transform.parent as RectTransform;
		Vector2 vector = triangle.localPosition;
		vector.x = Mathf.Clamp(vector.x, rectTransform.rect.xMin + triangle.rect.width / 2f, rectTransform.rect.xMax - triangle.rect.width / 2f);
		vector.y = rectTransform.rect.yMin + triangleOffset;
		triangle.localPosition = vector;
	}

	private Vector2 GetDesiredTrianglePosition()
	{
		if (targetTransform == null)
		{
			return triangle.localPosition;
		}
		RectTransform rectTransform = triangle.transform.parent as RectTransform;
		Vector2 result = QuickUnityExtensions.WorldToRectTransform(targetTransform.transform.position, rectTransform);
		result.y = rectTransform.rect.yMin + triangleOffset;
		return result;
	}

	protected virtual void MoveTriangleTowardsDesiredPosition(Vector3 desired)
	{
		if (isAnimating)
		{
			triangle.localPosition = desired;
		}
		else
		{
			triangle.localPosition = Vector3.Lerp(triangle.localPosition, desired, Time.deltaTime * triangleLerpSpeed);
		}
	}
}
