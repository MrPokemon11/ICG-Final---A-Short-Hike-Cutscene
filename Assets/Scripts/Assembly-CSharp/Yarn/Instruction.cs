namespace Yarn
{
	internal struct Instruction
	{
		public ByteCode operation;

		public object operandA;

		public object operandB;

		public string ToString(Program p, Library l)
		{
			if (operation == ByteCode.Label)
			{
				return operandA?.ToString() + ":";
			}
			string text = ((operandA != null) ? operandA.ToString() : "");
			string text2 = ((operandB != null) ? operandB.ToString() : "");
			string text3 = "";
			int num = 0;
			int num2 = 0;
			switch (operation)
			{
			case ByteCode.ShowOptions:
			case ByteCode.PushString:
			case ByteCode.PushNumber:
			case ByteCode.PushBool:
			case ByteCode.PushNull:
			case ByteCode.PushVariable:
				num2 = 1;
				break;
			case ByteCode.CallFunc:
			{
				FunctionInfo function = l.GetFunction((string)operandA);
				num = function.paramCount;
				if (function.returnsValue)
				{
					num2 = 1;
				}
				break;
			}
			case ByteCode.Pop:
				num = 1;
				break;
			case ByteCode.RunNode:
				text3 += "Clears stack";
				break;
			}
			if (num > 0 && num2 > 0)
			{
				text3 += $"Pops {num}, Pushes {num2}";
			}
			else if (num > 0)
			{
				text3 += $"Pops {num}";
			}
			else if (num2 > 0)
			{
				text3 += $"Pushes {num2}";
			}
			switch (operation)
			{
			case ByteCode.RunLine:
			case ByteCode.AddOption:
			case ByteCode.PushString:
				if ((string)operandA != null)
				{
					string arg = p.GetString((string)operandA);
					text3 += $"\"{arg}\"";
				}
				break;
			}
			if (text3 != "")
			{
				text3 = "; " + text3;
			}
			return $"{operation.ToString(),-15} {text,-10} {text2,-10} {text3,-10}";
		}
	}
}
