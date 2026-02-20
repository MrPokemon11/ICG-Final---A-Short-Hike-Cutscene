using System;
using UnityEngine;

namespace Yarn.Unity
{
	[Serializable]
	public class LocalisedStringGroup
	{
		public SystemLanguage language;

		public TextAsset[] stringFiles;
	}
}
