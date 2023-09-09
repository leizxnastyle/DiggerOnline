using System;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;

namespace CompilerGenerated
{
	[CompilerGenerated]
	[Serializable]
	public sealed class __GameObjectSpawner_0024callable1_002447_31__ : MulticastDelegate, ICallable
	{
		public extern __GameObjectSpawner_0024callable1_002447_31__(object instance, IntPtr method);

		public object Call(object[] args)
		{
			object obj2;
			object obj = obj2 = args[0];
			if (!(obj is Material))
			{
				obj2 = RuntimeServices.Coerce(obj, typeof(Material));
			}
			Material g = (Material)obj2;
			object obj4;
			object obj3 = obj4 = args[1];
			if (!(obj3 is Material))
			{
				obj4 = RuntimeServices.Coerce(obj3, typeof(Material));
			}
			return this(g, (Material)obj4);
		}

		public extern int Invoke(Material g1, Material g2);

		public extern IAsyncResult BeginInvoke(Material g1, Material g2, AsyncCallback callback, object asyncState);

		public extern int EndInvoke(IAsyncResult result);
	}
}
