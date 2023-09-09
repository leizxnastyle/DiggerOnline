using System;
using System.Text;
using UnityEngine;

public class Igor : bs
{
	public bool Deathmatch
	{
		get
		{
			return TeamBattle.Instance is Deathmatch;
		}
	}

	public string playerName
	{
		get
		{
			return PhotonNetwork.player.name;
		}
	}

	private void Awake()
	{
		if (!Debug.isDebugBuild)
		{
			this.debug = false;
		}
		if (!this.debug)
		{
			this.debug = (this.teamBattle = (this.enableLog = (this.autojoin = (this.autoJoin2 = false))));
		}
		Igor.Instance = this;
		if (this.debug)
		{
			QualitySettings.vSyncCount = 1;
		}
	}

	private void Update()
	{
		base.GetComponent<GUIText>().enabled = this.enableLog;
		base.GetComponent<GUIText>().text = bs.stringBuilder.ToString();
		bs.stringBuilder = new StringBuilder();
	}

	public static Igor Instance;

	public bool debug;

	public AudioClip shootSound;

	public AudioClip[] hitSound;

	public AudioClip[] damageSound;

	public AudioClip grenadeHit;

	public AudioClip fallSound;

	public AudioClip qdamageShoot;

	public AudioClip takeSound;

	public GameObject concerne;

	public GameObject hole;

	public GameObject explosion;

	public Bullet bullet;

	public AudioClip reload;

	public GameObject damageText;

	public bool autojoin;

	public bool autoJoin2;

	public bool autoChangeUser;

	public bool secondUser;

	internal bool enableLog = true;

	public bool teamBattle;

	public bool debugVersion;

	public Texture2D black;
}
