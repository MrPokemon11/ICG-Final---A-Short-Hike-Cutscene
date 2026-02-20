using System;
using UnityEngine;

namespace Yarn.Unity
{
	public abstract class VariableStorageBehaviour : MonoBehaviour, VariableStorage
	{
		public virtual void SetNumber(string variableName, float number)
		{
			throw new NotImplementedException();
		}

		public virtual float GetNumber(string variableName)
		{
			throw new NotImplementedException();
		}

		public virtual Value GetValue(string variableName)
		{
			return new Value(GetNumber(variableName));
		}

		public virtual void SetValue(string variableName, Value value)
		{
			SetNumber(variableName, value.AsNumber);
		}

		public virtual void Clear()
		{
			throw new NotImplementedException();
		}

		public abstract void ResetToDefaults();
	}
}
