using System;
using System.Collections.Generic;

namespace Yarn.Analysis
{
	public class Context
	{
		private IEnumerable<Type> _defaultAnalyserClasses;

		private List<CompiledProgramAnalyser> analysers;

		internal IEnumerable<Type> defaultAnalyserClasses
		{
			get
			{
				List<Type> list = new List<Type>();
				if (_defaultAnalyserClasses == null)
				{
					list = new List<Type>();
					Type[] types = GetType().Assembly.GetTypes();
					foreach (Type type in types)
					{
						if (type.IsSubclassOf(typeof(CompiledProgramAnalyser)) && !type.IsAbstract)
						{
							list.Add(type);
						}
					}
					_defaultAnalyserClasses = list;
				}
				return _defaultAnalyserClasses;
			}
		}

		public Context()
		{
			analysers = new List<CompiledProgramAnalyser>();
			foreach (Type defaultAnalyserClass in defaultAnalyserClasses)
			{
				analysers.Add((CompiledProgramAnalyser)Activator.CreateInstance(defaultAnalyserClass));
			}
		}

		public Context(params Type[] types)
		{
			analysers = new List<CompiledProgramAnalyser>();
			foreach (Type type in types)
			{
				analysers.Add((CompiledProgramAnalyser)Activator.CreateInstance(type));
			}
		}

		internal void AddProgramToAnalysis(Program program)
		{
			foreach (CompiledProgramAnalyser analyser in analysers)
			{
				analyser.Diagnose(program);
			}
		}

		public IEnumerable<Diagnosis> FinishAnalysis()
		{
			List<Diagnosis> list = new List<Diagnosis>();
			foreach (CompiledProgramAnalyser analyser in analysers)
			{
				list.AddRange(analyser.GatherDiagnoses());
			}
			return list;
		}
	}
}
