using System;
using System.Collections.Generic;
using UnityEngine;

public class TNT : EntityBase
{
	public override void OnButtonE(string player_name)
	{
		if (Info.Instance.GameMode != "BUILDING")
		{
			return;
		}
		if (this.Creator == player_name || Level.Instance.IsAdmin(null))
		{
			if (!this.is_start_activate)
			{
				WorldGameObjectX.Instance.TakeEnity(base.gameObject);
			}
		}
		else
		{
			Chat.SendInfoF(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS", null), false);
		}
	}

	public override void OnButtonF(string player_name)
	{
		if (Info.Instance.GameMode != "BUILDING")
		{
			return;
		}
		if (Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null))
		{
			base.photonView.RPC("DetonateRPC", PhotonTargets.MasterClient, new object[0]);
			base.photonView.RPC("DetonateShowToOther", PhotonTargets.All, new object[0]);
		}
		else
		{
			Chat.SendWarning(Localize.GetText("key4005", null), false);
		}
	}

	[PunRPC]
	private void DetonateShowToOther()
	{
		this.TntFuse.GetComponent<ParticleEmitter>().enabled = true;
		this.is_start_activate = true;
	}

	[PunRPC]
	private void DetonateRPC()
	{
		if (!this.is_start_detonating)
		{
			base.Invoke("Detonate", 5f);
			this.is_start_detonating = true;
		}
	}

	public void ChainDetonate()
	{
		if (!this.is_start_detonating)
		{
			base.Invoke("Detonate", 0.6f);
			this.is_start_detonating = true;
		}
	}

	protected override void Destruction()
	{
		if (this.is_start_activate)
		{
			if (!WorldGameObjectX.Instance.IsWorldGenerated)
			{
				this.DestroyAreaAt((int)base.gameObject.transform.position.x, (int)base.gameObject.transform.position.z, (int)base.gameObject.transform.position.y, 4, false);
			}
			else
			{
				UnityEngine.Object.Instantiate(this.DetonateEffect, base.gameObject.transform.position, Quaternion.identity);
				this.DestroyAreaAt((int)base.gameObject.transform.position.x, (int)base.gameObject.transform.position.z, (int)base.gameObject.transform.position.y, 4, true);
			}
		}
	}

	public void Detonate()
	{
		this.SelfDelete();
		this.ChainReaction(new List<EntityBase>(new EntityBase[]
		{
			this
		}));
	}

	public void ChainReaction(List<EntityBase> skipEntities)
	{
		foreach (EntityBase entityBase in EntityBase.Entities)
		{
			if (!(entityBase == this))
			{
				if (entityBase.Type == EntityType.TNT)
				{
					if ((base.gameObject.transform.position - entityBase.gameObject.transform.position).sqrMagnitude < 64f)
					{
						entityBase.GetComponent<TNT>().ChainDetonate();
					}
				}
				else if (entityBase.Type == EntityType.ATOMBOMB)
				{
					if ((base.gameObject.transform.position - entityBase.gameObject.transform.position).sqrMagnitude < 64f)
					{
						entityBase.GetComponent<AviaBomb>().ChainDetonate();
					}
				}
				else if ((base.gameObject.transform.position - entityBase.gameObject.transform.position).sqrMagnitude < 64f)
				{
					entityBase.SelfDelete();
				}
			}
		}
	}

	public void DestroyAreaAt(int blockX, int blockY, int blockZ, int radius, bool is_world_generated)
	{
		for (int i = blockX - radius; i <= blockX + radius; i++)
		{
			for (int j = blockY - radius; j <= blockY + radius; j++)
			{
				for (int k = blockZ - radius; k <= blockZ + radius; k++)
				{
					if (Vector3.Distance(new Vector3((float)blockX, (float)blockY, (float)blockZ), new Vector3((float)i, (float)j, (float)k)) <= (float)radius)
					{
						if (is_world_generated)
						{
							World.Instance.CubeDestroyEffect(new IntVect(i, j, k), true, null);
							WorldData.Instance.SetBlockTypeWithRegeneration(i, j, k, BlockType.Air, BlockKind.Default);
							if (Level.Instance.IsAdmin(null))
							{
								World.Instance.CurLevelDelta.Add(BlockType.Air, i, j, k, false, BlockKind.Default);
							}
						}
						else
						{
							WorldGameObjectX.Instance.LevelDeltaLoading.Add(BlockType.Air, i, j, k, false, BlockKind.Default);
						}
					}
				}
			}
		}
		World.Instance.Lighting.RecalculateLightingAroundBlock(blockX, blockY, blockZ, radius, true);
	}

	public GameObject DetonateEffect;

	public GameObject TntFuse;

	public bool is_start_detonating;

	private bool is_start_activate;
}
