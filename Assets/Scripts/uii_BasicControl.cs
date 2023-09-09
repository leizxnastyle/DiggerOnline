using System;
using LSD.Events;

public class uii_BasicControl : LSD_EventDispatcher
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		base.dispatchEvent(new LSD_Event("button_click"));
	}

	private void OnActivate()
	{
		base.dispatchEvent(new LSD_Event("activate"));
	}

	private void OnPress(bool isPress)
	{
		base.dispatchEvent(new LSD_Event("button_press"));
	}
}
