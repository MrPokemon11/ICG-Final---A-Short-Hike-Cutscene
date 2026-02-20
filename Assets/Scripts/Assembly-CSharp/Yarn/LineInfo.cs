namespace Yarn
{
	internal struct LineInfo
	{
		public int lineNumber;

		public string nodeName;

		public LineInfo(string nodeName, int lineNumber)
		{
			this.nodeName = nodeName;
			this.lineNumber = lineNumber;
		}
	}
}
