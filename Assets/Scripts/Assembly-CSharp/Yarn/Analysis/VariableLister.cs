using System.Collections.Generic;

namespace Yarn.Analysis
{
	internal class VariableLister : CompiledProgramAnalyser
	{
		private HashSet<string> variables = new HashSet<string>();

		public override void Diagnose(Program program)
		{
			foreach (KeyValuePair<string, Node> node in program.nodes)
			{
				_ = node.Key;
				foreach (Instruction instruction in node.Value.instructions)
				{
					ByteCode operation = instruction.operation;
					if ((uint)(operation - 14) <= 1u)
					{
						variables.Add((string)instruction.operandA);
					}
				}
			}
		}

		public override IEnumerable<Diagnosis> GatherDiagnoses()
		{
			List<Diagnosis> list = new List<Diagnosis>();
			foreach (string variable in variables)
			{
				Diagnosis item = new Diagnosis("Script uses variable " + variable, Diagnosis.Severity.Note);
				list.Add(item);
			}
			return list;
		}
	}
}
