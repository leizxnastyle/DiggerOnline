using System;
using System.Collections;
using UnityEngine;

public class fwButton : MonoBehaviour
{
	private void Start()
	{
		this.startPosition = base.transform.localPosition;
		this.IeActive = true;
	}

	private void Update()
	{
		if (ProfileINI.GetPurchaseValue(StorePurchase.NY_OFFER) > 0 && !GameType.BattleMode() && WorldGameObjectX.Instance.MainPlayer.GetComponent<SkinManager>().CurSkin == 1)
		{
			base.transform.localPosition = this.startPosition;
		}
		else
		{
			base.transform.localPosition = new Vector3(10000f, 10000f, 1f);
		}
		if (this.IeActive && UnityEngine.Input.GetKeyDown(this.runKey) && !Chat.IsEnabled() && this.IsCanRun())
		{
			this.IeActive = false;
			this.sprite.fillAmount = 0f;
			base.StartCoroutine(this.Reload());
		}
	}

	private bool IsCanRun()
	{
		return base.transform.localPosition == this.startPosition;
	}

	private IEnumerator Reload()
	{
		this.deleyTime = 5f;
		do
		{
			yield return new WaitForSeconds(0.5f);
			this.deleyTime -= 1f;
			this.sprite.fillAmount += 0.2f;
		}
		while (this.deleyTime != 0f);
		this.IeActive = true;
		yield break;
	}

	public KeyCode runKey;

	public int runFw;

	public UISprite sprite;

	private float deleyTime = 5f;

	private bool IeActive = true;

	private Vector3 startPosition;
}
