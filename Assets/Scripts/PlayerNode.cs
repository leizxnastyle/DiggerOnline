using System;
using UnityEngine;

public class PlayerNode
{
	public PlayerNode()
	{
		this.Life = this.MaxLife;
		if (!GameType.IsHGNoInstanceCheck)
		{
			this.Shield = 0f;
		}
		else
		{
			this.Shield = 0.06f;
		}
		this.IsMainPlayer = false;
		this.ZombieBlock = 60;
	}

	public float Life
	{
		get
		{
			return this.LifeEx;
		}
		set
		{
			this.LifeEx = value;
			if (this.IsMainPlayer)
			{
				KGUI.SetControlText("hud.battle.txt_health", (int)Math.Ceiling((double)(value * 100f)) + string.Empty);
				if (this.isLifeOfShieldChanged != null)
				{
					this.isLifeOfShieldChanged(this.Name, this.Life, this.Shield);
				}
			}
		}
	}

	public float MaxLife
	{
		get
		{
			return 1f + ((!this.IsZombie) ? 0f : 0f);
		}
	}

	public float Shield
	{
		get
		{
			return this.ShieldEx;
		}
		set
		{
			this.ShieldEx = value;
			if (this.IsMainPlayer)
			{
				KGUI.SetControlText("hud.battle.txt_shield", (int)Math.Ceiling((double)(value * 100f)) + string.Empty);
				if (this.isLifeOfShieldChanged != null)
				{
					this.isLifeOfShieldChanged(this.Name, this.Life, this.Shield);
				}
			}
		}
	}

	public float MaxShield
	{
		get
		{
			return 1f;
		}
	}

	public float UnarmedDamage
	{
		get
		{
			return 0.5f * ((!this.IsZombie) ? 1f : 2f);
		}
	}

	public bool IsZombie
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance is ZombieVirus && TeamBattle.Instance.GetPlayerTeam(this.Name) == 1;
		}
	}

	public int ZombieBlock
	{
		get
		{
			return this.ZombieBlockEx;
		}
		set
		{
			this.ZombieBlockEx = value;
		}
	}

	public bool IsMainPlayer { get; set; }

	public void DoDamage(float value, bool notifyPhotonServer)
	{
		if (!GameType.IsHungerGamesMode)
		{
			if (this.Life <= 0f)
			{
				return;
			}
			if (this.Shield > 0f)
			{
				this.Shield -= value;
				if (this.Shield < 0f)
				{
					value = -this.Shield;
					this.Shield = 0f;
				}
				else
				{
					value = 0f;
				}
			}
			this.Life -= value;
			if (this.Life < 0f)
			{
				this.Life = 0f;
			}
			if (notifyPhotonServer)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PhotonView>().RPC("DamageMe", PhotonTargets.AllViaServer, new object[]
				{
					value
				});
			}
			if (this.Life <= 0f && this == WorldGameObjectX.Instance.MainPlayerNode)
			{
				WorldGameObjectX.Instance.OnMainPlayerDead();
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PhotonView>().RPC("Die", PhotonTargets.All, new object[0]);
			}
		}
		else if (value > 0f)
		{
			this.Life -= value;
			if (this.Life < 0f)
			{
				this.Life = 0f;
			}
			if (this.Life <= 0f && this == WorldGameObjectX.Instance.MainPlayerNode)
			{
				WorldGameObjectX.Instance.OnMainPlayerDead();
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PhotonView>().RPC("Die", PhotonTargets.All, new object[0]);
			}
		}
	}

	public const float LavaLifeDamage = 0.4f;

	public LifeOfShieldChanged isLifeOfShieldChanged;

	public string Name;

	public string ViewerID;

	public int Skin;

	public PhotonPlayer NetPlayer;

	public GameObject Avatar;

	public int Level = 1;

	public bool Voice = true;

	public DateTime LastVoice;

	public int[] WeaponSkin;

	public SecuredValue<float> LifeEx;

	public SecuredValue<float> ShieldEx;

	public int ZombieBlockEx;

	[NonSerialized]
	public bool PlayerOnLadder;
}
