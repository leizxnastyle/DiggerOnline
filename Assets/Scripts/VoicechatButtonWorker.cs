using System;
using UnityEngine;

public class VoicechatButtonWorker : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (VoicechatButtonWorker.isPressedChat)
		{
			CameraController.Instance.SendVoiceOnChat();
			if (AudioListener.volume != 0.1f)
			{
				AudioListener.volume = 0.1f;
			}
		}
		else if (AudioListener.volume != 1f)
		{
			AudioListener.volume = 1f;
		}
		if (Input.touches.Length == 0)
		{
			VoicechatButtonWorker.isPressedChat = false;
		}
	}

	private void OnPress(bool isPressed)
	{
		VoicechatButtonWorker.isPressedChat = isPressed;
	}

	private void OnClick()
	{
		UnityEngine.Debug.Log("OnClick");
	}

	public static bool isPressedChat;
}
