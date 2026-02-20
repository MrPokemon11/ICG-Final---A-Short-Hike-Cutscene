using UnityEngine;

namespace RogueNoodle.GBCamera
{
	public class Rotate : MonoBehaviour
	{
		[Tooltip("Axis of rotation - use 0 or 1 only")]
		public Vector3 _rotationAxis = new Vector3(0f, 1f, 0f);

		[Tooltip("Speed of rotation in degrees per second")]
		public float _rotationSpeed = 90f;

		private Transform _transform;

		private void Start()
		{
			_transform = base.transform;
		}

		private void Update()
		{
			_transform.Rotate(_rotationAxis * _rotationSpeed * Time.deltaTime);
		}
	}
}
