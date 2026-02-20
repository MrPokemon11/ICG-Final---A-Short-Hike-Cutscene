using System.Threading.Tasks;

public class TaskWaitable : IWaitable
{
	private Task task;

	public bool isCompleted => task.IsCompleted;

	public TaskWaitable(Task task)
	{
		this.task = task;
	}
}
