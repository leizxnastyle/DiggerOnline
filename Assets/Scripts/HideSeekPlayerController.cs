using System;
using System.Collections;
using UnityEngine;

public class HideSeekPlayerController : MonoBehaviour
{
	public void Init(PlayerNetwork pn)
	{
		this.hide_time_sprite = KGUI.FindNode("hud.hide_seek.HideProgresBar.HideTimeProgres", false).GetComponent<UISprite>();
		this._playerNetwork = pn;
	}

	public void StartGame(int hii)
	{
		int playerTeam = TeamBattle.Instance.GetPlayerTeam(this._playerNetwork.PlayerName);
		UnityEngine.Debug.Log(hii);
		if (playerTeam == 1)
		{
			this.hide_item_id = hii;
			this.hideItemType = this.GetHideItemType(this.hide_item_id);
			if (this._playerNetwork.photonView.isMine)
			{
				this.isActive = true;
				CameraController.Instance.SetThirdPerson(true, 0f, true);
				this._playerNetwork.photonView.RPC("SetHSStart", PhotonTargets.Others, new object[]
				{
					1,
					this.hide_item_id
				});
			}
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
			{
				GameObject original = (GameObject)Resources.Load("HideSeekItems/CubeHS", typeof(GameObject));
				this.hide_go = (GameObject)UnityEngine.Object.Instantiate(original, base.transform.position, base.transform.rotation);
				this.hide_go.transform.parent = base.transform;
				this.hide_go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				this.hide_go.transform.localPosition = new Vector3(0f, 0.25f, 0f);
			}
			else if (this.hideItemType == HideSeekPlayerController.HideItemType.Item)
			{
				UnityEngine.Debug.Log(this.hide_item_id);
				GameObject gameObject = (GameObject)Resources.Load("HideSeekItems/HideSeekItems_" + this.hide_item_id, typeof(GameObject));
				UnityEngine.Debug.Log(gameObject.name);
				this.hide_go = (GameObject)UnityEngine.Object.Instantiate(gameObject, base.transform.position, base.transform.rotation);
				this.hide_go.transform.parent = base.transform;
				this.hide_go.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.hide_go.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
			}
			this._playerNetwork._SkinManager.DisableAllSkins();
		}
	}

	private HideSeekPlayerController.HideItemType GetHideItemType(int hide_item_id)
	{
		if (hide_item_id == 1)
		{
			return HideSeekPlayerController.HideItemType.Box;
		}
		if (hide_item_id > 1)
		{
			return HideSeekPlayerController.HideItemType.Item;
		}
		return HideSeekPlayerController.HideItemType.None;
	}

	private void Update()
	{
		if (this._playerNetwork.photonView.isMine && HG_WorkController.hgstatus == GameStatus.GS_START && this.isActive)
		{
			this.CalculateLocalVelocity();
			if (!this.isHide)
			{
				if (this.localeVelocity == Vector3.zero && this.last_set_time == 0f)
				{
					this.last_set_time = Time.time + 5f;
					KGUI.SetNodes("hud.hide_seek.HideProgresBar.txt_Hide", true, false);
					KGUI.SetNodes("hud.hide_seek.HideProgresBar", true, false);
					this.hide_time_sprite.fillAmount = 0f;
				}
				else if (this.localeVelocity == Vector3.zero && this.last_set_time != 0f)
				{
					if (Time.time > this.last_set_time)
					{
						KGUI.SetNodes("hud.hide_seek.HideProgresBar.txt_Hide", false, false);
						this.hide_time_sprite.fillAmount = 1f;
						this.Hide();
					}
					else
					{
						this.hide_time_sprite.fillAmount = 1f - (this.last_set_time - Time.time) / 5f;
					}
				}
				else if (this.localeVelocity != Vector3.zero && this.last_set_time != 0f)
				{
					this.last_set_time = 0f;
					KGUI.SetNodes("hud.hide_seek.HideProgresBar", false, false);
				}
			}
			else if (this.isHide && this.isRedy && this.localeVelocity != Vector3.zero)
			{
				this.Show();
				KGUI.SetNodes("hud.hide_seek.HideProgresBar", false, false);
			}
		}
	}

