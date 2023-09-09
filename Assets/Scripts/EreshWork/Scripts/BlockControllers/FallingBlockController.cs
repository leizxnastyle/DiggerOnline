using System;
using System.Collections.Generic;
using UnityEngine;

namespace EreshWork.Scripts.BlockControllers
{
	public class FallingBlockController : MonoBehaviour
	{
		private void Start()
		{
			this.fallingTimer = Time.time;
			if (this.curentShowBlock != null)
			{
				this.tex = ((!WorldData.Instance.BlockTextures.ContainsKey(this.blockType)) ? WorldData.Instance.BlockTextures[BlockType.HideWhenStep] : WorldData.Instance.BlockTextures[this.blockType]);
				if (this.tex == null)
				{
					FallingBlockController.fallingBlockCreated--;
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				FallingBlockController.fallingBlockCreated++;
				this.curentShowBlock.GetComponent<Renderer>().material.mainTexture = this.tex;
				this.curentShowBlock.GetComponent<Renderer>().material.color = new Color32(this.lightValue, this.lightValue, this.lightValue, byte.MaxValue);
				WorldGameObjectX.Instance.SoundRPC((int)WorldGameObjectX.Instance.BlockParametrs[(int)this.blockType].HitEffect, base.transform.position);
			}
		}

		public void SetKind(BlockKind kind, BlockType t, byte l)
		{
			this.blockType = t;
			this.lightValue = l;
			GameObject gameObject;
			if (kind.IsDefault())
			{
				gameObject = this.bo[0];
			}
			else if (kind.IsCorner())
			{
				gameObject = this.bo[1];
			}
			else if (kind.IsDiagonal())
			{
				gameObject = this.bo[2];
			}
			else if (kind.IsFence())
			{
				gameObject = this.bo[3];
			}
			else if (kind.IsHalf())
			{
				gameObject = this.bo[4];
			}
			else if (kind.IsQuarter())
			{
				gameObject = this.bo[5];
			}
			else if (kind.IsStair())
			{
				gameObject = this.bo[6];
			}
			else
			{
				gameObject = null;
			}
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				this.curentShowBlock = gameObject;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (Time.time - this.fallingTimer < 0.3f)
			{
				return;
			}
			if (collision.gameObject.name == "FallingBlock")
			{
				return;
			}
			PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayerByAvatar(collision.gameObject);
			if (playerNode != null)
			{
			}
			WorldGameObjectX.Instance.SoundRPC((int)WorldGameObjectX.Instance.BlockParametrs[(int)this.blockType].DestroyEffect, base.transform.position);
			IntVect hitPoint = new IntVect(base.transform.position.x, base.transform.position.z, base.transform.position.y);
			World.Instance.CubeDestroyEffect(hitPoint, false, this.tex);
			FallingBlockController.fallingBlockCreated--;
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void FixedUpdate()
		{
			if (base.transform.position.y < -100f)
			{
				FallingBlockController.fallingBlockCreated--;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public static int fallingBlockCreated;

		public static int maxBlockCreated = 10;

		public float fallingDamage = 0.1f;

		public BlockType blockType = BlockType.RestoreWhenStep;

		public byte lightValue = 192;

		private float fallingTimer;

		private Texture2D tex;

		public Material blockMaterial;

		private GameObject curentShowBlock;

		public List<GameObject> bo;
	}
}
