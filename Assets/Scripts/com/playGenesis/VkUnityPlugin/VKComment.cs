using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKComment
	{
		public static VKComment Deserialize(object Comment)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Comment;
			VKComment vkcomment = new VKComment();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkcomment.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("from_id", out obj2))
			{
				vkcomment.from_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("date", out obj3))
			{
				vkcomment.date = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("text", out obj4))
			{
				vkcomment.text = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("reply_to_user", out obj5))
			{
				vkcomment.reply_to_user = (long)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("reply_to_comment", out obj6))
			{
				vkcomment.reply_to_comment = (long)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("attachments", out obj7))
			{
				List<object> list = (List<object>)obj7;
				List<VKAttachment> list2 = new List<VKAttachment>();
				foreach (object attachment in list)
				{
					list2.Add(VKAttachment.Deserialize(attachment));
				}
				vkcomment.attachments = list2;
			}
			return vkcomment;
		}

		public long id { get; set; }

		public long from_id { get; set; }

		public long date { get; set; }

		private string text { get; set; }

		public long reply_to_user { get; set; }

		public long reply_to_comment { get; set; }

		public List<VKAttachment> attachments { get; set; }
	}
}
