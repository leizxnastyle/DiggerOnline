using System;
using UnityEngine;

public class TeamDoorOpenTriger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		PhotonView component = other.gameObject.GetComponent<PhotonView>();
		if (component == null)
		{
			return;
		}
		if (TeamBattle.Instance != null)
		{
			if (App.Instance.Settings.gameType == GameINI.GameType.DEATHMATCH)
			{
				this.doors.GetComponent<TeamDoor>().OpenDoorR();
			}
			else
			{
				int playerTeam = TeamBattle.Instance.GetPlayerTeam(component.owner.name);
				if (playerTeam != 0 && playerTeam == this.doors.GetComponent<TeamDoor>().team)
				{
					this.doors.GetComponent<TeamDoor>().OpenDoorR();
				}
			}
		}
		else
		{
			this.doors.GetComponent<TeamDoor>().OpenDoorR();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		PhotonView component = other.gameObject.GetComponent<PhotonView>();
		if (component == null)
		{
			return;
		}
		this.doors.GetComponent<TeamDoor>().CloseDoorR();
	}

	public GameObject doors;
}
