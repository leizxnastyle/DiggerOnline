using System;
using ExitGames.Client.Photon;

internal static class ScoreExtensions
{
	public static void SetScore(this PhotonPlayer player, int newScore)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["score"] = newScore;
		player.SetCustomProperties(hashtable, null, false);
	}

	public static void AddScore(this PhotonPlayer player, int scoreToAddToCurrent)
	{
		int num = player.GetScore();
		num += scoreToAddToCurrent;
		Hashtable hashtable = new Hashtable();
		hashtable["score"] = num;
		player.SetCustomProperties(hashtable, null, false);
	}

	public static int GetScore(this PhotonPlayer player)
	{
		object obj;
		if (player.customProperties.TryGetValue("score", out obj))
		{
			return (int)obj;
		}
		return 0;
	}
}
