using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[Serializable]
public class FPSWalker : MonoBehaviour
{
	public FPSWalker()
	{
		this.speed = 6f;
		this.jumpSpeed = 8f;
		this.gravity = 20f;
		this.moveDirection = Vector3.zero;
	}

	public virtual void FixedUpdate()
	{
		if (this.grounded)
		{
			this.moveDirection = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), (float)0, UnityEngine.Input.GetAxis("Vertical"));
			this.moveDirection = this.transform.TransformDirection(this.moveDirection);
			this.moveDirection *= this.speed;
			if (Input.GetButton("Jump"))
			{
				this.moveDirection.y = this.jumpSpeed;
			}
		}
		this.moveDirection.y = this.moveDirection.y - this.gravity * Time.deltaTime;
		CharacterController characterController = (CharacterController)this.GetComponent(typeof(CharacterController));
		CollisionFlags collisionFlags = characterController.Move(this.moveDirection * Time.deltaTime);
		this.grounded = ((collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
	}

	public virtual void Main()
	{
	}

	public float speed;

	public float jumpSpeed;

	public float gravity;

	private Vector3 moveDirection;

	private bool grounded;
}
