using System;
using UnityEngine;

public class AnimatedAlpha : MonoBehaviour
{
	private void Awake()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.mPanel = base.GetComponent<UIPanel>();
		this.Update();
	}

	private void Update()
	{
		if (this.mWidget != null)
		{
			this.mWidget.alpha = this.alpha;
		}
		if (this.mPanel != null)
		{
			this.mPanel.alpha = this.alpha;
		}
	}

	public float alpha = 1f;

	private UIWidget mWidget;

	private UIPanel mPanel;
}
