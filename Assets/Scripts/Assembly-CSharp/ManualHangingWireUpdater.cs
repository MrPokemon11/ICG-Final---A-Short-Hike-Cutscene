using System.Collections.Generic;
using UnityEngine;

public class ManualHangingWireUpdater : MonoBehaviour
{
	public static ManualHangingWireUpdater singleton;

	public List<IUpdateableWireInstance> objects = new List<IUpdateableWireInstance>();

	public static ManualHangingWireUpdater Initalize()
	{
		if (singleton == null)
		{
			singleton = new GameObject("ClassicHangingWireUpdater").AddComponent<ManualHangingWireUpdater>();
		}
		return singleton;
	}

	private void Update()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			objects[i].ManualUpdate();
		}
	}
}
