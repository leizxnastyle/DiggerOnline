using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HG_Bow_Controller : MonoBehaviour
{
	private void Start()
	{
		this.asource = base.gameObject.AddComponent<AudioSource>();
		this.bows = new Dictionary<int, GameObject>();
		for (int i = 0; i < this.bow_id.Count; i++)
		{
			this.bows.Add(this.bow_id[i], this.bow[i]);
			this.bow[i].SetActive(false);
		}
	}

	public bool CanShowBow(int b_id, out bool isLastBow)
	{
		int arrow_id = -1;
		isLastBow = false;
		if (this.last_show_bow_id == b_id && InventaryObjManager.inventary != null)
		{
			this.IsArrowExistBow(InventaryObjManager.inventary.CanShootFromBowMinusArrow(false, ref arrow_id), arrow_id);
			return true;
		}
		if (this.last_show_bow_id != 0)
		{
			base.StartCoroutine(this.ConcealBow());
			isLastBow = true;
		}
		if (this.bows.ContainsKey(b_id) && InventaryObjManager.inventary != null)
		{
			this.TakeBowR(b_id);
			this.IsArrowExistBow(InventaryObjManager.inventary.CanShootFromBowMinusArrow(false, ref arrow_id), arrow_id);
			return true;
		}
		this.last_show_bow_id = 0;
		return false;
	}

	public void CheckBowArrow()
	{
		if (InventaryObjManager.inventary != null)
		{
			int arrow_id = -1;
			this.IsArrowExistBow(InventaryObjManager.inventary.CanShootFromBowMinusArrow(false, ref arrow_id), arrow_id);
		}
	}

	public void HideBow()
	{
		if (this.last_show_bow_id != 0)
		{
			base.StartCoroutine(this.ConcealBow());
			this.arrow[0].SetActive(false);
		}
	}

	public IEnumerator TakeBow(int b_id)
	{
		yield return new WaitForSeconds(0.4f);
		this.bows[b_id].SetActive(true);
		this.bow_animator.SetBool("isShow", true);
		this.last_show_bow_id = b_id;
		this.PlaySound(this.bow_sounds[0], false);
		yield break;
	}

	public void TakeBowR(int b_id)
	{
		this.bows[b_id].SetActive(true);
		this.bow_animator.SetBool("isShow", true);
		this.last_show_bow_id = b_id;
		this.PlaySound(this.bow_sounds[0], false);
	}

	public IEnumerator ConcealBow()
	{
		this.bow_animator.SetBool("isShow", false);
		this.PlaySound(this.bow_sounds[4], false);
		yield return new WaitForSeconds(0.2f);
		if (this.bows.ContainsKey(this.last_show_bow_id))
		{
			this.bows[this.last_show_bow_id].SetActive(false);
		}
		this.last_show_bow_id = 0;
		yield break;
	}

	public void StretchBow()
	{
		this.bow_animator.SetBool("isShoot", false);
		if (this.bow_animator.GetFloat("shootStr") < 1f)
		{
			this.PlaySound(this.bow_sounds[2], false);
			base.StartCoroutine(this.WaitPrepear(this.bow_sounds[2].length - 0.3f));
		}
		this.bow_animator.SetFloat("shootStr", 1f);
	}

	private IEnumerator WaitPrepear(float t)
	{
		yield return new WaitForSeconds(t);
		if (this.bow_animator.GetFloat("shootStr") > 0f)
		{
			this.PlaySound(this.bow_sounds[1], true);
		}
		yield break;
	}

	public void ShootBow()
	{
		this.bow_animator.SetBool("isShoot", true);
		this.bow_animator.SetFloat("shootStr", 0f);
		this.PlaySound(this.bow_sounds[3], false);
		base.StartCoroutine(this.ReloadBow());
	}

	public IEnumerator ReloadBow()
	{
		this.isCanShoot = false;
		yield return new WaitForSeconds(1f);
		this.isCanShoot = true;
		yield break;
	}

	public void IsArrowExistBow(bool arrowExist, int arrow_id)
	{
		if (this.last_show_bow_id == 0 && this.arrow.Count > 0 && this.arrow[0] != null)
		{
			this.arrow[0].SetActive(false);
			return;
		}
		try
		{
			this.bow_animator.SetBool("arrowExist", arrowExist);
			if (arrowExist)
			{
				this.arrow[this.GetArrowIndexFromId(arrow_id)].SetActive(true);
			}
			else
			{
				this.arrow[this.GetArrowIndexFromId(arrow_id)].SetActive(false);
			}
		}
		catch
		{
			UnityEngine.Debug.Log("arrowExist ERROR");
		}
	}

	public bool CanShoot()
	{
		return this.isCanShoot && this.last_show_bow_id != 0;
	}

	private int GetArrowIndexFromId(int id)
	{
		if (id == 200)
		{
			return 0;
		}
		if (id == 201)
		{
			return 1;
		}
		if (id == 202)
		{
			return 2;
		}
		if (id == 203)
		{
			return 3;
		}
		return 0;
	}

	private void PlaySound(AudioClip clip, bool isLoop = false)
	{
		this.asource.Stop();
		this.asource.clip = clip;
		this.asource.loop = isLoop;
		this.asource.volume = ProfileINI.sound_volume;
		this.asource.Play();
	}

	public List<int> bow_id;

	public List<GameObject> bow;

	public List<GameObject> arrow;

	public Animator bow_animator;

	private Dictionary<int, GameObject> bows;

	public int last_show_bow_id;

	public bool isCanShoot = true;

	public List<AudioClip> bow_sounds;

	private AudioSource asource;

	public enum BOW_SOUNDS
	{
		BS_GET,
		BS_PREPARE_LOOP,
		BS_PREPARE,
		BS_SHOOT,
		BS_PUT
	}
}
