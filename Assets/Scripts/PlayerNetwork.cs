using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using SkinsSystem;
using UnityEngine;

public class PlayerNetwork : bs
{
	public event Action<int, int> isSkinUpdate;

	public int PlayerTeam
	{
		get
		{
			return (!(TeamBattle.Instance != null)) ? 0 : TeamBattle.Instance.GetPlayerTeam(this.PlayerName);
		}
	}

	public bool IsRedTeam
	{
		get
		{
			return this.PlayerTeam == 1;
		}
	}

	public bool IsBlueTeam
	{
		get
		{
			return this.PlayerTeam == 2;
		}
	}

	public bool IsObserver
	{
		get
		{
			return !this.IsRedTeam && !this.IsBlueTeam;
		}
	}

	public string PlayerName
	{
		get
		{
			return base.photonView.owner.name;
		}
	}

	internal int CurGunId
	{
		get
		{
			return this.CurGunIdEx;
		}
		set
		{
			this.CurGunIdEx = value;
		}
	}

	public Gun CurGun
	{
		get
		{
			return this.Guns[(this.CurGunId != -1) ? this.CurGunId : 0];
		}
	}

	public bool IsCanDMG
	{
		get
		{
			return this._isCanDmg;
		}
	}

	public HG_PlayerNetwork HGPlayerNetwork { get; private set; }

	public HG_Cheat_Detector Cheat_Detector { get; private set; }

	public RestoreBlockController restoreBlockController { get; private set; }

	public HideSeekPlayerController hideSeekPlayerController { get; private set; }

	public CameraController CC
	{
		get
		{
			return base.GetComponent<CameraController>();
		}
	}

	public IS_mdl_Item CurentSelectedItem { get; private set; }

