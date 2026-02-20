using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CheatCodeDetector : MonoBehaviour
{
	private class Cheat
	{
		private Action onActivate;

		private int cheatTypeIndex;

		private CheatCodeDetector controller;

		public string cheatCode { get; private set; }

		public Cheat(string cheatCode, Action onActivate, CheatCodeDetector controller)
		{
			this.cheatCode = cheatCode;
			this.onActivate = onActivate;
			cheatTypeIndex = 0;
			this.controller = controller;
		}

		public void Update()
		{
			if (Input.inputString[0] == cheatCode[cheatTypeIndex])
			{
				cheatTypeIndex++;
				if (cheatTypeIndex == cheatCode.Length)
				{
					cheatTypeIndex = 0;
					Activate();
				}
			}
			else
			{
				cheatTypeIndex = 0;
			}
		}

		public void Activate()
		{
			onActivate();
			controller.OnCheatActivation(cheatCode);
		}
	}

	private List<Cheat> cheats = new List<Cheat>();

	protected virtual void Update()
	{
		if (!(Input.inputString != ""))
		{
			return;
		}
		foreach (Cheat cheat in cheats)
		{
			cheat.Update();
		}
	}

	public void RegisterCheat(string cheatCode, Action onActivate)
	{
		if (!cheats.Any((Cheat c) => c.cheatCode == cheatCode))
		{
			cheats.Add(new Cheat(cheatCode, onActivate, this));
		}
	}

	public void TriggerCheat(string name)
	{
		cheats.FirstOrDefault((Cheat c) => c.cheatCode == name)?.Activate();
	}

	protected virtual void OnCheatActivation(string code)
	{
	}
}
