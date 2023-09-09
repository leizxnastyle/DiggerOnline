using System;
using System.Linq;
using UnityEngine;

public class Flag : EntityBase
{
	public bool IsBlue
	{
		get
		{
			return this.Type == EntityType.FLAG_BLUE;
		}
	}

	private bool _Captured
	{
		get
		{
			return this._Owner != null;
		}
	}

	private bool _CapturedByMe
	{
		get
		{
			return this._Owner != null && this._Owner.gameObject == WorldGameObjectX.Instance.MainPlayer;
		}
	}

	private string _MarkerName
	{
		get
		{
			return (!this.IsBlue) ? "hud.battle.flags.red" : "hud.battle.flags.blue";
		}
	}

	protected override void Creation(object[] data)
	{
		this._StartPos = base.transform.position;
		this._StartRot = base.transform.rotation;
		this._Position = base.transform.position;
		this._Rotation = base.transform.rotation;
		this._PoleNode = base.transform.Find("pole");
		this._OtherFlag = UnityEngine.Object.FindObjectsOfType(typeof(Flag)).Cast<Flag>().FirstOrDefault((Flag a) => a != this);
		this._MarkerEnabled = false;
		KGUI.SetNodes(this._MarkerName, true, false);
		KGUI.SetNodes(this._MarkerName, false, false);
	}

	protected override void Awake()
	{
	}

