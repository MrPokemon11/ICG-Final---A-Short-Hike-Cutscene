using UnityEngine;

public class BucketActions : MonoBehaviour, IHoldableAction, IHeavyItem
{
	public LayerMask waterableLayers;

	[Header("Motion and Animation")]
	public float averageVelocityLerpSpeed = 10f;

	public float rotationMultiplier = 2.5f;

	public float lerpBackTime = 0.25f;

	public float pourDistanceRight = 1f;

	public float pourDistanceUp = 0.25f;

	public float pourAnimationLerpSpeed = 15f;

	[Header("Links")]
	public Transform bucketMesh;

	public MeshRenderer bucketFilledMesh;

	public ParticleSystem waterDroplets;

	public ParticleSystem spillParticles;

	public AudioClip fillSound;

	public AudioClip spillSound;

	public AudioClip scoopSound;

	private bool _filled;

	private Holdable holdable;

	private bool rotateBucketDown = true;

	private bool pourAnimation;

	private Vector3 prevPosition;

	private Vector3 averageVelocity;

	private Quaternion originalMeshLocalRotation;

	private Vector3 originalMeshLocalPosition;

	private Quaternion startLerpBackRotation;

	private Vector3 startLerpBackLocalPosition;

	private float lerpBackCountdown;

	public bool filled
	{
		get
		{
			return _filled;
		}
		set
		{
			bool flag = _filled;
			_filled = value;
			if (_filled != flag)
			{
				bucketFilledMesh.enabled = _filled;
				(_filled ? fillSound : spillSound).Play();
				if (!_filled)
				{
					ParticleSystem.EmissionModule emission = waterDroplets.emission;
					emission.enabled = false;
					spillParticles?.Play();
				}
			}
		}
	}

	public bool isHeavy => filled;

	private void Start()
	{
		originalMeshLocalRotation = bucketMesh.localRotation;
		originalMeshLocalPosition = bucketMesh.localPosition;
		holdable = GetComponent<Holdable>();
		holdable.onReleased += OnReleased;
		holdable.onPickedUp += OnPickedUp;
		bucketFilledMesh.enabled = false;
	}

	private void OnPickedUp()
	{
		prevPosition = base.transform.position;
	}

	private void OnReleased()
	{
		filled = false;
	}

	private void Update()
	{
		if (holdable.anchoredTo == null)
		{
			bucketMesh.localRotation = originalMeshLocalRotation;
			averageVelocity = Vector3.zero;
		}
		else
		{
			Transform transform = holdable.anchoredTo.transform;
			Player component = transform.GetComponent<Player>();
			if (component.transform.position.y < component.waterY)
			{
				filled = true;
			}
			ParticleSystem.EmissionModule emission = waterDroplets.emission;
			emission.enabled = filled && component.body.velocity.sqrMagnitude > 1f;
			if (pourAnimation)
			{
				bucketMesh.rotation = Quaternion.Lerp(bucketMesh.rotation, Quaternion.LookRotation(Vector3.down, transform.forward), Time.deltaTime * pourAnimationLerpSpeed);
				bucketMesh.InverseTransformDirection(transform.forward);
				Vector3 b = originalMeshLocalPosition + bucketMesh.InverseTransformDirection(transform.right) * pourDistanceRight + bucketMesh.InverseTransformDirection(Vector3.up) * pourDistanceUp;
				bucketMesh.localPosition = Vector3.Lerp(bucketMesh.localPosition, b, Time.deltaTime * pourAnimationLerpSpeed);
			}
			if (rotateBucketDown)
			{
				Vector3 b2 = (base.transform.position - prevPosition) / Mathf.Max(Time.deltaTime, 0.001f);
				averageVelocity = Vector3.Lerp(averageVelocity, b2, Time.deltaTime * averageVelocityLerpSpeed);
				float angle = Vector3.Dot(holdable.anchoredTo.transform.forward, averageVelocity) * rotationMultiplier;
				bucketMesh.rotation = Quaternion.AngleAxis(angle, transform.right) * Quaternion.LookRotation(Vector3.up, transform.forward);
				if (lerpBackCountdown > 0f)
				{
					lerpBackCountdown -= Time.deltaTime;
					float t = 1f - lerpBackCountdown / lerpBackTime;
					bucketMesh.rotation = Quaternion.Lerp(startLerpBackRotation, bucketMesh.rotation, t);
					bucketMesh.localPosition = Vector3.Lerp(startLerpBackLocalPosition, originalMeshLocalPosition, t);
				}
			}
		}
		prevPosition = base.transform.position;
	}

	public void ActivateAction(int parameter)
	{
		if (parameter == 0)
		{
			rotateBucketDown = false;
		}
		if (parameter == 1)
		{
			pourAnimation = false;
			rotateBucketDown = true;
			lerpBackCountdown = lerpBackTime;
			startLerpBackLocalPosition = bucketMesh.localPosition;
			startLerpBackRotation = bucketMesh.rotation;
		}
		if (parameter == 2 && holdable.anchoredTo != null)
		{
			Player component = holdable.anchoredTo.GetComponent<Player>();
			if (component.isInWater && component.feetInWater)
			{
				filled = true;
			}
		}
		if (parameter == 3 && holdable.anchoredTo != null)
		{
			filled = false;
			Player component2 = holdable.anchoredTo.GetComponent<Player>();
			Vector3 atPoint = bucketMesh.position.SetY(component2.myCollider.bounds.min.y);
			DetectWaterableItems(atPoint);
		}
		if (parameter == 4)
		{
			pourAnimation = true;
			rotateBucketDown = false;
		}
	}

	private void DetectWaterableItems(Vector3 atPoint)
	{
		Collider[] array = Physics.OverlapSphere(atPoint, 0.75f, waterableLayers.value);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetComponent<IWaterable>()?.Water();
		}
	}
}
