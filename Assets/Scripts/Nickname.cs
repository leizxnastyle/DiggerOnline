using System;
using System.Collections;
using UnityEngine;

public class Nickname : MonoBehaviour
{
	private void OnEnable()
	{
		WorldGameObjectX.isPlayerLifeOfShieldChangedLocal = (Action<string>)Delegate.Combine(WorldGameObjectX.isPlayerLifeOfShieldChangedLocal, new Action<string>(this.SetLifeOrShieldChanged));
	}

	private void OnDisable()
	{
		WorldGameObjectX.isPlayerLifeOfShieldChangedLocal = (Action<string>)Delegate.Remove(WorldGameObjectX.isPlayerLifeOfShieldChangedLocal, new Action<string>(this.SetLifeOrShieldChanged));
	}

	public void SetPlayer(PlayerNode player)
	{
		if (this._GUIObject == null)
		{
			GameObject gameObject = KGUI.FindNode("hud.nicknames.nickname", false).gameObject;
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
			Utils.SetActiveRecursively(gameObject2, true);
			gameObject2.transform.parent = gameObject.transform.parent;
			gameObject2.transform.localScale = gameObject.transform.localScale;
			gameObject2.transform.localPosition = gameObject.transform.localPosition;
			gameObject2.transform.localRotation = gameObject.transform.localRotation;
			this._GUIObject = gameObject2;
			this._Text = this._GUIObject.transform.Find("txt_text").GetComponent<UILabel>();
			this._Level = this._GUIObject.transform.Find("level_icon");
			this._LifeBar = this._GUIObject.transform.Find("player_h").GetComponent<UISprite>();
			this._ShieldBar = this._GUIObject.transform.Find("player_a").GetComponent<UISprite>();
			UISprite component = this._GUIObject.transform.Find("player_h_bg").GetComponent<UISprite>();
			this._ShieldBarBg = this._GUIObject.transform.Find("player_a_bg").GetComponent<UISprite>();
			if (!GameType.BattleMode() || GameType.IsHideSeek)
			{
				UnityEngine.Object.Destroy(this._Level.gameObject);
				if (GameType.IsHideSeek)
				{
					UnityEngine.Object.Destroy(this._Text.gameObject);
				}
				UnityEngine.Object.Destroy(this._Level.gameObject);
				UnityEngine.Object.Destroy(this._LifeBar.gameObject);
				UnityEngine.Object.Destroy(this._ShieldBar.gameObject);
				UnityEngine.Object.Destroy(component.gameObject);
				UnityEngine.Object.Destroy(this._ShieldBarBg.gameObject);
				this._Level = null;
			}
			else
			{
				this.ThisPlayer = player;
				if (this.ThisPlayer != null)
				{
					this.SetLifeAndShield();
					this._ShieldBarBg.gameObject.SetActive(false);
					this._ShieldBar.gameObject.SetActive(false);
				}
			}
			if (Info.Instance.GameMode == "ARCADE")
			{
				UnityEngine.Object.Destroy(this._LifeBar.gameObject);
				UnityEngine.Object.Destroy(component.gameObject);
			}
			this._GUIObject.SetActive(true);
			this._Show = true;
		}
		this._Text.text = MainMenu.FixCollorName(player.Name);
		this.SetLevel(player.Level);
		this._Target = base.GetComponent<PlayerNetwork>().SpinBone.transform;
	}

	private void SetLifeOrShieldChanged(string pName)
	{
		if (this.ThisPlayer != null && pName == this.ThisPlayer.Name)
		{
			this.SetLifeAndShield();
		}
	}

	public void SetLevel(int level)
	{
		if (this._Level != null)
		{
			KGUI.SetControlSprite(this._Level, "level_" + level, 0f);
		}
	}

	public void SetColor(Color color)
	{
		if (this._Text != null)
		{
			float a = this._Text.color.a;
			this._Text.color = new Color(color.r, color.g, color.b, a);
		}
	}

	public void Hide()
	{
		if (this._GUIObject != null)
		{
			this._GUIObject.SetActive(false);
			this._Show = false;
		}
	}

	public void Show()
	{
		if (this._GUIObject != null)
		{
			this._GUIObject.SetActive(true);
			this._Show = true;
			this._IsOtherTeam = true;
		}
	}

	public void ShowMyTeamPlayer()
	{
		if (this._GUIObject != null)
		{
			this._GUIObject.SetActive(true);
			this._Show = true;
			this._IsOtherTeam = false;
		}
	}

	private void OnDestroy()
	{
		if (this._GUIObject != null)
		{
			UnityEngine.Object.Destroy(this._GUIObject);
		}
	}

