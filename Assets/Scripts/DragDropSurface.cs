using System;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface")]
public class DragDropSurface : MonoBehaviour
{
	private void OnDrop(GameObject go)
	{
		DragDropItem component = go.GetComponent<DragDropItem>();
		if (component != null)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, component.prefab);
			Transform transform = gameObject.transform;
			transform.position = UICamera.lastHit.point;
			if (this.rotatePlacedObject)
			{
				transform.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
			}
			UnityEngine.Object.Destroy(go);
		}
	}

	public bool rotatePlacedObject;
}
