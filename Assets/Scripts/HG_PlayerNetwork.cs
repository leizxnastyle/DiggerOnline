using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class HG_PlayerNetwork : MonoBehaviour
{
	public void Init()
	{
		this.last_items_inst = new List<GameObject>();
		this.last_active_hend = this.Hends[0];
		Transform parent = this.Hends[0].transform.parent.parent;
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			child.gameObject.SetActive(true);
			for (int j = 0; j < child.transform.childCount; j++)
			{
				child.GetChild(j).gameObject.SetActive(false);
			}
		}
		this.Hends[0].SetActive(true);
		base.StartCoroutine(ContentUpdater.UpdateTextures(this.Hends[0].transform, ContentUpdater.CharacterBundle));
		this.bow_arrow = (GameObject)Resources.Load("DWork/Prefabs/Weapons/Arrow", typeof(GameObject));
	}

	public void ReInit()
	{
		this.DestroyWeaponToMdl(this.last_active_hend);
		this.Init();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "arrow" && other.GetComponent<HG_Arrow_Controller>().CanDMG && base.transform.GetComponent<PlayerNetwork>().PlayerName != other.GetComponent<HG_Arrow_Controller>().sended_name && HG_WorkController._player.PlayerName == other.GetComponent<HG_Arrow_Controller>().sended_name)
		{
			WorldGameObjectX.Instance.ArrowPunchPlayer(base.transform.GetComponent<PhotonView>().owner, Vector3.zero, other.GetComponent<HG_Arrow_Controller>().GetDMG(), -1, base.gameObject, other.GetComponent<HG_Arrow_Controller>().sended_name);
			this.PlayeSoundKik();
		}
	}

	public void PlayeSoundKik()
	{
		AudioSource.PlayClipAtPoint(this.punch_sounds[UnityEngine.Random.Range(0, this.punch_sounds.Count)], base.transform.position, ProfileINI.sound_volume * ProfileINI.sound_scale);
	}

	public void SelectWeapon(int curGunID, bool isMine)
	{
		if (isMine)
		{
			if (curGunID > 0)
			{
				bool flag = false;
				if (this.bow_controller.CanShowBow(curGunID, out flag))
				{
					this.hend_weapon_point.transform.parent.parent.parent.parent.parent.parent.gameObject.SetActive(false);
					this.DestroyWeaponToMdl(this.last_inst_hend_model);
					this.InstanceWeaponToMdl(IS_Manager.GetItemById(curGunID).GoPath, this.weapon_point, ref this.last_inst_model);
				}
				else if (flag)
				{
					base.StartCoroutine(this.WaitAndShow(curGunID, 0.4f));
				}
				else
				{
					base.StartCoroutine(this.WaitAndShow(curGunID, 0f));
				}
			}
			else
			{
				this.bow_controller.HideBow();
				this.hend_weapon_point.transform.parent.parent.parent.parent.parent.parent.gameObject.SetActive(true);
				this.hend_weapon_point.transform.parent.parent.parent.parent.gameObject.SetActive(true);
				this.DestroyWeaponToMdl(this.last_inst_hend_model);
				this.DestroyWeaponToMdl(this.last_inst_model);
			}
		}
		else if (curGunID > 0)
		{
			this.InstanceWeaponToMdl(IS_Manager.GetItemById(curGunID).GoPath, this.weapon_point, ref this.last_inst_model);
		}
		else
		{
			this.DestroyWeaponToMdl(this.last_inst_model);
		}
	}

	private IEnumerator WaitAndShow(int curGunID, float wait_time)
	{
		yield return new WaitForSeconds(wait_time);
		this.hend_weapon_point.transform.parent.parent.parent.parent.parent.parent.gameObject.SetActive(true);
		this.hend_weapon_point.transform.parent.parent.parent.parent.gameObject.SetActive(true);
		string pp = IS_Manager.GetItemById(curGunID).GoPath;
		pp = pp.Insert(pp.LastIndexOf('/') + 1, "h");
		this.InstanceWeaponToMdl(pp, this.hend_weapon_point, ref this.last_inst_hend_model);
		this.InstanceWeaponToMdl(IS_Manager.GetItemById(curGunID).GoPath, this.weapon_point, ref this.last_inst_model);
		yield break;
	}

	public void SelectSkinHend(int body_part_id, int skin_id)
	{
		if (body_part_id == 1 && this.last_active_hend != this.Hends[skin_id])
		{
			this.last_active_hend.SetActive(false);
			this.Hends[skin_id].SetActive(true);
			this.last_active_hend = this.Hends[skin_id];
			base.StartCoroutine(ContentUpdater.UpdateTextures(this.Hends[skin_id].transform, ContentUpdater.CharacterBundle));
		}
	}

	private void DestroyWeaponToMdl(GameObject last)
	{
		if (last != null)
		{
			UnityEngine.Object.Destroy(last);
		}
	}

	private void InstanceWeaponToMdl(string preff_path, GameObject to, ref GameObject last)
	{
		GameObject x = (GameObject)Resources.Load(preff_path, typeof(GameObject));
		if (x != null)
		{
			if (last != null)
			{
				UnityEngine.Object.Destroy(last);
			}
			last = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(preff_path, typeof(GameObject)));
			base.StartCoroutine(ContentUpdater.UpdateTextures(last.transform, ContentUpdater.CharacterBundle));
			Vector3 localPosition = last.transform.localPosition;
			Vector3 localScale = last.transform.localScale;
			Quaternion localRotation = last.transform.localRotation;
			last.transform.parent = to.transform;
			last.transform.localPosition = localPosition;
			last.transform.localScale = localScale;
			last.transform.localRotation = localRotation;
		}
	}

	public void ArrowShoot(Vector3 pos, Vector3 dir, float throwSpeed, int dmg, int arrow_id)
	{
		if (throwSpeed < 1f)
		{
			throwSpeed = 1f;
		}
		else if (throwSpeed > 3f)
		{
			throwSpeed = 3f;
		}
		this.arrow_point[1].transform.rotation = Quaternion.LookRotation(dir);
		GameObject gameObject = new GameObject();
		if (this.arrow_point[0] != null)
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(this.bow_arrow, pos, this.arrow_point[1].transform.rotation);
		}
		else
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(this.bow_arrow, pos, this.arrow_point[1].transform.rotation);
		}
		gameObject.GetComponent<HG_Arrow_Controller>().Shoot(throwSpeed, base.transform.GetComponent<PlayerNetwork>().PlayerName, (!(this.arrow_point[0] != null)) ? 0 : dmg, arrow_id);
	}

	public void SelectWeapons(int[] ids, bool isMine)
	{
		if (this.last_items_inst.Count > 0)
		{
			foreach (GameObject obj in this.last_items_inst)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
		this.last_items_inst = new List<GameObject>();
		GameObject item = new GameObject();
		foreach (int item_id in ids)
		{
			if (isMine || IS_Manager.GetItemById(item_id).ItemType == eIS_ItemType.IT_ARROW)
			{
				this.bow_controller.CheckBowArrow();
				this.InstanceWeaponToMdl(IS_Manager.GetItemById(item_id).GoPath, this.quiver_point, ref item);
				this.last_items_inst.Add(item);
			}
		}
	}

	public int curent_selected_weapon;

	public List<GameObject> Hends;

	private GameObject last_inst_hend_model;

	private GameObject last_inst_model;

	private GameObject last_active_hend;

	private GameObject bow_arrow;

	private List<GameObject> last_items_inst;

	public GameObject hend_weapon_point;

	public GameObject weapon_point;

	public GameObject quiver_point;

	public GameObject[] arrow_point;

	public HG_Bow_Controller bow_controller;

	public GameObject hends;

	public List<AudioClip> punch_sounds;
}
