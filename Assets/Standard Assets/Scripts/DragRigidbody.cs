using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class DragRigidbody : MonoBehaviour
{
	public DragRigidbody()
	{
		this.spring = 50f;
		this.damper = 5f;
		this.drag = 10f;
		this.angularDrag = 5f;
		this.distance = 0.2f;
	}

	public virtual void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Camera camera = this.FindCamera();
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast(camera.ScreenPointToRay(UnityEngine.Input.mousePosition), out raycastHit, (float)100))
			{
				if (raycastHit.rigidbody && !raycastHit.rigidbody.isKinematic)
				{
					if (!this.springJoint)
					{
						GameObject gameObject = new GameObject("Rigidbody dragger");
						Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>() as Rigidbody;
						this.springJoint = gameObject.AddComponent<SpringJoint>();
						rigidbody.isKinematic = true;
					}
					this.springJoint.transform.position = raycastHit.point;
					if (this.attachToCenterOfMass)
					{
						Vector3 vector = this.transform.TransformDirection(raycastHit.rigidbody.centerOfMass) + raycastHit.rigidbody.transform.position;
						vector = this.springJoint.transform.InverseTransformPoint(vector);
						this.springJoint.anchor = vector;
					}
					else
					{
						this.springJoint.anchor = Vector3.zero;
					}
					this.springJoint.spring = this.spring;
					this.springJoint.damper = this.damper;
					this.springJoint.maxDistance = this.distance;
					this.springJoint.connectedBody = raycastHit.rigidbody;
					this.StartCoroutine("DragObject", raycastHit.distance);
				}
			}
		}
	}

	public virtual IEnumerator DragObject(float distance)
	{
		return new DragRigidbody._0024DragObject_002441(distance, this).GetEnumerator();
	}

	public virtual Camera FindCamera()
	{
		return (!this.GetComponent<Camera>()) ? Camera.main : this.GetComponent<Camera>();
	}

	public virtual void Main()
	{
	}

	public float spring;

	public float damper;

	public float drag;

	public float angularDrag;

	public float distance;

	public bool attachToCenterOfMass;

	private SpringJoint springJoint;

	[CompilerGenerated]
	[Serializable]
	internal sealed class _0024DragObject_002441 : GenericGenerator<object>
	{
		public _0024DragObject_002441(float distance, DragRigidbody self_)
		{
			this._0024distance_002448 = distance;
			this._0024self__002449 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new DragRigidbody._0024DragObject_002441._0024(this._0024distance_002448, this._0024self__002449);
		}

		internal float _0024distance_002448;

		internal DragRigidbody _0024self__002449;
	}
}
