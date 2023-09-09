using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKWallPost
	{
		public static VKWallPost Deserialize(object WallPost)
		{
			VKWallPost vkwallPost = new VKWallPost();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)WallPost;
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkwallPost.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("owner_id", out obj2))
			{
				vkwallPost.owner_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("from_id", out obj3))
			{
				vkwallPost.from_id = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("date", out obj4))
			{
				vkwallPost.date = (long)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("text", out obj5))
			{
				vkwallPost.text = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("reply_owner_id", out obj6))
			{
				vkwallPost.reply_owner_id = (long)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("reply_post_id", out obj7))
			{
				vkwallPost.reply_post_id = (long)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("friends_only", out obj8))
			{
				vkwallPost.friends_only = (int)((long)obj8);
			}
			object comments;
			if (dictionary.TryGetValue("comments", out comments))
			{
				vkwallPost.comments = VKComments.Deserialize(comments);
			}
			object likes;
			if (dictionary.TryGetValue("likes", out likes))
			{
				vkwallPost.likes = VKLikes.Deserialize(likes);
			}
			object reposts;
			if (dictionary.TryGetValue("reposts", out reposts))
			{
				vkwallPost.reposts = VKReposts.Deserialize(reposts);
			}
			object obj9;
			if (dictionary.TryGetValue("post_type", out obj9))
			{
				vkwallPost.post_type = (string)obj9;
			}
			object in_data;
			if (dictionary.TryGetValue("post_source", out in_data))
			{
				vkwallPost.post_source = VKPostSource.Deserialize(in_data);
			}
			object obj10;
			if (dictionary.TryGetValue("attachments", out obj10))
			{
				List<VKAttachment> list = new List<VKAttachment>();
				foreach (object attachment in ((List<object>)obj10))
				{
					list.Add(VKAttachment.Deserialize(attachment));
				}
				vkwallPost.attachments = list;
			}
			object geo;
			if (dictionary.TryGetValue("geo", out geo))
			{
				vkwallPost.geo = VKGeo.Deserialize(geo);
			}
			object obj11;
			if (dictionary.TryGetValue("signer_id", out obj11))
			{
				vkwallPost.signer_id = (long)obj11;
			}
			object obj12;
			if (dictionary.TryGetValue("copy_history", out obj12))
			{
				List<VKWallPost> list2 = new List<VKWallPost>();
				foreach (object wallPost in ((List<object>)obj12))
				{
					list2.Add(VKWallPost.Deserialize(wallPost));
				}
				vkwallPost.copy_history = list2;
			}
			object obj13;
			if (dictionary.TryGetValue("can_pin", out obj13))
			{
				vkwallPost.can_pin = (int)((long)obj13);
			}
			object obj14;
			if (dictionary.TryGetValue("is_pinned", out obj14))
			{
				vkwallPost.is_pinned = (int)((long)obj14);
			}
			return vkwallPost;
		}

		public long id { get; set; }

		public long owner_id { get; set; }

		public long from_id { get; set; }

		public long date { get; set; }

		public string text { get; set; }

		public long reply_owner_id { get; set; }

		public long reply_post_id { get; set; }

		public int friends_only { get; set; }

		public VKComments comments { get; set; }

		public VKLikes likes { get; set; }

		public VKReposts reposts { get; set; }

		public string post_type { get; set; }

		public VKPostSource post_source { get; set; }

		public List<VKAttachment> attachments { get; set; }

		public VKGeo geo { get; set; }

		public long signer_id { get; set; }

		public List<VKWallPost> copy_history { get; set; }

		public int can_pin { get; set; }

		public int is_pinned { get; set; }
	}
}
