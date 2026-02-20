using System.Collections;
using UnityEngine;

public class FadeMaterials : MonoBehaviour
{
	public Material[] _materials;

	public float _fadeAmountPerSecond;

	public IEnumerator FadeIn()
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			for (int i = 0; i < _materials.Length; i++)
			{
				float num = _materials[i].GetFloat("_Fade");
				num = Mathf.Min(1f, num + _fadeAmountPerSecond * Time.deltaTime);
				if (num < 1f)
				{
					complete = false;
				}
				_materials[i].SetFloat("_Fade", num);
			}
			yield return null;
		}
	}

	public IEnumerator FadeOut()
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			for (int i = 0; i < _materials.Length; i++)
			{
				float num = _materials[i].GetFloat("_Fade");
				num = Mathf.Max(0f, num - _fadeAmountPerSecond * Time.deltaTime);
				if (num > 0f)
				{
					complete = false;
				}
				_materials[i].SetFloat("_Fade", num);
			}
			yield return null;
		}
	}
}
