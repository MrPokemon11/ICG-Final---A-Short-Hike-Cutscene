using System;
using UnityEngine;
using UnityEngine.UI;

public class LogoSceneRandomizer : MonoBehaviour
{
	[Serializable]
	public class ColorPallet
	{
		public Color hill1_1;

		public Color hill1_2;

		public Color hill2_1;

		public Color hill2_2;

		public Color clouds_1;

		public Color clouds_2;

		public Color trees_1;

		public Color trees_2;

		public Color background_1;

		public Color background_2;

		public Color moon_1;

		public Color moon_2;

		public Color birdLogo_1;

		public Color birdLogo_2;
	}

	public ColorPallet[] pallets;

	public int palletIndex;

	[Header("Links")]
	public GameObject birdAnimation;

	public SpriteRenderer hill1;

	public SpriteRenderer hill2;

	public SpriteRenderer clouds;

	public MeshRenderer[] trees;

	public MeshRenderer background;

	public MeshRenderer moon;

	public Image birdLogo;

	public void SaveCurrentToPallet()
	{
		pallets[palletIndex].hill1_1 = hill1.sharedMaterial.GetColor("_Color1");
		pallets[palletIndex].hill1_2 = hill1.sharedMaterial.GetColor("_Color2");
		pallets[palletIndex].hill2_1 = hill2.sharedMaterial.GetColor("_Color1");
		pallets[palletIndex].hill2_2 = hill2.sharedMaterial.GetColor("_Color2");
		pallets[palletIndex].clouds_1 = clouds.sharedMaterial.GetColor("_Color1");
		pallets[palletIndex].clouds_2 = clouds.sharedMaterial.GetColor("_Color2");
		pallets[palletIndex].moon_1 = moon.sharedMaterial.GetColor("_Color1");
		pallets[palletIndex].moon_2 = moon.sharedMaterial.GetColor("_Color2");
		pallets[palletIndex].background_1 = background.sharedMaterial.GetColor("_Color1");
		pallets[palletIndex].background_2 = background.sharedMaterial.GetColor("_Color2");
		pallets[palletIndex].trees_1 = trees[0].sharedMaterial.GetColor("_Color1");
		pallets[palletIndex].trees_2 = trees[0].sharedMaterial.GetColor("_Color2");
		pallets[palletIndex].birdLogo_1 = birdLogo.material.GetColor("_Color1");
		pallets[palletIndex].birdLogo_2 = birdLogo.material.GetColor("_Color2");
	}

	public void SetCurrentToPallet()
	{
		hill1.material.SetColor("_Color1", pallets[palletIndex].hill1_1);
		hill1.material.SetColor("_Color2", pallets[palletIndex].hill1_2);
		hill2.material.SetColor("_Color1", pallets[palletIndex].hill2_1);
		hill2.material.SetColor("_Color2", pallets[palletIndex].hill2_2);
		clouds.material.SetColor("_Color1", pallets[palletIndex].clouds_1);
		clouds.material.SetColor("_Color2", pallets[palletIndex].clouds_2);
		moon.material.SetColor("_Color1", pallets[palletIndex].moon_1);
		moon.material.SetColor("_Color2", pallets[palletIndex].moon_2);
		background.material.SetColor("_Color1", pallets[palletIndex].background_1);
		background.material.SetColor("_Color2", pallets[palletIndex].background_2);
		MeshRenderer[] array = trees;
		foreach (MeshRenderer obj in array)
		{
			obj.material.SetColor("_Color1", pallets[palletIndex].trees_1);
			obj.material.SetColor("_Color2", pallets[palletIndex].trees_2);
		}
		birdLogo.material.SetColor("_Color1", pallets[palletIndex].birdLogo_1);
		birdLogo.material.SetColor("_Color2", pallets[palletIndex].birdLogo_2);
	}

	public void SetCurrentToPalletEditor()
	{
		hill1.sharedMaterial.SetColor("_Color1", pallets[palletIndex].hill1_1);
		hill1.sharedMaterial.SetColor("_Color2", pallets[palletIndex].hill1_2);
		hill2.sharedMaterial.SetColor("_Color1", pallets[palletIndex].hill2_1);
		hill2.sharedMaterial.SetColor("_Color2", pallets[palletIndex].hill2_2);
		clouds.sharedMaterial.SetColor("_Color1", pallets[palletIndex].clouds_1);
		clouds.sharedMaterial.SetColor("_Color2", pallets[palletIndex].clouds_2);
		moon.sharedMaterial.SetColor("_Color1", pallets[palletIndex].moon_1);
		moon.sharedMaterial.SetColor("_Color2", pallets[palletIndex].moon_2);
		background.sharedMaterial.SetColor("_Color1", pallets[palletIndex].background_1);
		background.sharedMaterial.SetColor("_Color2", pallets[palletIndex].background_2);
		MeshRenderer[] array = trees;
		foreach (MeshRenderer obj in array)
		{
			obj.sharedMaterial.SetColor("_Color1", pallets[palletIndex].trees_1);
			obj.sharedMaterial.SetColor("_Color2", pallets[palletIndex].trees_2);
		}
		birdLogo.material.SetColor("_Color1", pallets[palletIndex].birdLogo_1);
		birdLogo.material.SetColor("_Color2", pallets[palletIndex].birdLogo_2);
	}

	private void Start()
	{
		birdAnimation.SetActive(UnityEngine.Random.value < 0.33f);
		palletIndex = UnityEngine.Random.Range(0, pallets.Length);
		SetCurrentToPallet();
	}
}
