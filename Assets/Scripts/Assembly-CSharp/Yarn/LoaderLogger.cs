namespace Yarn
{
	public interface LoaderLogger
	{
		Logger LogDebugMessage { get; }

		Logger LogErrorMessage { get; }
	}
}
