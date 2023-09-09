using System;
using UnityEngine;

public class HG_Cheat_Wall_Detector : MonoBehaviour
{
	public event Action isTriggerWallOn;

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Collider>().gameObject.layer == 9 && base.transform.parent.GetComponent<HG_Cheat_Detector>())
		{
			BlockType blockType = WorldData.Instance.GetBlockType((int)base.transform.parent.transform.position.x, (int)base.transform.parent.transform.position.z, (int)base.transform.parent.transform.position.y);
			if (blockType == BlockType.Water)
			{
				return;
			}
			BlockType blockType2 = WorldData.Instance.GetBlockType((int)base.transform.parent.transform.position.x, (int)base.transform.parent.transform.position.z + 1, (int)base.transform.parent.transform.position.y);
			if (blockType2 == BlockType.Water)
			{
				return;
			}
			BlockType blockType3 = WorldData.Instance.GetBlockType((int)base.transform.parent.transform.position.x + 1, (int)base.transform.parent.transform.position.z, (int)base.transform.parent.transform.position.y);
			if (blockType3 == BlockType.Water)
			{
				return;
			}
			BlockType blockType4 = WorldData.Instance.GetBlockType((int)base.transform.parent.transform.position.x - 1, (int)base.transform.parent.transform.position.z, (int)base.transform.parent.transform.position.y);
			if (blockType4 == BlockType.Water)
			{
				return;
			}
			BlockType blockType5 = WorldData.Instance.GetBlockType((int)base.transform.parent.transform.position.x, (int)base.transform.parent.transform.position.z, (int)base.transform.parent.transform.position.y + 1);
			if (blockType5 == BlockType.Water)
			{
				return;
			}
			BlockType blockType6 = WorldData.Instance.GetBlockType((int)base.transform.parent.transform.position.x, (int)base.transform.parent.transform.position.z, (int)base.transform.parent.transform.position.y - 1);
			if (blockType6 == BlockType.Water)
			{
				return;
			}
			if (this.isTriggerWallOn != null)
			{
				this.isTriggerWallOn();
			}
		}
	}
}
