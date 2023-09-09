using System;

namespace LSD.Events
{
	public class LSD_Event : EventArgs
	{
		public LSD_Event(string name)
		{
			this.name = name;
		}

		public const string BUTTON_CLICK = "button_click";

		public const string ACTIVATE = "activate";

		public const string BUTTON_PRESS = "button_press";

		public string name;
	}
}