	public void InitWeapons()
	{
		if (Info.Instance.GameMode == "BUILDING" || Info.Instance.GameMode == "HUNGER_GAMES" || Info.Instance.GameMode == "ARCADE" || Info.Instance.GameMode == "HIDE_SEEK")
		{
			return;
		}
		Gun gun = this.FindGun("knife");
		gun.melee = true;
		Gun gun2 = gun;
		int num = gun.bulletsMag = (gun.bulletsMax = 0);
		gun.bullets = num;
		gun2.bulletsLeft = num;
		Gun gun3 = this.FindGun("luger");
		gun3.pistol = true;
		gun3.accuracy = 0.01f;
		gun3.damage = 0.2f;
		gun3.bulletsMag = 8;
		gun3.bulletsMax = 40;
		gun3.shootInterval = 0.5f;
		Gun gun4 = this.FindGun("glock");
		gun4.pistol = true;
		gun4.accuracy = 0.01f;
		gun4.damage = 0.15f;
		gun4.bulletsMag = 17;
		gun4.bulletsMax = 40;
		gun4.shootInterval = 0.2f;
		Gun gun5 = this.FindGun("revolver");
		gun5.pistol = true;
		gun5.accuracy = 0.01f;
		gun5.RecoilUp = 0.1f;
		gun5.damage = 0.33f;
		gun5.shootInterval = 1f;
		gun5.bulletsMag = 6;
		gun5.bulletsMax = 40;
		Gun gun6 = this.FindGun("colt");
		gun6.pistol = true;
		gun6.accuracy = 0.01f;
		gun6.RecoilUp = 0.1f;
		gun6.damage = 0.3f;
		gun6.shootInterval = 0.7f;
		gun6.bulletsMag = 7;
		gun6.bulletsMax = 40;
		Gun gun7 = this.FindGun("sawn_off");
		gun7.shootBullets = 5;
		gun7.damage = 0.2f;
		gun7.RecoilRand = 0.01f;
		gun7.RecoilUp = 0.1f;
		gun7.shootInterval = 1f;
		gun7.reloadSpeed = 0.7f;
		gun7.accuracy = 0.1f;
		gun7.bulletsMag = 2;
		gun7.bulletsMax = 20;
		Gun gun8 = this.FindGun("winchester");
		gun8.shootBullets = 5;
		gun8.damage = 0.25f;
		gun8.RecoilRand = 0.01f;
		gun8.RecoilUp = 0.1f;
		gun8.shootInterval = 1f;
		gun8.reloadSpeed = 0.7f;
		gun8.accuracy = 0.1f;
		gun8.bulletsMag = 5;
		gun8.bulletsMax = 40;
		Gun gun9 = this.FindGun("m1014");
		gun9.shootBullets = 5;
		gun9.damage = 0.3f;
		gun9.RecoilRand = 0.01f;
		gun9.RecoilUp = 0.1f;
		gun9.shootInterval = 1f;
		gun9.reloadSpeed = 0.7f;
		gun9.accuracy = 0.1f;
		gun9.bulletsMag = 7;
		gun9.bulletsMax = 40;
		Gun gun10 = this.FindGun("rifle");
		gun10.damage = 0.5f;
		gun10.shootInterval = 1f;
		gun10.bulletSpeed = 300f;
		gun10.bulletsMag = 1;
		gun10.bulletsMax = 40;
		gun10.reloadSpeed = 0.8f;
		Gun gun11 = this.FindGun("sniper");
		gun11.sniper = true;
		gun11.damage = 0.6f;
		gun11.bulletSpeed = 300f;
		gun11.shootInterval = 1f;
		gun11.reloadSpeed = 0.8f;
		gun11.bulletsMag = 1;
		gun11.bulletsMax = 40;
		Gun gun12 = this.FindGun("svd");
		gun12.sniper = true;
		gun12.damage = 0.7f;
		gun12.bulletsMax = 40;
		gun12.bulletsMag = 10;
		gun12.bulletSpeed = 300f;
		gun12.shootInterval = 1f;
		gun12.reloadSpeed = 0.7f;
		gun12.accuracy = 0.01f;
		Gun gun13 = this.FindGun("mp5");
		gun13.damage = 0.08f;
		gun13.shootInterval = 0.11f;
		Gun gun14 = this.FindGun("ak47");
		gun14.damage = 0.1f;
		gun14.shootInterval = 0.1f;
		Gun gun15 = this.FindGun("m16");
		gun15.damage = 0.12f;
		gun15.shootInterval = 0.1f;
		gun15.RecoilUp = 0.02f;
		gun15.RecoilRand = 0.015f;
		Gun gun16 = this.FindGun("ppsh");
		gun16.bulletsMag = 70;
		gun16.RecoilUp = 0.01f;
		gun16.RecoilRand = 0.01f;
		gun16.shootInterval = 0.14f;
		gun16.bulletsMax = 140;
		gun16.damage = 0.15f;
		Gun gun17 = this.FindGun("mp40");
		gun17.bulletsMag = 32;
		gun17.RecoilUp = 0.02f;
		gun17.RecoilRand = 0.015f;
		gun17.damage = 0.14f;
		gun17.bulletsMax = 98;
		gun17.shootInterval = 0.13f;
		Gun gun18 = this.FindGun("thompson");
		gun18.bulletsMag = 30;
		gun18.RecoilUp = 0.01f;
		gun18.RecoilRand = 0.01f;
		gun18.bulletsMax = 90;
		gun18.shootInterval = 0.12f;
		gun18.damage = 0.13f;
		Gun gun19 = this.FindGun("grenade");
		gun19.grenade = true;
		gun19.damage = 0.5f;
		gun19.shootInterval = 0.2f;
		Gun gun20 = gun19;
		num = (gun19.bulletsMag = (gun19.bulletsMax = 0));
		gun19.bullets = num;
		gun20.bulletsLeft = num;
		Gun gun21 = this.FindGun("grenade_launcher");
		gun21.bazuka = true;
		gun21.reloadSpeed = 0.6f;
		gun21.BulletGravity = 0.5f;
		gun21.expDist = 6f;
		gun21.shootInterval = 0.9f;
		gun21.damage = 0.5f;
		gun21.shootInterval = 1f;
		gun21.bulletsMag = 1;
		gun21.bulletsMax = 6;
		Gun gun22 = this.FindGun("grenade_launcher2");
		gun22.bazuka = true;
		gun22.reloadSpeed = 0.6f;
		gun22.BulletGravity = 0.5f;
		gun22.expDist = 6f;
		gun22.shootInterval = 0.5f;
		gun22.damage = 0.9f;
		gun22.shootInterval = 1f;
		gun22.bulletsMag = 1;
		gun22.bulletsMax = 3;
		Gun gun23 = this.FindGun("health");
		gun23.custom = true;
		gun23.customUseCheck = (() => WorldGameObjectX.Instance.MainPlayerNode.Life < WorldGameObjectX.Instance.MainPlayerNode.MaxLife);
		gun23.customUse = delegate()
		{
			base.StartCoroutine(MainMenu.Instance.BattleWeaponCooldownProcess());
			WorldGameObjectX.Instance.MainPlayerNode.Life = WorldGameObjectX.Instance.MainPlayerNode.MaxLife;
		};
		Gun gun24 = this.FindGun("shield");
		gun24.custom = true;
		gun24.customUseCheck = (() => WorldGameObjectX.Instance.MainPlayerNode.Shield < WorldGameObjectX.Instance.MainPlayerNode.MaxShield);
		gun24.customUse = delegate()
		{
			base.StartCoroutine(MainMenu.Instance.BattleWeaponCooldownProcess());
			WorldGameObjectX.Instance.MainPlayerNode.Shield = WorldGameObjectX.Instance.MainPlayerNode.MaxShield;
		};
		Gun gun25 = this.FindGun("sg550");
		gun25.damage = 0.32f;
		gun25.shootInterval = 0.3f;
		gun25.bulletSpeed = 300f;
		gun25.bulletsMax = 48;
		gun25.bulletsMag = 8;
		Gun gun26 = this.FindGun("sg552");
		gun26.sniper = true;
		gun26.damage = 0.34f;
		gun26.shootInterval = 0.28f;
		gun26.bulletSpeed = 300f;
		gun26.bulletsMax = 48;
		gun26.bulletsMag = 8;
		Gun gun27 = this.FindGun("aug");
		gun27.sniper = true;
		gun27.damage = 0.1f;
		gun27.bulletSpeed = 300f;
		gun27.shootInterval = 0.1f;
		gun27.RecoilUp = 0.02f;
		gun27.RecoilRand = 0.015f;
		foreach (Gun gun28 in this.Guns)
		{
			gun28.Reset();
		}
	}

