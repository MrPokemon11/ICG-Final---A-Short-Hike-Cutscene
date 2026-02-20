using System.Collections;
using UnityEngine;

namespace Yarn.Unity
{
	public abstract class DialogueUIBehaviour : MonoBehaviour
	{
		public virtual IEnumerator DialogueStarted()
		{
			yield break;
		}

		public abstract IEnumerator RunLine(Line line);

		public abstract IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser);

		public abstract IEnumerator RunCommand(Command command);

		public virtual IEnumerator NodeComplete(string nextNode)
		{
			yield break;
		}

		public virtual IEnumerator DialogueComplete()
		{
			yield break;
		}
	}
}
