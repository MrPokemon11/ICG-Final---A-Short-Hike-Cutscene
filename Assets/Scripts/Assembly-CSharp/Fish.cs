using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class Fish
{
	public enum SizeCategory
	{
		Tiny = 0,
		Normal = 1,
		Big = 2
	}

	public static readonly Dictionary<SizeCategory, string> SIZE_I18N_ID = new Dictionary<SizeCategory, string>
	{
		{
			SizeCategory.Tiny,
			"tinyFish"
		},
		{
			SizeCategory.Normal,
			""
		},
		{
			SizeCategory.Big,
			"bigFish"
		}
	};

	public static readonly Dictionary<SizeCategory, Func<string>> SIZE_TEXT_COLOR = new Dictionary<SizeCategory, Func<string>>
	{
		{
			SizeCategory.Tiny,
			() => I18n.STRINGS.tinyFish
		},
		{
			SizeCategory.Normal,
			() => ""
		},
		{
			SizeCategory.Big,
			() => I18n.STRINGS.bigFish
		}
	};

	public static Action<Fish> onFishCaught;

	[NonSerialized]
	private FishSpecies _species;

	public float size;

	public bool rare;

	private string speciesName;

	public FishSpecies species
	{
		get
		{
			if (_species == null)
			{
				_species = FishSpecies.Load(speciesName);
			}
			return _species;
		}
		set
		{
			_species = value;
		}
	}

	public SizeCategory sizeCategory
	{
		get
		{
			if (size > Mathf.Lerp(species.size.min, species.size.max, 0.8f))
			{
				return SizeCategory.Big;
			}
			if (size < Mathf.Lerp(species.size.min, species.size.max, 0.2f))
			{
				return SizeCategory.Tiny;
			}
			return SizeCategory.Normal;
		}
	}

	public Fish(FishSpecies fishSpecies, bool rare)
	{
		species = fishSpecies;
		size = fishSpecies.size.Random();
		speciesName = fishSpecies.name;
		this.rare = rare;
	}

	public Fish Clone()
	{
		return new Fish(species, rare)
		{
			size = size
		};
	}

	public IEnumerator CatchFishRoutine()
	{
		Singleton<GlobalData>.instance.gameData.inventory.AddFish(this);
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		player.TurnToFace(Camera.main.transform);
		StackResourceSortingKey emotionKey = player.ikAnimator.ShowEmotion(Emotion.Happy);
		Action undoPose = player.ikAnimator.Pose(Pose.RaiseArms);
		FishCollectPrompt prompt = Singleton<GameServiceLocator>.instance.ui.CreateFishCatchPrompt(this);
		yield return new WaitUntil(() => prompt == null);
		emotionKey.ReleaseResource();
		undoPose();
		onFishCaught?.Invoke(this);
	}

	public string GetTitleWithSize()
	{
		string arg = I18n.Localize(species.readableName);
		string text = SIZE_TEXT_COLOR[sizeCategory]();
		if (!string.IsNullOrEmpty(text))
		{
			text = I18n.UpdateTextGender(text, SIZE_I18N_ID[sizeCategory]);
		}
		return CleanStringSpaces(string.Format(I18n.STRINGS.fishTitleOrder, text, rare ? I18n.Localize(species.GetRarePrefix()) : "", arg).Trim());
	}

	public string GetTitle()
	{
		string arg = I18n.Localize(species.readableName);
		return CleanStringSpaces(string.Format(I18n.STRINGS.fishTitleOrder, "", rare ? I18n.Localize(species.GetRarePrefix()) : "", arg).Trim());
	}

	private string CleanStringSpaces(string text)
	{
		return Regex.Replace(text, "\\s+", " ");
	}

	public Sprite GetSprite()
	{
		if (!rare)
		{
			return species.sprite;
		}
		if (!species.rareSprite)
		{
			return species.sprite;
		}
		return species.rareSprite;
	}
}
