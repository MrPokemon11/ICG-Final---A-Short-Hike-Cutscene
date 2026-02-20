using UnityEngine.Playables;

namespace Cinemachine.Timeline
{
	internal sealed class CinemachineShotPlayable : PlayableBehaviour
	{
		public CinemachineVirtualCameraBase VirtualCamera;

		public bool IsValid => VirtualCamera != null;
	}
}
