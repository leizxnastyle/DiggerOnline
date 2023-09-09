using System;
using System.Collections.Generic;
using System.Text;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatChannel
	{
		public ChatChannel(string name)
		{
			this.Name = name;
		}

		public bool IsPrivate { get; protected internal set; }

		public int MessageCount
		{
			get
			{
				return this.Messages.Count;
			}
		}

		public void Add(string sender, object message)
		{
			this.Senders.Add(sender);
			this.Messages.Add(message);
		}

		public void Add(string[] senders, object[] messages)
		{
			this.Senders.AddRange(senders);
			this.Messages.AddRange(messages);
		}

		public void ClearMessages()
		{
			this.Senders.Clear();
			this.Messages.Clear();
		}

		public string ToStringMessages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Messages.Count; i++)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", this.Senders[i], this.Messages[i]));
			}
			return stringBuilder.ToString();
		}

		public readonly string Name;

		public readonly List<string> Senders = new List<string>();

		public readonly List<object> Messages = new List<object>();
	}
}
