using System;

namespace LSD.Events
{
	public class LSD_EventListener
	{
		public LSD_EventListener(LSD_EventDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		private event LSD_EventHandler handler;

		public void addHandler(LSD_EventHandler handler)
		{
			this.handler = (LSD_EventHandler)Delegate.Combine(this.handler, handler);
		}

		public void removeHandler(LSD_EventHandler handler)
		{
			this.handler = (LSD_EventHandler)Delegate.Remove(this.handler, handler);
		}

		public void callHandler(LSD_Event evt)
		{
			this.handler(this.dispatcher, evt);
		}

		private LSD_EventDispatcher dispatcher;
	}
}
