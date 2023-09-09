using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Component")]
public class UICheckboxControlledComponent : MonoBehaviour
{
	private void Start()
	{
		UICheckbox component = base.GetComponent<UICheckbox>();
		if (component != null)
		{
			this.mUsingDelegates = true;
			UICheckbox uicheckbox = component;
			uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uicheckbox.onStateChange, new UICheckbox.OnStateChange(this.OnActivateDelegate));
		}
	}

	private void OnActivateDelegate(bool isActive)
	{
		if (base.enabled && this.target != null)
		{
			this.target.enabled = ((!this.inverse) ? isActive : (!isActive));
		}
	}

	private void OnActivate(bool isActive)
	{
		if (!this.mUsingDelegates)
		{
			this.OnActivateDelegate(isActive);
		}
	}

	public MonoBehaviour target;

	public bool inverse;

	private bool mUsingDelegates;
}
