using System;
using System.Collections.Generic;
using EreshWork.Scripts.BlockControllers;
using EreshWork.Scripts.Common;
using Photon;
using UnityEngine;

namespace EreshWork.Scripts.GameTypes
{
	public class KillBlocksController : Photon.MonoBehaviour
	{
		private void Start()
		{
			WorldGameObjectX.Instance.World.OnRemoveBlock += this.OnRemoveBlock;
		}

		private void OnDestroy()
		{
			WorldGameObjectX.Instance.World.OnRemoveBlock -= this.OnRemoveBlock;
			this.activeCollisions.Clear();
		}

		private void OnRemoveBlock(IntVect p, BlockType type, byte lightValue, BlockKind kind = BlockKind.Default)
		{
			if (FallingBlockController.fallingBlockCreated > FallingBlockController.maxBlockCreated || type == BlockType.Lava || type == BlockType.HideWhenStep)
			{
				return;
			}
			Quaternion cubePrefabs = this.GetCubePrefabs(kind);
			Vector3 position = this.SetSelectelPosition(kind, p);
			if (WorldData.Instance.GetBlockType(p.X, p.Y, p.Z - 1) == BlockType.Air)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.fallingBlockPrefab, position, cubePrefabs) as GameObject;
				gameObject.name = this.fallingBlockPrefab.name;
				gameObject.GetComponent<FallingBlockController>().SetKind(kind, type, lightValue);
			}
		}

		private void Update()
		{
			List<CollisionInfo> newList = this.TestCollisionPlayersAndBlocks();
			this.UpdateCollisionList(newList);
		}

		private List<CollisionInfo> TestCollisionPlayersAndBlocks()
		{
			List<CollisionInfo> list = new List<CollisionInfo>();
			Vector3[] array = new Vector3[]
			{
				base.transform.position + base.transform.up * 0.5f + base.transform.forward * 0.4f,
				base.transform.position + base.transform.up * 0.5f - base.transform.forward * 0.4f,
				base.transform.position + base.transform.up * 0.5f + base.transform.right * 0.4f,
				base.transform.position + base.transform.up * 0.5f - base.transform.right * 0.4f,
				base.transform.position + base.transform.up * 0.5f
			};
			Vector3 direction = -base.transform.up * 1f;
			for (int i = 0; i < 5; i++)
			{
				RaycastHit hit;
				if (Physics.Raycast(array[i], direction, out hit, direction.magnitude, 1 << LayerMask.NameToLayer("Terrain")) && hit.collider.tag != "Water")
				{
					Vector3 vector = hit.point - hit.normal * 0.1f;
					IntVect blockPos = new IntVect(vector.x, vector.z, vector.y);
					BlockType blockType = WorldData.Instance.GetBlockType(blockPos.X, blockPos.Y, blockPos.Z);
					if (blockType != BlockType.HideWhenStep && blockType != BlockType.RestoreWhenStep)
					{
						if (blockType == BlockType.Lava)
						{
							PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayerByAvatar(base.gameObject);
							playerNode.Avatar.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, new object[]
							{
								Vector3.up,
								base.gameObject.transform.position
							});
							WorldGameObjectX.Instance.PunchPlayer(playerNode.NetPlayer, Vector3.one.normalized, 1f, 770077, null, false);
						}
						else
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
				if (Time.time - keyValuePair.Value.timer > this.crashBlockAfterTime && !keyValuePair.Value.startedKill)
				{
					keyValuePair.Value.startedKill = true;
					IntVect blockPos = keyValuePair.Value.blockPos;
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
				else if (Time.time - keyValuePair.Value.timer > 2f && keyValuePair.Value.startedKill)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (int key2 in list)
			{
				if (this.activeCollisions.ContainsKey(key2))
				{
					this.activeCollisions.Remove(key2);
				}
			}
			list.Clear();
		}

		private Quaternion GetCubePrefabs(BlockKind kind)
		{
			Quaternion result = Quaternion.identity;
			switch (kind)
			{
			case BlockKind.Half:
				result = Quaternion.Euler(270f, 0f, 0f);
				break;
			default:
				switch (kind)
				{
				case (BlockKind)131:
					result = Quaternion.Euler(90f, 90f, 0f);
					break;
				case (BlockKind)132:
					result = Quaternion.Euler(90f, 270f, 0f);
					break;
				case (BlockKind)133:
					result = Quaternion.Euler(90f, 180f, 0f);
					break;
				case (BlockKind)134:
					result = Quaternion.Euler(90f, 0f, 0f);
					break;
				case (BlockKind)135:
					result = Quaternion.Euler(90f, 270f, 0f);
					break;
				case (BlockKind)136:
					result = Quaternion.Euler(90f, 90f, 0f);
					break;
				case (BlockKind)137:
					result = Quaternion.Euler(90f, 0f, 0f);
					break;
				case (BlockKind)138:
					result = Quaternion.Euler(90f, 180f, 0f);
					break;
				default:
					switch (kind)
					{
					case (BlockKind)190:
						result = Quaternion.Euler(90f, 180f, 0f);
						break;
					case (BlockKind)191:
						result = Quaternion.Euler(90f, 270f, 0f);
						break;
					case (BlockKind)192:
						result = Quaternion.Euler(90f, 90f, 0f);
						break;
					case (BlockKind)193:
						result = Quaternion.Euler(90f, 0f, 0f);
						break;
					default:
						result = Quaternion.Euler(270f, 0f, 0f);
						break;
					}
					break;
				}
				break;
			case BlockKind.DiagonalWest:
				result = Quaternion.Euler(270f, 270f, 0f);
				break;
			case BlockKind.DiagonalEast:
				result = Quaternion.Euler(270f, 90f, 0f);
				break;
			case BlockKind.DiagonalNorth:
				result = Quaternion.Euler(270f, 180f, 0f);
				break;
			case BlockKind.StairNorth:
				result = Quaternion.Euler(270f, 270f, 0f);
				break;
			case BlockKind.StairSouth:
				result = Quaternion.Euler(270f, 90f, 0f);
				break;
			case BlockKind.StairEast:
				result = Quaternion.Euler(270f, 180f, 0f);
				break;
			case BlockKind.HalfWallNorth:
				result = Quaternion.Euler(180f, 0f, 0f);
				break;
			case BlockKind.HalfWallSouth:
				result = Quaternion.Euler(0f, 0f, 0f);
				break;
			case BlockKind.HalfWallEast:
				result = Quaternion.Euler(180f, 270f, 0f);
				break;
			case BlockKind.HalfWallWest:
				result = Quaternion.Euler(180f, 90f, 0f);
				break;
			case BlockKind.Quarter:
				result = Quaternion.Euler(270f, 0f, 0f);
				break;
			case BlockKind.QuarterOnWallEast:
				result = Quaternion.Euler(180f, 270f, 0f);
				break;
			case BlockKind.QuarterOnWallNorth:
				result = Quaternion.Euler(180f, 0f, 0f);
				break;
			case BlockKind.QuarterOnWallSouth:
				result = Quaternion.Euler(0f, 0f, 0f);
				break;
			case BlockKind.QuarterOnWallWest:
				result = Quaternion.Euler(180f, 90f, 0f);
				break;
			case BlockKind.FenceOnWallSouthNorth:
				result = Quaternion.Euler(180f, 0f, 0f);
				break;
			case BlockKind.FenceOnWallEastWest:
				result = Quaternion.Euler(180f, 90f, 0f);
				break;
			case BlockKind.DiagonalOnWallWestRight:
				result = Quaternion.Euler(0f, 270f, 90f);
				break;
			case BlockKind.DiagonalOnWallWestLeft:
				result = Quaternion.Euler(0f, 0f, 90f);
				break;
			case BlockKind.DiagonalOnWallWestTop:
				result = Quaternion.Euler(270f, 90f, 0f);
				break;
			case BlockKind.DiagonalOnWallWestBottom:
				result = Quaternion.Euler(0f, 270f, 180f);
				break;
			case BlockKind.DiagonalOnWallEastRight:
				result = Quaternion.Euler(0f, 90f, 90f);
				break;
			case BlockKind.DiagonalOnWallEastLeft:
				result = Quaternion.Euler(0f, 90f, 270f);
				break;
			case BlockKind.DiagonalOnWallEastTop:
				result = Quaternion.Euler(270f, 270f, 0f);
				break;
			case BlockKind.DiagonalOnWallEastBottom:
				result = Quaternion.Euler(0f, 90f, 180f);
				break;
			case BlockKind.DiagonalOnWallSouthRight:
				result = Quaternion.Euler(0f, 90f, 270f);
				break;
			case BlockKind.DiagonalOnWallSouthLeft:
				result = Quaternion.Euler(0f, 270f, 90f);
				break;
			case BlockKind.DiagonalOnWallSouthTop:
				result = Quaternion.Euler(270f, 0f, 0f);
				break;
			case BlockKind.DiagonalOnWallSouthBottom:
				result = Quaternion.Euler(0f, 180f, 180f);
				break;
			case BlockKind.DiagonalOnWallNorthRight:
				result = Quaternion.Euler(0f, 0f, 90f);
				break;
			case BlockKind.DiagonalOnWallNorthLeft:
				result = Quaternion.Euler(0f, 0f, -90f);
				break;
			case BlockKind.DiagonalOnWallNorthTop:
				result = Quaternion.Euler(270f, 180f, 0f);
				break;
			case BlockKind.DiagonalOnWallNorthBottom:
				result = Quaternion.Euler(0f, 0f, 180f);
				break;
			case BlockKind.CornerEast:
				result = Quaternion.Euler(270f, 270f, 0f);
				break;
			case BlockKind.CornerNorth:
				result = Quaternion.Euler(270f, 180f, 0f);
				break;
			case BlockKind.CornerSouth:
				result = Quaternion.Euler(270f, 90f, 0f);
				break;
			case BlockKind.EastFenceWest:
			case BlockKind.EastFenceEast:
			case BlockKind.EastFenceNorth:
			case BlockKind.EastFenceSouth:
				result = Quaternion.Euler(0f, 90f, 0f);
				break;
			case BlockKind.WestFenceWest:
			case BlockKind.WestFenceEast:
			case BlockKind.WestFenceNorth:
			case BlockKind.WestFenceSouth:
				result = Quaternion.Euler(0f, 90f, 0f);
				break;
			case BlockKind.NorthFenceWest:
			case BlockKind.NorthFenceEast:
			case BlockKind.NorthFenceNorth:
			case BlockKind.NorthFenceSouth:
				result = Quaternion.Euler(0f, 0f, 0f);
				break;
			case BlockKind.SouthFenceWest:
			case BlockKind.SouthFenceEast:
			case BlockKind.SouthFenceNorth:
			case BlockKind.SouthFenceSouth:
				result = Quaternion.Euler(0f, 0f, 0f);
				break;
			}
			return result;
		}

		private Vector3 SetSelectelPosition(BlockKind kind, IntVect position)
		{
			switch (kind)
			{
			case BlockKind.Half:
				return new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
			default:
				switch (kind)
				{
				case (BlockKind)129:
					return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
				default:
					switch (kind)
					{
					case (BlockKind)190:
					case (BlockKind)191:
					case (BlockKind)192:
					case (BlockKind)193:
						break;
					default:
						return new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
					}
					break;
				case (BlockKind)131:
				case (BlockKind)132:
				case (BlockKind)133:
				case (BlockKind)134:
				case (BlockKind)135:
				case (BlockKind)136:
				case (BlockKind)137:
				case (BlockKind)138:
					break;
				case (BlockKind)144:
					return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.75f, (float)position.Y + 0.5f);
				}
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 1f, (float)position.Y + 0.5f);
			case BlockKind.HalfWallNorth:
			case BlockKind.HalfWallSouth:
			case BlockKind.HalfWallEast:
			case BlockKind.HalfWallWest:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.Quarter:
				return new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
			case BlockKind.QuarterOnWallEast:
				return new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.QuarterOnWallNorth:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			case BlockKind.QuarterOnWallSouth:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 1f);
			case BlockKind.QuarterOnWallWest:
				return new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.FenceOnWallSouthNorth:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 1f);
			case BlockKind.FenceOnWallEastWest:
				return new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallWestRight:
				return new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallWestLeft:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			case BlockKind.DiagonalOnWallWestBottom:
				return new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallEastRight:
				return new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallEastLeft:
				return new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallEastBottom:
				return new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallSouthRight:
				return new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallSouthLeft:
				return new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			case BlockKind.DiagonalOnWallSouthBottom:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 1f);
			case BlockKind.DiagonalOnWallNorthRight:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			case BlockKind.DiagonalOnWallNorthLeft:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			case BlockKind.DiagonalOnWallNorthBottom:
				return new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			case BlockKind.FenceWest:
				return new Vector3((float)position.X + 0.17f, (float)position.Z, (float)position.Y + 0.17f);
			case BlockKind.FenceEast:
				return new Vector3((float)position.X + 0.83f, (float)position.Z, (float)position.Y + 0.17f);
			case BlockKind.FenceNorth:
				return new Vector3((float)position.X + 0.17f, (float)position.Z, (float)position.Y + 0.83f);
			case BlockKind.FenceSouth:
				return new Vector3((float)position.X + 0.83f, (float)position.Z, (float)position.Y + 0.17f);
			case BlockKind.EastFenceWest:
				return new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.83f);
			case BlockKind.EastFenceEast:
				return new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.17f);
			case BlockKind.EastFenceNorth:
				return new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.83f);
			case BlockKind.EastFenceSouth:
				return new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.17f);
			case BlockKind.WestFenceWest:
				return new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.83f);
			case BlockKind.WestFenceEast:
				return new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.17f);
			case BlockKind.WestFenceNorth:
				return new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.83f);
			case BlockKind.WestFenceSouth:
				return new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.17f);
			case BlockKind.NorthFenceWest:
				return new Vector3((float)position.X + 0.17f, (float)position.Z + 0.17f, (float)position.Y);
			case BlockKind.NorthFenceEast:
				return new Vector3((float)position.X + 0.83f, (float)position.Z + 0.17f, (float)position.Y);
			case BlockKind.NorthFenceNorth:
				return new Vector3((float)position.X + 0.17f, (float)position.Z + 0.83f, (float)position.Y);
			case BlockKind.NorthFenceSouth:
				return new Vector3((float)position.X + 0.83f, (float)position.Z + 0.83f, (float)position.Y);
			case BlockKind.SouthFenceWest:
				return new Vector3((float)position.X + 0.17f, (float)position.Z + 0.17f, (float)position.Y);
			case BlockKind.SouthFenceEast:
				return new Vector3((float)position.X + 0.83f, (float)position.Z + 0.17f, (float)position.Y);
			case BlockKind.SouthFenceNorth:
				return new Vector3((float)position.X + 0.17f, (float)position.Z + 0.83f, (float)position.Y);
			case BlockKind.SouthFenceSouth:
				return new Vector3((float)position.X + 0.83f, (float)position.Z + 0.83f, (float)position.Y);
			}
		}

		public const float RESTORE_AFTER_TIME = 1.5f;

		[NonSerialized]
		public float crashBlockAfterTime = 0.5f;

		[NonSerialized]
		public GameObject fallingBlockPrefab;

		private Dictionary<int, CollisionInfo> activeCollisions = new Dictionary<int, CollisionInfo>();

		private Dictionary<int, CollisionInfo> restoreBlocks = new Dictionary<int, CollisionInfo>();
	}
}
