using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSD.Events
{
	public class LSD_EventDispatcher : MonoBehaviour
	{
		public LSD_EventDispatcher()
		{
			this.listeners = new Dictionary<string, LSD_EventListener>();
		}

		public void addEventListener(string eventName, LSD_EventHandler handler)
		{
			LSD_EventListener lsd_EventListener = null;
			try
			{
				lsd_EventListener = this.listeners[eventName];
			}
			catch (KeyNotFoundException ex)
			{
				lsd_EventListener = new LSD_EventListener(this);
				this.listeners[eventName] = lsd_EventListener;
			}
			finally
			{
				lsd_EventListener.addHandler(handler);
			}
		}

		public void removeEventListener(string eventName, LSD_EventHandler handler)
		{
			LSD_EventListener lsd_EventListener = null;
			try
			{
				lsd_EventListener = this.listeners[eventName];
			}
			catch (KeyNotFoundException ex)
			{
				return;
			}
			lsd_EventListener.removeHandler(handler);
		}

		public void dispatchEvent(LSD_Event evt)
		{
			LSD_EventListener lsd_EventListener = null;
			try
			{
				lsd_EventListener = this.listeners[evt.name];
			}
			catch (KeyNotFoundException ex)
			{
				return;
			}
			lsd_EventListener.callHandler(evt);
		}

		private Dictionary<string, LSD_EventListener> listeners;
	}
}
