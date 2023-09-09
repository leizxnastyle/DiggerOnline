using System;
using System.Collections;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class Downloader : MonoBehaviour
	{
		public void download(DownloadRequest d)
		{
			base.StartCoroutine(this._download(d));
		}

		private IEnumerator _download(DownloadRequest d)
		{
			string request = d.url;
			WWW www = new WWW(Uri.EscapeUriString(request));
			yield return www;
			d.DownloadResult = www;
			if (d.onFinished != null)
			{
				d.onFinished(d);
			}
			yield break;
		}
	}
}
