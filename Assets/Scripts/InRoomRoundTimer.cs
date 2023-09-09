using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class InRoomRoundTimer : MonoBehaviour
{
	private void StartRoundNow()
	{
		if (PhotonNetwork.time < 9.9999997473787516E-05)
		{
			this.startRoundWhenTimeIsSynced = true;
			return;
		}
		this.startRoundWhenTimeIsSynced = false;
		Hashtable hashtable = new Hashtable();
		hashtable["st"] = PhotonNetwork.time;
		PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
	}

	public void OnJoinedRoom()
	{
		if (PhotonNetwork.isMasterClient)
		{
			this.StartRoundNow();
		}
		else
		{
			UnityEngine.Debug.Log("StartTime already set: " + PhotonNetwork.room.customProperties.ContainsKey("st"));
		}
	}

	public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey("st"))
		{
			this.StartTime = (double)propertiesThatChanged["st"];
		}
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (!PhotonNetwork.room.customProperties.ContainsKey("st"))
		{
			UnityEngine.Debug.Log("The new master starts a new round, cause we didn't start yet.");
			this.StartRoundNow();
		}
	}

	private void Update()
	{
		if (this.startRoundWhenTimeIsSynced)
		{
			this.StartRoundNow();
		}
	}

	public void OnGUI()
	{
		double num = PhotonNetwork.time - this.StartTime;
		double num2 = (double)this.SecondsPerTurn - num % (double)this.SecondsPerTurn;
		int num3 = (int)(num / (double)this.SecondsPerTurn);
		GUILayout.BeginArea(this.TextPos);
		GUILayout.Label(string.Format("elapsed: {0:0.000}", num), new GUILayoutOption[0]);
		GUILayout.Label(string.Format("remaining: {0:0.000}", num2), new GUILayoutOption[0]);
		GUILayout.Label(string.Format("turn: {0:0}", num3), new GUILayoutOption[0]);
		if (GUILayout.Button("new round", new GUILayoutOption[0]))
		{
			this.StartRoundNow();
		}
		GUILayout.EndArea();
	}

	private const string StartTimeKey = "st";

	public int SecondsPerTurn = 5;

	public double StartTime;

	public Rect TextPos = new Rect(0f, 80f, 150f, 300f);

	private bool startRoundWhenTimeIsSynced;
}
