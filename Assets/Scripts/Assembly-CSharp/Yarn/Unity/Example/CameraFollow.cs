using UnityEngine;

namespace Yarn.Unity.Example
{
	public class CameraFollow : MonoBehaviour
	{
		public Transform target;

		public float minPosition = -5.3f;

		public float maxPosition = 5.3f;

		public float moveSpeed = 1f;

		private void Update()
		{
			if (!(target == null))
			{
				Vector3 position = Vector3.Lerp(base.transform.position, target.position, moveSpeed * Time.deltaTime);
				position.x = Mathf.Clamp(position.x, minPosition, maxPosition);
				position.y = base.transform.position.y;
				position.z = base.transform.position.z;
				base.transform.position = position;
			}
		}
	}
}
