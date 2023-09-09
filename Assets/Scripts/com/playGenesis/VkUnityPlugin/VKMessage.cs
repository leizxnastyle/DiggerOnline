using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKMessage
	{
		public static VKMessage Deserialize(object message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)message;
			VKMessage vkmessage = new VKMessage();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkmessage.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("user_id", out obj2))
			{
				vkmessage.user_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("date", out obj3))
			{
				vkmessage.date = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("read_state", out obj4))
			{
				vkmessage.read_state = (int)((long)obj4);
			}
			object obj5;
			if (dictionary.TryGetValue("out", out obj5))
			{
				vkmessage.@out = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("title", out obj6))
			{
				vkmessage.title = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("body", out obj7))
			{
				vkmessage.body = (string)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("attachments", out obj8))
			{
				List<VKAttachment> list = new List<VKAttachment>();
				List<object> list2 = (List<object>)obj8;
				foreach (object attachment in list2)
				{
					list.Add(VKAttachment.Deserialize(attachment));
				}
				vkmessage.attachments = list;
			}
			object geo;
			if (dictionary.TryGetValue("geo", out geo))
			{
				vkmessage.geo = VKGeo.Deserialize(geo);
			}
			object obj9;
			if (dictionary.TryGetValue("fwd_messages", out obj9))
			{
				List<VKMessage> list3 = new List<VKMessage>();
				List<VKMessage> list4 = (List<VKMessage>)obj9;
				foreach (VKMessage message2 in list4)
				{
					list3.Add(VKMessage.Deserialize(message2));
				}
				vkmessage.fwd_messages = list3;
			}
			object obj10;
			if (dictionary.TryGetValue("emoji", out obj10))
			{
				vkmessage.emoji = (int)((long)obj10);
			}
			object obj11;
			if (dictionary.TryGetValue("important", out obj11))
			{
				vkmessage.important = (int)((long)obj11);
			}
			object obj12;
			if (dictionary.TryGetValue("deleted", out obj12))
			{
				vkmessage.deleted = (int)((long)obj12);
			}
			object obj13;
			if (dictionary.TryGetValue("chat_id", out obj13))
			{
				vkmessage.chat_id = (long)obj13;
			}
			object obj14;
			if (dictionary.TryGetValue("chat_active", out obj14))
			{
				vkmessage.chat_active = new List<long>();
				foreach (object obj15 in ((List<object>)obj14))
				{
					vkmessage.chat_active.Add((long)obj15);
				}
			}
			object obj16;
			if (dictionary.TryGetValue("users_count", out obj16))
			{
				vkmessage.users_count = (int)((long)obj16);
			}
			object obj17;
			if (dictionary.TryGetValue("admin_id", out obj17))
			{
				vkmessage.admin_id = (long)obj17;
			}
			object settings;
			if (dictionary.TryGetValue("push_settings", out settings))
			{
				vkmessage.push_settings = VKPushSettings.Deserialize(settings);
			}
			object obj18;
			if (dictionary.TryGetValue("action", out obj18))
			{
				vkmessage.action = (string)obj18;
			}
			object obj19;
			if (dictionary.TryGetValue("action_mid", out obj19))
			{
				vkmessage.action_mid = (long)obj19;
			}
			object obj20;
			if (dictionary.TryGetValue("action_email", out obj20))
			{
				vkmessage.action_email = (string)obj20;
			}
			object obj21;
			if (dictionary.TryGetValue("action_text", out obj21))
			{
				vkmessage.action_text = (string)obj21;
			}
			object obj22;
			if (dictionary.TryGetValue("photo_50", out obj22))
			{
				vkmessage.photo_50 = (string)obj22;
			}
			object obj23;
			if (dictionary.TryGetValue("photo_100", out obj23))
			{
				vkmessage.photo_100 = (string)obj23;
			}
			object obj24;
			if (dictionary.TryGetValue("photo_200", out obj24))
			{
				vkmessage.photo_200 = (string)obj24;
			}
			return vkmessage;
		}

		public long id { get; set; }

		public long user_id { get; set; }

		public long date { get; set; }

		public int read_state { get; set; }

		public int @out { get; set; }

		public string title { get; set; }

		public string body { get; set; }

		public List<VKAttachment> attachments { get; set; }

		public VKGeo geo { get; set; }

		public List<VKMessage> fwd_messages { get; set; }

		public int emoji { get; set; }

		public int important { get; set; }

		public int deleted { get; set; }

		public long chat_id { get; set; }

		public List<long> chat_active { get; set; }

		public int users_count { get; set; }

		public long admin_id { get; set; }

		public VKPushSettings push_settings { get; set; }

		public string action
		{
			get
			{
				return this._action;
			}
			set
			{
				this._action = (value ?? string.Empty);
			}
		}

		public long action_mid { get; set; }

		public string action_email { get; set; }

		public string action_text { get; set; }

		public string photo_50 { get; set; }

		public string photo_100 { get; set; }

		public string photo_200 { get; set; }

		private string _action = string.Empty;
	}
}
