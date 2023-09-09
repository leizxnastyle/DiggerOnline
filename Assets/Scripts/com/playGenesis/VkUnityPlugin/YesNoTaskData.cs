using System;

namespace com.playGenesis.VkUnityPlugin
{
	public class YesNoTaskData
	{
		public Action OnYesButton { get; set; }

		public Action OnNoButton { get; set; }

		public string Message { get; set; }

		public object[] CustomData { get; set; }
	}
}
