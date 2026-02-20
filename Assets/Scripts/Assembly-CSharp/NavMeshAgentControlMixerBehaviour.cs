using UnityEngine.AI;
using UnityEngine.Playables;

public class NavMeshAgentControlMixerBehaviour : PlayableBehaviour
{
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		NavMeshAgent navMeshAgent = playerData as NavMeshAgent;
		if (!navMeshAgent)
		{
			return;
		}
		int inputCount = playable.GetInputCount();
		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);
			NavMeshAgentControlBehaviour behaviour = ((ScriptPlayable<NavMeshAgentControlBehaviour>)playable.GetInput(i)).GetBehaviour();
			if (inputWeight > 0.5f && !behaviour.destinationSet && (bool)behaviour.destination && navMeshAgent.isOnNavMesh)
			{
				navMeshAgent.SetDestination(behaviour.destination.position);
				behaviour.destinationSet = true;
			}
		}
	}
}
