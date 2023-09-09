using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survival : GameType
{
	private void Awake()
	{
		Survival.Instance = this;
		this._GameTime = Time.time;
	}

	private IEnumerator Start()
	{
		while (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			yield return 0;
		}
		foreach (Survival.Player player in this._Players)
		{
			this.SetupPlayer(player.Name);
		}
		if (App.Instance.Settings.isServer)
		{
			PhotonView photonView = base.photonView;
			string methodName = "AddPlayer";
			PhotonTargets target = PhotonTargets.All;
			object[] array = new object[2];
			array[0] = PhotonNetwork.playerName;
			photonView.RPC(methodName, target, array);
		}
		yield break;
	}

	private void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("SetData", connectedPlayer, new object[]
			{
				this._GameEnded,
				Time.time - this._GameTime
			});
			foreach (Survival.Player player in this._Players)
			{
				base.photonView.RPC("AddPlayer", connectedPlayer, new object[]
				{
					player.Name,
					player.Data
				});
			}
			PhotonView photonView = base.photonView;
			string methodName = "AddPlayer";
			PhotonTargets target = PhotonTargets.All;
			object[] array = new object[2];
			array[0] = connectedPlayer.name;
			photonView.RPC(methodName, target, array);
		}
	}

	[PunRPC]
	private void SetData(bool gameEnded, float gameDuration)
	{
		this._GameEnded = gameEnded;
		this._GameTime = Time.time + gameDuration;
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer disconnectedPlayer)
	{
		int num = this._Players.FindIndex((Survival.Player p) => p.Name == disconnectedPlayer.name);
		if (num != -1)
		{
			this._Players.RemoveAt(num);
		}
	}

	[PunRPC]
	public void AddPlayer(string playerName, object[] playerData)
	{
		this._Players.Add(new Survival.Player(playerName, playerData));
		this.SetupPlayer(playerName);
		this.SortPlayers();
	}

	[PunRPC]
	public void UpdatePlayerData(string playerName, object[] playerData)
	{
		Survival.Player player = this._Players.Find((Survival.Player p) => p.Name == playerName);
		if (player != null)
		{
			player.Data = playerData;
		}
		this.SortPlayers();
	}

	private void SetupPlayer(string playerName)
	{
		if (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
	}

	private void Update()
	{
		if (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
		if (!base.photonView.isMine)
		{
			return;
		}
		if (this._GameEnded)
		{
			if (Time.time - this._GameTime >= 20f)
			{
				base.photonView.RPC("StartGame", PhotonTargets.All, new object[]
				{
					false
				});
			}
			return;
		}
		if (Time.time - this._LastSpawnTime >= 3f)
		{
			this._SpawnIndex++;
			this._LastSpawnTime = Time.time;
			if (UnityEngine.Object.FindObjectsOfType(typeof(NPC)).Length < 50)
			{
				this.SpawnEnemy(this._SpawnIndex % 10 == 0);
			}
		}
		if (Time.time - this._GameTime >= 300f)
		{
			base.photonView.RPC("EndGame", PhotonTargets.All, new object[0]);
		}
	}

	private void SpawnEnemy(bool heavy)
	{
		Quaternion identity = Quaternion.identity;
		SpawnArrow[] array = UnityEngine.Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
		if (array.Length > 0)
		{
			Vector3 vector = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
		}
		else
		{
			Vector3 vector = GameType.FindRandomPlace(null, 10f);
		}
	}

	public void OnKillEnemy(PhotonPlayer photonPlayer)
	{
		if (this._GameEnded)
		{
			return;
		}
		Survival.Player player = this._Players.Find((Survival.Player p) => p.Name == photonPlayer.name);
		if (player != null)
		{
			player.Score += 10;
			base.photonView.RPC("UpdatePlayerData", PhotonTargets.All, new object[]
			{
				player.Name,
				player.Data
			});
			PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayer(photonPlayer);
		}
	}

	private void SortPlayers()
	{
		this._Players.Sort((Survival.Player a, Survival.Player b) => b.Score - a.Score);
	}

	[PunRPC]
	private void EndGame()
	{
		this._GameEnded = true;
		this._GameTime = Time.time;
		WorldGameObjectX.Instance.DisableMouseLook();
		Screen.lockCursor = false;
	}

	[PunRPC]
	private void StartGame()
	{
		WorldGameObjectX.Instance.RestartLevel();
		this._GameEnded = false;
		this._GameTime = Time.time;
		foreach (Survival.Player player in this._Players)
		{
			player.Data = new object[]
			{
				0
			};
		}
	}

	public const float GameDuration = 300f;

	public const float RestartDuration = 20f;

	public const int MaxEnemies = 50;

	public const float EnemySpawnTime = 3f;

	public const float EasyEnemySpeed = 2f;

	public const float HeavyEnemySpeed = 4f;

	public const int EasyEnemyLife = 5;

	public const int HeavyEnemyLife = 20;

	public const int EasyEnemyDamage = 1;

	public const int HeavyEnemyDamage = 2;

	public const int ScoreForEnemyKill = 10;

	public static Survival Instance;

	private float _LastSpawnTime;

	private int _SpawnIndex;

	private List<Survival.Player> _Players = new List<Survival.Player>();

	private bool _GameEnded;

	private float _GameTime;

	private class Player
	{
		public Player(string name, object[] data)
		{
			this.Name = name;
			if (data != null)
			{
				this.Data = data;
			}
		}

		public object[] Data
		{
			get
			{
				return new object[]
				{
					this.Score
				};
			}
			set
			{
				this.Score = (int)value[0];
			}
		}

		public string Name;

		public int Score;
	}
}
