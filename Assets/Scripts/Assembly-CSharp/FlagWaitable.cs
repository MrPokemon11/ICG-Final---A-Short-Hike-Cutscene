public class FlagWaitable : IWaitable
{
	private static FlagWaitable _finished = new FlagWaitable
	{
		isCompleted = true
	};

	public bool isCompleted { get; private set; }

	public void Finish()
	{
		isCompleted = true;
	}

	public static IWaitable FinishedWaitable()
	{
		return _finished;
	}
}
