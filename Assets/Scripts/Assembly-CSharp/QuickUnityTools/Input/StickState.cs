using UnityEngine;

namespace QuickUnityTools.Input
{
	public struct StickState
	{
		public const float TAP_DISTANCE_SQR = 0.25f;

		public Vector2 vector;

		private Vector2 prevVector;

		public StickState(Vector2 vector, Vector2 prevVector)
		{
			this.vector = vector;
			this.prevVector = prevVector;
		}

		public bool WasDirectionTapped(Vector2 direction)
		{
			float sqrMagnitude = (vector - direction).sqrMagnitude;
			float sqrMagnitude2 = (prevVector - direction).sqrMagnitude;
			if (sqrMagnitude < 0.25f)
			{
				return sqrMagnitude2 >= 0.25f;
			}
			return false;
		}
	}
}
