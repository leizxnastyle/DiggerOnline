using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinDataManager : MonoBehaviour
{
	private void Start()
	{
		SkinDataManager._gameSkins = new Dictionary<WeaponSkinType, List<Texture>>
		{
			{
				WeaponSkinType.ak47,
				this.Ak47SkinsList
			},
			{
				WeaponSkinType.glock,
				this.GlockSkinsList
			},
			{
				WeaponSkinType.m16,
				this.M16SkinsList
			},
			{
				WeaponSkinType.m1014,
				this.M1014SkinsList
			},
			{
				WeaponSkinType.sg552,
				this.Sg552SkinsList
			},
			{
				WeaponSkinType.aug,
				this.AugSkinsList
			},
			{
				WeaponSkinType.glauncher,
				this.GlauncherSkinsList
			},
			{
				WeaponSkinType.mp5,
				this.Mp5SkinsList
			},
			{
				WeaponSkinType.swd,
				this.SwdSkinsList
			},
			{
				WeaponSkinType.sg550,
				this.Sg550SkinsList
			},
			{
				WeaponSkinType.sawn_off,
				this.SawnOffSkinsList
			}
		};
	}

	public static Texture GetCurentSkin(WeaponSkinType skinType, int skinId)
	{
		return SkinDataManager._gameSkins[skinType][skinId];
	}

	public List<Texture> Ak47SkinsList;

	public List<Texture> GlockSkinsList;

	public List<Texture> M16SkinsList;

	public List<Texture> M1014SkinsList;

	public List<Texture> Sg552SkinsList;

	public List<Texture> AugSkinsList;

	public List<Texture> GlauncherSkinsList;

	public List<Texture> Mp5SkinsList;

	public List<Texture> SwdSkinsList;

	public List<Texture> Sg550SkinsList;

	public List<Texture> SawnOffSkinsList;

	private static Dictionary<WeaponSkinType, List<Texture>> _gameSkins;
}
