using System;

namespace com.playGenesis.VkUnityPlugin
{
	public class PersistentToke
	{
		public string user_id
		{
			get
			{
				return this._user_id;
			}
			set
			{
				this._user_id = value;
			}
		}

		public string access_token;

		public int expires_in;

		public string _user_id;

		public DateTime tokenRecievedTime;

		public bool tokenFromEditor;
	}
}
