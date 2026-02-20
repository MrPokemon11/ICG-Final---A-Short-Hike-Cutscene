public class ServiceLocator : AbstractServiceLocator<ServiceLocator>
{
	public T Locate<T>(bool allowFail = false) where T : ServiceMonoBehaviour
	{
		return LocateServiceInActiveScene<T>(allowFail);
	}
}
