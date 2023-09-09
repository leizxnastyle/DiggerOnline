using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKAttachment
	{
		public static VKAttachment Deserialize(object attachment)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)attachment;
			VKAttachment vkattachment = new VKAttachment();
			object obj;
			if (dictionary.TryGetValue("type", out obj))
			{
				vkattachment.type = (string)obj;
			}
			object audio;
			if (dictionary.TryGetValue("audio", out audio))
			{
				vkattachment.audio = VKAudio.Deserialize(audio);
			}
			object photo;
			if (dictionary.TryGetValue("photo", out photo))
			{
				vkattachment.photo = VKPhoto.Deserialize(photo);
			}
			object poll;
			if (dictionary.TryGetValue("poll", out poll))
			{
				vkattachment.poll = VKPoll.Deserialize(poll);
			}
			object doc;
			if (dictionary.TryGetValue("doc", out doc))
			{
				vkattachment.doc = VKDocument.Deserialize(doc);
			}
			object link;
			if (dictionary.TryGetValue("link", out link))
			{
				vkattachment.link = VKLink.Deserialize(link);
			}
			object wallPost;
			if (dictionary.TryGetValue("wall", out wallPost))
			{
				vkattachment.wall = VKWallPost.Deserialize(wallPost);
			}
			object note;
			if (dictionary.TryGetValue("note", out note))
			{
				vkattachment.note = VKNote.Deserialize(note);
			}
			object page;
			if (dictionary.TryGetValue("Page", out page))
			{
				vkattachment.Page = VKPage.Deserialize(page);
			}
			return vkattachment;
		}

		public string type { get; set; }

		public VKAudio audio { get; set; }

		public VKVideo video { get; set; }

		public VKPhoto photo { get; set; }

		public VKPoll poll { get; set; }

		public VKDocument doc { get; set; }

		public VKLink link { get; set; }

		public VKWallPost wall { get; set; }

		public VKNote note { get; set; }

		public VKPage Page { get; set; }
	}
}
