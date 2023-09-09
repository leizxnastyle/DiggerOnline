using System;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Root")]
public class DragDropRoot : MonoBehaviour
{
	private void Awake()
	{
		DragDropRoot.root = base.transform;
	}

	public static Transform root;
}
