using System;
using QuickUnityTools.Input;
using UnityEngine;

public class KillOnBackButton : MonoBehaviour
{
	public FocusableUserInput input;

	public bool killOnConfirm;

	public bool killOnMenu = true;

	public event Action onKill;

	private void Update()
	{
		if (input.GetCancelButton().ConsumePress() | (killOnMenu && input.WasOpenMenuPressed()) | (killOnConfirm && input.GetConfirmButton().ConsumePress()))
		{
			IKillable component = GetComponent<IKillable>();
			this.onKill?.Invoke();
			if (component != null)
			{
				component.Kill();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
