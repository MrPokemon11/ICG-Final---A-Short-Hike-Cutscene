using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InstancedHangingWireInstance : IHangingWireInstance
{
	private class InstancedHangingWireUpdater : MonoBehaviour
	{
		public struct WireStyle
		{
			public Material material;

			public int nodes;
		}

		public class WireStyleInstances
		{
			public List<InstancedHangingWireInstance> instances = new List<InstancedHangingWireInstance>();

			public List<Matrix4x4> matrices = new List<Matrix4x4>();

			public Mesh mesh;

			public void Add(InstancedHangingWireInstance instance, Matrix4x4 matrix)
			{
				instances.Add(instance);
				matrices.Add(matrix);
			}

			public void Remove(InstancedHangingWireInstance wire)
			{
				int num = instances.IndexOf(wire);
				if (num != -1)
				{
					instances.RemoveAt(num);
					matrices.RemoveAt(num);
				}
			}

			public void UpdateMatrix(InstancedHangingWireInstance wire, Matrix4x4 matrix)
			{
				int num = instances.IndexOf(wire);
				if (num != -1)
				{
					matrices[num] = matrix;
				}
			}
		}

		public static InstancedHangingWireUpdater singleton;

		private Dictionary<WireStyle, WireStyleInstances> wires = new Dictionary<WireStyle, WireStyleInstances>();

		private Dictionary<WireStyle, WireStyleInstances>.KeyCollection keys;

		public static InstancedHangingWireUpdater Initalize()
		{
			if (singleton == null)
			{
				singleton = new GameObject("InstancedHangingWireUpdater").AddComponent<InstancedHangingWireUpdater>();
			}
			return singleton;
		}

		private void Awake()
		{
			keys = wires.Keys;
		}

		public void Register(InstancedHangingWireInstance newWire)
		{
			WireStyle style = GetStyle(newWire);
			if (!wires.TryGetValue(style, out var value))
			{
				value = new WireStyleInstances();
				wires.Add(style, value);
				Mesh mesh = new Mesh();
				mesh.vertices = (from i in Enumerable.Range(0, style.nodes)
					select new Vector3(0f, 0f, (float)i / (float)(style.nodes - 1))).ToArray();
				mesh.colors = (from i in Enumerable.Range(0, style.nodes)
					select new Color(HangingWireRenderer.GetDroop((float)i / (float)(style.nodes - 1)), 0f, 0f)).ToArray();
				mesh.SetIndices(Enumerable.Range(0, style.nodes).ToArray(), MeshTopology.LineStrip, 0);
				mesh.RecalculateBounds();
				value.mesh = mesh;
			}
			value.Add(newWire, GenerateMatrix(newWire));
		}

		public void Unregister(InstancedHangingWireInstance wire)
		{
			wires[GetStyle(wire)].Remove(wire);
		}

		private void Update()
		{
			foreach (WireStyle key in keys)
			{
				WireStyleInstances wireStyleInstances = wires[key];
				Graphics.DrawMeshInstanced(wireStyleInstances.mesh, 0, key.material, wireStyleInstances.matrices);
			}
		}

		private Matrix4x4 GenerateMatrix(InstancedHangingWireInstance wire)
		{
			Vector3 forward = wire.obj.endPosition - wire.obj.transform.position;
			return Matrix4x4.TRS(wire.obj.transform.position, Quaternion.LookRotation(forward), new Vector3(1f, 1f, forward.magnitude));
		}

		public void UpdateEndpoint(InstancedHangingWireInstance instance)
		{
			wires[GetStyle(instance)].UpdateMatrix(instance, GenerateMatrix(instance));
		}

		private static WireStyle GetStyle(InstancedHangingWireInstance hangingWireRenderer)
		{
			return new WireStyle
			{
				material = hangingWireRenderer.obj.material,
				nodes = hangingWireRenderer.obj.nodes
			};
		}
	}

	private HangingWireRenderer obj;

	public InstancedHangingWireInstance(HangingWireRenderer obj)
	{
		this.obj = obj;
	}

	public void Enable()
	{
		InstancedHangingWireUpdater.Initalize().Register(this);
	}

	public void Disable()
	{
		if (InstancedHangingWireUpdater.singleton != null)
		{
			InstancedHangingWireUpdater.singleton.Unregister(this);
		}
	}

	public void UpdateEndPoint()
	{
		InstancedHangingWireUpdater.singleton.UpdateEndpoint(this);
	}
}
