using System;

namespace com.playGenesis.VkUnityPlugin
{
	public class ServerRespOnPhotoUpload
	{
		public int server { get; set; }

		public string photos_list { get; set; }

		public int aid { get; set; }

		public string hash { get; set; }

		public int gid { get; set; }
	}
}
