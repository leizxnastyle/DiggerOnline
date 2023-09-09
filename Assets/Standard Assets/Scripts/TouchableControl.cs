using System;
using UnityEngine;

[Serializable]
public class TouchableControl
{
	public virtual void Init(TouchController joy)
	{
		this.joy = joy;
		this.visible = true;
		this.enabled = true;
	}

	public virtual TouchController.HitTestResult HitTest(Vector2 pos, int touchId)
	{
		return new TouchController.HitTestResult(false);
	}

	public virtual TouchController.EventResult OnTouchStart(int touchId, Vector2 pos)
	{
		return TouchController.EventResult.NOT_HANDLED;
	}

	public virtual TouchController.EventResult OnTouchEnd(int touchId, bool cancel = false)
	{
		return TouchController.EventResult.NOT_HANDLED;
	}

	public virtual TouchController.EventResult OnTouchMove(int touchId, Vector2 pos)
	{
		return TouchController.EventResult.NOT_HANDLED;
	}

	public virtual void OnReset()
	{
	}

	public virtual void OnPrePoll()
	{
	}

	public virtual void OnPostPoll()
	{
	}

	public virtual void OnUpdate(bool firstPostPollUpdate)
	{
	}

	public virtual void OnPostUpdate(bool firstPostPollUpdate)
	{
	}

	public virtual void OnLayoutAddContent()
	{
	}

	public virtual void OnLayout()
	{
	}

	public virtual void DrawGUI()
	{
	}

	public virtual void ReleaseTouches()
	{
	}

	public virtual void TakeoverTouches(TouchableControl controlToUntouch)
	{
	}

	public virtual void ResetRect()
	{
	}

	public void DisableGUI()
	{
		this.disableGui = true;
	}

	public void EnableGUI()
	{
		this.disableGui = false;
	}

	public bool DefaultGUIEnabled()
	{
		return !this.disableGui;
	}

	public bool Enabled()
	{
		return this.enabled;
	}

	public virtual void Enable(bool skipAnimation)
	{
		this.enabled = true;
	}

	public void Enable()
	{
		this.Enable(false);
	}

	public virtual void Disable(bool skipAnimation)
	{
		this.enabled = false;
		this.ReleaseTouches();
	}

	public void Disable()
	{
		this.Disable(false);
	}

	public virtual void Show(bool skipAnim)
	{
		this.visible = true;
	}

	public void Show()
	{
		this.Show(false);
	}

	public virtual void Hide(bool skipAnim)
	{
		this.visible = false;
		this.ReleaseTouches();
	}

	public void Hide()
	{
		this.Hide(false);
	}

	public bool initiallyDisabled;

	public bool initiallyHidden;

	protected bool enabled;

	protected bool visible;

	public int prio;

	public float hitDistScale;

	public string name;

	public bool disableGui;

	public int guiDepth;

	public int layoutBoxId;

	public bool acceptSharedTouches;

	protected TouchController joy;
}
