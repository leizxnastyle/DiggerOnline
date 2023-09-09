using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	protected override void OnEnable()
	{
		if (this.isEnabled)
		{
			base.OnEnable();
		}
		else
		{
			this.UpdateColor(false, true);
		}
	}

	public override void OnHover(bool isOver)
	{
		if (this.isEnabled)
		{
			base.OnHover(isOver);
		}
	}

	public override void OnPress(bool isPressed)
	{
		if (this.isEnabled)
		{
			base.OnPress(isPressed);
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
				this.UpdateColor(value, false);
			}
		}
	}

	public void UpdateColor(bool shouldBeEnabled, bool immediate)
	{
		if (this.tweenTarget)
		{
			if (!this.mStarted)
			{
				this.mStarted = true;
				base.Init();
			}
			Color color = (!shouldBeEnabled) ? this.disabledColor : base.defaultColor;
			TweenColor tweenColor = TweenColor.Begin(this.tweenTarget, 0.15f, color);
			if (immediate)
			{
				tweenColor.color = color;
				tweenColor.enabled = false;
			}
		}
	}

	public Color disabledColor = Color.grey;
}
