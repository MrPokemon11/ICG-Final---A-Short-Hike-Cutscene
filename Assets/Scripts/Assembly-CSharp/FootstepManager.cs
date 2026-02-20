using System;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
	public struct FootprintData
	{
		public Renderer renderer;

		public float life;

		public MaterialPropertyBlock block;
	}

	private static readonly Quaternion FOOTPRINT_ROTATION = Quaternion.Euler(90f, 0f, 0f);

	private static int FADE_ID = Shader.PropertyToID("_Fade");

	private const int MAX_FOOTPRINTS = 32;

	public GameObject footprintPrefab;

	public float fadeTime;

	public AnimationCurve fadeCurve;

	private FootprintData[] footprints = new FootprintData[32];

	private int startIndex;

	private int length;

	public void SpawnFootstep(Color color, Vector3 pos, Vector3 forward)
	{
		int num = (startIndex + length) % 32;
		if (footprints[num].renderer == null)
		{
			GameObject obj = footprintPrefab.Clone();
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			Renderer component = obj.GetComponent<Renderer>();
			footprints[num] = new FootprintData
			{
				renderer = component,
				life = fadeTime,
				block = block
			};
		}
		FootprintData footprintData = footprints[num];
		footprintData.block.SetFloat(FADE_ID, 1f);
		footprintData.block.SetColor("_Color", color);
		footprintData.renderer.SetPropertyBlock(footprintData.block);
		Transform obj2 = footprintData.renderer.transform;
		obj2.position = pos + Vector3.up * 0.25f;
		obj2.rotation = Quaternion.LookRotation(forward, Vector3.up) * FOOTPRINT_ROTATION;
		obj2.gameObject.SetActive(value: true);
		footprints[num].life = fadeTime;
		length++;
		if (length > 32)
		{
			startIndex = (startIndex + 1) % 32;
			length--;
		}
	}

	private void Update()
	{
		int num = startIndex + length;
		int num2 = Math.Min(num, 32);
		for (int i = startIndex; i < num2; i++)
		{
			UpdatePrint(i);
		}
		num2 = num - 32;
		for (int j = 0; j < num2; j++)
		{
			UpdatePrint(j);
		}
	}

	private void UpdatePrint(int index)
	{
		footprints[index].life -= Time.deltaTime;
		FootprintData footprintData = footprints[index];
		footprintData.block.SetFloat(FADE_ID, fadeCurve.Evaluate(footprintData.life / fadeTime));
		footprintData.renderer.SetPropertyBlock(footprintData.block);
		if (footprintData.life <= 0f)
		{
			footprintData.renderer.gameObject.SetActive(value: false);
			startIndex = (startIndex + 1) % 32;
			length--;
		}
	}
}
