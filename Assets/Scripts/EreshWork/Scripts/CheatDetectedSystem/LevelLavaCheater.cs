using System;
using UnityEngine;

namespace EreshWork.Scripts.CheatDetectedSystem
{
	public class LevelLavaCheater : MonoBehaviour
	{
		private void Start()
		{
			for (int i = 0; i < WorldData.Instance.ChunksWide; i++)
			{
				for (int j = 0; j < WorldData.Instance.ChunksHigh; j++)
				{
					for (int k = 0; k < WorldData.Instance.ChunksDeep; k++)
					{
						Chunk chunk = WorldData.Instance.Chunks[i, j, k];
						for (int l = 0; l < WorldData.Instance.ChunkBlockWidth; l++)
						{
							for (int m = 0; m < WorldData.Instance.ChunkBlockHeight; m++)
							{
								for (int n = 0; n < WorldData.Instance.ChunkBlockDepth; n++)
								{
									BlockType blockType = chunk.GetBlockType(l, m, n);
									if (blockType == BlockType.Lava)
									{
										int num = k * WorldData.Instance.ChunkBlockDepth + n;
										if (this.lowLavaLevel == -1 || this.lowLavaLevel > num)
										{
											this.lowLavaLevel = num;
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.lowLavaLevel == -1)
			{
				this.lowLavaLevel = 0;
			}
			UnityEngine.Debug.Log("Low laval level is " + this.lowLavaLevel);
		}

		private void FixedUpdate()
		{
			if (this.timer == -1f && base.transform.position.y <= (float)(this.lowLavaLevel + 2))
			{
				this.timer = 0f;
			}
			if (this.timer != -1f)
			{
				this.timer += Time.deltaTime;
				if (this.timer >= (float)this.cheaterAfterTime)
				{
					this.ThisPlayerIsCheater();
				}
				if (base.transform.position.y > (float)(this.lowLavaLevel + 2))
				{
					this.timer = -1f;
				}
			}
		}

		private void ThisPlayerIsCheater()
		{
			UnityEngine.Debug.Log("Player " + PhotonNetwork.playerName + " is cheater");
		}

		private int lowLavaLevel = -1;

		private int cheaterAfterTime = 30;

		private float timer = -1f;
	}
}
