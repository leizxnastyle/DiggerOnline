using System;
using UnityEngine;

public class CFInput
{
	public static bool ControllerActive()
	{
		return CFInput.ctrl != null && CFInput.ctrl.enabled;
	}

	public static Vector3 mousePosition
	{
		get
		{
			if (CFInput.ControllerActive())
			{
				return CFInput.ctrl.GetMousePos();
			}
			return UnityEngine.Input.mousePosition;
		}
	}

	public static bool GetKey(KeyCode key)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			bool keyEx = CFInput.ctrl.GetKeyEx(key, out flag);
			if (flag)
			{
				return keyEx;
			}
		}
		return UnityEngine.Input.GetKey(key);
	}

	public static bool GetKeyDown(KeyCode key)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			bool keyDownEx = CFInput.ctrl.GetKeyDownEx(key, out flag);
			if (flag)
			{
				return keyDownEx;
			}
		}
		return UnityEngine.Input.GetKeyDown(key);
	}

	public static bool GetKeyUp(KeyCode key)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			bool keyUpEx = CFInput.ctrl.GetKeyUpEx(key, out flag);
			if (flag)
			{
				return keyUpEx;
			}
		}
		return UnityEngine.Input.GetKeyUp(key);
	}

	public static bool GetButton(string axisName)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			bool buttonEx = CFInput.ctrl.GetButtonEx(axisName, out flag);
			if (flag)
			{
				return buttonEx;
			}
		}
		try
		{
			return Input.GetButton(axisName);
		}
		catch (UnityException)
		{
		}
		return false;
	}

	public static bool GetButtonDown(string axisName)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			bool buttonDownEx = CFInput.ctrl.GetButtonDownEx(axisName, out flag);
			if (flag)
			{
				return buttonDownEx;
			}
		}
		try
		{
			return Input.GetButtonDown(axisName);
		}
		catch (UnityException)
		{
		}
		return false;
	}

	public static bool GetButtonUp(string axisName)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			bool buttonUpEx = CFInput.ctrl.GetButtonUpEx(axisName, out flag);
			if (flag)
			{
				return buttonUpEx;
			}
		}
		try
		{
			return Input.GetButtonUp(axisName);
		}
		catch (UnityException)
		{
		}
		return false;
	}

	public static float GetAxis(string axisName)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			float axisEx = CFInput.ctrl.GetAxisEx(axisName, out flag);
			if (flag)
			{
				return axisEx;
			}
		}
		try
		{
			return UnityEngine.Input.GetAxis(axisName);
		}
		catch (UnityException)
		{
		}
		return 0f;
	}

	public static float GetAxisRaw(string axisName)
	{
		if (CFInput.ControllerActive())
		{
			bool flag = false;
			float axisEx = CFInput.ctrl.GetAxisEx(axisName, out flag);
			if (flag)
			{
				return axisEx;
			}
		}
		try
		{
			return UnityEngine.Input.GetAxisRaw(axisName);
		}
		catch (UnityException)
		{
		}
		return 0f;
	}

	public static float GetAxisPx(string axisName, float refResolution = 1280f, float maxDragInches = 1f)
	{
		if (CFInput.ControllerActive() && TouchController.IsSupported())
		{
			float actualDPI = CFInput.ctrl.GetActualDPI();
			float axis = CFInput.GetAxis(axisName);
			return axis / (actualDPI * maxDragInches) * refResolution;
		}
		int width = Screen.currentResolution.width;
		return CFInput.GetAxis(axisName) * ((width != 0) ? (refResolution / (float)width) : 1f);
	}

	public static bool GetMouseButton(int i)
	{
		if (CFInput.ControllerActive())
		{
			return CFInput.ctrl.GetMouseButton(i);
		}
		return Input.GetMouseButton(i);
	}

	public static bool GetMouseButtonDown(int i)
	{
		if (CFInput.ControllerActive())
		{
			return CFInput.ctrl.GetMouseButtonDown(i);
		}
		return Input.GetMouseButtonDown(i);
	}

	public static bool GetMouseButtonUp(int i)
	{
		if (CFInput.ControllerActive())
		{
			return CFInput.ctrl.GetMouseButtonUp(i);
		}
		return Input.GetMouseButtonUp(i);
	}

	public static void ResetInputAxes()
	{
		Input.ResetInputAxes();
		if (CFInput.ControllerActive())
		{
			CFInput.ctrl.ReleaseTouches();
		}
	}

	public static TouchController ctrl;
}
