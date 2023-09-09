using System;
using System.Collections;
using UnityEngine;

public class Protect : MonoBehaviour
{
	private void Awake()
	{
		if (Protect.Instance == null)
		{
			Protect.Instance = this;
		}
	}

	public void CheckDLL(int Number)
	{
		Transform[] array = UnityEngine.Object.FindObjectsOfType(typeof(Transform)) as Transform[];
		foreach (Transform transform in array)
		{
			if (transform.GetComponent(ProtectManager.DLLName[Number]) != null)
			{
				base.StartCoroutine(this.SetBan("CheckDLL " + ProtectManager.DLLName[Number]));
				PlayerBanned.Instance.ShowBan();
				return;
			}
		}
	}

	public IEnumerator SetBan(string Reason)
	{
		WWWForm Form = new WWWForm();
		Form.AddField("PlayerID", VKAPI.INSTANCE._viewerId);
		Form.AddField("AuthKey", VKAPI.INSTANCE._authKey);
		Form.AddField("Reason", Reason);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[2] + "SetBan.php", Form);
		yield return phpLoad;
		if (phpLoad.text == "Successful")
		{
			ProtectManager.PlayerBanned = true;
		}
		yield break;
	}

	public IEnumerator GetBan()
	{
		WWWForm Form = new WWWForm();
		Form.AddField("PlayerID", VKAPI.INSTANCE._viewerId);
		Form.AddField("AuthKey", VKAPI.INSTANCE._authKey);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[2] + "GetBan.php", Form);
		yield return phpLoad;
		if (phpLoad.text == "BAN_ON")
		{
			ProtectManager.PlayerBanned = true;
		}
		yield break;
	}

	public static Protect Instance;
}
