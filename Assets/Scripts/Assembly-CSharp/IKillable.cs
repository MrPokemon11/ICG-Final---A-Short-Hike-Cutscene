using System;
using UnityEngine;

public interface IKillable
{
	GameObject gameObject { get; }

	event Action onKill;

	void Kill();
}
