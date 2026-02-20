public class TimerWaitable : IWaitable
{
	private Timer timer;

	public bool isCompleted => timer.IsDone();

	public TimerWaitable(Timer timer)
	{
		this.timer = timer;
	}
}
