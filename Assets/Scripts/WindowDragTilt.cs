using System;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Window Drag Tilt")]
public class WindowDragTilt : MonoBehaviour
{
	private void Start()
	{
		UpdateManager.AddCoroutine(this, this.updateOrder, new UpdateManager.OnUpdate(this.CoroutineUpdate));
	}

	private void OnEnable()
	{
		this.mInit = true;
	}

	private void CoroutineUpdate(float delta)
	{
		if (this.mInit)
		{
			this.mInit = false;
			this.mTrans = base.transform;
			this.mLastPos = this.mTrans.position;
		}
		Vector3 vector = this.mTrans.position - this.mLastPos;
		this.mLastPos = this.mTrans.position;
		this.mAngle += vector.x * this.degrees;
		this.mAngle = NGUIMath.SpringLerp(this.mAngle, 0f, 20f, delta);
		this.mTrans.localRotation = Quaternion.Euler(0f, 0f, -this.mAngle);
	}

	public int updateOrder;

	public float degrees = 30f;

	private Vector3 mLastPos;

	private Transform mTrans;

	private float mAngle;

	private bool mInit = true;
}
