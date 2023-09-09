using System;
using System.Collections.Generic;
using UnityEngine;

public static class CheatFinderManager
{
	public static void ReportCheat(string user_id, CheatFinderManager.CheatType ct)
	{
		WorldGameObjectX.Instance.photonView.RPC("ReportCMSG", PhotonTargets.All, new object[]
		{
			user_id,
			(int)ct
		});
	}

	public static void SetStayPosition()
	{
		CheatFinderManager.setPosition = new List<Vector3>();
		foreach (PlayerNode playerNode in WorldGameObjectX.Instance.PlayerList)
		{
			Vector3 item = new Vector3((float)((int)playerNode.Avatar.transform.position.x), (float)((int)playerNode.Avatar.transform.position.y), (float)((int)playerNode.Avatar.transform.position.z));
			CheatFinderManager.setPosition.Add(item);
		}
	}

	internal static void CheckStayPosition()
	{
		if (CheatFinderManager.setPosition != null && CheatFinderManager.setPosition.Count > 0)
		{
			for (int i = 0; i < CheatFinderManager.setPosition.Count; i++)
			{
				PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[i];
				Vector3 vector = new Vector3((float)((int)playerNode.Avatar.transform.position.x), (float)((int)playerNode.Avatar.transform.position.y), (float)((int)playerNode.Avatar.transform.position.z));
			}
		}
	}

	private static List<Vector3> setPosition;

	public enum CheatType
	{
		CT_HG_MOVE_IF_NOT,
		CT_ALL_SPEED_CHEAT,
		CT_ALL_FLY_CHEAT,
		CT_WALL_MOVE,
		CT_LAVA_STAY
	}
}
