using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPoll
	{
		public static VKPoll Deserialize(object poll)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)poll;
			VKPoll vkpoll = new VKPoll();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkpoll.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("owner_id", out obj2))
			{
				vkpoll.owner_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("created", out obj3))
			{
				vkpoll.created = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("is_closed", out obj4))
			{
				vkpoll.is_closed = (int)((long)obj4);
			}
			object obj5;
			if (dictionary.TryGetValue("question", out obj5))
			{
				vkpoll.question = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("votes", out obj6))
			{
				vkpoll.votes = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("answer_id", out obj7))
			{
				vkpoll.answer_id = (long)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("answers", out obj8))
			{
				List<VKPollAnswer> list = new List<VKPollAnswer>();
				List<object> list2 = (List<object>)obj8;
				foreach (object answer in list2)
				{
					list.Add(VKPollAnswer.Dererialize(answer));
				}
				vkpoll.answers = list;
			}
			return vkpoll;
		}

		public long id { get; set; }

		public long owner_id { get; set; }

		public long created { get; set; }

		public int is_closed { get; set; }

		public string question { get; set; }

		public int votes { get; set; }

		public long answer_id { get; set; }

		public List<VKPollAnswer> answers { get; set; }
	}
}
