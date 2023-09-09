using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Object")]
public class UICheckboxControlledObject : MonoBehaviour
{
	private void OnEnable()
	{
		UICheckbox component = base.GetComponent<UICheckbox>();
		if (component != null)
		{
			this.OnActivate(component.isChecked);
		}
	}

	private void OnActivate(bool isActive)
	{
		if (this.target != null)
		{
			NGUITools.SetActive(this.target, (!this.inverse) ? isActive : (!isActive));
			UIPanel uipanel = NGUITools.FindInParents<UIPanel>(this.target);
			if (uipanel != null)
			{
				uipanel.Refresh();
			}
		}
	}

	public GameObject target;

	public bool inverse;
}
