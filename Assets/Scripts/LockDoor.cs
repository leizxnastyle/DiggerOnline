using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoor : EntityBase
{
	public override void OnButtonF(string playerName)
	{
		if (this.LockerModel != null && !Level.Instance.IsAdmin(null) && !Level.Instance.IsModerator(null))
		{
			List<LockKey> list = new List<LockKey>((LockKey[])UnityEngine.Object.FindObjectsOfType(typeof(LockKey)));
			int count = list.FindAll((LockKey i) => i.LockID == this.LockID && i.KeyModel != null).Count;
			if (count > 0)
			{
				Chat.SendInfoF(Localize.GetText("LOCK_REMANING_KEYS", null) + ((count <= 1) ? string.Empty : (count + " ")) + Localize.GetText("PURCHASE_" + list[0].Name, null), false);
				return;
			}
			base.StartCoroutine(this.UnlockDoorProcess());
		}
		else if (!this._Opened)
		{
			this.OpenDoor();
		}
		else
		{
			this.CloseDoor();
		}
	}

	private IEnumerator UnlockDoorProcess()
	{
		float a = 1f;
		while (a > 0f)
		{
			a = Mathf.MoveTowards(a, 0f, Time.deltaTime);
			MaterialExt.SetTransparent(this.LockerModel, a);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.5f);
		UnityEngine.Object.Destroy(this.LockerModel);
		this.LockerModel = null;
		this.OpenDoor();
		yield break;
	}

	private void OpenDoor()
	{
		iTween.RotateTo(this.InnerModel, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(-90f, 90f, 0f),
			"time",
			1f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		this._Opened = true;
		this.InnerModel.GetComponent<Collider>().isTrigger = true;
		SoundManager.Instance.Play(SoundManager.Sound.DoorOpen, base.GetComponent<AudioSource>());
	}

	private void CloseDoor()
	{
		iTween.RotateTo(this.InnerModel, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(-90f, 0f, 0f),
			"time",
			1f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		this._Opened = false;
		this.InnerModel.GetComponent<Collider>().isTrigger = false;
		SoundManager.Instance.Play(SoundManager.Sound.DoorClose, base.GetComponent<AudioSource>());
	}

	public int LockID;

	public GameObject LockerModel;

	public GameObject InnerModel;

	private bool _Opened;
}
