using System;
using System.Collections;
using UnityEngine;

public class Levels : MonoBehaviour
{
	private void Awake()
	{
		if (Levels.Instance == null)
		{
			Levels.Instance = this;
		}
	}

	public IEnumerator SetLevel()
	{
		if (ProfileINI.level < 30 && ProfileINI.level > 1)
		{
			WWWForm Form = new WWWForm();
			Form.AddField("PlayerID", VKAPI.INSTANCE._viewerId);
			Form.AddField("AuthKey", VKAPI.INSTANCE._authKey);
			Form.AddField("Level", ProfileINI.level);
			WWW phpLoad = new WWW(SettingsManager.ServerURL[2] + "SetLevel.php", Form);
			yield return phpLoad;
			UnityEngine.Debug.Log("Level: " + phpLoad.text);
		}
		yield break;
	}

	public static Levels Instance;
}
