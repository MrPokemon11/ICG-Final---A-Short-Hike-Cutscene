using UnityEngine;
using UnityEngine.Serialization;

public abstract class PhysicsMovement : MonoBehaviour
{
	private const float DESIRED_DELTA_TIME = 1f / 60f;

	private const float EPSILON = 0.001f;

	[SerializeField]
	[FormerlySerializedAs("maxVelocity")]
	private float _maxSpeed = 5f;

	public float movementForce = 8000f;

	public float midairForce = 8000f;

	public float groundStickyForce = 1f;

	public float midairRaycastDistance = 0.2f;

	public float midairRaycastBottomOffset = -0.3f;

	public float midairRayRadius = 0.25f;

	public float turnSpeed;

	public LayerMask groundRaycastMask = -5;

	public GameObject forceApplyPosition;

	public float standingFriction = 1f;

	public float movingFriction;

	protected Vector3 boundsCenterOffset;

	private float boundsYExtents;

	private bool linked;

	private Rigidbody _body;

	private Collider _collider;

	public float maxSpeed
	{
		get
		{
			return _maxSpeed;
		}
		set
		{
			_maxSpeed = value;
		}
	}

	public bool isGrounded { get; protected set; }

	public bool disableFriction { get; set; }

	public Vector3 upDirection => -Physics.gravity.normalized;

	public Rigidbody body
	{
		get
		{
			EnsureLinked();
			return _body;
		}
	}

	public RaycastHit? groundHit { get; protected set; }

	public Collider myCollider
	{
		get
		{
			EnsureLinked();
			return _collider;
		}
	}

	private Vector3 groundRaycastStart => base.transform.position + boundsCenterOffset + Vector3.down * (boundsYExtents + midairRaycastBottomOffset);

	protected virtual void Awake()
	{
		isGrounded = true;
		EnsureLinked();
		boundsCenterOffset = myCollider.bounds.center - base.transform.position;
		boundsYExtents = myCollider.bounds.extents.y;
		if (forceApplyPosition == null)
		{
			forceApplyPosition = base.gameObject;
		}
	}

	private void EnsureLinked()
	{
		if (!linked)
		{
			_body = GetComponent<Rigidbody>();
			_collider = GetComponent<Collider>();
			linked = true;
		}
	}

	protected virtual void Update()
	{
	}

	protected virtual void FixedUpdate()
	{
		UpdateMovement(out var desiredDirection);
		if (isGrounded)
		{
			body.AddForce(-base.transform.up * groundStickyForce);
		}
		if (body.linearVelocity.SetY(0f).sqrMagnitude <= 1.0000001E-06f)
		{
			body.linearVelocity = body.linearVelocity.SetX(0f).SetZ(0f);
		}
		bool flag = desiredDirection * ResolveMaximumVelocity() == Vector3.zero;
		float num = ((isGrounded && flag && !disableFriction) ? standingFriction : movingFriction);
		if (myCollider.material.dynamicFriction != num)
		{
			myCollider.material.dynamicFriction = num;
			myCollider.material.staticFriction = num;
		}
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(groundRaycastStart, groundRaycastStart + Vector3.down * midairRaycastDistance);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(groundRaycastStart + Vector3.down * midairRaycastDistance, midairRayRadius);
	}

	protected virtual void UpdateMovement(out Vector3 desiredDirection)
	{
		isGrounded = CheckIfGrounded();
		desiredDirection = GetDesiredMovementVector();
		if (ResolveCanMove())
		{
			Vector3 desiredVelocityInternal = GetDesiredVelocityInternal(desiredDirection);
			Vector3 vector = body.linearVelocity.SetY(0f);
			Vector3 vector2 = desiredVelocityInternal - vector;
			float num = ResolveMovementForce();
			body.AddForceAtPosition(vector2 * num * Time.fixedDeltaTime, forceApplyPosition.transform.position);
		}
		Vector3 desiredRotateDirection = GetDesiredRotateDirection();
		if (ResolveCanRotate() && desiredRotateDirection != Vector3.zero)
		{
			float num2 = Vector3.Angle(base.transform.forward.SetY(0f), desiredRotateDirection) * Mathf.Sign(Vector3.Cross(base.transform.forward.SetY(0f), desiredRotateDirection).y);
			body.MoveRotation(body.rotation * Quaternion.AngleAxis(num2 * ResolveTurnSpeed() * Time.fixedDeltaTime, upDirection));
		}
	}

	public virtual bool CheckIfGrounded()
	{
		RaycastHit hitInfo;
		bool flag = Physics.SphereCast(new Ray(groundRaycastStart, Vector3.down), midairRayRadius, out hitInfo, midairRaycastDistance, groundRaycastMask.value);
		groundHit = (flag ? new RaycastHit?(hitInfo) : ((RaycastHit?)null));
		return flag;
	}

	public abstract Vector3 GetDesiredMovementVector();

	protected virtual Vector3 GetDesiredVelocityInternal(Vector3 desiredDirection)
	{
		return desiredDirection * ResolveMaximumVelocity();
	}

	protected virtual Vector3 GetDesiredRotateDirection()
	{
		return GetDesiredMovementVector();
	}

	protected virtual bool ResolveCanRotate()
	{
		return ResolveCanMove();
	}

	protected virtual bool ResolveCanMove()
	{
		return true;
	}

	public virtual float ResolveMovementForce()
	{
		if (!isGrounded)
		{
			return midairForce;
		}
		return movementForce;
	}

	public virtual float ResolveTurnSpeed()
	{
		return turnSpeed;
	}

	public virtual float ResolveMaximumVelocity()
	{
		return _maxSpeed;
	}
}
