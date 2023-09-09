using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

namespace SkinsSystem
{
	public class SkinsController : MonoBehaviour
	{
		private void Disable()
		{
			this._player.isSkinUpdate -= this.HandleisSkinUpdate;
		}

		public void Init(PlayerNetwork p)
		{
			this._current_skin = new Dictionary<eIS_Arrmor_SUBT, SS_mdl_Body_Path>();
			this._current_skin.Add(eIS_Arrmor_SUBT.AS_HEAD, SkinsManager.server_skins[0].GetPart(eIS_Arrmor_SUBT.AS_HEAD));
			this._current_skin.Add(eIS_Arrmor_SUBT.AS_BODY, SkinsManager.server_skins[0].GetPart(eIS_Arrmor_SUBT.AS_BODY));
			this._current_skin.Add(eIS_Arrmor_SUBT.AS_LEGS, SkinsManager.server_skins[0].GetPart(eIS_Arrmor_SUBT.AS_LEGS));
			this._player = p;
			this._player.isSkinUpdate += this.HandleisSkinUpdate;
			this.player_mdl = base.transform.FindChild("characters").gameObject;
			if (!this._player.photonView.isMine)
			{
				base.StartCoroutine(this.ShowStandartSkin());
			}
		}

		private IEnumerator ShowStandartSkin()
		{
			yield return new WaitForSeconds(1f);
			this.DeactivateAll(this.player_mdl);
			this.UpdateInventorySkin(0, 0);
			this.UpdateInventorySkin(1, 0);
			this.UpdateInventorySkin(2, 0);
			yield break;
		}

		private void HandleisSkinUpdate(int body_part_id, int skin_id)
		{
			this.UpdateSkin(skin_id, (eIS_Arrmor_SUBT)body_part_id);
		}

		public void UpdateInventorySkin(int body_part_id, int skin_id)
		{
			this.UpdateSkin(skin_id, (eIS_Arrmor_SUBT)body_part_id);
		}

		public void UpdateSkin(int skin_id, eIS_Arrmor_SUBT skin_part)
		{
			if (skin_id != -1)
			{
				if (this._current_skin[skin_part] != null)
				{
					this.HideObj(SkinsManager.server_skins[this._current_skin[skin_part].skin_id].skin_name, this._current_skin[skin_part].bp_part_Name);
				}
				SS_mdl_Body_Path part = SkinsManager.server_skins[skin_id].GetPart(skin_part);
				this.ShowObj(SkinsManager.server_skins[skin_id].skin_name, part.bp_part_Name);
				this._current_skin[skin_part] = part;
			}
		}

		private void HideObj(string mdl_name, List<string> part_name)
		{
			GameObject gameObject = this.player_mdl.transform.FindChild(mdl_name).gameObject;
			foreach (string name in part_name)
			{
				GameObject gameObject2 = gameObject.transform.FindChild(name).gameObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			if (gameObject.activeSelf && !this.GetActive(gameObject))
			{
				gameObject.SetActive(false);
			}
		}

		private void ShowObj(string mdl_name, List<string> part_name)
		{
			GameObject gameObject = this.player_mdl.transform.FindChild(mdl_name).gameObject;
			base.StartCoroutine(ContentUpdater.UpdateTextures(gameObject.transform, ContentUpdater.CharacterBundle));
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
				this.DeactivateChildren(gameObject, false);
			}
			foreach (string name in part_name)
			{
				GameObject gameObject2 = gameObject.transform.FindChild(name).gameObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(true);
				}
			}
		}

		private bool GetActive(GameObject go)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				if (go.transform.GetChild(i).gameObject.activeSelf)
				{
					return true;
				}
			}
			return false;
		}

		private void DeactivateChildren(GameObject g, bool a)
		{
			foreach (object obj in g.transform)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.SetActive(a);
			}
		}

		public int GetArrmorCount()
		{
			int num = 0;
			foreach (SS_mdl_Body_Path ss_mdl_Body_Path in this._current_skin.Values)
			{
				IS_mdl_Armor is_mdl_Armor = (IS_mdl_Armor)IS_Manager.GetItemById(ss_mdl_Body_Path.item_id);
				num += is_mdl_Armor.Defense;
			}
			return num;
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

		private PlayerNetwork _player;

		private GameObject player_mdl;

		private Dictionary<eIS_Arrmor_SUBT, SS_mdl_Body_Path> _current_skin = new Dictionary<eIS_Arrmor_SUBT, SS_mdl_Body_Path>();
	}
}
