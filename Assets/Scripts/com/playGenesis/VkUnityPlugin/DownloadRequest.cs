using System;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class DownloadRequest
	{
		public string url { get; set; }

		public Action<DownloadRequest> onFinished { get; set; }

		public WWW DownloadResult { get; set; }

		public object[] CustomData { get; set; }
	}
}
