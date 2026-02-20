using System;

namespace Yarn.Analysis
{
	public class Diagnosis
	{
		public enum Severity
		{
			Error = 0,
			Warning = 1,
			Note = 2
		}

		public string message;

		public string nodeName;

		public int lineNumber;

		public int columnNumber;

		public Severity severity;

		public Diagnosis(string message, Severity severity, string nodeName = null, int lineNumber = -1, int columnNumber = -1)
		{
			this.message = message;
			this.nodeName = nodeName;
			this.lineNumber = lineNumber;
			this.columnNumber = columnNumber;
			this.severity = severity;
		}

		public override string ToString()
		{
			return ToString(showSeverity: false);
		}

		public string ToString(bool showSeverity)
		{
			string text = "";
			if (showSeverity)
			{
				text = severity switch
				{
					Severity.Error => "ERROR: ", 
					Severity.Warning => "WARNING: ", 
					Severity.Note => "Note: ", 
					_ => throw new ArgumentOutOfRangeException(), 
				};
			}
			if (nodeName != null)
			{
				text += nodeName;
				if (lineNumber != -1)
				{
					text += $": {lineNumber}";
					if (columnNumber != -1)
					{
						text += $":{columnNumber}";
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return message;
			}
			return $"{text}: {message}";
		}
	}
}
