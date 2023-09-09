using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Mouse Orbit - Unluck Software")]
public class SmoothCameraOrbit : MonoBehaviour
{
	private void Start()
	{
		this.Init();
	}

	private void OnEnable()
	{
		this.Init();
	}

	public void Init()
	{
		if (!this.target)
		{
			this.target = new GameObject("Cam Target")
			{
				transform = 
				{
					position = base.transform.position + base.transform.forward * this.distance
				}
			}.transform;
		}
		this.currentDistance = this.distance;
		this.desiredDistance = this.distance;
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.currentRotation = base.transform.rotation;
		this.desiredRotation = base.transform.rotation;
		this.xDeg = Vector3.Angle(Vector3.right, base.transform.right);
		this.yDeg = Vector3.Angle(Vector3.up, base.transform.up);
		this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
	}

	private void LateUpdate()
	{
		if (Input.GetMouseButton(2) && UnityEngine.Input.GetKey(KeyCode.LeftAlt) && UnityEngine.Input.GetKey(KeyCode.LeftControl))
		{
			this.desiredDistance -= UnityEngine.Input.GetAxis("Mouse Y") * 0.02f * (float)this.zoomRate * 0.125f * Mathf.Abs(this.desiredDistance);
		}
		else if (Input.GetMouseButton(0))
		{
			this.xDeg += UnityEngine.Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
			this.yDeg -= UnityEngine.Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
			this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
			this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
			this.currentRotation = base.transform.rotation;
			this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, 0.02f * this.zoomDampening);
			base.transform.rotation = this.rotation;
			this.idleTimer = 0f;
			this.idleSmooth = 0f;
		}
		else
		{
			this.idleTimer += 0.02f;
			if (this.idleTimer > this.autoRotate && this.autoRotate > 0f)
			{
				this.idleSmooth += (0.02f + this.idleSmooth) * 0.005f;
				this.idleSmooth = Mathf.Clamp(this.idleSmooth, 0f, 1f);
				this.xDeg += this.xSpeed * Time.deltaTime * this.idleSmooth * this.autoRotateSpeed;
			}
			this.yDeg = SmoothCameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
			this.desiredRotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
			this.currentRotation = base.transform.rotation;
			this.rotation = Quaternion.Lerp(this.currentRotation, this.desiredRotation, 0.02f * this.zoomDampening * 2f);
			base.transform.rotation = this.rotation;
		}
		this.desiredDistance -= UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 0.02f * (float)this.zoomRate * Mathf.Abs(this.desiredDistance);
		this.desiredDistance = Mathf.Clamp(this.desiredDistance, this.minDistance, this.maxDistance);
		this.currentDistance = Mathf.Lerp(this.currentDistance, this.desiredDistance, 0.02f * this.zoomDampening);
		this.position = this.target.position - (this.rotation * Vector3.forward * this.currentDistance + this.targetOffset);
		base.transform.position = this.position;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public Transform target;

	public Vector3 targetOffset;

	public float distance = 5f;

	public float maxDistance = 20f;

	public float minDistance = 0.6f;

	public float xSpeed = 200f;

	public float ySpeed = 200f;

	public int yMinLimit = -80;

	public int yMaxLimit = 80;

	public int zoomRate = 40;

	public float panSpeed = 0.3f;

	public float zoomDampening = 5f;

	public float autoRotate = 1f;

	public float autoRotateSpeed = 0.1f;

	private float xDeg;

	private float yDeg;

	private float currentDistance;

	private float desiredDistance;

	private Quaternion currentRotation;

	private Quaternion desiredRotation;

	private Quaternion rotation;

	private Vector3 position;

	private float idleTimer;

	private float idleSmooth;
}
