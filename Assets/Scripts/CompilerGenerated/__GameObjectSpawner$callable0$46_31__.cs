using System;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;

namespace CompilerGenerated
{
	[CompilerGenerated]
	[Serializable]
	public sealed class __GameObjectSpawner_0024callable0_002446_31__ : MulticastDelegate, ICallable
	{
		public extern __GameObjectSpawner_0024callable0_002446_31__(object instance, IntPtr method);

		public object Call(object[] args)
		{
			object obj2;
			object obj = obj2 = args[0];
			if (!(obj is GameObject))
			{
				obj2 = RuntimeServices.Coerce(obj, typeof(GameObject));
			}
			GameObject g = (GameObject)obj2;
			object obj4;
			object obj3 = obj4 = args[1];
			if (!(obj3 is GameObject))
			{
				obj4 = RuntimeServices.Coerce(obj3, typeof(GameObject));
			}
			return this(g, (GameObject)obj4);
		}

		public extern int Invoke(GameObject g1, GameObject g2);

		public extern IAsyncResult BeginInvoke(GameObject g1, GameObject g2, AsyncCallback callback, object asyncState);

		public extern int EndInvoke(IAsyncResult result);
	}
}
