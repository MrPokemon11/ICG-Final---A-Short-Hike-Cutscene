using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
	public class BezierWalkerWithSpeed : MonoBehaviour
	{
		public enum TravelMode
		{
			Once = 0,
			Loop = 1,
			PingPong = 2
		}

		private Transform cachedTransform;

		public BezierSpline spline;

		public TravelMode travelMode;

		public float speed = 5f;

		private float progress;

		[Range(0f, 0.06f)]
		public float relaxationAtEndPoints = 0.01f;

		public float rotationLerpModifier = 10f;

		public bool lookForward = true;

		private bool isGoingForward = true;

		public UnityEvent onPathCompleted = new UnityEvent();

		private bool onPathCompletedCalledAt1;

		private bool onPathCompletedCalledAt0;

		public float NormalizedT
		{
			get
			{
				return progress;
			}
			set
			{
				progress = value;
			}
		}

		private void Awake()
		{
			cachedTransform = base.transform;
		}

		private void Update()
		{
			float num = (isGoingForward ? speed : (0f - speed));
			Vector3 position = spline.MoveAlongSpline(ref progress, num * Time.deltaTime);
			cachedTransform.position = position;
			bool flag = speed > 0f == isGoingForward;
			if (lookForward)
			{
				Quaternion b = ((!flag) ? Quaternion.LookRotation(-spline.GetTangent(progress)) : Quaternion.LookRotation(spline.GetTangent(progress)));
				cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, b, rotationLerpModifier * Time.deltaTime);
			}
			if (flag)
			{
				if (progress >= 1f - relaxationAtEndPoints)
				{
					if (!onPathCompletedCalledAt1)
					{
						onPathCompleted.Invoke();
						onPathCompletedCalledAt1 = true;
					}
					if (travelMode == TravelMode.Once)
					{
						progress = 1f;
						return;
					}
					if (travelMode == TravelMode.Loop)
					{
						progress -= 1f;
						return;
					}
					progress = 2f - progress;
					isGoingForward = !isGoingForward;
				}
				else
				{
					onPathCompletedCalledAt1 = false;
				}
			}
			else if (progress <= relaxationAtEndPoints)
			{
				if (!onPathCompletedCalledAt0)
				{
					onPathCompleted.Invoke();
					onPathCompletedCalledAt0 = true;
				}
				if (travelMode == TravelMode.Once)
				{
					progress = 0f;
					return;
				}
				if (travelMode == TravelMode.Loop)
				{
					progress += 1f;
					return;
				}
				progress = 0f - progress;
				isGoingForward = !isGoingForward;
			}
			else
			{
				onPathCompletedCalledAt0 = false;
			}
		}
	}
}
