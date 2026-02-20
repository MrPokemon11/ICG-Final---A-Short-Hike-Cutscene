using UnityEngine;

namespace RogueNoodle.GBCamera
{
	public class RandomPalette : MonoBehaviour
	{
		public Texture[] _palettes;

		public Material[] _materials;

		private int _previousPaletteIndex;

		public void SelectRandomPalette()
		{
			if (_palettes.Length != 0 && _materials.Length != 0)
			{
				int num;
				for (num = _previousPaletteIndex; num == _previousPaletteIndex; num = Random.Range(0, _palettes.Length))
				{
				}
				for (int i = 0; i < _materials.Length; i++)
				{
					_materials[i].SetTexture("_Palette", _palettes[num]);
				}
				_previousPaletteIndex = num;
			}
		}
	}
}
