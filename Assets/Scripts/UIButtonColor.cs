using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : MonoBehaviour
{
	public Color defaultColor
	{
		get
		{
			if (!this.mStarted)
			{
				this.Init();
			}
			return this.mColor;
		}
		set
		{
			this.mColor = value;
		}
	}

	private void Start()
	{
		if (!this.mStarted)
		{
			this.Init();
			this.mStarted = true;
		}
	}

	protected virtual void OnEnable()
	{
		if (this.mStarted && this.mHighlighted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenColor component = this.tweenTarget.GetComponent<TweenColor>();
			if (component != null)
			{
				component.color = this.mColor;
				component.enabled = false;
			}
		}
	}

	protected void Init()
	{
		if (this.tweenTarget == null)
		{
			this.tweenTarget = base.gameObject;
		}
		UIWidget component = this.tweenTarget.GetComponent<UIWidget>();
		if (component != null)
		{
			this.mColor = component.color;
		}
		else
		{
			Renderer component2 = this.tweenTarget.GetComponent<Renderer>();
			if (component2 != null)
			{
				this.mColor = component2.material.color;
			}
			else
			{
				Light component3 = this.tweenTarget.GetComponent<Light>();
				if (component3 != null)
				{
					this.mColor = component3.color;
				}
				else
				{
					UnityEngine.Debug.LogWarning(NGUITools.GetHierarchy(base.gameObject) + " has nothing for UIButtonColor to color", this);
					base.enabled = false;
				}
			}
		}
		this.OnEnable();
	}

	public virtual void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenColor.Begin(this.tweenTarget, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mColor : this.hover) : this.pressed);
		}
	}

	public virtual void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenColor.Begin(this.tweenTarget, this.duration, (!isOver) ? this.mColor : this.hover);
			this.mHighlighted = isOver;
		}
	}

	public GameObject tweenTarget;

	public Color hover = new Color(0.6f, 1f, 0.2f, 1f);

	public Color pressed = Color.grey;

	public float duration = 0.2f;

	protected Color mColor;

	protected bool mStarted;

	protected bool mHighlighted;
}
