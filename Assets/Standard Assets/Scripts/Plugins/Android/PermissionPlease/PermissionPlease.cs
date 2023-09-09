using System;
using UnityEngine;

namespace Plugins.Android.PermissionPlease
{
	public class PermissionPlease : MonoBehaviour
	{
		public static void GrantPermission(PermissionPlease.AndroidPermission permission, Action<bool> requestCallback = null, bool enableLogging = false)
		{
			if (!PermissionPlease.initialized)
			{
				PermissionPlease.initialize();
			}
			PermissionPlease.PermissionRequestCallback = requestCallback;
			PermissionPlease.PermissionPleaseClass.CallStatic("grantPermission", new object[]
			{
				PermissionPlease.activity,
				(int)permission,
				enableLogging
			});
		}

		public void Awake()
		{
			PermissionPlease.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (base.name != "PermissionPlease")
			{
				base.name = "PermissionPlease";
			}
		}

		private static void initialize()
		{
			if (PermissionPlease.instance == null)
			{
				GameObject gameObject = new GameObject();
				PermissionPlease.instance = gameObject.AddComponent<PermissionPlease>();
				gameObject.name = "PermissionPlease";
			}
			PermissionPlease.PermissionPleaseClass = new AndroidJavaClass("net.jimmar.unityplugins.PermissionPlease");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			PermissionPlease.activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			PermissionPlease.initialized = true;
		}

		private void PermissionRequestCallbackInternal(string message)
		{
			bool obj = message == "PERMISSION_GRANTED";
			if (PermissionPlease.PermissionRequestCallback != null)
			{
				PermissionPlease.PermissionRequestCallback(obj);
			}
		}

		private const string PERMISSION_GRANTED = "PERMISSION_GRANTED";

		private const string PERMISSION_DENIED = "PERMISSION_DENIED";

		private const string PERMISSION_PLEASE_NAME = "PermissionPlease";

		private static Action<bool> PermissionRequestCallback;

		private static PermissionPlease instance;

		private static bool initialized;

		private static AndroidJavaClass PermissionPleaseClass;

		private static AndroidJavaObject activity;

		public enum AndroidPermission
		{
			READ_CALENDAR,
			WRITE_CALENDAR,
			CAMERA,
			READ_CONTACTS,
			WRITE_CONTACTS,
			GET_ACCOUNTS,
			ACCESS_FINE_LOCATION,
			ACCESS_COARSE_LOCATION,
			RECORD_AUDIO,
			READ_PHONE_STATE,
			READ_PHONE_NUMBERS,
			CALL_PHONE,
			ANSWER_PHONE_CALLS,
			READ_CALL_LOG,
			WRITE_CALL_LOG,
			ADD_VOICEMAIL,
			USE_SIP,
			PROCESS_OUTGOING_CALLS,
			BODY_SENSORS,
			SEND_SMS,
			RECEIVE_SMS,
			READ_SMS,
			RECEIVE_WAP_PUSH,
			RECEIVE_MMS,
			WRITE_EXTERNAL_STORAGE,
			READ_EXTERNAL_STORAGE
		}
	}
}