	private Gun FindGun(string gunName)
	{
		return this.Guns.First((Gun x) => x.name == gunName);
	}

	public string GetPrefix(Animation bodyAnimation, string s)
	{
		string text = s + ((!this.CurGun.pistol) ? string.Empty : "_pi") + ((!this.CurGun.rifle) ? string.Empty : "_rf") + ((!this.Crouch) ? string.Empty : "_cr");
		if (bodyAnimation[text] != null)
		{
			return text;
		}
		if (this.Crouch)
		{
			text = s + ((!this.CurGun.pistol) ? string.Empty : "_pi") + ((!this.CurGun.rifle) ? string.Empty : "_rf");
			if (bodyAnimation[text] != null)
			{
				return text;
			}
		}
		return s;
	}

	public int CurentLife
	{
		get
		{
			return (int)Math.Ceiling((double)(bs._WorldGameObjectX.FindPlayerByNameX(this.PlayerName).Life * 100f));
		}
	}

	public void SetDMGAndArrmor(int dmg, int arrmor)
	{
		bs._WorldGameObjectX.FindPlayerByNameX(this.PlayerName).Shield = (float)arrmor / 100f;
	}

	private void Awake()
	{
		this._CharacterController = base.GetComponent<CharacterController>();
	}

	private void Start()
	{
		this.InitWeapons();
		if (base.photonView.isMine)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<SkinnedMeshRenderer>())
				{
					transform.GetComponent<SkinnedMeshRenderer>().enabled = false;
				}
			}
			base.GetComponent<SkinManager>().DisableAllSkins();
			this.GunModels.gameObject.SetActive(false);
			base.GetComponent<SkinManager>().SetHand(ProfileINI.GetActualSkin());
			this.CharacterModelMain = base.transform.FindChild("characters").gameObject;
			this.HendModelsMain = base.transform.FindChild("recoil").FindChild("Main Camera").FindChild("hands").gameObject;
			if (GameType.IsHungerGamesMode)
			{
				this.SetFirstPerson(true);
			}
		}
		else
		{
			if (WorldGameObjectX.Instance.MainPlayer != null)
			{
				PlayerNode player = WorldGameObjectX.Instance.FindPlayer(base.photonView.owner);
				base.GetComponent<Nickname>().SetPlayer(player);
				int skin = WorldGameObjectX.Instance.FindPlayer(base.photonView.owner).Skin;
				base.GetComponent<SkinManager>().SetSkin(skin);
			}
			UnityEngine.Object.Destroy(base.GetComponent<PlayerInput>());
			UnityEngine.Object.Destroy(base.GetComponent<MouseLook>());
			UnityEngine.Object.Destroy(base.GetComponent<PlayerMotor>());
			UnityEngine.Object.Destroy(base.GetComponent<CameraController>());
			GameObject gameObject = base.gameObject;
			gameObject.name += "Remote";
			foreach (object obj2 in base.transform)
			{
				Transform transform2 = (Transform)obj2;
				if (transform2.name == "recoil")
				{
					UnityEngine.Object.Destroy(transform2.gameObject);
				}
			}
		}
		WorldGameObjectX.Instance.FindPlayer(base.photonView.owner).Avatar = base.gameObject;
		this._VoiceChatPlayer = base.GetComponent<VoiceChatPlayer>();
		base.gameObject.layer = LayerMask.NameToLayer("Players");
		this._SkinManager = base.GetComponentInChildren<SkinManager>();
		if (!base.photonView.isMine)
		{
			base.photonView.RPC("OnPlConnected", base.photonView.owner, new object[]
			{
				PhotonNetwork.player
			});
		}
		else
		{
			this.OnLoaded(ProfileINI.GetActualSkin());
		}
		this._StandartOldPos = this.Standart.localPosition;
		this._StandartOldRot = this.Standart.localRotation;
		if (!GameType.IsHungerGamesMode)
		{
			this.Reset(false);
		}
		for (int i = 0; i < this.Guns.Length; i++)
		{
			this.Guns[i].arrayId = i;
		}
		if (GameType.BattleMode())
		{
			base.gameObject.GetComponent<SkinManager>().DisableAllSkins();
			base.gameObject.GetComponent<Nickname>().Show();
		}
		if (base.photonView.isMine)
		{
			this.SoundFoots();
		}
		if (GameType.IsHungerGamesMode)
		{
			SkinParser.ParseSkins();
			this.SkinControl = base.gameObject.AddComponent<SkinsController>();
			this.SkinControl.Init(this);
			this.HGPlayerNetwork = base.transform.GetComponent<HG_PlayerNetwork>();
			this.HGPlayerNetwork.Init();
		}
		if (GameType.IsArcadeMode && base.photonView.isMine)
		{
			this.Cheat_Detector = base.gameObject.AddComponent<HG_Cheat_Detector>();
			this.Cheat_Detector.Init(WorldGameObjectX.Instance.PlayerList.IndexOf(WorldGameObjectX.Instance.FindPlayerByNameX(this.PlayerName)), 1f, WorldGameObjectX.Instance.FindPlayerByNameX(this.PlayerName).ViewerID);
		}
		if (base.photonView.isMine)
		{
			this.restoreBlockController = base.gameObject.AddComponent<RestoreBlockController>();
		}
		if (GameType.IsHideSeek)
		{
			this.hideSeekPlayerController = base.gameObject.AddComponent<HideSeekPlayerController>();
			this.hideSeekPlayerController.Init(this);
		}
		if (base.photonView.isMine)
		{
			base.StartCoroutine(this.WaitAndCheckName());
		}
	}

	private IEnumerator WaitAndCheckName()
	{
		yield return new WaitForSeconds(4f);
		if (WorldGameObjectX.Instance.PlayerList == null)
		{
			yield break;
		}
		foreach (PlayerNode pNode in WorldGameObjectX.Instance.PlayerList)
		{
			try
			{
				if (pNode != WorldGameObjectX.Instance.MainPlayerNode)
				{
					if (pNode.Name == WorldGameObjectX.Instance.MainPlayerNode.Name || pNode.Name == this.PlayerName)
					{
						WorldGameObjectX.Instance.ExitGame(string.Empty);
						yield break;
					}
				}
			}
			catch (Exception)
			{
				UnityEngine.Debug.Log("Error Check Name");
			}
		}
		yield break;
	}

	[PunRPC]
	public void OnLoaded(int skin)
	{
		this.RefreshNativeWeapon(skin);
		foreach (Gun gun in this.Guns)
		{
			gun.EnableWeapon(false, base.photonView.isMine, this.PlayerName);
		}
		this.Guns[0].EnableWeapon(true, base.photonView.isMine, this.PlayerName);
	}

	private static Transform FindWeapon(GameObject b)
	{
		return bs.GetTransforms(b.transform).FirstOrDefault((Transform a) => new string[]
		{
			"_knife",
			"_sword",
			"_axe",
			"handle",
			"_pick",
			"_bat"
		}.Any((string c) => a.name.EndsWith(c, StringComparison.OrdinalIgnoreCase)));
	}

	public void RefreshNativeWeapon(int skin)
	{
		Transform transform = null;
		if (this._SkinManager.Skins.Length > skin)
		{
			transform = PlayerNetwork.FindWeapon(this._SkinManager.Skins[skin]);
		}
		if (transform != null)
		{
			this.Guns[0].weapon = transform.gameObject;
		}
		if (base.photonView.isMine && !GameType.IsHungerGamesMode)
		{
			transform = this._SkinManager.Hands[skin].transform.GetChild(0);
			if (transform != null)
			{
				this.Guns[0].hands = transform.parent.gameObject;
			}
		}
	}

	public void UpdateSkin(int body_part_id, int skin_id)
	{
		base.photonView.RPC("ChangePlayerSkin", PhotonTargets.All, new object[]
		{
			body_part_id,
			skin_id
		});
	}

	[PunRPC]
	public void ChangePlayerSkin(int body_part_id, int skin_id)
	{
		if (this.SkinControl != null)
		{
			if (!base.photonView.isMine)
			{
				this.SkinControl.UpdateInventorySkin(body_part_id, skin_id);
			}
			else
			{
				this.SkinControl.UpdateInventorySkin(body_part_id, skin_id);
				this.HGPlayerNetwork.SelectSkinHend(body_part_id, skin_id);
			}
		}
	}

	[PunRPC]
	public void OnPlConnected(PhotonPlayer player)
	{
		base.photonView.RPC("OnLoaded", player, new object[]
		{
			ProfileINI.GetActualSkin()
		});
		base.photonView.RPC("EnableWeapon", player, new object[]
		{
			this.CurGunId
		});
	}

	public void SelectWeapon(int id)
	{
		base.photonView.RPC("EnableWeapon", PhotonTargets.All, new object[]
		{
			id
		});
	}

	public void SetWeaponAll(List<int> ids)
	{
		base.photonView.RPC("EnableWeapon", PhotonTargets.All, new object[]
		{
			ids.ToArray()
		});
	}

	[PunRPC]
	public void EnableWeapon(int id)
	{
		if (GameType.IsHungerGamesMode)
		{
			if (this.HGPlayerNetwork != null)
			{
				if (id != -1)
				{
					IS_mdl_Item itemById = IS_Manager.GetItemById(id);
					if (itemById != null && itemById.ItemType != eIS_ItemType.IT_ARROW)
					{
						this.HGPlayerNetwork.SelectWeapon(id, base.photonView.isMine);
						this.CurentSelectedItem = IS_Manager.GetItemById(id);
					}
				}
				else
				{
					this.CurentSelectedItem = null;
					this.HGPlayerNetwork.SelectWeapon(id, base.photonView.isMine);
				}
			}
		}
		else
		{
			this.CurGun.EnableWeapon(false, base.photonView.isMine, this.PlayerName);
			this.CurGunId = id;
			if (id != -1)
			{
				this.Guns[id].EnableWeapon(true, base.photonView.isMine, this.PlayerName);
			}
		}
	}

	[PunRPC]
	public void EnableWeapon(int[] ids)
	{
		if (GameType.IsHungerGamesMode && this.HGPlayerNetwork != null)
		{
			this.HGPlayerNetwork.SelectWeapons(ids, base.photonView.isMine);
		}
	}

	[PunRPC]
	public void Shoot(Vector3 pos, Vector3 dir, float throwSpeed, bool qdamage, int gunid)
	{
		this.LastTimeShoot = Time.time;
		if (GameType.IsHungerGamesMode)
		{
			int num = 0;
			if (this.CurentSelectedItem is IS_mdl_Bow && gunid != -1)
			{
				num = this.CurentSelectedItem.GetItemType<IS_mdl_Bow>().AttackPwr;
				num += IS_Manager.GetItemById(gunid).GetItemType<IS_mdl_Arrow>().AttackBonus;
			}
			this.HGPlayerNetwork.ArrowShoot(pos, dir, throwSpeed, num, gunid);
			return;
		}
		base.PlayOneShot((!(this.CurGun.shootSound == null)) ? this.CurGun.shootSound : bs._Igor.shootSound);
		for (int i = 0; i < this.CurGun.shootBullets; i++)
		{
			Bullet bullet = (Bullet)UnityEngine.Object.Instantiate(this.CurGun.bullet ?? bs._Igor.bullet, pos, Quaternion.LookRotation(dir + this.CurGun.accuracy * UnityEngine.Random.insideUnitSphere));
			bullet.gun = this.Guns[gunid];
			bullet.throwFactorSpeed = throwSpeed;
			bullet.Update2(UnityEngine.Random.Range(0f, 0.05f));
			if (qdamage)
			{
				foreach (Renderer renderer in bullet.GetComponentsInChildren<Renderer>())
				{
					if (renderer.material.shader.name != "Diffuse")
					{
						renderer.material.SetColor("_TintColor", Color.red);
					}
				}
			}
		}
	}

	[PunRPC]
	public void SendAudio(byte[] data, int length, int id)
	{
		if (this._VoiceChatPlayer == null)
		{
			return;
		}
		if (WorldGameObjectX.Instance.MainPlayerNode != null && !WorldGameObjectX.Instance.MainPlayerNode.Voice)
		{
			return;
		}
		if (TeamBattle.Instance != null && !TeamBattle.Instance.EnableVoiceChat)
		{
			return;
		}
		PhotonPlayer photonPlayer = PhotonPlayer.Find(id);
		if (photonPlayer == null)
		{
			return;
		}
		PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayer(photonPlayer);
		if (playerNode == null || !playerNode.Voice)
		{
			return;
		}
		playerNode.LastVoice = DateTime.Now;
		this._VoiceChatPlayer.OnNewSample(new VoiceChatPacket
		{
			Compression = VoiceChatCompression.Speex,
			Data = data,
			Length = length,
			NetworkId = id
		});
	}

	private void SoundFoots()
	{
		if (WorldGameObjectX.Instance.MainPlayer != null && !this.Crouch)
		{
			Vector3 position = WorldGameObjectX.Instance.MainPlayer.transform.position;
			this._CurDownCube = WorldData.Instance.GetBlockType((int)position.x, (int)position.z, (int)(position.y - 0.1f));
			if (this._CurDownCube != BlockType.Air && this._CurDownCube != BlockType.Water && this._CurDownCube != BlockType.Lava && (this._ForcedSound || UnityEngine.Input.GetAxis("Vertical") != 0f || UnityEngine.Input.GetAxis("Horizontal") != 0f))
			{
				this._ForcedSound = false;
				if (!WorldGameObjectX.Instance.MainPlayerDead && !GameType.IsObserving() && !Chat.IsEnabled())
				{
					WorldGameObjectX.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, new object[]
					{
						(int)WorldGameObjectX.Instance.BlockParametrs[(int)this._CurDownCube].FootEffect,
						WorldGameObjectX.Instance.MainPlayer.gameObject.transform.position
					});
					SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)this._CurDownCube].FootEffect, WorldGameObjectX.Instance.MainPlayerEyes.GetComponent<AudioSource>());
				}
			}
		}
		base.Invoke("SoundFoots", 0.35f);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(!WorldGameObjectX.Instance.MainPlayerDead && !this.InTrolley && Holder.Active == null);
			stream.SendNext(this.PlayerCamera.transform.eulerAngles.x);
		}
		else
		{
			this._CorrectPlayerPos = (Vector3)stream.ReceiveNext();
			this._CorrectPlayerRot = (Quaternion)stream.ReceiveNext();
			this._CameraRotEnabled = (bool)stream.ReceiveNext();
			this._CameraRotX = (float)stream.ReceiveNext();
		}
	}

	private void Update()
	{
		if (this._OldTeam != this.PlayerTeam && base.photonView.isMine)
		{
			base.StartCoroutine(this.OnPlayerSelectedTeam());
		}
		this._OldTeam = this.PlayerTeam;
		if (bs.DebugKey(KeyCode.T))
		{
			this.SetTransparent();
		}
		foreach (Transform transform in from a in new Transform[]
		{
			this.MuzzleFlashFirst,
			this.MuzzleFlashThird
		}
		where a != null
		select a)
		{
			transform.GetComponent<Renderer>().enabled = (Time.time - this.LastTimeShoot < 0.05f);
		}
		if (!base.photonView.isMine)
		{
			if (!this.InVehicle)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this._CorrectPlayerPos, Time.deltaTime * 5f);
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this._CorrectPlayerRot, Time.deltaTime * 5f);
		}
		if (base.photonView.isMine && WorldGameObjectX.Instance.MainPlayer != null && this._CurDownCube == BlockType.Air)
		{
			Camera mainPlayerEyes = WorldGameObjectX.Instance.MainPlayerEyes;
			this._CurDownCube = WorldData.Instance.GetBlockType((int)mainPlayerEyes.transform.position.x, (int)mainPlayerEyes.transform.position.z, (int)mainPlayerEyes.transform.position.y - 2);
			if (this._CurDownCube != BlockType.Air)
			{
				base.CancelInvoke();
				this._ForcedSound = true;
				this.SoundFoots();
			}
		}
	}

	private void LateUpdate()
	{
		if (!base.photonView.isMine && this._CameraRotEnabled)
		{
			if (this._CameraRotX < 360f && this._CameraRotX >= 270f)
			{
				this._CameraRotX -= 360f;
			}
			this._CurrentHeadX = Mathf.Lerp(this._CurrentHeadX, -this._CameraRotX / 3f - 90f, Time.deltaTime * 5f);
			if (!this.CurGun.rifle)
			{
				this.SpinBone.transform.rotation = Quaternion.Euler(this.SpinBone.transform.rotation.eulerAngles.x, this.SpinBone.transform.rotation.eulerAngles.y, this._CurrentHeadX + ((!this.Crouch) ? 0f : this.CrouchCamOffset));
			}
			Vector3 eulerAngles = this.HeadBone.transform.root.eulerAngles;
			this.HeadBone.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y - 90f, this._CurrentHeadX);
		}
	}

	[PunRPC]
	public void Die()
	{
		this.Reset(true);
		if (GameType.IsHideSeek)
		{
			this.SetNewteam();
		}
		if (this.HitDir != Vector3.zero && this._KilledByGun != -1)
		{
			foreach (Rigidbody rigidbody in this.Standart.GetComponentsInChildren<Rigidbody>())
			{
				rigidbody.AddForceAtPosition(this.HitDir, this.HitPos);
			}
		}
		if (this.Killer != null && this.Killer.ID == PhotonNetwork.player.ID)
		{
			PlayerNetwork.MultiKill = 1;
			if (!GameType.IsHungerGamesMode)
			{
				if (TeamBattle.Instance != null)
				{
					TeamBattle.Instance.OnDead(this.Killer.name, base.photonView.owner.name, PlayerNetwork.MultiKill);
					bool flag = ProfileINI.GetPurchaseValue(StorePurchase.MORE_EXPERIENCE) != 0;
					int num = 1 * ((!flag) ? 1 : 2);
					if (!GameType.IsArcadeMode && !GameType.IsHideSeek)
					{
						Chat.SendInfoF(string.Concat(new object[]
						{
							"+",
							(float)num / 2f,
							" ",
							Localize.GetText("CHAT_MSG_GOLD", null),
							", +",
							num,
							" ",
							Localize.GetText("CHAT_MSG_EXP", null)
						}), false);
					}
				}
			}
			else if (GameType.IsHungerGamesMode)
			{
				TeamBattle.Instance.OnDead(this.Killer.name, base.photonView.owner.name, PlayerNetwork.MultiKill);
				TeamBattle.Instance.AddKill(this.Killer.name);
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Die333 ",
					this.Killer.ID,
					" ",
					this.Killer.name,
					" -------------"
				}));
			}
		}
		if (base.GetComponent<CameraController>() != null)
		{
			PlayerNetwork.MultiKill = 0;
		}
		string name = bs._WorldGameObjectX.FindPlayer(base.photonView.owner).Name;
		if (this.Killer != null)
		{
			string name2 = bs._WorldGameObjectX.FindPlayer(this.Killer).Name;
			if (TeamBattle.Instance != null && this._KilledByGun != -1)
			{
				KillsList.AddKill(name2, name, this._KilledByGun);
			}
		}
		if (TeamBattle.Instance is HideSeek)
		{
			HideSeek hideSeek = (HideSeek)TeamBattle.Instance;
			hideSeek.SendKillingInfo(name);
		}
		if (GameType.IsHungerGamesMode)
		{
			if (base.photonView.isMine)
			{
				string dropItem = InventaryObjManager.GetDropItem();
				if (dropItem != string.Empty)
				{
					string text = string.Concat(new object[]
					{
						(int)base.transform.position.x,
						",",
						(int)base.transform.position.y,
						",",
						(int)base.transform.position.z
					});
					WorldGameObjectX.Instance.photonView.RPC("BagInst", PhotonTargets.All, new object[]
					{
						-1,
						-1,
						dropItem,
						text
					});
				}
				InventaryObjManager.SetZeroItemInPlayer();
				SkinsManager.ReInit();
				this.HGPlayerNetwork.ReInit();
			}
			else
			{
				this.SkinControl.Init(this);
				if (this.Killer != null && bs._WorldGameObjectX.FindPlayer(base.photonView.owner).Name == this.PlayerName)
				{
					HG_WorkController.kills++;
				}
			}
		}
	}

	public void DropItem(int item_id)
	{
		string text = item_id.ToString();
		string text2 = string.Concat(new object[]
		{
			(int)base.transform.position.x + UnityEngine.Random.Range(-2, 2),
			",",
			(int)base.transform.position.y,
			",",
			(int)base.transform.position.z + UnityEngine.Random.Range(-2, 2)
		});
		WorldGameObjectX.Instance.photonView.RPC("BagInst", PhotonTargets.All, new object[]
		{
			-1,
			-1,
			text,
			text2
		});
	}

	[PunRPC]
	public void HitBy(PhotonPlayer pl, int gunId)
	{
		this.Killer = pl;
		this._KilledByGun = gunId;
	}

	[PunRPC]
	public void OnHit(Vector3 dir, Vector3 pos)
	{
		this.HitDir = dir;
		this.HitPos = pos;
		this.HitTime = Time.time;
		if (GameType.IsHungerGamesMode)
		{
			this.HGPlayerNetwork.PlayeSoundKik();
		}
	}

	public void DisableMuzzleflash()
	{
		foreach (Transform transform in from a in new Transform[]
		{
			this.MuzzleFlashFirst,
			this.MuzzleFlashThird
		}
		where a != null
		select a)
		{
			transform.GetComponent<Renderer>().enabled = false;
		}
	}

	public IEnumerator OnPlayerSelectedTeam()
	{
		if (TeamBattle.Instance != null && TeamBattle.Instance.IsObserving(base.photonView.owner.name))
		{
			this.SelectWeapon(-1);
		}
		yield return null;
		yield break;
	}

	public void Reset(bool die)
	{
		if (base.GetComponent<CameraController>() == null && !die)
		{
			this.SetTransparent();
		}
		this.ResetTime = Time.time;
		foreach (Collider collider in this.Standart.GetComponentsInChildren<Collider>())
		{
			collider.gameObject.layer = LayerMask.NameToLayer("Players");
			collider.enabled = die;
			if (collider.GetComponent<Rigidbody>() != null)
			{
				collider.GetComponent<Rigidbody>().isKinematic = !die;
			}
		}
		if (this.CurGun.weapon != null && die)
		{
			this.DisableMuzzleflash();
			Transform transform = this.CurGun.weapon.transform;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.CurGun.weapon, transform.position, transform.rotation);
			gameObject.transform.localScale = transform.lossyScale;
			gameObject.AddComponent<BoxCollider>();
			gameObject.AddComponent<Rigidbody>();
			gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
			gameObject.transform.parent = null;
			UnityEngine.Object.Destroy(gameObject, 5f);
			this.CurGun.EnableWeapon(false, base.photonView.isMine, this.PlayerName);
		}
		this._CharacterController.enabled = !die;
		base.GetComponent<Animation>().enabled = !die;
		if (!die)
		{
			this.Standart.localPosition = this._StandartOldPos;
			this.Standart.localRotation = this._StandartOldRot;
		}
		if (this.CurGun.weapon != null && !die)
		{
			this.SelectWeapon(this.CurGunId);
		}
	}

	private void SetTransparent()
	{
		if (GameType.IsDeathmatchGamesMode)
		{
			this._isCanDmg = false;
		}
		this._SkinManager.SetTransparent(0.5f);
		base.StartCoroutine(bs.AddMethod(3f, delegate()
		{
			this._SkinManager.SetTransparent(bs.immportalTime);
			this._isCanDmg = true;
			UnityEngine.Debug.Log("SetTransparent END");
		}));
	}

	[PunRPC]
	public void SetCrouch(bool b)
	{
		this.Crouch = b;
	}

	[PunRPC]
	public void SetPos(Vector3 p)
	{
		base.transform.position = p;
	}

	internal void SpawnToSpectator()
	{
		this._CharacterController.enabled = true;
	}

	public void SetFirstPerson(bool firstPerson)
	{
		if (firstPerson)
		{
			this.CharacterModelMain.SetActive(false);
			this.CharacterModelMain.transform.parent.FindChild("Root").gameObject.SetActive(false);
			this.HendModelsMain.SetActive(true);
		}
		else
		{
			this.CharacterModelMain.SetActive(true);
			this.CharacterModelMain.transform.parent.FindChild("Root").gameObject.SetActive(true);
			this.HendModelsMain.SetActive(true);
		}
	}

	[PunRPC]
	public void SetHSStart(int hsid, int hii)
	{
		this.hideSeekPlayerController.StartGame(hii);
	}

	[PunRPC]
	public void IsHGShow(bool ishsshow)
	{
		this.hideSeekPlayerController.RPCShowHide(ishsshow);
	}

	internal void SetNewteam()
	{
		this.hideSeekPlayerController.SetNewTeam();
	}

	public Transform Standart;

	public GameObject PlayerCamera;

	public GameObject HeadBone;

	public GameObject SpinBone;

	public Transform Head;

	internal bool InVehicle;

	internal bool InTrolley;

	private BlockType _CurDownCube;

	private bool _ForcedSound;

	private Vector3 _CorrectPlayerPos = Vector3.zero;

	private Quaternion _CorrectPlayerRot = Quaternion.identity;

	private bool _CameraRotEnabled;

	private float _CameraRotX;

	private float _CurrentHeadX;

	internal Vector3 HitDir = Vector3.zero;

	internal Vector3 HitPos;

	internal float HitTime;

	internal SecuredValue<int> CurGunIdEx = new SecuredValue<int>();

	internal Transform MuzzleFlashFirst;

	internal Transform MuzzleFlashThird;

	internal float LastTimeShoot;

	internal float ResetTime;

	public SkinManager _SkinManager;

	private CharacterController _CharacterController;

	private VoiceChatPlayer _VoiceChatPlayer;

	private Vector3 _StandartOldPos;

	private Quaternion _StandartOldRot;

	private int _KilledByGun = -1;

	private int _OldTeam;

	public Transform RHand;

	public Transform GunModels;

	public Transform FlagPlaceHolder;

	public Gun[] Guns;

	public float CrouchCamOffset;

	internal float QDamageTime = float.MinValue;

	internal PhotonPlayer Killer;

	internal Gun[] MainWeapon = new Gun[3];

	internal bool Crouch;

	public Gun Pistol;

	public Gun Knife;

	public Gun Grenade;

	public Gun Health;

	public Gun Shield;

	public static int MultiKill;

	private GameObject CharacterModelMain;

	private GameObject HendModelsMain;

	private bool _isCanDmg = true;

	public SkinsController SkinControl;

	public GameObject TransformHandR;

	public GameObject TransformHandL;
}
