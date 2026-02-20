using System.Collections.Generic;
using InControl;
using UnityEngine;

public class TouchScreenControls : MonoBehaviour
{
	public TouchScreenStick leftStick;

	public List<TouchScreenButton> buttons;

	public List<InputControlType> buttonMappings;

	private void Start()
	{
		InputManager.AttachDevice(new TouchScreenInputDevice(this));
	}

	public static Vector2? GetScreenPointInsideRect(RectTransform rect, Canvas canvas = null)
	{
		Camera cam = ((canvas != null) ? canvas.worldCamera : null);
		Vector2? result = null;
		if (Input.touchCount == 0)
		{
			if (Input.GetMouseButton(0) && RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, cam, out var localPoint) && rect.rect.Contains(localPoint))
			{
				result = localPoint;
			}
		}
		else
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.GetTouch(i).position, cam, out var localPoint2) && rect.rect.Contains(localPoint2))
				{
					result = localPoint2;
					break;
				}
			}
		}
		return result;
	}
}