	private void Show()
	{
		if (this.isHide)
		{
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
			{
				WorldGameObjectX.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, new object[]
				{
					this.lastSet.X,
					this.lastSet.Y,
					this.lastSet.Z,
					false,
					false
				});
			}
			this.isHide = false;
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
			{
				this.hide_go.SetActive(true);
			}
			this.isRedy = false;
			this.last_set_time = 0f;
			this._playerNetwork.photonView.RPC("IsHGShow", PhotonTargets.Others, new object[]
			{
				true
			});
		}
	}

	private void Hide()
	{
		Vector3 origin = base.transform.position + base.transform.up * 0.5f;
		Vector3 direction = -base.transform.up * 1f;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, direction, out raycastHit, direction.magnitude, 1 << LayerMask.NameToLayer("Terrain")) && raycastHit.collider.tag != "Water")
		{
			Vector3 vector = raycastHit.point - raycastHit.normal * 0.1f;
			IntVect intVect = new IntVect(vector.x, vector.z, vector.y + 1f);
			this.lastSet = new IntVect(intVect.X, intVect.Y, intVect.Z);
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
			{
				base.transform.position = new Vector3((float)this.lastSet.X, (float)this.lastSet.Z, (float)this.lastSet.Y);
			}
			else if (this.hideItemType == HideSeekPlayerController.HideItemType.Item)
			{
				base.transform.position = new Vector3((float)this.lastSet.X + 0.5f, (float)this.lastSet.Z, (float)this.lastSet.Y + 0.5f);
			}
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Item)
			{
				this.hide_go.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
			}
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
			{
				WorldGameObjectX.Instance.photonView.RPC("AddBlockAt", PhotonTargets.All, new object[]
				{
					1,
					0,
					intVect.X,
					intVect.Y,
					intVect.Z
				});
			}
			this.isHide = true;
			this.last_set_time = 0f;
			if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
			{
				this.hide_go.SetActive(false);
			}
			this._playerNetwork.photonView.RPC("IsHGShow", PhotonTargets.Others, new object[]
			{
				false
			});
			base.StartCoroutine(this.WT());
		}
	}

	public void RPCShowHide(bool isShowRpc)
	{
		if (this.hideItemType == HideSeekPlayerController.HideItemType.Box)
		{
			if (isShowRpc)
			{
				this.hide_go.SetActive(true);
			}
			else
			{
				this.hide_go.SetActive(false);
			}
		}
		else if (this.hideItemType == HideSeekPlayerController.HideItemType.Item)
		{
			this.hide_go.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
		}
	}

	private void CalculateLocalVelocity()
	{
		this.localeVelocity = (base.transform.position - this.previous) / Time.deltaTime;
		this.previous = base.transform.position;
	}

	private IEnumerator WT()
	{
		yield return new WaitForSeconds(1f);
		this.isRedy = true;
		yield break;
	}

	internal void SetNewTeam()
	{
		this.isActive = false;
		UnityEngine.Object.Destroy(this.hide_go);
		this._playerNetwork._SkinManager.ShowSkin();
	}

	private PlayerNetwork _playerNetwork;

	private GameObject hide_go;

	private float last_set_time;

	private bool isHide;

	private bool isRedy;

	private int hide_item_id;

	private HideSeekPlayerController.HideItemType hideItemType;

	private bool isActive;

	private IntVect lastSet = IntVect.Zero;

	private Vector3 localeVelocity = Vector3.zero;

	private Vector3 previous = Vector3.zero;

	private UISprite hide_time_sprite;

	private enum HideItemType
	{
		None,
		Box,
		Item
	}
}
