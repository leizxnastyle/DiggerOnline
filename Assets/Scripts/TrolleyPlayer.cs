using System;
using UnityEngine;

public class TrolleyPlayer : MonoBehaviour
{
	public void Initialize(Trolley trolley, GameObject player)
	{
		CapsuleCollider capsuleCollider = base.gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.isTrigger = true;
		base.gameObject.AddComponent<Rigidbody>();
		base.GetComponent<Rigidbody>().isKinematic = true;
		CharacterController component = player.GetComponent<CharacterController>();
		capsuleCollider.height = component.height;
		capsuleCollider.radius = component.radius;
		capsuleCollider.center = component.center;
		base.transform.position = component.transform.position;
		base.transform.rotation = component.transform.rotation;
		base.transform.localScale = component.transform.localScale;
		base.transform.parent = trolley.Model.transform;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == TrolleyPlayer._TerrainLayer)
		{
			Trolley component = base.transform.root.GetComponent<Trolley>();
			if (component.Player != null)
			{
				component.PlayerExit(false);
			}
		}
	}

	private static int _TerrainLayer = LayerMask.NameToLayer("Terrain");
}
