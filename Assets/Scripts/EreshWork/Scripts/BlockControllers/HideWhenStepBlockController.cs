using System;
using System.Collections;
using System.Collections.Generic;
using EreshWork.Scripts.Common;
using UnityEngine;

namespace EreshWork.Scripts.BlockControllers
{
	public class HideWhenStepBlockController : MonoBehaviour
	{
		private IEnumerator Start()
		{
			while (!WorldGameObjectX.Instance.IsWorldGenerated)
			{
				yield return 0;
			}
			WorldGameObjectX.Instance.World.OnRemoveBlock += this.OnRemoveBlock;
			yield break;
		}

		private void OnRemoveBlock(IntVect p, BlockType type, byte lightValue, BlockKind kind)
		{
			if (type == BlockType.HideWhenStep && WorldData.Instance.GetBlockType(p.X, p.Y, p.Z - 1) == BlockType.Air)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.fallingBlockPrefab, new Vector3((float)p.X + 0.5f, (float)p.Z + 0.5f, (float)p.Y + 0.5f), Quaternion.identity) as GameObject;
				gameObject.name = this.fallingBlockPrefab.name;
				gameObject.GetComponent<FallingBlockController>().lightValue = lightValue;
			}
		}

		private void Update()
		{
			if (WorldGameObjectX.Instance == null)
			{
				return;
			}
			if (!WorldGameObjectX.Instance.IsWorldGenerated)
			{
				return;
			}
			List<CollisionInfo> newList = this.TestCollisionPlayersAndBlocks();
			this.UpdateCollisionList(newList);
		}

		private List<CollisionInfo> TestCollisionPlayersAndBlocks()
		{
			List<CollisionInfo> list = new List<CollisionInfo>();
			if (WorldGameObjectX.Instance.MainPlayer == null)
			{
				return list;
			}
			Transform transform = WorldGameObjectX.Instance.MainPlayer.transform;
			Vector3[] array = new Vector3[]
			{
				transform.position + transform.forward * 0.3f,
				transform.position - transform.forward * 0.3f,
				transform.position + transform.right * 0.3f,
				transform.position - transform.right * 0.3f
			};
			Vector3 direction = -transform.up * 0.2f;
			for (int i = 0; i < 4; i++)
			{
				RaycastHit hit;
				if (Physics.Raycast(array[i], direction, out hit, direction.magnitude, 1 << LayerMask.NameToLayer("Terrain")) && hit.collider.tag != "Water")
				{
					Vector3 vector = hit.point - hit.normal * 0.1f;
					IntVect blockPos = new IntVect(vector.x, vector.z, vector.y);
					BlockType blockType = WorldData.Instance.GetBlockType(blockPos.X, blockPos.Y, blockPos.Z);
					if (blockType == BlockType.HideWhenStep)
					{
						list.Add(new CollisionInfo
						{
							blockPos = blockPos,
							timer = Time.time,
							chunk = hit.collider.GetComponent<ChunkGameObject>(),
							hit = hit,
							type = blockType,
							kind = WorldData.Instance.GetBlockKind(blockPos.X, blockPos.Y, blockPos.Z)
						});
					}
				}
			}
			return list;
		}

		private void UpdateCollisionList(List<CollisionInfo> newList)
		{
			if (newList == null)
			{
				return;
			}
			for (int i = 0; i < newList.Count; i++)
			{
				CollisionInfo collisionInfo = newList[i];
				int key = collisionInfo.blockPos.X * 1000000 + collisionInfo.blockPos.Y * 1000 + collisionInfo.blockPos.Z;
				if (!this.activeCollisions.ContainsKey(key))
				{
					this.activeCollisions[key] = collisionInfo;
				}
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, CollisionInfo> keyValuePair in this.activeCollisions)
			{
				if (Time.time - keyValuePair.Value.timer > this.hideAfterTime && !keyValuePair.Value.startedKill)
				{
					keyValuePair.Value.startedKill = true;
					IntVect blockPos = keyValuePair.Value.blockPos;
					list.Add(keyValuePair.Key);
					BlockType blockType = WorldData.Instance.GetBlockType(blockPos.X, blockPos.Y, blockPos.Z - 1);
					if (blockType != BlockType.Air)
					{
						WorldGameObjectX.Instance.World.Dig2(blockPos, keyValuePair.Value.hit.point, 4f, true);
					}
					else
					{
						WorldGameObjectX.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, new object[]
						{
							blockPos.X,
							blockPos.Y,
							blockPos.Z,
							true,
							true
						});
					}
				}
			}
			foreach (int key2 in list)
			{
				if (this.activeCollisions.ContainsKey(key2))
				{
					this.activeCollisions[key2].timer = Time.time;
					this.restoreBlocks[key2] = this.activeCollisions[key2];
					this.activeCollisions.Remove(key2);
				}
			}
			list.Clear();
			foreach (KeyValuePair<int, CollisionInfo> keyValuePair2 in this.restoreBlocks)
			{
				if (Time.time - keyValuePair2.Value.timer > this.showAfterHide && keyValuePair2.Value.startedKill)
				{
					keyValuePair2.Value.startedKill = false;
					IntVect blockPos2 = keyValuePair2.Value.blockPos;
					list.Add(keyValuePair2.Key);
					WorldGameObjectX.Instance.World.AddBlockAt(keyValuePair2.Value.blockPos, keyValuePair2.Value.type, keyValuePair2.Value.kind);
				}
			}
			foreach (int key3 in list)
			{
				if (this.restoreBlocks.ContainsKey(key3))
				{
					this.restoreBlocks.Remove(key3);
				}
			}
		}

		private void OnDestroy()
		{
			if (WorldGameObjectX.Instance != null && WorldGameObjectX.Instance.World != null)
			{
				WorldGameObjectX.Instance.World.OnRemoveBlock -= this.OnRemoveBlock;
			}
		}

		[SerializeField]
		private GameObject fallingBlockPrefab;

		[SerializeField]
		private float hideAfterTime = 0.5f;

		[SerializeField]
		private float showAfterHide = 2.5f;

		private Dictionary<int, CollisionInfo> activeCollisions = new Dictionary<int, CollisionInfo>();

		private Dictionary<int, CollisionInfo> restoreBlocks = new Dictionary<int, CollisionInfo>();
	}
}
