using System;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class MessageHandler : MonoBehaviour
	{
		private void Awake()
		{
			this.vkapi = base.GetComponent<VkApi>();
		}

		public void ReceiveNewTokenMessage(string message)
		{
			VKToken e = VKToken.ParseSerializeTokenFromNaviteSdk(message);
			this.vkapi.onReceiveNewToken(e);
		}

		public void AccessDeniedMessage(string errormessage)
		{
			Error error = Error.ParseSerializedFromFromNativeSdk(errormessage);
			UnityEngine.Debug.Log("Access Denied " + error.error_msg);
			this.vkapi.onAccessDenied(error);
		}

		public void NoVkApp(string msg)
		{
			UnityEngine.Debug.Log("No vk app");
			VkApi.VkSetts.forceOAuth = true;
			this.vkapi.Login();
		}

		private VkApi vkapi;
	}
}
