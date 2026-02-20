using System.Collections.Generic;

namespace Yarn.Analysis
{
	internal abstract class CompiledProgramAnalyser
	{
		public abstract void Diagnose(Program program);

		public abstract IEnumerable<Diagnosis> GatherDiagnoses();
	}
}
