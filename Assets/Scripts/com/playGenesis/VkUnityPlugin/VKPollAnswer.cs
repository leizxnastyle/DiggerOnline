using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPollAnswer
	{
		public static VKPollAnswer Dererialize(object answer)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)answer;
			VKPollAnswer vkpollAnswer = new VKPollAnswer();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkpollAnswer.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("text", out obj2))
			{
				vkpollAnswer.text = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("votes", out obj3))
			{
				vkpollAnswer.votes = (int)((long)obj3);
			}
			object obj4;
			if (dictionary.TryGetValue("rate", out obj4))
			{
				vkpollAnswer.rate = (double)obj4;
			}
			return vkpollAnswer;
		}

		public long id { get; set; }

		public string text { get; set; }

		public int votes { get; set; }

		public double rate { get; set; }
	}
}
