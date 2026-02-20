using System;
using UnityEngine;

namespace Yarn.Unity.Example
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteSwitcher : MonoBehaviour
	{
		[Serializable]
		public struct SpriteInfo
		{
			public string name;

			public Sprite sprite;
		}

		public SpriteInfo[] sprites;

		[YarnCommand("setsprite")]
		public void UseSprite(string spriteName)
		{
			Sprite sprite = null;
			SpriteInfo[] array = sprites;
			for (int i = 0; i < array.Length; i++)
			{
				SpriteInfo spriteInfo = array[i];
				if (spriteInfo.name == spriteName)
				{
					sprite = spriteInfo.sprite;
					break;
				}
			}
			if (sprite == null)
			{
				Debug.LogErrorFormat("Can't find sprite named {0}!", spriteName);
			}
			else
			{
				GetComponent<SpriteRenderer>().sprite = sprite;
			}
		}
	}
}
