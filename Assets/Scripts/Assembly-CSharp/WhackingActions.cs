using System.Linq;
using UnityEngine;

public class WhackingActions : MonoBehaviour, IHoldableAction
{
	public Transform detectPoint;

	public float radius = 0.5f;

	public LayerMask detectMask;

	public ParticleSystem smackParticles;

	private IWhackable scheduledWhack;

	public void ActivateAction(int parameter)
	{
		IWhackable whackable = null;
		if (scheduledWhack != null)
		{
			whackable = scheduledWhack;
			scheduledWhack = null;
		}
		else
		{
			Collider[] array = Physics.OverlapSphere(detectPoint.position, radius, detectMask.value);
			if (array.Length != 0)
			{
				whackable = (from i in array
					select i.GetComponent<IWhackable>() into i
					where i != null
					orderby i.priority descending
					select i).FirstOrDefault();
			}
		}
		if (whackable != null)
		{
			whackable.Whack(base.gameObject);
			smackParticles.Play();
		}
	}

	public void ForceNextWhack(IWhackable whackable)
	{
		scheduledWhack = whackable;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(detectPoint.position, radius);
	}
}
