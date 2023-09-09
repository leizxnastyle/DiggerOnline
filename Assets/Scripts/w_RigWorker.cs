using System;
using UnityEngine;

public class w_RigWorker : MonoBehaviour
{
	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
	}

	private void Start()
	{
		w_RigWorker.RigWorker = this;
		this.isActive = false;
		this.sprite.fillAmount = 0f;
	}

	public void StartPress(float presed_time)
	{
		this.isActive = true;
		this.sprite.gameObject.SetActive(true);
		this.bg_sprite.gameObject.SetActive(true);
		this.sprite.fillAmount = presed_time;
	}

	public void StopPress()
	{
		this.isActive = false;
		this.sprite.gameObject.SetActive(false);
		this.bg_sprite.gameObject.SetActive(false);
		this.sprite.fillAmount = 0f;
	}

	public static w_RigWorker RigWorker;

	public UISprite sprite;

	public UISprite bg_sprite;

	private bool isActive;
}
