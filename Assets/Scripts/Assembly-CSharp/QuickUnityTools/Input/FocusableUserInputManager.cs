using System.Collections.Generic;
using System.Linq;

namespace QuickUnityTools.Input
{
	public class FocusableUserInputManager : Singleton<FocusableUserInputManager>
	{
		private SortedList<PrioritySortingKey, FocusableUserInput> registeredUserInputs = new SortedList<PrioritySortingKey, FocusableUserInput>();

		private HashSet<Button> buttonsConsumedOnFrame = new HashSet<Button>();

		public FocusableUserInput inputWithFocus
		{
			get
			{
				if (sortedUserInputs.Count <= 0)
				{
					return null;
				}
				return sortedUserInputs[0];
			}
		}

		public int inputStackCount => sortedUserInputs.Count;

		public IList<FocusableUserInput> sortedUserInputs { get; private set; }

		private void Awake()
		{
			sortedUserInputs = registeredUserInputs.Values;
		}

		private void Update()
		{
			for (int num = sortedUserInputs.Count - 1; num >= 0; num--)
			{
				if (sortedUserInputs[num] == null)
				{
					registeredUserInputs.RemoveAt(num);
				}
			}
		}

		private void LateUpdate()
		{
			buttonsConsumedOnFrame.Clear();
		}

		public void RegisterInputReceiver(FocusableUserInput input)
		{
			registeredUserInputs.Add(new PrioritySortingKey(input.priority, input.registerTime), input);
		}

		public bool UnregisterInputReceiver(FocusableUserInput userInput)
		{
			KeyValuePair<PrioritySortingKey, FocusableUserInput> keyValuePair = registeredUserInputs.FirstOrDefault((KeyValuePair<PrioritySortingKey, FocusableUserInput> item) => item.Value == userInput);
			if (keyValuePair.Key != null)
			{
				registeredUserInputs.Remove(keyValuePair.Key);
				return true;
			}
			return false;
		}

		public bool ConsumePressForFrame(Button button)
		{
			if (!buttonsConsumedOnFrame.Contains(button))
			{
				buttonsConsumedOnFrame.Add(button);
				return true;
			}
			return false;
		}

		public string GetInputStack()
		{
			string text = "";
			foreach (KeyValuePair<PrioritySortingKey, FocusableUserInput> registeredUserInput in registeredUserInputs)
			{
				text = text + "\n" + registeredUserInput.Key?.ToString() + " - " + registeredUserInput.Value.name;
			}
			return text;
		}
	}
}
