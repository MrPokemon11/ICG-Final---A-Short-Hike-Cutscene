using UnityEngine;

namespace SubjectNerd.Utilities
{
	public class ReorderableAttribute : PropertyAttribute
	{
		public string ElementHeader { get; protected set; }

		public bool HeaderZeroIndex { get; protected set; }

		public bool ElementSingleLine { get; protected set; }

		public ReorderableAttribute()
		{
			ElementHeader = string.Empty;
			HeaderZeroIndex = false;
			ElementSingleLine = false;
		}

		public ReorderableAttribute(string headerString = "", bool isZeroIndex = true, bool isSingleLine = false)
		{
			ElementHeader = headerString;
			HeaderZeroIndex = isZeroIndex;
			ElementSingleLine = isSingleLine;
		}
	}
}
