using System;
using UnityEngine;

[Serializable]
public class ExplosionAtPoint : MonoBehaviour
{
	public virtual void Update()
	{
		if (UnityEngine.Input.GetKeyDown("mouse 0"))
		{
			this.nextCopy = Time.time + this.rate;
			Ray ray = this.GetComponent<Camera>().ScreenPointToRay(UnityEngine.Input.mousePosition);
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast(ray, out raycastHit))
			{
				Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
				UnityEngine.Object.Instantiate(this.explosionPrefab, raycastHit.point, Quaternion.identity);
			}
		}
	}

	public virtual void Main()
	{
	}

	public Transform explosionPrefab;

	public float rate;

	public float nextCopy;
}