	protected override void Updating()
	{
		if (TeamBattle.Instance == null || !(TeamBattle.Instance is Ctf))
		{
			if (GameType.BattleMode())
			{
				this.SelfDelete();
			}
			return;
		}
		if (this._PoleNode.GetComponent<Rigidbody>() == null)
		{
			this._PoleNode.gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		}
		this._PoleNode.GetComponent<Rigidbody>().isKinematic = (this._Owner != null || !base.photonView.isMine);
		PlayerNetwork component = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		bool mainPlayerDead = WorldGameObjectX.Instance.MainPlayerDead;
		if (!component.IsObserver && Vector3.Distance(component.pos, this._PoleNode.position) < 3f && !this._Captured && !mainPlayerDead)
		{
			if (component.IsBlueTeam == this.IsBlue && Vector3.Distance(this._StartPos, this._PoleNode.position) > 3f)
			{
				base.photonView.RPC("ResetPos", PhotonTargets.All, new object[]
				{
					false
				});
			}
			if (component.IsBlueTeam != this.IsBlue)
			{
				base.photonView.RPC("SetOwner", PhotonTargets.All, new object[]
				{
					component.photonView.viewID
				});
			}
		}
		if (this._CapturedByMe && (mainPlayerDead || component.IsObserver || component.IsBlueTeam == this.IsBlue))
		{
			base.photonView.RPC("DropFlag", PhotonTargets.All, new object[]
			{
				this._Owner.FlagPlaceHolder.position
			});
		}
		if (base.photonView.isMine && Vector3.Distance(this._PoleNode.position, this._OtherFlag._StartPos) < 3f && !this._OtherFlag._Captured)
		{
			int teamIndex = (!this._OtherFlag.IsBlue) ? 1 : 2;
			TeamBattle.Instance.AddTeamScore(teamIndex, 30);
			base.photonView.RPC("ResetPos", PhotonTargets.All, new object[]
			{
				true
			});
		}
		if (this._Owner != null)
		{
			this._PoleNode.position = this._Owner.FlagPlaceHolder.position;
			this._PoleNode.rotation = this._Owner.FlagPlaceHolder.rotation;
			this._PoleNode.gameObject.SetActive(!(this._Owner == component) || CameraController.Instance.IsThirdPerson);
		}
		else if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this._Position, Time.deltaTime * 5f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this._Rotation, Time.deltaTime * 5f);
		}
		else
		{
			base.FixWorldBounds();
		}
		bool flag = false;
		if (this._Owner == null)
		{
			Vector3 position = this._PoleNode.position + this._PoleNode.up * this._PoleNode.GetComponent<Collider>().bounds.size.y * 1.25f;
			Vector3 vector = CameraController.RaycastCamera.WorldToScreenPoint(position);
			if (this._MarkerPos != vector)
			{
				this._MarkerPos = vector;
				KGUI.FindNode(this._MarkerName, false).localPosition = Utils.ScreenToNGUIPos(vector);
			}
			if (this._MarkerPos.z >= 0f)
			{
				flag = true;
			}
		}
		if (this._MarkerEnabled != flag)
		{
			this._MarkerEnabled = flag;
			KGUI.SetNodes(this._MarkerName, flag, false);
		}
		if (this._CapturedByMe)
		{
			if (!this._CapturedTip)
			{
				this._CapturedTip = true;
				KGUI.SetNodes("hud.battle.txt_flag_captured_tip", true, false);
			}
			KGUI.FindNode("hud.battle.txt_flag_captured_tip", false).GetComponent<UIWidget>().alpha = 0.3f + Mathf.PingPong(Time.time, 0.7f);
		}
		else if (this._CapturedTip)
		{
			this._CapturedTip = false;
			KGUI.SetNodes("hud.battle.txt_flag_captured_tip", false, false);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (PhotonNetwork.isMasterClient && this._Owner != null)
		{
			base.photonView.RPC("SetOwner", newPlayer, new object[]
			{
				this._Owner.photonView.viewID
			});
		}
	}

	[PunRPC]
	public void ResetPos(bool delivered)
	{
		this._Owner = null;
		this._PoleNode.gameObject.SetActive(true);
		this._PoleNode.position = this._StartPos;
		this._PoleNode.rotation = this._StartRot;
		this.CtfMessage((!delivered) ? "RETURNED" : "DELIVERED");
	}

	[PunRPC]
	public void DropFlag(Vector3 pos)
	{
		this._Owner = null;
		this._PoleNode.gameObject.SetActive(true);
		this._PoleNode.position = pos;
		this._PoleNode.rotation = Quaternion.Euler(0f, this._PoleNode.eulerAngles.y, 0f);
		this.CtfMessage("DROPPED");
	}

	[PunRPC]
	public void SetOwner(int playerPhotonViewID)
	{
		PhotonView photonView = PhotonView.Find(playerPhotonViewID);
		this._Owner = photonView.GetComponent<PlayerNetwork>();
		this._PoleNode.gameObject.SetActive(true);
		this.CtfMessage("CAPTURED");
	}

	private void CtfMessage(string message)
	{
		int playerTeam = TeamBattle.Instance.GetPlayerTeam(null);
		string str;
		if (playerTeam == 1 || playerTeam == 2)
		{
			bool flag = (playerTeam == 1 && !this.IsBlue) || (playerTeam == 2 && this.IsBlue);
			str = ((!flag) ? "OTHER" : "OUR");
			if (message == "CAPTURED")
			{
				SoundManager.Instance.Play((!flag) ? SoundManager.Sound.CtfFlagCaptured : SoundManager.Sound.CtfFlagStolen, null);
			}
			else if (message == "DELIVERED")
			{
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagDelivered, null);
			}
		}
		else
		{
			if (!this.IsBlue)
			{
				str = "RED";
			}
			else
			{
				str = "BLUE";
			}
			if (message == "CAPTURED")
			{
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagCaptured, null);
			}
			else if (message == "DELIVERED")
			{
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagDelivered, null);
			}
		}
		Chat.SendInfoF(Localize.GetText("CTF_FLAG_" + message + "_" + str, null), false);
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if (TeamBattle.Instance != null)
		{
			return;
		}
		base.OnLeftMouseHit(playerName);
	}

	public override void OnButtonE(string playerName)
	{
		if (TeamBattle.Instance != null)
		{
			return;
		}
		base.OnButtonE(playerName);
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
		}
		else
		{
			this._Position = (Vector3)stream.ReceiveNext();
			this._Rotation = (Quaternion)stream.ReceiveNext();
		}
	}

	private Vector3 _StartPos;

	private Quaternion _StartRot;

	private Transform _PoleNode;

	private Flag _OtherFlag;

	private PlayerNetwork _Owner;

	private bool _CapturedTip;

	private bool _MarkerEnabled;

	private Vector3 _MarkerPos;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;
}
