using System;
using UnityEngine.UI;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKYesNoMessageBox : QueueWorker<YesNoTaskData>
	{
		private void Start()
		{
			this.Message = base.GetComponentInChildren<Text>();
		}

		protected override void StartProcessing()
		{
			base.gameObject.SetActive(true);
			this.Message.text = this._current.Element.Message;
		}

		public void OkButtonClicked()
		{
			this._current.Element.OnYesButton();
			if (this._current.NextElement == null)
			{
				base.gameObject.SetActive(false);
			}
			base.ProccessNext();
		}

		public void CancelButtonClicked()
		{
			this._current.Element.OnNoButton();
			if (this._current.NextElement == null)
			{
				base.gameObject.SetActive(false);
			}
			base.ProccessNext();
		}

		public Text Message;
	}
}
