using System;
using UnityEngine;

namespace EreshWork.Scripts.Common
{
	public class CollisionInfo
	{
		public ChunkGameObject chunk;

		public IntVect blockPos;

		public RaycastHit hit;

		public float timer;

		public bool startedKill;

		public BlockType type;

		public BlockKind kind;
	}
}
