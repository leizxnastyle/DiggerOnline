using System;
using System.Collections;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class VkApi : MonoBehaviour
	{
		public event EventHandler<Error> AccessDenied;

		public event EventHandler<VKToken> ReceivedNewToken;

		public event Action LoggedIn;

		public event Action LoggedOut;

		private event Action<VKRequest> GlobalErrorHandler;

		public void KillAllReqeusts()
		{
			base.StopAllCoroutines();
		}

		public void Login()
		{
			if (!this.loginProccessSterted)
			{
				this.nativeBridge.Login();
				base.StartCoroutine(this.LockLogin5Sec());
			}
		}

		private IEnumerator LockLogin5Sec()
		{
			this.loginProccessSterted = true;
			yield return new WaitForSeconds(5f);
			this.loginProccessSterted = false;
			yield break;
		}

		public void Logout()
		{
			this.nativeBridge.Logout();
		}

		public void onReceiveNewToken(VKToken e)
		{
			VkApi.CurrentToken.access_token = e.access_token;
			VkApi.CurrentToken.expires_in = e.expires_in;
			VkApi.CurrentToken.tokenRecievedTime = e.tokenRecievedTime;
			VkApi.CurrentToken.user_id = e.user_id;
			VkApi.CurrentToken.Save();
			if (this.ReceivedNewToken != null)
			{
				this.ReceivedNewToken(this, e);
			}
			this.onLoggedIn();
			UnityEngine.Debug.Log("New token is" + e.access_token);
		}

		public void onLoggedIn()
		{
			base.StartCoroutine(this.WaitAndGoOn());
		}

		private IEnumerator WaitAndGoOn()
		{
			while (string.IsNullOrEmpty(VkApi.CurrentToken.access_token))
			{
				yield return null;
			}
			this.IsUserLoggedIn = true;
			if (this.LoggedIn != null)
			{
				this.LoggedIn();
			}
			yield break;
		}

		public void onLoggedOut()
		{
			this.IsUserLoggedIn = false;
			if (this.LoggedOut != null)
			{
				this.LoggedOut();
			}
			VKToken.ResetToken();
		}

		public void onAccessDenied(Error e)
		{
			if (this.AccessDenied != null)
			{
				this.AccessDenied(this, e);
			}
		}

		public void CheckEditorSetup()
		{
			if (string.IsNullOrEmpty(VkApi.CurrentToken.access_token) || !VKToken.IsValidToken(VkApi.CurrentToken))
			{
				VkApi.VkSetts.ProcessAuthUrl();
			}
			if (!VKToken.IsValidToken(VkApi.CurrentToken))
			{
				UnityEngine.Debug.LogError("Token has expired, please relogin to vk in editor");
				UnityEngine.Debug.Break();
			}
		}

		public void SubscribeToGlobalErrorEvent(Action<VKRequest> handler)
		{
			if (this.GlobalErrorHandler != null)
			{
				Delegate[] invocationList = this.GlobalErrorHandler.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					this.GlobalErrorHandler = (Action<VKRequest>)Delegate.Remove(this.GlobalErrorHandler, @delegate as Action<VKRequest>);
				}
			}
			this.GlobalErrorHandler = (Action<VKRequest>)Delegate.Combine(this.GlobalErrorHandler, handler);
		}

		public void UnsubscribeFromGlobalErrorEvent(Action<VKRequest> handler)
		{
			this.GlobalErrorHandler = (Action<VKRequest>)Delegate.Remove(this.GlobalErrorHandler, handler);
		}

		private void InitToken()
		{
			VkApi.CurrentToken = VKToken.LoadPersistent();
		}

		private void Awake()
		{
			VkApi.VkSetts = Resources.Load<VkSettings>("VkSettings");
			this.InitToken();
			UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			if (VkApi.VkApiInstance == null)
			{
				VkApi.VkApiInstance = this;
			}
			if (VkApi.Downloader == null)
			{
				VkApi.Downloader = base.GetComponent<Downloader>();
			}
			if (VKToken.IsValidToken(VkApi.CurrentToken))
			{
				this.IsUserLoggedIn = true;
			}
			else
			{
				this.IsUserLoggedIn = false;
			}
		}

		private WWW GenerateWWWForm(VKRequest httprequest)
		{
			WWWForm wwwform = new WWWForm();
			wwwform.AddBinaryData("file", (byte[])httprequest.data[0], (string)httprequest.data[1], (string)httprequest.data[2]);
			return new WWW(Uri.EscapeUriString(httprequest.fullurl), wwwform);
		}

		private WWW GenerateWWWForm(VKRequest httprequest, FileForUpload file)
		{
			WWWForm wwwform = new WWWForm();
			wwwform.AddBinaryData("file", file.data, file.filename, file.mimeType);
			return new WWW(Uri.EscapeUriString(httprequest.fullurl), wwwform);
		}

		private void HandleTokenExpired(VKRequest httprequest)
		{
			UnityEngine.Debug.Log("Invalid token. Are you logged in?");
			httprequest.response = string.Empty;
			httprequest.error = new Error
			{
				error_code = "401",
				error_msg = "invalid token"
			};
			if (this.GlobalErrorHandler != null)
			{
				this.GlobalErrorHandler(httprequest);
			}
			else if (httprequest.CallBackFunction != null)
			{
				httprequest.CallBackFunction(httprequest);
			}
		}

		private void HandleWWWError(WWW www, VKRequest httprequest)
		{
			Error error = new Error
			{
				error_code = "404",
				error_msg = www.error
			};
			httprequest.response = www.text;
			httprequest.error = error;
			if (this.GlobalErrorHandler != null)
			{
				this.GlobalErrorHandler(httprequest);
			}
			else if (httprequest.CallBackFunction != null)
			{
				httprequest.CallBackFunction(httprequest);
			}
		}

		private void HandleVKError(WWW www, VKRequest httprequest)
		{
			Error error = Error.ParseVkError(www.text);
			httprequest.response = www.text;
			httprequest.error = error;
			if (this.GlobalErrorHandler != null)
			{
				this.GlobalErrorHandler(httprequest);
			}
			else if (httprequest.CallBackFunction != null)
			{
				httprequest.CallBackFunction(httprequest);
			}
		}

		private void HandleNoError(WWW www, VKRequest httprequest)
		{
			httprequest.response = www.text;
			if (httprequest.CallBackFunction != null)
			{
				httprequest.CallBackFunction(httprequest);
			}
		}

		private void HandleResponse(WWW www, VKRequest httpRequest)
		{
			Error error = Error.ParseVkError(www.text);
			if (!string.IsNullOrEmpty(www.error))
			{
				this.HandleWWWError(www, httpRequest);
			}
			if (string.IsNullOrEmpty(www.error) && error != null)
			{
				this.HandleVKError(www, httpRequest);
			}
			if (string.IsNullOrEmpty(www.error) && error == null)
			{
				this.HandleNoError(www, httpRequest);
			}
		}

		public void Call(VKRequest httprequest)
		{
			httprequest.error = null;
			base.StartCoroutine(this._Call(httprequest));
		}

		private IEnumerator _Call(VKRequest httprequest)
		{
			httprequest.url = ((!httprequest.url.Contains("?")) ? (httprequest.url + "?") : httprequest.url);
			if (string.IsNullOrEmpty(httprequest.fullurl))
			{
				if (httprequest.url.StartsWith("http"))
				{
					httprequest.fullurl = httprequest.url;
				}
				else
				{
					httprequest.fullurl = Utilities.GenerateFullHttpReqString(httprequest.url);
				}
			}
			if (VKToken.IsValidToken(VkApi.CurrentToken))
			{
				WWW www = new WWW(httprequest.fullurl);
				yield return www;
				this.HandleResponse(www, httprequest);
			}
			else
			{
				this.HandleTokenExpired(httprequest);
			}
			yield break;
		}

		public void UploadToserverCall(VKRequest httprequest)
		{
			if (string.IsNullOrEmpty(httprequest.fullurl))
			{
				if (httprequest.url.StartsWith("http"))
				{
					httprequest.fullurl = httprequest.url;
				}
				else
				{
					httprequest.fullurl = Utilities.GenerateFullHttpReqString(httprequest.url);
				}
			}
			base.StartCoroutine(this._UploadToserverCall(httprequest));
		}

		public void UploadToserverCall(VKRequest requestString, FileForUpload file)
		{
			base.StartCoroutine(this._UploadToserverCall(requestString, file));
		}

		private IEnumerator _UploadToserverCall(VKRequest httprequest)
		{
			WWW www = this.GenerateWWWForm(httprequest);
			yield return www;
			this.HandleResponse(www, httprequest);
			yield break;
		}

		private IEnumerator _UploadToserverCall(VKRequest httprequest, FileForUpload file)
		{
			WWW www = this.GenerateWWWForm(httprequest, file);
			yield return www;
			this.HandleResponse(www, httprequest);
			yield break;
		}

		public bool IsUserLoggedIn;

		public string VkRequestUrlBase = "https://api.vk.com/method/";

		public static VkApi VkApiInstance;

		public static VKToken CurrentToken;

		public static VkSettings VkSetts;

		public static Downloader Downloader;

		public LoginLogoutBridge nativeBridge = new LoginLogoutBridge();

		private bool loginProccessSterted;
	}
}
