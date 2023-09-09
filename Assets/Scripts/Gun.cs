using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gun : bs
{
	internal int bullets
	{
		get
		{
			return this.m_bullets - Gun.rand;
		}
		set
		{
			this.m_bullets = value + Gun.rand;
			KGUI.SetControlText("hud.battle.anchor_bottom_right.Bullet.Text", value + "/" + this.bulletsLeft);
		}
	}

	internal int bulletsLeft
	{
		get
		{
			return this.bulletsLeftEx;
		}
		set
		{
			this.bulletsLeftEx = value;
			KGUI.SetControlText("hud.battle.anchor_bottom_right.Bullet.Text", this.bullets + "/" + value);
		}
	}

	internal float RecoilUp
	{
		get
		{
			return base.Getfloat("recup", 0.03f);
		}
		set
		{
			base.Setfloat("recup", value);
		}
	}

	internal float RecoilRand
	{
		get
		{
			return base.Getfloat("recRand", 0.015f);
		}
		set
		{
			base.Setfloat("recRand", value);
		}
	}

	internal float RecoilRecover
	{
		get
		{
			return base.Getfloat("recRec", 3f);
		}
		set
		{
			base.Setfloat("recRec", value);
		}
	}

	internal bool isCooldown
	{
		get
		{
			return ProfileINI.GetPurchaseCooldown(this.storePurchaseCount) > 0f;
		}
	}

	private void Awake()
	{
		this.playerCC = base.transform.root.GetComponent<CameraController>();
		this.pl = base.transform.root.GetComponent<PlayerNetwork>();
		if (this.hands != null)
		{
			this.handsPos = this.hands.transform.localPosition;
		}
		this.Reset();
	}

	public void Reset()
	{
		if (this.storePurchaseCount != StorePurchase.NONE)
		{
			this.bulletsMag = ProfileINI.GetPurchaseValue(this.storePurchaseCount);
		}
		this.bullets = this.bulletsMag;
		this.bulletsLeft = this.bulletsMax;
	}

	public void EnableWeapon(bool enable, bool isLocalePlayer, string playerName)
	{
		if (enable)
		{
			if (this.weapon != null)
			{
				this.muzzleFlashThird = (this.pl.MuzzleFlashThird = this.weapon.transform.Find("muzzleFlash"));
			}
			if (this.hands != null)
			{
				this.muzzleFlashFirst = (this.pl.MuzzleFlashFirst = this.hands.transform.Find("muzzleFlash"));
			}
		}
		this.DisableMuzzleFlash();
		foreach (GameObject gameObject in from a in new GameObject[]
		{
			this.hands,
			this.weapon
		}
		where a != null
		select a)
		{
			foreach (Transform transform in bs.GetTransforms(gameObject.transform).Where((Transform a) => a.GetComponent<Renderer>() != null))
			{
				transform.GetComponent<Renderer>().enabled = enable;
				if (enable && this.WeaponSkin != null && this.WeaponSkin.Count > 0 && !transform.gameObject.name.Contains("muzzleFlash"))
				{
					if (transform.GetComponent<Renderer>().material.mainTexture.name == this.WeaponSkin[0].name)
					{
						this.WeaponSkin[0] = transform.GetComponent<Renderer>().material.mainTexture;
					}
					if (isLocalePlayer)
					{
						transform.GetComponent<Renderer>().material.mainTexture = this.WeaponSkin[ProfileINI.WeaponSkinData.GetSelectedSkin(this.WeaponId)];
					}
					else
					{
						transform.GetComponent<Renderer>().material.mainTexture = this.WeaponSkin[WorldGameObjectX.Instance.FindPlayerByNameX(playerName).WeaponSkin[(int)this.WeaponId]];
					}
				}
			}
		}
		this.DisableMuzzleFlash();
		this.pl.DisableMuzzleflash();
	}

	public void DisableMuzzleFlash()
	{
		if (this.muzzleFlashFirst != null)
		{
			this.muzzleFlashFirst.GetComponent<Renderer>().enabled = false;
		}
		if (this.muzzleFlashThird != null)
		{
			this.muzzleFlashThird.GetComponent<Renderer>().enabled = false;
		}
	}

	public StorePurchase storePurchase = StorePurchase.NONE;

	public StorePurchase storePurchaseCount = StorePurchase.NONE;

	public Texture2D bulletIcon;

	public bool pistol;

	public GameObject hands;

	public GameObject weapon;

	public Bullet bullet;

	public bool melee;

	public bool rifle;

	public int ragdollForce = 1;

	public AudioClip shootSound;

	public AudioClip reloadSound;

	public float explosionTime = 10f;

	public Vector3 GrenadeThrowForce;

	public WeaponSkinType WeaponId;

	public List<Texture> WeaponSkin;

	internal int arrayId;

	internal Vector3 handsPos;

	internal CameraController playerCC;

	internal PlayerNetwork pl;

	private static int rand = new System.Random().Next(-1000, 1000);

	internal int m_bullets = Gun.rand;

	internal SecuredValue<float> shootInterval = 0.1f;

	internal SecuredValue<int> bulletsMax = 90;

	internal SecuredValue<int> bulletsLeftEx = 0;

	internal SecuredValue<int> bulletsMag = 30;

	internal SecuredValue<float> CrouchRecoilFactor = 0.5f;

	internal SecuredValue<float> accuracy = 0f;

	internal SecuredValue<int> shootBullets = 1;

	internal SecuredValue<float> damage = 0.1f;

	internal SecuredValue<float> bulletSpeed = 100f;

	internal SecuredValue<float> reloadSpeed = 1f;

	internal SecuredValue<float> HeadHit = 1.5f;

	internal SecuredValue<float> BulletGravity = 0f;

	internal SecuredValue<bool> sniper = false;

	internal SecuredValue<bool> bazuka = false;

	internal SecuredValue<bool> grenade = false;

	internal SecuredValue<float> expDist = 5f;

	internal SecuredValue<bool> custom = false;

	internal Func<bool> customUseCheck;

	internal Action customUse;

	private Transform muzzleFlashThird;

	private Transform muzzleFlashFirst;
}
