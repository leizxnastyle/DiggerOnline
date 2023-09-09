using System;
using hd;
using UnityEngine;

public class CustomController : MonoBehaviour
{
	private void Start()
	{
		this.m_WorldData = this.World.WorldData;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		this.m_Velocity = Vector3.zero;
		this.m_Forward = base.transform.forward;
		if (Input.inputString.Contains("w"))
		{
			this.m_Velocity = this.m_Forward;
		}
		else if (Input.inputString.Contains("s"))
		{
			this.m_Velocity = -this.m_Forward;
		}
		this.m_Velocity += this.m_Gravity;
		float y;
		if (this.CollidesWithY(position, this.m_Velocity.y, out y))
		{
			this.m_Velocity.y = y;
		}
		base.transform.position += this.m_Velocity;
	}

	private bool CollidesWithX(Vector3 position, float movement, out float newVelocity)
	{
		float num = this.Speed * (float)Math.Sign(movement);
		Vector3 vector = new Vector3(position.x + num + this.m_BoxWidth, position.y, position.z);
		BlockType blockType = this.m_WorldData.GetBlockType((int)vector.x, (int)vector.y, (int)vector.z);
		if (blockType != BlockType.Air)
		{
			newVelocity = 0f;
			return true;
		}
		newVelocity = movement;
		return false;
	}

	private bool CollidesWithY(Vector3 position, float movement, out float newVelocity)
	{
		float num = this.Speed * (float)Math.Sign(movement);
		Vector3 vector = new Vector3(position.x, position.y + num + this.m_BoxWidth, position.z);
		hd.Debug.Log(string.Concat(new object[]
		{
			"Checking ",
			vector.x,
			", ",
			vector.z,
			", ",
			vector.y
		}));
		BlockType blockType = this.m_WorldData.GetBlockType((int)vector.x, (int)vector.z, (int)vector.y);
		if (blockType != BlockType.Air)
		{
			hd.Debug.Log(string.Concat(new object[]
			{
				"Block ",
				blockType,
				" hit at ",
				vector.x,
				", ",
				vector.z,
				", ",
				vector.y
			}));
			newVelocity = 0f;
			return true;
		}
		newVelocity = movement;
		return false;
	}

	private Vector3 m_Gravity = new Vector3(0f, -0.98f, 0f);

	private float m_BoxWidth = 0.5f;

	private Vector3 m_Velocity;

	private Vector3 m_Forward;

	public float Speed = 0.1f;

	public WorldGameObjectX World;

	private WorldData m_WorldData;
}
