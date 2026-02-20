namespace Yarn
{
	internal enum ByteCode
	{
		Label = 0,
		JumpTo = 1,
		Jump = 2,
		RunLine = 3,
		RunCommand = 4,
		AddOption = 5,
		ShowOptions = 6,
		PushString = 7,
		PushNumber = 8,
		PushBool = 9,
		PushNull = 10,
		JumpIfFalse = 11,
		Pop = 12,
		CallFunc = 13,
		PushVariable = 14,
		StoreVariable = 15,
		Stop = 16,
		RunNode = 17
	}
}