	private void LateUpdate()
	{
		if (GameType.IsHungerGamesMode)
		{
			if (this._GUIObject != null && HG_WorkController.IsStartPlay)
			{
				if (!this._GUIObject.gameObject.activeSelf)
				{
					this._GUIObject.SetActive(true);
				}
				this.ShowPositionFormating();
			}
			else if (this._GUIObject != null && !HG_WorkController.IsStartPlay)
			{
				if (!this._GUIObject.gameObject.activeSelf)
				{
					this._GUIObject.SetActive(true);
				}
				this._RaycastFail = false;
				this.ShowPositionFormating();
			}
			return;
		}
		if (this._Show && !Nickname.HideAll && this._GUIObject != null && this._Target != null && CameraController.Instance != null)
		{
			this.ShowPositionFormating();
		}
		else if (this._GUIObject != null && this._GUIObject.activeSelf)
		{
			this._GUIObject.SetActive(false);
		}
	}

	private void SetLifeAndShield()
	{
		if (this.ThisPlayer != null)
		{
			if (this.ThisPlayer.Shield <= 0f)
			{
				if (this._ShieldBar.gameObject.activeSelf)
				{
					this._ShieldBarBg.gameObject.SetActive(false);
					this._ShieldBar.gameObject.SetActive(false);
				}
			}
			else
			{
				if (!this._ShieldBar.gameObject.activeSelf)
				{
					this._ShieldBarBg.gameObject.SetActive(true);
					this._ShieldBar.gameObject.SetActive(true);
				}
				this._ShieldBar.fillAmount = this.ThisPlayer.Shield;
			}
			this._LifeBar.fillAmount = this.ThisPlayer.Life;
		}
	}

	public void ShowPositionFormating()
	{
		try
		{
			Vector3 pos = CameraController.RaycastCamera.WorldToScreenPoint(this._Target.transform.position + Vector3.up * 1.2f);
			bool flag = pos.z > 0f && pos.z <= 500f && !this._RaycastFail;
			if (this._GUIObject.activeSelf != flag)
			{
				this._GUIObject.SetActive(flag);
			}
			if (flag)
			{
				this._GUIObject.transform.localPosition = KGUI.ScreenToGUIPoint(pos);
				if (this.ThisPlayer != null && this.ThisPlayer.Shield <= 0f)
				{
					if (this._ShieldBar.gameObject.activeSelf)
					{
						this._ShieldBarBg.gameObject.SetActive(false);
						this._ShieldBar.gameObject.SetActive(false);
					}
					else
					{
						this._ShieldBarBg.gameObject.SetActive(true);
						this._ShieldBar.gameObject.SetActive(true);
					}
				}
			}
		}
		catch
		{
			UnityEngine.Debug.Log("NO SET ACTIVE NICK");
		}
	}

	public void SetRaycast(bool enable)
	{
		base.StopCoroutine("RaycastPlayerProcess");
		if (enable)
		{
			base.StartCoroutine("RaycastPlayerProcess");
		}
		else
		{
			this._RaycastFail = false;
		}
	}

	private IEnumerator RaycastPlayerProcess()
	{
		for (;;)
		{
			this.RaycastPlayer();
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	private void RaycastPlayer()
	{
		this._RaycastFail = true;
		if (CameraController.Instance == null)
		{
			return;
		}
		if (base.gameObject == WorldGameObjectX.Instance.MainPlayer)
		{
			this._RaycastFail = false;
			return;
		}
		Vector3 position = CameraController.RaycastCamera.transform.position;
		PlayerNetwork component = base.gameObject.GetComponent<PlayerNetwork>();
		Vector3 vector;
		if (component != null && component.HeadBone != null && component.HeadBone.transform.childCount > 0)
		{
			vector = component.HeadBone.transform.GetChild(0).transform.position;
		}
		else if (component != null && component.HeadBone != null)
		{
			vector = component.HeadBone.transform.position;
		}
		else
		{
			vector = base.gameObject.GetComponent<Collider>().bounds.center;
		}
		float num = Vector3.Distance(position, vector);
		if (num > 500f)
		{
			return;
		}
		if (this._IsOtherTeam && num > 30f)
		{
			return;
		}
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		if (Physics.Raycast(position, (vector - position).normalized, num, layerMask))
		{
			return;
		}
		this._RaycastFail = false;
	}

	public const float MaxDistance = 500f;

	private const float MaxShowDistanceOtherTeam = 30f;

	public static bool HideAll;

	private GameObject _GUIObject;

	private UILabel _Text;

	private Transform _Level;

	private UISprite _LifeBar;

	private UISprite _ShieldBar;

	private UISprite _ShieldBarBg;

	private bool _Show;

	private bool _IsOtherTeam;

	private Transform _Target;

	private bool _RaycastFail;

	private PlayerNode ThisPlayer;
}
