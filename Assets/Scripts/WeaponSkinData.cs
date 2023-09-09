using System;
using System.Collections.Generic;
using System.Linq;

public class WeaponSkinData
{
	public WeaponSkinData()
	{
		this._selectedWeaponSkins = new Dictionary<WeaponSkinType, int>
		{
			{
				WeaponSkinType.ak47,
				0
			},
			{
				WeaponSkinType.glock,
				0
			},
			{
				WeaponSkinType.m16,
				0
			},
			{
				WeaponSkinType.m1014,
				0
			},
			{
				WeaponSkinType.sg552,
				0
			},
			{
				WeaponSkinType.aug,
				0
			},
			{
				WeaponSkinType.glauncher,
				0
			},
			{
				WeaponSkinType.mp5,
				0
			},
			{
				WeaponSkinType.swd,
				0
			},
			{
				WeaponSkinType.sg550,
				0
			},
			{
				WeaponSkinType.sawn_off,
				0
			}
		};
	}

	public void Load(string data)
	{
		string[] array = data.Split(new char[]
		{
			';'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (this._selectedWeaponSkins.ContainsKey((WeaponSkinType)i))
			{
				this._selectedWeaponSkins[(WeaponSkinType)i] = int.Parse(array[i]);
			}
		}
	}

	public string Save()
	{
		return string.Join(";", (from x in this._selectedWeaponSkins.Values.ToList<int>()
		select x.ToString()).ToArray<string>());
	}

	public void SelectSkin(WeaponSkinType skinType, int skinId)
	{
		if (this._selectedWeaponSkins.ContainsKey(skinType))
		{
			this._selectedWeaponSkins[skinType] = skinId;
		}
	}

	public int GetSelectedSkin(WeaponSkinType skinType)
	{
		return (!this._selectedWeaponSkins.ContainsKey(skinType)) ? 0 : this._selectedWeaponSkins[skinType];
	}

	private readonly Dictionary<WeaponSkinType, int> _selectedWeaponSkins;
}
