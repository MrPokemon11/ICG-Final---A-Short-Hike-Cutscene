using UnityEngine;

public class FishingLine : MonoBehaviour
{
	public class RopeParticle
	{
		public Vector3 position;

		public Vector3 lastPosition;

		public Vector3 acceleration;
	}

	public int segments = 10;

	public float restLength = 0.1f;

	public float gravity = 1f;

	public float drag = 0.2f;

	public float constraintIterations = 3f;

	public Transform endConstraint;

	private WireLineRenderer line;

	private RopeParticle[] ropeParticles;

	private Vector3[] linePositions;

	public bool initalized => ropeParticles != null;

	private void Awake()
	{
		line = GetComponent<WireLineRenderer>();
	}

	private void Start()
	{
		this.RegisterTimer(0.1f, Initalize);
	}

	private void Initalize()
	{
		ropeParticles = new RopeParticle[segments];
		linePositions = new Vector3[segments];
		for (int i = 0; i < ropeParticles.Length; i++)
		{
			Vector3 position = base.transform.position;
			ropeParticles[i] = new RopeParticle
			{
				position = position,
				lastPosition = position
			};
			linePositions[i] = position;
		}
	}

	private bool IsVisible()
	{
		if (!Camera.main.IsPointInView(linePositions[0]))
		{
			return Camera.main.IsPointInView(linePositions[linePositions.Length - 1]);
		}
		return true;
	}

	public Ray GetLineEndRay()
	{
		Vector3 vector = linePositions[linePositions.Length - 2];
		Vector3 vector2 = linePositions[linePositions.Length - 1];
		return new Ray(vector, vector2 - vector);
	}

	public void SetTotalRestLength(float totalRestLength)
	{
		restLength = totalRestLength / (float)segments;
	}

	private void Update()
	{
		if (initalized && IsVisible())
		{
			float t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
			for (int i = 0; i < ropeParticles.Length; i++)
			{
				linePositions[i] = Vector3.Lerp(ropeParticles[i].lastPosition, ropeParticles[i].position, t);
			}
			linePositions[0] = base.transform.position;
			if ((bool)endConstraint)
			{
				linePositions[linePositions.Length - 1] = endConstraint.position;
			}
			line.SetPoints(linePositions);
		}
	}

	public void ShakeLine(float amount)
	{
		for (int i = 0; i < ropeParticles.Length; i++)
		{
			ropeParticles[i].lastPosition += Random.onUnitSphere * amount;
		}
	}

	private void FixedUpdate()
	{
		if (initalized && IsVisible())
		{
			UpdateRope(Time.fixedDeltaTime);
		}
	}

	private void UpdateRope(float deltaTime)
	{
		for (int i = 0; i < ropeParticles.Length; i++)
		{
			ropeParticles[i].acceleration += Vector3.down * gravity;
		}
		for (int j = 0; j < ropeParticles.Length; j++)
		{
			Verlet(ropeParticles[j], deltaTime);
			ropeParticles[j].acceleration = Vector3.zero;
		}
		for (int k = 0; (float)k < constraintIterations; k++)
		{
			for (int l = 0; l < ropeParticles.Length - 1; l++)
			{
				PoleConstraint(ropeParticles[l], ropeParticles[l + 1], restLength);
			}
			ropeParticles[0].position = base.transform.position;
			if ((bool)endConstraint)
			{
				ropeParticles[ropeParticles.Length - 1].position = endConstraint.position;
			}
		}
	}

	private RopeParticle Verlet(RopeParticle particle, float deltaTime)
	{
		Vector3 position = particle.position;
		Vector3 vector = (particle.position - particle.lastPosition) * (1f - drag);
		particle.position += vector + particle.acceleration * deltaTime * deltaTime;
		particle.lastPosition = position;
		return particle;
	}

	private void PoleConstraint(RopeParticle p1, RopeParticle p2, float restLength)
	{
		Vector3 vector = p2.position - p1.position;
		float magnitude = vector.magnitude;
		if (!Mathf.Approximately(magnitude, 0f))
		{
			float num = (magnitude - restLength) / magnitude;
			p1.position += vector * num * 0.5f;
			p2.position -= vector * num * 0.5f;
		}
	}
}
