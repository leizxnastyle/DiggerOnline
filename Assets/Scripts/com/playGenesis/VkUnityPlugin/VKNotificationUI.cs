using System;
using UnityEngine.UI;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKNotificationUI : QueueWorker<string>
	{
		private void Start()
		{
			this.message = base.gameObject.GetComponentInChildren<Text>();
		}

		public void Notity(string message)
		{
			base.Add(message);
		}

		protected override void StartProcessing()
		{
			base.gameObject.SetActive(true);
			base.transform.SetAsLastSibling();
			this.message.text = this._current.Element;
		}

		public void Notify(VKRequest r)
		{
			if (!string.IsNullOrEmpty(r.error.error_msg))
			{
				base.Add(r.error.error_msg);
			}
		}

		public void onOkButton()
		{
			this.message.text = string.Empty;
			if (this._current.NextElement == null)
			{
				base.gameObject.SetActive(false);
			}
			base.ProccessNext();
		}

		public Text message;
	}
}
