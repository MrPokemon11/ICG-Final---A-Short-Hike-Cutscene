using UnityEngine;
using UnityEngine.Serialization;

namespace Yarn.Unity.Example
{
	public class NPC : MonoBehaviour
	{
		public string characterName = "";

		[FormerlySerializedAs("startNode")]
		public string talkToNode = "";

		[Header("Optional")]
		public TextAsset scriptToLoad;

		private void Start()
		{
			if (scriptToLoad != null)
			{
				Object.FindObjectOfType<DialogueRunner>().AddScript(scriptToLoad);
			}
		}

		private void Update()
		{
		}
	}
}
