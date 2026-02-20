using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCIKAnimator : MonoBehaviour, ITalkingAnimator, IEmotionAnimator, ICanLook, IPoseAnimator
{
	private class NPCIKAnimatorUpdater : MonoBehaviour
	{
		public static NPCIKAnimatorUpdater instance;

		public List<NPCIKAnimator> objects = new List<NPCIKAnimator>();

		public static NPCIKAnimatorUpdater Initalize()
		{
			if (instance == null)
			{
				instance = new GameObject("NPCIKAnimatorUpdater").AddComponent<NPCIKAnimatorUpdater>();
			}
			return instance;
		}

		private void Update()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				NPCIKAnimator nPCIKAnimator = objects[i];
				if (nPCIKAnimator.isActiveAndEnabled)
				{
					nPCIKAnimator.ManualUpdate();
				}
			}
		}
	}

	public static int SPEED_SQR_ID = Animator.StringToHash("SpeedSqr");

	public Animator animator;

	public Animator headAnimator;

	public float lookAtPlayerRadius = 8f;

	public float headRotateSpeed = 10f;

	public float blendMixSpeed = 0.5f;

	public float headMaxBlend = 1f;

	public Renderer myRenderer;

	private Rigidbody body;

	private Vector3 lookDirection;

	private float ikBlend;

	private SortedList<StackResourceSortingKey, Emotion> emotionStack = new SortedList<StackResourceSortingKey, Emotion>();

	private bool isTalking;

	private LevelController levelController;

	public Transform lookAt { get; set; }

	bool ITalkingAnimator.isTalking => isTalking;

	private bool useIK
	{
		get
		{
			if (!lookAt)
			{
				if (levelController.player != null)
				{
					return (levelController.player.transform.position - base.transform.position).sqrMagnitude < lookAtPlayerRadius.Sqr();
				}
				return false;
			}
			return true;
		}
	}

	private void Start()
	{
		levelController = Singleton<GameServiceLocator>.instance.levelController;
		body = GetComponentInParent<Rigidbody>();
		lookDirection = GetDesiredLookDirection();
		NPCIKAnimatorUpdater.Initalize().objects.Add(this);
	}

	private void OnDestroy()
	{
		if (NPCIKAnimatorUpdater.instance != null)
		{
			NPCIKAnimatorUpdater.Initalize().objects.Remove(this);
		}
	}

	public void SetTalking(bool isTalking)
	{
		this.isTalking = isTalking;
		headAnimator.SetBool("Talking", isTalking);
		animator.SetBool("Talking", isTalking);
	}

	private void ManualUpdate()
	{
		if (myRenderer.isVisible)
		{
			if (body != null)
			{
				animator.SetFloat(SPEED_SQR_ID, body.linearVelocity.SetY(0f).sqrMagnitude);
			}
			Vector3 desiredLookDirection = GetDesiredLookDirection();
			lookDirection = Vector3.RotateTowards(lookDirection, desiredLookDirection, headRotateSpeed * Time.deltaTime, float.PositiveInfinity);
			ikBlend = Mathf.MoveTowards(ikBlend, useIK ? headMaxBlend : 0f, Time.deltaTime * blendMixSpeed);
		}
	}

	private void OnAnimatorIK(int layerIndex)
	{
		animator.SetLookAtPosition(base.transform.position + lookDirection * 1000f);
		animator.SetLookAtWeight(ikBlend);
	}

	private Vector3 GetDesiredLookDirection()
	{
		if (useIK)
		{
			if (lookAt != null)
			{
				return lookAt.position - headAnimator.transform.position;
			}
			return (levelController.player.headTarget - headAnimator.transform.position).normalized;
		}
		return base.transform.forward;
	}

	public StackResourceSortingKey ShowEmotion(Emotion emotion, int priority = 0)
	{
		StackResourceSortingKey stackResourceSortingKey = new StackResourceSortingKey(priority, ClearEmotion);
		emotionStack.Add(stackResourceSortingKey, emotion);
		UpdateEmotionAppearance();
		return stackResourceSortingKey;
	}

	private void ClearEmotion(StackResourceSortingKey key)
	{
		emotionStack.Remove(key);
		UpdateEmotionAppearance();
	}

	private void UpdateEmotionAppearance()
	{
		Emotion emotion = (Emotion)(-1);
		IList<Emotion> values = emotionStack.Values;
		if (values.Count > 0)
		{
			emotion = values[0];
		}
		foreach (Emotion value in Enum.GetValues(typeof(Emotion)))
		{
			headAnimator.SetBool(value.ToString(), value == emotion);
		}
	}

	public Action Pose(Pose pose)
	{
		animator.SetBool(pose.ToString(), value: true);
		return delegate
		{
			animator.SetBool(pose.ToString(), value: false);
		};
	}

	public static int FindNearby(Vector3 position, float honkRadius, NPCIKAnimator[] nearbyArray)
	{
		List<NPCIKAnimator> objects = NPCIKAnimatorUpdater.Initalize().objects;
		int num = 0;
		for (int i = 0; i < objects.Count; i++)
		{
			if ((objects[i].transform.position - position).sqrMagnitude < honkRadius.Sqr())
			{
				nearbyArray[num] = objects[i];
				num++;
				if (num >= nearbyArray.Length)
				{
					return num;
				}
			}
		}
		return num;
	}
}
