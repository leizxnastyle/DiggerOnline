using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class Chat : Photon.MonoBehaviour
{
	private void Awake()
	{
		Chat._Instance = this;
		this._Grid = KGUI.FindNode("hud.chat.grid_messages", false).gameObject;
		this._Input = KGUI.FindNode("hud.chat.inp_message", false).GetComponent<UIInput>();
		this._Input.onSubmit = new UIInput.OnSubmit(this.OnSubmit);
		KGUI.ResizeGrid("hud.chat.grid_messages", 0, delegate(GameObject slot, int index)
		{
		}, null);
	}

	public void OpenChat()
	{
		if (!MainMenu.Instance.MenuActive)
		{
			if (!this._IgnoreNextEnter && !this._Input.selected)
			{
				UnityEngine.Debug.Log("OpenChat");
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().StopMovement();
				this._Input.selected = true;
				this.ActivateGrid(true);
				this.RefreshAlpha();
			}
			this._IgnoreNextEnter = false;
		}
	}

	private void Update()
	{
		if ((UnityEngine.Input.GetKeyUp(KeyCode.Return) || UnityEngine.Input.GetKeyUp(KeyCode.KeypadEnter)) && !MainMenu.Instance.MenuActive)
		{
			if (!this._IgnoreNextEnter && !this._Input.selected)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().StopMovement();
				this._Input.selected = true;
				this.ActivateGrid(true);
				this.RefreshAlpha();
			}
			this._IgnoreNextEnter = false;
		}
		if (this._Grid.activeInHierarchy)
		{
			this.RefreshAlpha();
		}
		if (!this._Input.selected && this._Input.text.Length > 0)
		{
			this._Input.text = string.Empty;
		}
	}

	public static bool IsEnabled()
	{
		return !(Chat._Instance == null) && Chat._Instance._Input.selected;
	}

	public static void SendTextF(string message, bool forAll)
	{
		if (Chat._Instance == null)
		{
			return;
		}
		if (forAll)
		{
			Chat._Instance.photonView.RPC("Text", PhotonTargets.All, new object[]
			{
				message,
				GameType.IsObserving()
			});
		}
		else
		{
			Chat._Instance.Text(message, GameType.IsObserving());
		}
	}

	public static void SendInfoF(string message, bool forAll)
	{
		if (Chat._Instance == null)
		{
			return;
		}
		if (GameType.IsObserving())
		{
			return;
		}
		if (forAll)
		{
			Chat._Instance.photonView.RPC("Info", PhotonTargets.All, new object[]
			{
				message
			});
		}
		else
		{
			Chat._Instance.Info(message);
		}
	}

	public static void SendWarning(string message, bool forAll)
	{
		if (Chat._Instance == null)
		{
			return;
		}
		if (forAll)
		{
			Chat._Instance.photonView.RPC("Warning", PhotonTargets.All, new object[]
			{
				message
			});
		}
		else
		{
			Chat._Instance.Warning(message);
		}
	}

	public static void SendEmotion(string emotionName, bool forAll)
	{
		if (Chat._Instance == null)
		{
			return;
		}
		if (GameType.IsObserving())
		{
			return;
		}
		if (forAll)
		{
			Chat._Instance.photonView.RPC("Emotion", PhotonTargets.All, new object[]
			{
				ProfileINI.nickname,
				emotionName
			});
		}
		else
		{
			Chat._Instance.Emotion(ProfileINI.nickname, emotionName);
		}
	}

	[PunRPC]
	private void Text(string message, bool spectate)
	{
		int num = message.IndexOf(": ");
		if (num != -1 && !spectate)
		{
			PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayerByNameX(message.Substring(0, num));
			if (playerNode != null && playerNode != WorldGameObjectX.Instance.MainPlayerNode && playerNode.Avatar != null)
			{
				SpeechBubbles.AddText(playerNode.Avatar, message.Substring(num + 2, message.Length - num - 2));
			}
			if (playerNode != null && playerNode == WorldGameObjectX.Instance.MainPlayerNode)
			{
				SpeechBubbles.AddText(WorldGameObjectX.Instance.MainPlayer, message.Substring(num + 2, message.Length - num - 2));
			}
		}
		this.AddMessage(message, "[FFFFFF]");
	}

	[PunRPC]
	private void Info(string message)
	{
		this.AddMessage(message, "[FFFF00]");
	}

	[PunRPC]
	private void Warning(string message)
	{
		if (WorldGameObjectX.Instance != null && WorldGameObjectX.Instance.MainPlayer != null)
		{
			SoundManager.Instance.Play(SoundManager.Sound.Note2, WorldGameObjectX.Instance.MainPlayerEyes.GetComponent<AudioSource>());
		}
		this.AddMessage(message, "[FF0000]");
	}

	[PunRPC]
	private void Emotion(string playerName, string emotionName)
	{
		SpeechBubbles.Emotion emotion = SpeechBubbles.GetEmotion(emotionName);
		if (emotion == null)
		{
			return;
		}
		PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayerByNameX(playerName);
		if (playerNode != null && playerNode != WorldGameObjectX.Instance.MainPlayerNode && playerNode.Avatar != null)
		{
			SpeechBubbles.AddEmotion(playerNode.Avatar, emotion.Picture.name);
		}
		if (playerNode != null && playerNode == WorldGameObjectX.Instance.MainPlayerNode)
		{
			SpeechBubbles.AddEmotion(WorldGameObjectX.Instance.MainPlayer, emotion.Picture.name);
		}
		if (!string.IsNullOrEmpty(emotion.Text) && emotion.Text != "voiceChat")
		{
			this.AddMessage(MainMenu.FixCollorName(playerName) + ": **" + Localize.GetText(emotion.Text, null) + "**", "[FFFFFF]");
		}
	}

	private void OnSubmit(string inputText)
	{
		inputText = NGUITools.StripSymbols(inputText);
		if (!string.IsNullOrEmpty(inputText))
		{
			inputText = App.Instance.AntiMatSystem.GetGoodText(inputText);
			Chat.SendTextF(NGUITools.StripSymbols(ProfileINI.nickname) + ": " + inputText, true);
			this._Input.text = string.Empty;
			this._Input.selected = false;
		}
		this._IgnoreNextEnter = true;
	}

	private void AddMessage(string text, string color)
	{
		this._Messages.Add(new Chat.Message
		{
			Text = color + MainMenu.FixCollorName(text),
			StartTime = Time.time
		});
		while (this._Messages.Count > 15)
		{
			this._Messages.RemoveAt(0);
		}
		this.ActivateGrid(true);
		KGUI.ResizeGrid("hud.chat.grid_messages", this._Messages.Count, delegate(GameObject slot, int index)
		{
			Chat.Message message = this._Messages[this._Messages.Count - index - 1];
			UILabel componentInChildren = slot.GetComponentInChildren<UILabel>();
			componentInChildren.text = message.Text;
			message.Label = componentInChildren;
		}, null);
		this.RefreshAlpha();
	}

	private void RefreshAlpha()
	{
		float num = 0f;
		foreach (Chat.Message message in this._Messages)
		{
			float num2 = 1f;
			if (!this._Input.selected)
			{
				num2 = Mathf.Clamp01(1f - (Time.time - message.StartTime - 5f) / 1f);
			}
			if (message.Label != null)
			{
				message.Label.alpha = num2;
			}
			num = Mathf.Max(num, num2);
		}
		if (num == 0f)
		{
			this.ActivateGrid(false);
		}
	}

	private void ActivateGrid(bool activate)
	{
		this._Grid.SetActive(activate);
		if (activate)
		{
			this._Grid.transform.Find("slot_prototype").gameObject.SetActive(false);
		}
	}

	public const int MaxMessages = 15;

	public const float DisappearDelay = 5f;

	public const float DisappearTime = 1f;

	public static Chat _Instance;

	private List<Chat.Message> _Messages = new List<Chat.Message>();

	private GameObject _Grid;

	private UIInput _Input;

	private bool _IgnoreNextEnter;

	private class Message
	{
		public string Text;

		public float StartTime;

		public UILabel Label;
	}
}
