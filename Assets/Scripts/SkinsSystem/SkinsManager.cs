using System;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

namespace SkinsSystem
{
	public class SkinsManager : MonoBehaviour
	{
		internal static Dictionary<int, SS_mdl_Skin> server_skins
		{
			get
			{
				return SkinParser.temp_skins;
			}
		}

		public static void Init()
		{
			SkinsManager.curent_skin = new Dictionary<eIS_Arrmor_SUBT, SS_mdl_Body_Path>();
			if (SkinsManager.server_skins.Count == 0)
			{
				SkinParser.ParseSkins();
			}
			SkinsManager.curent_skin.Add(eIS_Arrmor_SUBT.AS_HEAD, null);
			SkinsManager.curent_skin.Add(eIS_Arrmor_SUBT.AS_BODY, null);
			SkinsManager.curent_skin.Add(eIS_Arrmor_SUBT.AS_LEGS, null);
			SkinsManager.UpdateSkin(0, eIS_Arrmor_SUBT.AS_HEAD);
			SkinsManager.UpdateSkin(0, eIS_Arrmor_SUBT.AS_BODY);
			SkinsManager.UpdateSkin(0, eIS_Arrmor_SUBT.AS_LEGS);
		}

		public static void ReInit()
		{
			if (SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_HEAD] != null)
			{
				SkinsManager.HideObj(SkinsManager.server_skins[SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_HEAD].skin_id].skin_name, SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_HEAD].bp_part_Name);
			}
			if (SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_BODY] != null)
			{
				SkinsManager.HideObj(SkinsManager.server_skins[SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_BODY].skin_id].skin_name, SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_BODY].bp_part_Name);
			}
			if (SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_LEGS] != null)
			{
				SkinsManager.HideObj(SkinsManager.server_skins[SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_LEGS].skin_id].skin_name, SkinsManager.curent_skin[eIS_Arrmor_SUBT.AS_LEGS].bp_part_Name);
			}
			SkinsManager.Init();
		}

		private void Awake()
		{
			SkinsManager.player_mdl = base.transform.FindChild("characters").gameObject;
			SkinsManager.Init();
		}

		public static void ShowMdl()
		{
			SkinsManager.player_mdl.SetActive(true);
		}

		public static void HideMdl()
		{
			SkinsManager.player_mdl.SetActive(false);
		}

		public static void UpdateSkin(int skin_id, eIS_Arrmor_SUBT skin_part)
		{
			if (skin_id != -1)
			{
				if (SkinsManager.curent_skin[skin_part] != null)
				{
					SkinsManager.HideObj(SkinsManager.server_skins[SkinsManager.curent_skin[skin_part].skin_id].skin_name, SkinsManager.curent_skin[skin_part].bp_part_Name);
				}
				SS_mdl_Body_Path part = SkinsManager.server_skins[skin_id].GetPart(skin_part);
				SkinsManager.ShowObj(SkinsManager.server_skins[skin_id].skin_name, part.bp_part_Name);
				SkinsManager.curent_skin[skin_part] = part;
				HG_WorkController.UpdateSkin((int)skin_part, skin_id);
			}
		}

		private static void HideObj(string mdl_name, List<string> part_name)
		{
			GameObject gameObject = SkinsManager.player_mdl.transform.FindChild(mdl_name).gameObject;
			foreach (string name in part_name)
			{
				GameObject gameObject2 = gameObject.transform.FindChild(name).gameObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
		}

		private static void ShowObj(string mdl_name, List<string> part_name)
		{
			GameObject gameObject = SkinsManager.player_mdl.transform.FindChild(mdl_name).gameObject;
			App.Instance.StartCoroutine(ContentUpdater.UpdateTextures(gameObject.transform, ContentUpdater.CharacterBundle));
			foreach (string name in part_name)
			{
				GameObject gameObject2 = gameObject.transform.FindChild(name).gameObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(true);
				}
			}
		}

		public void DeactivateAll(GameObject obj)
		{
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				transform.gameObject.SetActive(false);
				this.DeactivateAll(transform.gameObject);
			}
		}

		internal static Dictionary<eIS_Arrmor_SUBT, SS_mdl_Body_Path> curent_skin = new Dictionary<eIS_Arrmor_SUBT, SS_mdl_Body_Path>();

		public static GameObject player_mdl;
	}
}
