using System;
using UnityEngine;

public class VoiceChatPickUi : MonoBehaviour
{
	private void OnGUI()
	{
		GUILayout.Window(0, new Rect((float)(Screen.width / 2 - 50), (float)(Screen.height / 2 - 50), 100f, 100f), new GUI.WindowFunction(this.Window), string.Empty, GUIStyle.none, new GUILayoutOption[0]);
	}

	private void Window(int id)
	{
		GUI.Box(new Rect(0f, 0f, 100f, 100f), string.Empty);
		if (GUILayout.Button("Start Server", new GUILayoutOption[0]))
		{
			base.gameObject.GetComponent<VoiceChatUnityServer>().enabled = true;
			base.gameObject.AddComponent<VoiceChatServerUi>();
			UnityEngine.Object.Destroy(this);
		}
		if (GUILayout.Button("Start Client", new GUILayoutOption[0]))
		{
			base.gameObject.GetComponent<VoiceChatUnityClient>().enabled = true;
			base.gameObject.AddComponent<VoiceChatUi>();
			UnityEngine.Object.Destroy(this);
		}
	}
}
