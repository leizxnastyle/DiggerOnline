using System;
using UnityEngine;

public class VoiceChatUi : MonoBehaviour
{
	private void Start()
	{
		Application.RequestUserAuthorization(UserAuthorization.Microphone);
	}

	private void OnGUI()
	{
		GUILayout.Window(1, new Rect(10f, 10f, (float)(Screen.width - 20), (float)(Screen.height - 20)), new GUI.WindowFunction(this.Window), string.Empty, GUIStyle.none, new GUILayoutOption[0]);
	}

	private void Window(int id)
	{
		GUI.Box(new Rect(0f, 0f, (float)(Screen.width - 20), (float)(Screen.height - 20)), string.Empty);
		if (VoiceChatRecorder.Instance.IsRecording)
		{
			GUILayout.Label(VoiceChatRecorder.Instance.Device, new GUILayoutOption[0]);
			if (GUILayout.Button("Stop Recording", new GUILayoutOption[0]))
			{
				VoiceChatRecorder.Instance.StopRecording();
			}
		}
		else
		{
			GUILayout.Label("Select microphone to start recording", new GUILayoutOption[0]);
			foreach (string text in VoiceChatRecorder.Instance.AvailableDevices)
			{
				if (GUILayout.Button(text, new GUILayoutOption[0]))
				{
					VoiceChatRecorder.Instance.Device = text;
					VoiceChatRecorder.Instance.StartRecording();
				}
			}
		}
		if (VoiceChatRecorder.Instance.Device != null)
		{
			GUILayout.Label("Push-to-talk key: " + VoiceChatRecorder.Instance.PushToTalkKey, new GUILayoutOption[0]);
			GUILayout.Label("Toggle-to-talk key: " + VoiceChatRecorder.Instance.ToggleToTalkKey, new GUILayoutOption[0]);
			GUILayout.Label("Auto detect speech: " + ((!VoiceChatRecorder.Instance.AutoDetectSpeech) ? "Off" : "On"), new GUILayoutOption[0]);
			if (GUILayout.Button("Toggle Auto Detect", new GUILayoutOption[0]))
			{
				VoiceChatRecorder.Instance.AutoDetectSpeech = !VoiceChatRecorder.Instance.AutoDetectSpeech;
			}
			GUI.color = ((!VoiceChatRecorder.Instance.IsTransmitting) ? Color.red : Color.green);
			GUILayout.Label((!VoiceChatRecorder.Instance.IsTransmitting) ? "Silent" : "Transmitting", new GUILayoutOption[0]);
		}
	}
}
