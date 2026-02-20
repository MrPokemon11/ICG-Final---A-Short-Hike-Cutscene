using UnityEngine;

public class DynamicWaterFanMesh : MonoBehaviour
{
	public MeshRenderer meshRenderer;

	public MeshFilter filter;

	public ParticleSystem particles;

	public bool two;

	private Mesh mesh;

	private ParticleSystem.Particle[] particleArray;

	private int[] particleArrayBuffer;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private int[] triangles;

	private bool cleanedUp;

	private void Start()
	{
		mesh = new Mesh();
		mesh.MarkDynamic();
		filter.mesh = mesh;
		particleArray = new ParticleSystem.Particle[particles.main.maxParticles];
		particleArrayBuffer = new int[particleArray.Length];
		vertices = new Vector3[particleArray.Length + 1];
		triangles = new int[(particleArray.Length - 1) * 3];
		uvs = new Vector2[particleArray.Length + 1];
	}

	private void LateUpdate()
	{
		if (particles.particleCount != 0 || !cleanedUp)
		{
			int num = particles.GetParticles(particleArray);
			for (int i = 0; i < num; i++)
			{
				particleArrayBuffer[i] = i;
			}
			HeapSort(particleArray, particleArrayBuffer, num);
			for (int j = 0; j < num; j++)
			{
				vertices[j] = base.transform.InverseTransformPoint(particleArray[particleArrayBuffer[j]].position);
				uvs[j] = new Vector2((float)j / ((float)num - 1f), 1f);
			}
			int num2 = particleArray.Length;
			vertices[num2] = Vector3.zero;
			uvs[num2] = new Vector2(0.5f, 0f);
			for (int k = 0; k < num - 1; k++)
			{
				triangles[k * 3] = k;
				triangles[k * 3 + 1] = k + 1;
				triangles[k * 3 + 2] = num2;
			}
			for (int l = num; l < particleArray.Length - 1; l++)
			{
				triangles[l * 3] = num2;
				triangles[l * 3 + 1] = num2;
				triangles[l * 3 + 2] = num2;
			}
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			mesh.RecalculateBounds();
			cleanedUp = num == 0;
		}
	}

	private static void HeapSort(ParticleSystem.Particle[] array, int[] buffer, int n)
	{
		for (int num = n / 2 - 1; num >= 0; num--)
		{
			Heapify(array, buffer, n, num);
		}
		for (int num2 = n - 1; num2 >= 0; num2--)
		{
			int num3 = buffer[0];
			buffer[0] = buffer[num2];
			buffer[num2] = num3;
			Heapify(array, buffer, num2, 0);
		}
	}

	private static void Heapify(ParticleSystem.Particle[] array, int[] buffer, int n, int i)
	{
		int num = i;
		int num2 = 2 * i + 1;
		int num3 = 2 * i + 2;
		if (num2 < n && array[buffer[num2]].remainingLifetime > array[buffer[num]].remainingLifetime)
		{
			num = num2;
		}
		if (num3 < n && array[buffer[num3]].remainingLifetime > array[buffer[num]].remainingLifetime)
		{
			num = num3;
		}
		if (num != i)
		{
			int num4 = buffer[i];
			buffer[i] = buffer[num];
			buffer[num] = num4;
			Heapify(array, buffer, n, num);
		}
	}
}
