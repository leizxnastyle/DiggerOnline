using System;
using UnityEngine;

public class HG_TeleportCheat : MonoBehaviour
{
	private void Start()
	{
		this.lastPosition = base.transform.position;
	}

	private void Update()
	{
		if (GameType.IsHungerGamesMode)
		{
			HungerGames hungerGames = (HungerGames)TeamBattle.Instance;
			if (HG_WorkController.hgstatus == GameStatus.GS_START && Vector3.Distance(this.lastPosition, base.transform.position) > 10f)
			{
				WorldGameObjectX.Instance.ExitGame(string.Empty);
			}
			this.lastPosition = base.transform.position;
		}
	}

	private const float CHEAT_DIST = 10f;

	private Vector3 lastPosition = Vector3.zero;
}
