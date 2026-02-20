using System;
using QuickUnityTools.Input;
using UnityEngine;
using UnityEngine.EventSystems;

[Obsolete]
public class UserInputEventSystemModule : BaseInputModule
{
	private static readonly Vector2[] TAP_DIRECTIONS = new Vector2[4]
	{
		Vector2.up,
		Vector2.down,
		Vector2.left,
		Vector2.right
	};

	private FocusableUserInput userInput;

	protected override void Awake()
	{
		base.Awake();
		userInput = GetComponent<FocusableUserInput>();
	}

	public override void ActivateModule()
	{
		base.ActivateModule();
		GameObject gameObject = base.eventSystem.currentSelectedGameObject;
		if (gameObject == null)
		{
			gameObject = base.eventSystem.firstSelectedGameObject;
		}
		base.eventSystem.SetSelectedGameObject(null, GetBaseEventData());
		base.eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
	}

	public override void Process()
	{
		Vector2[] tAP_DIRECTIONS = TAP_DIRECTIONS;
		for (int i = 0; i < tAP_DIRECTIONS.Length; i++)
		{
			Vector2 direction = tAP_DIRECTIONS[i];
			if (userInput.leftStick.WasDirectionTapped(direction))
			{
				AxisEventData axisEventData = GetAxisEventData(direction.x, direction.y, 0.5f);
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			}
		}
	}
}
