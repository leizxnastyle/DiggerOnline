using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public bool isPressed
	{
		get
		{
			return this._isPressed;
		}
		set
		{
			this._isPressed = value;
			this.UpdateImage();
		}
	}

	public bool isEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			return component && component.enabled;
		}
		set
		{
			Collider component = base.GetComponent<Collider>();
			if (!component)
			{
				return;
			}
			if (component.enabled != value)
			{
				component.enabled = value;
				this.UpdateImage();
			}
		}
	}

	private void Awake()
	{
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<UISprite>();
		}
		this._canHover = true;
	}

	private void OnEnable()
	{
		this.UpdateImage();
	}

	private void UpdateImage()
	{
		if (this.target != null)
		{
			if (this.isPressed)
			{
				this.target.spriteName = this.pressedSprite;
			}
			else if (this.isEnabled && this._canHover)
			{
				this.target.spriteName = ((!UICamera.IsHighlighted(base.gameObject)) ? this.normalSprite : this.hoverSprite);
			}
			else
			{
				this.target.spriteName = this.normalSprite;
			}
			this.target.MakePixelPerfect();
		}
	}

	private IEnumerator WaitAndOn()
	{
		yield return new WaitForSeconds(1f);
		this._canHover = true;
		yield break;
	}

	private void OnHover(bool isOver)
	{
		if (!this.isPressed && this.isEnabled && this.target != null && this._canHover)
		{
			this.target.spriteName = ((!isOver) ? this.normalSprite : this.hoverSprite);
			this.target.MakePixelPerfect();
		}
	}

	private void OnPress(bool pressed)
	{
		if (this.isPressed)
		{
			return;
		}
		if (pressed)
		{
			this._canHover = false;
			this.target.spriteName = this.pressedSprite;
			this.target.MakePixelPerfect();
		}
		else
		{
			this.UpdateImage();
			base.StartCoroutine(this.WaitAndOn());
		}
	}

	public UISprite target;

	public string normalSprite;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;

	private bool _isPressed;

	private bool _canHover = true;
}
