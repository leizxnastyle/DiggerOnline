using System;
using UnityEngine;

public class Ladder : EntityBase
{
	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject == WorldGameObjectX.Instance.MainPlayer)
		{
			WorldGameObjectX.Instance.MainPlayer.SendMessage("SetLadderBody", false);
			this.players_stay = false;
			WorldGameObjectX.Instance.player_on_ladder = false;
			MainMenu.Instance.RefreshFlying();
		}
		else if (other.gameObject.GetComponent<HG_Cheat_Detector>() != null)
		{
			HG_Cheat_Detector component = other.gameObject.GetComponent<HG_Cheat_Detector>();
			WorldGameObjectX.Instance.FindPlayerByViewerID(component.View_ID).PlayerOnLadder = false;
			this.last_player_id = string.Empty;
		}
	}

	protected override void Destruction()
	{
		if (this.players_stay)
		{
			WorldGameObjectX.Instance.MainPlayer.SendMessage("SetLadderBody", false);
			WorldGameObjectX.Instance.player_on_ladder = false;
			MainMenu.Instance.RefreshFlying();
			WorldGameObjectX.Instance.FindPlayerByViewerID(this.last_player_id).PlayerOnLadder = false;
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (other.gameObject == WorldGameObjectX.Instance.MainPlayer)
		{
			WorldGameObjectX.Instance.MainPlayer.SendMessage("SetLadderBody", true);
			this.players_stay = true;
			WorldGameObjectX.Instance.player_on_ladder = true;
			WorldGameObjectX.Instance.last_ladder_delta = 0f;
		}
		else if (other.gameObject.GetComponent<HG_Cheat_Detector>() != null)
		{
			HG_Cheat_Detector component = other.gameObject.GetComponent<HG_Cheat_Detector>();
			WorldGameObjectX.Instance.FindPlayerByViewerID(component.View_ID).PlayerOnLadder = true;
			this.last_player_id = component.View_ID;
		}
	}

	private bool players_stay;

	private string last_player_id;
}
