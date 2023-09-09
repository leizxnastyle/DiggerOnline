using System;
using System.Collections;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
	public Animation Anim
	{
		get
		{
			return this._Anim;
		}
	}

	public int CurSkin
	{
		get
		{
			return this._CurSkin;
		}
	}

	public int _CurHend { get; private set; }

	private void Awake()
	{
		this.UpdateTextures();
		this.DisableAllSkins();
		this.DisableAllHands();
		if (SkinManager._TransparentShader == null)
		{
			SkinManager._TransparentShader = Shader.Find("Transparent/VertexLit with Z");
		}
		TimeOfDay.Affect(base.gameObject);
	}

	public void DisableAllSkins()
	{
		for (int i = 0; i < this.Skins.Length; i++)
		{
			this.Skins[i].SetActive(false);
		}
		this._CurSkin = -1;
	}

	public void DisableAllHands()
	{
		if (!GameType.IsHungerGamesMode)
		{
			for (int i = 0; i < this.Hands.Length; i++)
			{
				this.Hands[i].SetActive(false);
			}
		}
	}

	public void SetHand(int ind)
	{
		this._CurHend = ind;
		this._Anim = base.GetComponent<Animation>();
		for (int i = 0; i < this.Hands.Length; i++)
		{
			if (i == ind)
			{
				base.StartCoroutine(ContentUpdater.UpdateTextures(this.Hands[i].transform, ContentUpdater.CharacterBundle));
				if (this.Hands[i].GetComponent<Animation>() != null)
				{
					this._Anim = this.Hands[i].GetComponent<Animation>();
				}
				this.Hands[i].SetActive(true);
			}
			else if (!GameType.IsHungerGamesMode)
			{
				this.Hands[i].SetActive(false);
			}
		}
	}

	public void SetSkin(int ind)
	{
		this._Anim = base.GetComponent<Animation>();
		this._CurSkin = ind;
		if (!GameType.IsHungerGamesMode)
		{
			this.DisableAllSkins();
			for (int i = 0; i < this.Skins.Length; i++)
			{
				if (i == ind)
				{
					if (this.isAnimal)
					{
						base.StartCoroutine(ContentUpdater.UpdateTextures(this.Skins[i].transform, ContentUpdater.EntitiesBundle));
					}
					else
					{
						base.StartCoroutine(ContentUpdater.UpdateTextures(this.Skins[i].transform, ContentUpdater.CharacterBundle));
					}
					this.Skins[i].SetActive(true);
					if (this.Skins[i].GetComponent<Animation>() != null)
					{
						this._Anim = this.Skins[i].GetComponent<Animation>();
					}
				}
			}
		}
	}

	public void ShowSkin()
	{
		this.Skins[this._CurSkin].SetActive(true);
	}

	public void HideSkin()
	{
		UnityEngine.Debug.Log(this._CurSkin);
		if (this._CurSkin == -1)
		{
			this._CurSkin = 0;
		}
		try
		{
			if (this.Skins != null)
			{
				this.Skins[this._CurSkin].SetActive(false);
			}
		}
		catch (Exception message)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public void SetTransparent(float alpha)
	{
		alpha = Mathf.Clamp01(alpha);
		if ((this._CurAlpha < 1f && alpha == 1f) || (this._CurAlpha == 1f && alpha < 1f))
		{
			MaterialExt.SetShader(base.gameObject, (alpha >= 1f) ? null : SkinManager._TransparentShader);
		}
		this._CurAlpha = alpha;
		MaterialExt.SetTransparent(base.gameObject, this._CurAlpha);
	}

	public void AffectColor(float r, float g, float b)
	{
		MaterialExt.AffectColor(base.gameObject, r, g, b);
	}

	public void Redness()
	{
		base.StopCoroutine("RednessProcess");
		this.AffectColor(0f, 0f, 0f);
		base.StartCoroutine("RednessProcess");
	}

	private IEnumerator RednessProcess()
	{
		float duration = 0.25f;
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			this.AffectColor((Time.time - startTime) / duration, 0f, 0f);
			yield return new WaitForSeconds(0.05f);
		}
		startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			this.AffectColor(1f - (Time.time - startTime) / duration, 0f, 0f);
			yield return new WaitForSeconds(0.05f);
		}
		this.AffectColor(0f, 0f, 0f);
		yield break;
	}

	public void UpdateTextures()
	{
		this.SetSkin(this._CurSkin);
		this.SetHand(this._CurSkin);
		Utils.UpdateTextures(base.transform.Find("recoil/Main Camera/anims/anims"), ContentUpdater.WeaponTextures);
		Utils.UpdateTextures(base.transform.Find("Root/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/gunModels"), ContentUpdater.WeaponTextures);
	}

	public static string[] SkinNames = new string[]
	{
		"Digger",
		"Digger_Hand",
		"Pirate",
		"Pirate_Hands",
		"Kinght",
		"Knight_Hands",
		"EpicKnight",
		"EpicKnight_Hands",
		"Pirate2",
		"Pirate2_Hands",
		"Viking",
		"Viking_Hands",
		"Zombie",
		"Zombie_Hands",
		"Death_Knight",
		"DeathKnight_Hands",
		"Soldier02",
		"Soldier_Hands",
		"Soviet",
		"Soviet_Hands",
		"Swat",
		"Swat_Hands",
		"Soldier03",
		"Soviet_Hands",
		"German_officer",
		"German_officer_Hands",
		"Knight01",
		"Knight_Hand",
		"Skeleton",
		"Skelet_Hand",
		"IronMan",
		"Ironman_Hands",
		"Batman",
		"Batman_Hands",
		"Digger_girl01",
		"DiggerGirl01_Hands",
		"Digger_girl02",
		"DiggerGirl02_Hands",
		"Archer",
		"Archer_Hands",
		" Terrorist",
		"Terrorist_Hands",
		"Cook",
		"Cook_Hands",
		"DarkStalker",
		"DarkStalker_Hands",
		"mercenary",
		"Mercenary_Hands",
		"Smelter",
		"Smelter_Hands",
		"Root",
		"Standard",
		"German",
		"German_Hands",
		"Soldier",
		"Standard",
		"Standard",
		"Standard",
		"Santa",
		"Santa_Hands",
		"GirlPoliceman",
		"GirlPoliceman_Hands"
	};

	public GameObject[] Skins;

	public GameObject[] Hands;

	public bool isAnimal;

	private static Shader _TransparentShader = null;

	private float _CurAlpha = 1f;

	private Animation _Anim;

	private int _CurSkin = -1;
}
