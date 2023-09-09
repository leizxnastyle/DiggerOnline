using System;

namespace com.playGenesis.VkUnityPlugin
{
	[Serializable]
	public class VKRequest
	{
		public string url
		{
			get
			{
				return this._url;
			}
			set
			{
				this._url = value;
				this.fullurl = string.Empty;
			}
		}

		private string _url = string.Empty;

		public string fullurl = string.Empty;

		public string response;

		public Error error;

		public int attempt;

		public Action<VKRequest> CallBackFunction;

		public object[] data;

		public bool needsToBeeConfirmed;
	}
}
