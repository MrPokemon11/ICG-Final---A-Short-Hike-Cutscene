using System.Collections.Generic;

namespace Yarn.Analysis
{
	internal class UnusedVariableChecker : CompiledProgramAnalyser
	{
		private HashSet<string> readVariables = new HashSet<string>();

		private HashSet<string> writtenVariables = new HashSet<string>();

		public override void Diagnose(Program program)
		{
			foreach (KeyValuePair<string, Node> node in program.nodes)
			{
				_ = node.Key;
				foreach (Instruction instruction in node.Value.instructions)
				{
					switch (instruction.operation)
					{
					case ByteCode.PushVariable:
						readVariables.Add((string)instruction.operandA);
						break;
					case ByteCode.StoreVariable:
						writtenVariables.Add((string)instruction.operandA);
						break;
					}
				}
			}
		}

		public override IEnumerable<Diagnosis> GatherDiagnoses()
		{
			HashSet<string> hashSet = new HashSet<string>(readVariables);
			hashSet.ExceptWith(writtenVariables);
			HashSet<string> hashSet2 = new HashSet<string>(writtenVariables);
			hashSet2.ExceptWith(readVariables);
			List<Diagnosis> list = new List<Diagnosis>();
			foreach (string item in hashSet)
			{
				string message = $"Variable {item} is read from, but never assigned";
				list.Add(new Diagnosis(message, Diagnosis.Severity.Warning));
			}
			foreach (string item2 in hashSet2)
			{
				string message2 = $"Variable {item2} is assigned, but never read from";
				list.Add(new Diagnosis(message2, Diagnosis.Severity.Warning));
			}
			return list;
		}
	}
}
