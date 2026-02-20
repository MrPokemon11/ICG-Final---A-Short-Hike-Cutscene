using System.Collections.Generic;

namespace Yarn.Analysis
{
	internal abstract class ASTAnalyser
	{
		public abstract IEnumerable<Diagnosis> Diagnose(Parser.Node node);
	}
}
