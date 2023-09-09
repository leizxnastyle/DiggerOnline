using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKScope
	{
		public static List<string> ParseVKPermissionsFromInteger(int permissionsValue)
		{
			List<string> list = new List<string>();
			if ((permissionsValue & 1) > 0)
			{
				list.Add("notify");
			}
			if ((permissionsValue & 2) > 0)
			{
				list.Add("friends");
			}
			if ((permissionsValue & 4) > 0)
			{
				list.Add("photos");
			}
			if ((permissionsValue & 8) > 0)
			{
				list.Add("audio");
			}
			if ((permissionsValue & 16) > 0)
			{
				list.Add("video");
			}
			if ((permissionsValue & 128) > 0)
			{
				list.Add("pages");
			}
			if ((permissionsValue & 1024) > 0)
			{
				list.Add("status");
			}
			if ((permissionsValue & 2048) > 0)
			{
				list.Add("notes");
			}
			if ((permissionsValue & 4096) > 0)
			{
				list.Add("messages");
			}
			if ((permissionsValue & 8192) > 0)
			{
				list.Add("wall");
			}
			if ((permissionsValue & 32768) > 0)
			{
				list.Add("ads");
			}
			if ((permissionsValue & 65536) > 0)
			{
				list.Add("offline");
			}
			if ((permissionsValue & 131072) > 0)
			{
				list.Add("docs");
			}
			if ((permissionsValue & 262144) > 0)
			{
				list.Add("groups");
			}
			if ((permissionsValue & 524288) > 0)
			{
				list.Add("notifications");
			}
			if ((permissionsValue & 1048576) > 0)
			{
				list.Add("stats");
			}
			return list;
		}

		public const string NOTIFY = "notify";

		public const string FRIENDS = "friends";

		public const string PHOTOS = "photos";

		public const string AUDIO = "audio";

		public const string VIDEO = "video";

		public const string DOCS = "docs";

		public const string NOTES = "notes";

		public const string PAGES = "pages";

		public const string STATUS = "status";

		public const string WALL = "wall";

		public const string GROUPS = "groups";

		public const string MESSAGES = "messages";

		public const string NOTIFICATIONS = "notifications";

		public const string STATS = "stats";

		public const string ADS = "ads";

		public const string OFFLINE = "offline";

		public const string NOHTTPS = "nohttps";

		public const string DIRECT = "direct";
	}
}
