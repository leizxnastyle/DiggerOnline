using System;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
	private World m_World { get; set; }

	private void Start()
	{
		this.MovePlayerToCenterOfMap();
	}

	private void Update()
	{
	}

	private void CenterWorldAroundPlayer()
	{
	}

	private void MoveMapShiftDetectionRectangle(Vector3 amountToMove)
	{
		this.m_ShiftDetectionRectangle.x = this.m_ShiftDetectionRectangle.x + amountToMove.x;
		this.m_ShiftDetectionRectangle.y = this.m_ShiftDetectionRectangle.y + amountToMove.y;
	}

	private void MovePlayerToCenterOfMap()
	{
	}

	public Transform World_Prefab;

	private Rect m_ShiftDetectionRectangle;
}
