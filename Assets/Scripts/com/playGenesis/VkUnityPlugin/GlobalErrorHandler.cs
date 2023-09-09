using System;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class GlobalErrorHandler : MonoBehaviour
	{
		private void Awake()
		{
			if (GlobalErrorHandler.Instance == null)
			{
				GlobalErrorHandler.Instance = this;
			}
		}

		private void Update()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				if (base.transform.GetChild(i).gameObject.activeSelf)
				{
					base.transform.SetAsLastSibling();
					return;
				}
				base.transform.SetAsFirstSibling();
			}
		}

		private void Start()
		{
			VkApi.VkApiInstance.SubscribeToGlobalErrorEvent(new Action<VKRequest>(this.OnGlobalError));
		}

		public void OnGlobalError(VKRequest request)
		{
			if (request.error.error_code == "17")
			{
				this.handle_17_Error(request);
			}
			else if (request.error.error_code == "14")
			{
				this.handleCaptchaError(request);
			}
			else if (request.error.error_code == "24")
			{
				this.handleNeedToShowMessageToUser(request);
			}
			else if (request.error.error_code == "404")
			{
				this.handleNetworkError(request);
			}
			else
			{
				request.CallBackFunction(request);
			}
		}

		private void handleNetworkError(VKRequest request)
		{
			if (request.attempt < 5)
			{
				request.attempt++;
				VkApi.VkApiInstance.Call(request);
			}
			else
			{
				request.CallBackFunction(request);
			}
		}

		private void handleCaptchaError(VKRequest request)
		{
			this.CaptchaDialog.Add(request);
		}

		private void handleNeedToShowMessageToUser(VKRequest r)
		{
			this.YesNoMessageBox.Add(new YesNoTaskData
			{
				OnYesButton = delegate
				{
					VKRequest r2 = r;
					r2.fullurl += "&confirm=1";
					VkApi.VkApiInstance.Call(r);
				},
				OnNoButton = delegate
				{
					r.CallBackFunction(r);
				},
				Message = Utilities.ParseConfirmationText(r.response)
			});
		}

		private void handle_17_Error(VKRequest request)
		{
			this.Error_17_worker.Add(request);
		}

		public static GlobalErrorHandler Instance;

		public CaptchaDialog CaptchaDialog;

		public Eroor_17_Behaviour Error_17_worker;

		public VKNotificationUI Notification;

		public VKYesNoMessageBox YesNoMessageBox;
	}
}
