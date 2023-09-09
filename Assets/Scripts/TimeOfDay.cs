using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class TimeOfDay : Photon.MonoBehaviour
{
	public static float NormalizedTime
	{
		get
		{
			return TimeOfDay._Instance._NormalizedTime;
		}
	}

	public static Color FogColor
	{
		get
		{
			return TimeOfDay._Instance._FogColor;
		}
	}

	private void Awake()
	{
		TimeOfDay._Instance = this;
		this._Skybox = RenderSettings.skybox;
		this._LastChangeTime = Time.time;
	}

	private void OnJoinedRoom()
	{
		if (App.Instance.Settings.isServer)
		{
			TimeOfDay.InitializeTime(App.Instance.Settings.mapTime);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("InitializeTimeInternal", newPlayer, new object[]
			{
				this._NormalizedTimeTarget,
				(int)this._MapTime,
				Time.time - this._LastChangeTime
			});
		}
	}

	public static void InitializeTime(GameINI.MapTime mapTime)
	{
		if (TimeOfDay._Instance == null)
		{
			return;
		}
		TimeOfDay._Instance.photonView.RPC("InitializeTimeInternal", PhotonTargets.All, new object[]
		{
			(mapTime != GameINI.MapTime.NIGHT) ? 0f : 1f,
			(int)mapTime,
			0f
		});
	}

	[PunRPC]
	private void InitializeTimeInternal(float normalizedTime, int mapTime, float passedTime)
	{
		base.StopAllCoroutines();
		this._NormalizedTimeTarget = normalizedTime;
		this._MapTime = (GameINI.MapTime)mapTime;
		this._LastChangeTime = Time.time - passedTime;
		this.SetNormalizedTime(normalizedTime);
	}

	public static void SwitchDayNight()
	{
		if (TimeOfDay._Instance == null)
		{
			return;
		}
		float num = (TimeOfDay._Instance._NormalizedTimeTarget != 0f) ? 0f : 1f;
		TimeOfDay._Instance.photonView.RPC("ChangeTimeInternal", PhotonTargets.All, new object[]
		{
			num
		});
	}

	[PunRPC]
	private void ChangeTimeInternal(float normalizedTime)
	{
		base.StopAllCoroutines();
		this._NormalizedTimeTarget = normalizedTime;
		this._LastChangeTime = Time.time;
		base.StartCoroutine(this.ChangeTimeProcess());
	}

	public static void Affect(GameObject go)
	{
		if (TimeOfDay._Instance == null)
		{
			return;
		}
		float num = TimeOfDay._Instance._NormalizedTime * -0.4f;
		MaterialExt.AffectColor(go, num, num, num);
		TimeOfDay._Instance._ColoredObjects.Add(go);
	}

	private IEnumerator ChangeTimeProcess()
	{
		while (this._NormalizedTime != this._NormalizedTimeTarget)
		{
			this.SetNormalizedTime(Mathf.MoveTowards(this._NormalizedTime, this._NormalizedTimeTarget, 0.05f));
			yield return new WaitForSeconds(0.05f);
		}
		yield break;
	}

	private void SetNormalizedTime(float value)
	{
		this._NormalizedTime = value;
		this._Skybox.SetFloat("_Blend", this._NormalizedTime);
		this._FogColor = Color.Lerp(TimeOfDay.DayFogColor, TimeOfDay.NightFogColor, this._NormalizedTime);
		if (RenderSettings.skybox != null)
		{
			RenderSettings.fogColor = this._FogColor;
		}
		if (WorldGameObjectX.Instance.ChunkObjects != null)
		{
			ChunkGameObject[,,,] chunkObjects = WorldGameObjectX.Instance.ChunkObjects;
			int length = chunkObjects.GetLength(0);
			int length2 = chunkObjects.GetLength(1);
			int length3 = chunkObjects.GetLength(2);
			int length4 = chunkObjects.GetLength(3);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						for (int l = 0; l < length4; l++)
						{
							ChunkGameObject chunkGameObject = chunkObjects[i, j, k, l];
							if (chunkGameObject != null)
							{
								chunkGameObject.GetComponent<MeshRenderer>().material.SetFloat("_TimeOfDay", this._NormalizedTime);
							}
						}
					}
				}
			}
		}
		float num = this._NormalizedTime * -0.4f;
		for (int m = 0; m < this._ColoredObjects.Count; m++)
		{
			if (this._ColoredObjects[m] != null)
			{
				MaterialExt.AffectColor(this._ColoredObjects[m], num, num, num);
			}
			else
			{
				this._ColoredObjects.RemoveAt(m--);
			}
		}
		if (KGUI.FindNode("hud.bonuses.day_night", false).gameObject.activeInHierarchy)
		{
			TimeOfDay.RefreshBonusIcon();
		}
	}

	public static void RefreshBonusIcon()
	{
		if (TimeOfDay._Instance == null)
		{
			return;
		}
		Transform transform = KGUI.FindNode("hud.bonuses.day_night", false);
		KGUI.SetNodes("hud.bonuses.day_night.on", TimeOfDay._Instance._NormalizedTime < 0.5f, false);
		KGUI.SetNodes("hud.bonuses.day_night.off", TimeOfDay._Instance._NormalizedTime >= 0.5f, false);
		Vector3 localEulerAngles = new Vector3(0f, 0f, TimeOfDay._Instance._NormalizedTime * -360f);
		transform.Find("on").localEulerAngles = localEulerAngles;
		transform.Find("off").localEulerAngles = localEulerAngles;
	}

	private void Update()
	{
		if (PhotonNetwork.isMasterClient && this._MapTime == GameINI.MapTime.SWITCH && Time.time >= this._LastChangeTime + 900f)
		{
			TimeOfDay.SwitchDayNight();
		}
	}

	public const float SwitchTime = 900f;

	public const float SwitchSpeed = 1f;

	public static Color DayFogColor = new Color(0.8f, 0.84f, 0.96f);

	public static Color NightFogColor = new Color(0.1f, 0.1f, 0.1f);

	public static Color WaterFogColor = new Color(0f, 0.4f, 0.7f, 0.6f);

	private static TimeOfDay _Instance;

	private float _NormalizedTime;

	private float _NormalizedTimeTarget;

	private Color _FogColor = TimeOfDay.DayFogColor;

	private Material _Skybox;

	private List<GameObject> _ColoredObjects = new List<GameObject>();

	private GameINI.MapTime _MapTime;

	private float _LastChangeTime;
}
