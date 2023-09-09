using System;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[ExecuteInEditMode]
public class AnimatedColor : MonoBehaviour
{
	private void Awake()
	{
		this.mWidget = base.GetComponent<UIWidget>();
	}

	private void Update()
	{
		this.mWidget.color = this.color;
	}

	public Color color = Color.white;

	private UIWidget mWidget;
}
