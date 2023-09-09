using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HG_Arrow_Controller : MonoBehaviour
{
	public bool CanDMG
	{
		get
		{
			return this.canDmg;
		}
	}

	private void Awake()
	{
		this.indexes_arrows = new Dictionary<int, GameObject>();
		for (int i = 0; i < this.arrows_id.Count; i++)
		{
			this.indexes_arrows.Add(this.arrows_id[i], this.arrows[i]);
		}
	}

	private void Start()
	{
	}

	public void Shoot(float throwSpeed, string pName, int d, int arrow_id)
	{
		this.curentArrow_id = arrow_id;
		this.indexes_arrows[this.curentArrow_id].SetActive(true);
		this.indexes_arrows = new Dictionary<int, GameObject>();
		this.sended_name = pName;
		this.dmg = d;
		base.StartCoroutine(this.Wait());
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().AddForce(base.transform.forward * (20f * throwSpeed), ForceMode.VelocityChange);
		if (throwSpeed > 2.2f)
		{
			this.froward_coff = 4f;
		}
		else if (throwSpeed > 1.5f)
		{
			this.froward_coff = 3.5f;
		}
		else
		{
			this.froward_coff = 3f;
		}
	}

	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		Vector3 position = base.transform.position;
		RaycastHit hit;
		if (Physics.Raycast(new Ray(position - base.transform.forward / this.froward_coff, base.transform.forward), out hit, 100f) && hit.distance < 0.5f && hit.collider.tag != "arrow")
		{
			base.GetComponent<Rigidbody>().isKinematic = true;
			this.hasHit = true;
			base.GetComponent<Collider>().isTrigger = true;
			this.canDmg = false;
			base.transform.parent = hit.transform;
			if (hit.transform.gameObject.layer != 13)
			{
				this.PlayArrowTerrainSound(ray, hit);
				base.StartCoroutine(this.WaitAndHide());
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		if (base.GetComponent<Rigidbody>().velocity != Vector3.zero && !this.hasHit)
		{
			base.GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(base.GetComponent<Rigidbody>().velocity);
		}
		if (!this._isPlay_nf_sounds && this.sended_name != HG_WorkController._player.PlayerName)
		{
			float num = Vector3.Distance(base.transform.position, WorldGameObjectX.Instance.MainPlayer.transform.position);
			if (num < 2f)
			{
				this._isPlay_nf_sounds = true;
				NGUITools.PlaySound(this.nf_arrow_sounds[UnityEngine.Random.Range(0, this.nf_arrow_sounds.Count)]);
			}
		}
	}

	private void PlayArrowTerrainSound(Ray ray, RaycastHit hit)
	{
		if (!this._isPlay_nf_sounds)
		{
			this._isPlay_nf_sounds = true;
			Vector3 position = hit.point + ray.direction.normalized * 0.01f;
			IntVect intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
			BlockType blockType = WorldGameObjectX.Instance.WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
			if (blockType != BlockType.Leaves && blockType != BlockType.Water)
			{
				UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.Sparks, position, Quaternion.identity);
			}
			SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, WorldGameObjectX.Instance.MainPlayer.gameObject.GetComponent<AudioSource>());
		}
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(1f);
		yield return new WaitForSeconds(0.5f);
		this.sended_name = string.Empty;
		yield break;
	}

	private IEnumerator WaitAndHide()
	{
		yield return new WaitForSeconds(60f);
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	public int GetDMG()
	{
		return this.dmg;
	}

	private int dmg;

	private Quaternion q;

	private Vector3 v3;

	private bool hasHit;

	private bool canDmg = true;

	public GameObject arrow_mdl;

	public List<int> arrows_id;

	public List<GameObject> arrows;

	private Dictionary<int, GameObject> indexes_arrows;

	private int curentArrow_id;

	public List<AudioClip> nf_arrow_sounds;

	private bool _isPlay_nf_sounds;

	private float froward_coff = 3f;

	public string sended_name;
}
