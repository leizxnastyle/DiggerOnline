using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class World : IWorld
{
	public World()
	{
	}

	public World(WorldData worldData, ITerrainGenerator terrainGenerator, ILightProcessor lightProcessor, IMeshGenerator meshGenerator, IWorldDecorator worldDecorator)
	{
		World.Instance = this;
		this._WorldData = worldData;
		this.Lighting = lightProcessor;
		this.MeshGen = meshGenerator;
		this._WorldDecorator = worldDecorator;
		this._TerrainGenerator = terrainGenerator;
	}

	public event Action<IntVect, BlockType, byte, BlockKind> OnRemoveBlock;

	public void InitializeGridChunks()
	{
		this._WorldData.InitializeGridChunks();
	}

	public void GenerateWorld()
	{
		PChunkList allChunks = this._WorldData.AllChunks;
		DateTime now = DateTime.Now;
		this.GenerateTerrain(allChunks);
		UnityEngine.Debug.Log(DateTime.Now - now);
		this.GenerateWorldDecorations(allChunks);
		UnityEngine.Debug.Log(DateTime.Now - now);
		this.GenerateLighting();
		UnityEngine.Debug.Log(DateTime.Now - now);
		allChunks.Sort(new Comparison<Chunk>(this.ChunksComparedByDistanceFromMapCenter));
		this.MeshGen.GenerateLandMeshes(allChunks);
		World.ClearRegenerationLandStatus(allChunks);
		UnityEngine.Debug.Log(DateTime.Now - now);
		allChunks.Release();
	}

	public void RegenerateAllLight()
	{
		PChunkList allChunks = this._WorldData.AllChunks;
		World.SetRegenerationStatus(allChunks);
		this.GenerateLighting();
		allChunks.Release();
	}

	public IEnumerator GenerateWorldCourutin()
	{
		PChunkList allChunks = this._WorldData.AllChunks;
		for (int i = 0; i < allChunks.Count; i++)
		{
			this._TerrainGenerator.GenerateTerrain(allChunks[i]);
			MainMenu.Instance.SetLoadingText("LOADING_LANDSCAPE_GENERATION", " " + i * 100 / allChunks.Count + "%");
			if (allChunks[i].Z == 0)
			{
				yield return 0;
			}
		}
		for (int j = 0; j < allChunks.Count; j++)
		{
			this._WorldDecorator.GenerateDecorationsForChunk(allChunks[j]);
			MainMenu.Instance.SetLoadingText("LOADING_DECORATIONS_GENERATION", " " + j * 100 / allChunks.Count + "%");
			if (allChunks[j].Z == 0)
			{
				yield return 0;
			}
		}
		allChunks.Release();
		yield return WorldGameObjectX.Instance.StartCoroutine(this.SaveMapToServer(App.Instance.Settings.slotID, null));
		this.ZipLevel();
		IEnumerator e = this.LoadWorldProcess();
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	public IEnumerator LoadWorldProcess()
	{
		KGUI.SetNodes("loading.button_fight", true, false);
		PChunkList allChunks = this._WorldData.AllChunks;
		allChunks.Sort(new Comparison<Chunk>(this.ChunksComparedByDistanceFromMapCenter));
		MainMenu.Instance.SetLoadingText("LOADING_LANDSCAPE_LIGHTING", string.Empty);
		yield return 0;
		float t = Time.realtimeSinceStartup;
		World.Instance.Lighting.InitializeLighting();
		for (int i = 0; i < allChunks.Count; i++)
		{
			World.Instance.Lighting.InitializeLightingChunk(allChunks[i]);
			MainMenu.Instance.SetLoadingText("LOADING_LANDSCAPE_LIGHTING", " " + i * 100 / allChunks.Count + "%");
			if (Time.realtimeSinceStartup - t >= 0.3f)
			{
				t = Time.realtimeSinceStartup;
				yield return 0;
			}
		}
		yield return 0;
		MainMenu.Instance.SetLoadingText("LOADING_MAP_CREATION", string.Empty);
		yield return 0;
		for (int j = 0; j < allChunks.Count; j++)
		{
			this.MeshGen.GenerateLandMesh(allChunks[j]);
			this.MeshGen.GenerateGlassMesh(allChunks[j]);
			this.MeshGen.GenerateWaterMesh(allChunks[j]);
			MainMenu.Instance.SetLoadingText("LOADING_MAP_CREATION", " " + j * 100 / allChunks.Count + "%");
			if (Time.realtimeSinceStartup - t >= 0.3f)
			{
				t = Time.realtimeSinceStartup;
				yield return 0;
			}
		}
		yield return 0;
		World.ClearRegenerationLandStatus(allChunks);
		World.ClearRegenerationWaterStatus(allChunks);
		World.ClearRegenerationGlassStatus(allChunks);
		allChunks.Release();
		WorldGameObjectX.Instance.CreateFinishedChunk();
		KGUI.SetNodes("loading.button_fight", false, false);
		WorldGameObjectX.Instance.CompleteWorldGeneration();
		yield break;
	}

	public void ZipLevel()
	{
		this.zipedLevel.level = new byte[2 * this._WorldData.ChunksWide * this._WorldData.ChunksHigh * this._WorldData.ChunksDeep * WorldData.Instance.m_ChunkBufferLength];
		int num = 0;
		for (int i = 0; i < this._WorldData.ChunksWide; i++)
		{
			for (int j = 0; j < this._WorldData.ChunksHigh; j++)
			{
				for (int k = 0; k < this._WorldData.ChunksDeep; k++)
				{
					byte[] blocksBuffer = this._WorldData.Chunks[i, j, k].GetBlocksBuffer();
					blocksBuffer.CopyTo(this.zipedLevel.level, num);
					num += blocksBuffer.Length;
					byte[] blocksKindBuffer = this._WorldData.Chunks[i, j, k].GetBlocksKindBuffer();
					blocksKindBuffer.CopyTo(this.zipedLevel.level, num);
					num += blocksKindBuffer.Length;
				}
			}
		}
		new Thread(new ThreadStart(this.zipedLevel.Run))
		{
			Priority = System.Threading.ThreadPriority.BelowNormal
		}.Start();
	}

	public IEnumerator SaveMapToServer(int slotID, Action endCallback = null)
	{
		UnityEngine.Debug.Log("Save world to slot " + slotID);
		MainMenu.Instance.ShowLoading("LOADING_SAVING_MAP", string.Empty);
		yield return 0;
		string error = null;
		MemoryStream ms = new MemoryStream();
		BinaryWriter bw = new BinaryWriter(ms);
		bw.Write(5);
		bw.Write(this._WorldData.ChunkBlockWidth);
		bw.Write(this._WorldData.ChunkBlockHeight);
		bw.Write(this._WorldData.ChunkBlockDepth);
		bw.Write(this._WorldData.ChunksWide);
		bw.Write(this._WorldData.ChunksHigh);
		bw.Write(this._WorldData.ChunksDeep);
		bw.Write((byte)App.Instance.Settings.mapType);
		bw.Write(VKAPI.INSTANCE._viewerId);
		bw.Write(0);
		if (RestoreBlockController.RestoreBlock.Count != 0)
		{
			for (int i = 0; i < RestoreBlockController.RestoreBlock.Count; i++)
			{
				this.CurLevelDelta.Add(BlockType.RestoreWhenStep, RestoreBlockController.RestoreBlock[i].X, RestoreBlockController.RestoreBlock[i].Y, RestoreBlockController.RestoreBlock[i].Z, false, BlockKind.Default);
			}
		}
		PChunkList allChunks = this._WorldData.AllChunks;
		foreach (Chunk chunk in allChunks)
		{
			bw.Write(chunk.GetBlocksBuffer());
			bw.Write(chunk.GetBlocksKindBuffer());
		}
		allChunks.Release();
		byte[] enityBuffer = this.GetEntityBuffer();
		bw.Write(enityBuffer.Length);
		bw.Write(enityBuffer);
		byte[] buffer = ms.ToArray();
		byte[] zipBuffer = Utils.ZipByte(buffer);
		MemoryStream ms2 = new MemoryStream();
		BinaryWriter bw2 = new BinaryWriter(ms2);
		bw2.Write(buffer.Length);
		bw2.Write(zipBuffer);
		string fileName = VKAPI.INSTANCE._viewerId + ".map";
		WWWForm saveMapToSlotForm = new WWWForm();
		saveMapToSlotForm.AddField("user_id", VKAPI.INSTANCE._viewerId.ToString());
		saveMapToSlotForm.AddField("slot_id", slotID);
		saveMapToSlotForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		saveMapToSlotForm.AddBinaryData("buffer", ms2.ToArray(), fileName);
		WWW saveMapToSlot = new WWW(SettingsManager.ServerURL[1] + "save_map_to_slot.php", saveMapToSlotForm);
		yield return saveMapToSlot;
		if (saveMapToSlot.error == null)
		{
			if (saveMapToSlot.text == "OK")
			{
				saveMapToSlotForm = new WWWForm();
				saveMapToSlotForm.AddField("user_id", VKAPI.INSTANCE._viewerId.ToString());
				saveMapToSlotForm.AddField("slot_id", slotID);
				saveMapToSlotForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
				saveMapToSlot = new WWW(SettingsManager.ServerURL[0] + "update_slot_info.php", saveMapToSlotForm);
				yield return saveMapToSlot;
				UnityEngine.Debug.Log(saveMapToSlot.text);
			}
			else
			{
				error = saveMapToSlot.text;
			}
		}
		else
		{
			error = saveMapToSlot.error;
		}
		if (WorldGameObjectX.Instance.MainPlayerNode.Avatar != null)
		{
			MainMenu.Instance.HideLoading();
		}
		if (error == null)
		{
			MainMenu.Instance.HideMenu();
		}
		else
		{
			UnityEngine.Debug.Log(error);
			MainMenu.Instance.ShowHint("Не удалось сохранить карту!\nОшибка:\n" + error, false);
		}
		if (endCallback != null)
		{
			endCallback();
		}
		yield break;
	}

	private void GenerateWorldDecorations(PChunkList chunks)
	{
		this._WorldDecorator.GenerateWorldDecorations(chunks);
	}

	public static void ClearRegenerationLandStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsRegeneration = false;
		}
	}

	public static void ClearRegenerationGlassStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsGlassRegen = false;
		}
	}

	public static void ClearRegenerationWaterStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsWaterRegen = false;
		}
	}

	private static void SetRegenerationStatus(IEnumerable<Chunk> chunks)
	{
		foreach (Chunk chunk in chunks)
		{
			chunk.NeedsRegeneration = true;
		}
	}

	private void GenerateLighting()
	{
		UnityEngine.Debug.Log("GenerateLighting");
		PChunkList allChunks = this._WorldData.AllChunks;
		this.Lighting.InitializeLighting();
		for (int i = 0; i < allChunks.Count; i++)
		{
			this.Lighting.InitializeLightingChunk(allChunks[i]);
		}
		allChunks.Release();
	}

	private void GenerateTerrain(PChunkList chunks)
	{
		this._TerrainGenerator.GenerateChunkTerrain(chunks);
	}

	public int ChunksComparedByDistanceFromMapCenter(Chunk firstChunk, Chunk secondChunk)
	{
		Vector3 b = new Vector3((float)this._WorldData.CenterChunkX, (float)this._WorldData.CenterChunkY, 0f);
		return Vector3.Distance(new Vector3((float)firstChunk.X, (float)firstChunk.Y, (float)firstChunk.Z), b).CompareTo((float)((int)Vector3.Distance(new Vector3((float)secondChunk.X, (float)secondChunk.Y, (float)secondChunk.Z), b)));
	}

	public void RegenerateChunks(int chunkX, int chunkY, int chunkZ)
	{
		PChunkList chunksNeedingLandRegeneration = this._WorldData.ChunksNeedingLandRegeneration;
		if (chunksNeedingLandRegeneration.Count != 0)
		{
			Chunk item = this._WorldData.Chunks[chunkX, chunkY, chunkZ];
			if (chunksNeedingLandRegeneration.Contains(item))
			{
				chunksNeedingLandRegeneration.Remove(item);
				chunksNeedingLandRegeneration.Insert(0, item);
			}
			this.RegenerateLandChunks(chunksNeedingLandRegeneration);
		}
		chunksNeedingLandRegeneration.Release();
	}

	public void RegenerateChunks()
	{
		PChunkList chunksNeedingLandRegeneration = this._WorldData.ChunksNeedingLandRegeneration;
		if (chunksNeedingLandRegeneration.Count != 0)
		{
			this.RegenerateLandChunks(chunksNeedingLandRegeneration);
		}
		chunksNeedingLandRegeneration.Release();
		PChunkList chunksNeedingWaterRegeneration = this._WorldData.ChunksNeedingWaterRegeneration;
		if (chunksNeedingWaterRegeneration.Count != 0)
		{
			this.RegenerateWaterChunks(chunksNeedingWaterRegeneration);
		}
		chunksNeedingWaterRegeneration.Release();
		PChunkList chunksNeedingGlassRegeneration = this._WorldData.ChunksNeedingGlassRegeneration;
		if (chunksNeedingGlassRegeneration.Count != 0)
		{
			this.RegenerateGlassChunks(chunksNeedingGlassRegeneration);
		}
		chunksNeedingGlassRegeneration.Release();
	}

	public void RegenerateGlassChunks(PChunkList chunksNeedingRegeneration)
	{
		PChunkList pchunkList = PChunkList.Acquire();
		foreach (Chunk chunk in chunksNeedingRegeneration)
		{
			if (!chunk.GlassRegeneration)
			{
				pchunkList.Add(chunk);
			}
		}
		this.MeshGen.GenerateGlassMeshes(pchunkList);
		World.ClearRegenerationGlassStatus(pchunkList);
		foreach (Chunk chunk2 in pchunkList)
		{
			chunk2.GlassRegeneration = true;
		}
		pchunkList.Release();
	}

	public void RegenerateWaterChunks(PChunkList chunksNeedingRegeneration)
	{
		PChunkList pchunkList = PChunkList.Acquire();
		foreach (Chunk chunk in chunksNeedingRegeneration)
		{
			if (!chunk.WaterRegeneration)
			{
				pchunkList.Add(chunk);
			}
		}
		this.MeshGen.GenerateWaterMeshes(pchunkList);
		World.ClearRegenerationWaterStatus(pchunkList);
		foreach (Chunk chunk2 in pchunkList)
		{
			chunk2.WaterRegeneration = true;
		}
		pchunkList.Release();
	}

	public void RegenerateLandChunks(PChunkList chunksNeedingRegeneration)
	{
		PChunkList pchunkList = PChunkList.Acquire();
		foreach (Chunk chunk in chunksNeedingRegeneration)
		{
			if (!chunk.LandRegeneration)
			{
				pchunkList.Add(chunk);
			}
		}
		this.MeshGen.GenerateLandMeshes(pchunkList);
		World.ClearRegenerationLandStatus(pchunkList);
		foreach (Chunk chunk2 in pchunkList)
		{
			chunk2.LandRegeneration = true;
		}
		pchunkList.Release();
	}

	public void CubeDestroyEffect(IntVect hitPoint, bool one_part, Texture2D tex = null)
	{
		if (this.lastFrame == (float)Time.frameCount)
		{
			return;
		}
		this.lastFrame = (float)Time.frameCount;
		BlockType blockType = this._WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z);
		Texture2D texture2D = (!(tex != null)) ? ((!this._WorldData.BlockTextures.ContainsKey(blockType)) ? null : this._WorldData.BlockTextures[blockType]) : tex;
		if (!one_part)
		{
			if (GameType.GetOptionsDestroy() == 1)
			{
				for (int i = 0; i < 12; i++)
				{
					float num = UnityEngine.Random.Range(-0.4f, 0.4f);
					float num2 = UnityEngine.Random.Range(-0.4f, 0.4f);
					float num3 = UnityEngine.Random.Range(-0.4f, 0.4f);
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.SparksCubes1, new Vector3((float)hitPoint.X + 0.5f + num, (float)hitPoint.Z + 0.5f + num3, (float)hitPoint.Y + 0.5f + num2), Quaternion.identity);
					gameObject.layer = LayerMask.NameToLayer("ParticleCubesDestroy");
					gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(num * 300f, num2 * 300f, num3 * 300f));
					gameObject.GetComponent<Renderer>().material.mainTexture = texture2D;
				}
			}
			if (GameType.GetOptionsDestroy() == 2)
			{
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.SparksCubes2, new Vector3((float)hitPoint.X + 0.44f, (float)hitPoint.Z, (float)hitPoint.Y + 0.45f), Quaternion.identity);
				gameObject2.GetComponent<CubeSparcs>().SetTexture(texture2D);
			}
		}
		else if (GameType.GetOptionsDestroy() == 1 && blockType != BlockType.Air)
		{
			float num4 = UnityEngine.Random.Range(-0.4f, 0.4f);
			float num5 = UnityEngine.Random.Range(-0.4f, 0.4f);
			float num6 = UnityEngine.Random.Range(-0.4f, 0.4f);
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.SparksCubes1, new Vector3((float)hitPoint.X + 0.5f + num4, (float)hitPoint.Z + 0.5f + num6, (float)hitPoint.Y + 0.5f + num5), Quaternion.identity);
			gameObject3.layer = LayerMask.NameToLayer("ParticleCubesDestroy");
			gameObject3.GetComponent<Rigidbody>().AddForce(new Vector3(num4 * 300f, num5 * 300f, num6 * 300f));
			gameObject3.GetComponent<Renderer>().material.mainTexture = texture2D;
		}
	}

	public void RemoveBlockAt(IntVect hitPoint, bool silent, bool isFalling = false)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		if (!silent)
		{
			this.CubeDestroyEffect(hitPoint, false, null);
		}
		BlockType blockType = this._WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z);
		byte blockLight = World.Instance.Lighting.GetBlockLight(hitPoint.X, hitPoint.Y, hitPoint.Z);
		BlockKind blockKind = this._WorldData.GetBlockKind(hitPoint.X, hitPoint.Y, hitPoint.Z);
		if (isFalling)
		{
			if (this.OnRemoveBlock != null)
			{
				this.OnRemoveBlock(hitPoint, blockType, blockLight, blockKind);
			}
			int dictKey = hitPoint.X * 1000000 + hitPoint.Y * 1000 + hitPoint.Z;
			if (WorldGameObjectX.Instance.MainPlayer.GetComponent<RestoreBlockController>() != null)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<RestoreBlockController>().SetCollisionInfo(dictKey, hitPoint);
			}
		}
		this.CurLevelDelta.Add(BlockType.Air, hitPoint.X, hitPoint.Y, hitPoint.Z, false, BlockKind.Default);
		this._WorldData.SetBlockTypeWithRegeneration(hitPoint.X, hitPoint.Y, hitPoint.Z, BlockType.Air, BlockKind.Default);
		this.CheckGlassNear(hitPoint);
		this.CheckWaterNear(hitPoint);
		if (!silent)
		{
			if (this.Lighting.IsLight(hitPoint.X, hitPoint.Y, hitPoint.Z))
			{
				this.Lighting.RemoveLight(hitPoint.X, hitPoint.Y, hitPoint.Z, true);
			}
			else
			{
				this.Lighting.RecalculateLightingAroundBlock(hitPoint.X, hitPoint.Y, hitPoint.Z, 0, false);
			}
			BlocksAvulsion.VerifyAvulsionAroundBlocks(hitPoint.X, hitPoint.Y, hitPoint.Z, 0, false);
		}
	}

	public void RemoveBlocksAt(IntVect hitPoint, int radius)
	{
		bool lightChanged = false;
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				for (int k = -radius; k <= radius; k++)
				{
					IntVect hitPoint2 = new IntVect(hitPoint.X + i, hitPoint.Y + j, hitPoint.Z + k);
					BlockType blockType = this._WorldData.GetBlockType(hitPoint2.X, hitPoint2.Y, hitPoint2.Z);
					if (blockType != BlockType.Air && blockType != BlockType.TeamBlue && blockType != BlockType.TeamRed)
					{
						if (i == 0 && j == 0 && k == 0)
						{
							this.CubeDestroyEffect(hitPoint2, false, null);
						}
						this.RemoveBlockAt(hitPoint2, true, false);
						if (this.Lighting.IsLight(hitPoint2.X, hitPoint2.Y, hitPoint2.Z))
						{
							this.Lighting.RemoveLight(hitPoint2.X, hitPoint2.Y, hitPoint2.Z, false);
							lightChanged = true;
						}
					}
				}
			}
		}
		World.Instance.Lighting.RecalculateLightingAroundBlock(hitPoint.X, hitPoint.Y, hitPoint.Z, radius, lightChanged);
		BlocksAvulsion.VerifyAvulsionAroundBlocks(hitPoint.X, hitPoint.Y, hitPoint.Z, radius, false);
	}

	public void DropEntityFrom(IntVect hitPoint, BlockType blockType)
	{
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].dropEntity == EntityType.AIR)
		{
			return;
		}
		this.AddEntityLocal(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].dropEntity, new Vector3((float)hitPoint.X + 0.5f, (float)hitPoint.Z + 0.5f, (float)hitPoint.Y + 0.5f));
	}

	public void CheckWaterNear(IntVect pos)
	{
		if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z + 1) == BlockType.Water)
		{
			this._WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z + 1);
			WorldGameObjectX.Instance.StartCoroutine(this.AddWater(pos));
		}
		if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Water)
		{
			this._WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z - 1);
		}
		if (this._WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Water)
		{
			this._WorldData.SetChunkWaterRegen(pos.X - 1, pos.Y, pos.Z);
		}
		if (this._WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Water)
		{
			this._WorldData.SetChunkWaterRegen(pos.X + 1, pos.Y, pos.Z);
		}
		if (this._WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Water)
		{
			this._WorldData.SetChunkWaterRegen(pos.X, pos.Y + 1, pos.Z);
		}
		if (this._WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Water)
		{
			this._WorldData.SetChunkWaterRegen(pos.X, pos.Y - 1, pos.Z);
		}
	}

	public bool CheckWaterNearBool(IntVect pos)
	{
		return this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z) == BlockType.Water;
	}

	public void CheckGlassNear(IntVect pos)
	{
		if (WorldGameObjectX.Instance.BlockParametrs[(int)this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z + 1)].is_glass)
		{
			this._WorldData.GetChunk(pos.X, pos.Y, pos.Z + 1).NeedsGlassRegen = true;
		}
		if (WorldGameObjectX.Instance.BlockParametrs[(int)this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1)].is_glass)
		{
			this._WorldData.GetChunk(pos.X, pos.Y, pos.Z - 1).NeedsGlassRegen = true;
		}
		if (WorldGameObjectX.Instance.BlockParametrs[(int)this._WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z)].is_glass)
		{
			this._WorldData.GetChunk(pos.X - 1, pos.Y, pos.Z).NeedsGlassRegen = true;
		}
		if (WorldGameObjectX.Instance.BlockParametrs[(int)this._WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z)].is_glass)
		{
			this._WorldData.GetChunk(pos.X + 1, pos.Y, pos.Z).NeedsGlassRegen = true;
		}
		if (WorldGameObjectX.Instance.BlockParametrs[(int)this._WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z)].is_glass)
		{
			this._WorldData.GetChunk(pos.X, pos.Y + 1, pos.Z).NeedsGlassRegen = true;
		}
		if (WorldGameObjectX.Instance.BlockParametrs[(int)this._WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z)].is_glass)
		{
			this._WorldData.GetChunk(pos.X, pos.Y - 1, pos.Z).NeedsGlassRegen = true;
		}
	}

	private void AddSand(IntVect pos)
	{
		if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z) == BlockType.Sand)
		{
			return;
		}
		if (pos.Z > 85 || pos.X < 0 || pos.Y < 0 || pos.X > this._WorldData.GetMaxBlockX() || pos.Y > this._WorldData.GetMaxBlockY())
		{
			return;
		}
		this._WorldData.SetBlockTypeWithLightRegeneration(pos.X, pos.Y, pos.Z, BlockType.Sand, BlockKind.Default);
		if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Air)
		{
			this.AddSand(new IntVect(pos.X, pos.Y, pos.Z - 1));
		}
		if (this._WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Air)
		{
			this.AddSand(new IntVect(pos.X - 1, pos.Y, pos.Z));
		}
		if (this._WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Air)
		{
			this.AddSand(new IntVect(pos.X + 1, pos.Y, pos.Z));
		}
		if (this._WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Air)
		{
			this.AddSand(new IntVect(pos.X, pos.Y - 1, pos.Z));
		}
		if (this._WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Air)
		{
			this.AddSand(new IntVect(pos.X, pos.Y + 1, pos.Z));
		}
	}

	private IEnumerator AddWater(IntVect pos)
	{
		if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z) == BlockType.Water)
		{
			yield return 0;
		}
		else if (pos.X < 0 || pos.Y < 0 || pos.X > this._WorldData.GetMaxBlockX() || pos.Y > this._WorldData.GetMaxBlockY())
		{
			yield return 0;
		}
		else
		{
			this._WorldData.SetBlockType(pos.X, pos.Y, pos.Z, BlockType.Water);
			this._WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z);
			if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z + 1) == BlockType.Air)
			{
				this._WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z + 1);
			}
			if (this._WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Air)
			{
				this._WorldData.SetChunkWaterRegen(pos.X - 1, pos.Y, pos.Z);
			}
			if (this._WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Air)
			{
				this._WorldData.SetChunkWaterRegen(pos.X + 1, pos.Y, pos.Z);
			}
			if (this._WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Air)
			{
				this._WorldData.SetChunkWaterRegen(pos.X, pos.Y + 1, pos.Z);
			}
			if (this._WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Air)
			{
				this._WorldData.SetChunkWaterRegen(pos.X, pos.Y - 1, pos.Z);
			}
			yield return new WaitForSeconds(0.5f);
			if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Air)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.AddWater(new IntVect(pos.X, pos.Y, pos.Z - 1)));
			}
		}
		yield break;
	}

	private IEnumerator AddDestroyer(IntVect pos)
	{
		if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z) == BlockType.Water)
		{
			yield return 0;
		}
		else if (pos.X < 16 || pos.Y < 16 || pos.X > this._WorldData.GetMaxBlockX() || pos.Y > this._WorldData.GetMaxBlockY())
		{
			yield return 0;
		}
		else
		{
			this._WorldData.SetBlockType(pos.X, pos.Y, pos.Z, BlockType.Water);
			this._WorldData.SetChunkWaterRegen(pos.X, pos.Y, pos.Z);
			yield return new WaitForSeconds(0.5f);
			if (this._WorldData.GetBlockType(pos.X, pos.Y, pos.Z - 1) == BlockType.Air)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.AddWater(new IntVect(pos.X, pos.Y, pos.Z - 1)));
			}
			if (this._WorldData.GetBlockType(pos.X + 1, pos.Y, pos.Z) == BlockType.Air)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.AddWater(new IntVect(pos.X + 1, pos.Y, pos.Z)));
			}
			if (this._WorldData.GetBlockType(pos.X, pos.Y + 1, pos.Z) == BlockType.Air)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.AddWater(new IntVect(pos.X, pos.Y + 1, pos.Z)));
			}
			if (this._WorldData.GetBlockType(pos.X - 1, pos.Y, pos.Z) == BlockType.Air)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.AddWater(new IntVect(pos.X - 1, pos.Y, pos.Z)));
			}
			if (this._WorldData.GetBlockType(pos.X, pos.Y - 1, pos.Z) == BlockType.Air)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.AddWater(new IntVect(pos.X, pos.Y - 1, pos.Z)));
			}
		}
		yield break;
	}

	public void AddBlockAt(IntVect hitPoint, BlockType type, BlockKind kind)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		SoundManager.Instance.PlayAtPoint(WorldGameObjectX.Instance.BlockParametrs[(int)type].HitEffect, new Vector3((float)hitPoint.X, (float)hitPoint.Z, (float)hitPoint.Y));
		if (this._WorldData.GetBlockType(hitPoint.X, hitPoint.Y, hitPoint.Z) != BlockType.Water)
		{
			this.CheckWaterNear(hitPoint);
		}
		if (type == BlockType.Water)
		{
			this.CurLevelDelta.Add(type, hitPoint.X, hitPoint.Y, hitPoint.Z, false, kind);
			WorldGameObjectX.Instance.StartCoroutine(this.AddWater(hitPoint));
			return;
		}
		this.CurLevelDelta.Add(type, hitPoint.X, hitPoint.Y, hitPoint.Z, false, kind);
		this._WorldData.SetBlockTypeWithLightRegeneration(hitPoint.X, hitPoint.Y, hitPoint.Z, type, kind);
		if (type == BlockType.Lava && Info.Instance.GameMode != "ARCADE")
		{
			World.Instance.Lighting.AddLight(hitPoint.X, hitPoint.Y, hitPoint.Z, true);
		}
		World.Instance.Lighting.RecalculateLightingAroundBlock(hitPoint.X, hitPoint.Y, hitPoint.Z, 0, false);
	}

	public void AddEntityNetwork(EntityType type, Vector3 pos, Quaternion rot, string creator, string creatorID, object[] data)
	{
		if (this.EntityCount < 955)
		{
			GameObject gameObject = WorldGameObjectX.Instance.EnityParametrs[(int)type].gameObject;
			int num = (data == null) ? 0 : data.Length;
			object[] array = new object[3 + num];
			array[0] = type;
			array[1] = creator;
			array[2] = creatorID;
			for (int i = 0; i < num; i++)
			{
				array[3 + i] = data[i];
			}
			PhotonNetwork.InstantiateSceneObject(gameObject.name, pos, rot, 0, array);
			this.EntityCount++;
			return;
		}
	}

	public GameObject AddEntityLocal(EntityType type, Vector3 pos)
	{
		GameObject gameObject = WorldGameObjectX.Instance.EnityParametrs[(int)type].gameObject;
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, pos, gameObject.transform.rotation);
		gameObject2.GetComponent<EntityBase>().Type = WorldGameObjectX.Instance.EnityParametrs[(int)type].Type;
		gameObject2.GetComponent<EntityBase>().Name = WorldGameObjectX.Instance.EnityParametrs[(int)type].name;
		gameObject2.layer = LayerMask.NameToLayer("Entity");
		return gameObject2;
	}

	public byte[] GetEntityBuffer()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(EntityBase.Entities.Count);
		foreach (EntityBase entityBase in EntityBase.Entities)
		{
			int type = (int)entityBase.Type;
			object[] data = entityBase.gameObject.GetComponent<EntityBase>().GetData();
			bool flag = data != null && data.Length > 0;
			binaryWriter.Write(type * ((!flag) ? 1 : -1));
			if (entityBase is Boat)
			{
				binaryWriter.Write(entityBase.gameObject.transform.position.x);
				binaryWriter.Write(entityBase.gameObject.transform.position.y);
				binaryWriter.Write(entityBase.gameObject.transform.position.z);
			}
			else
			{
				binaryWriter.Write(entityBase.gameObject.transform.position.x);
				binaryWriter.Write(entityBase.gameObject.transform.position.y);
				binaryWriter.Write(entityBase.gameObject.transform.position.z);
			}
			binaryWriter.Write(entityBase.gameObject.transform.localEulerAngles.x);
			binaryWriter.Write(entityBase.gameObject.transform.localEulerAngles.y);
			binaryWriter.Write(entityBase.gameObject.transform.localEulerAngles.z);
			if (flag || entityBase.Type >= EntityType.RAIL)
			{
				if (data != null && data.Length > 0)
				{
					if (data.Length < 255)
					{
						binaryWriter.Write((byte)data.Length);
					}
					else
					{
						binaryWriter.Write(byte.MaxValue);
						binaryWriter.Write(data.Length);
					}
					foreach (object obj in data)
					{
						if (obj == null)
						{
							binaryWriter.Write(0);
						}
						else if (obj is string)
						{
							binaryWriter.Write(1);
							binaryWriter.Write((string)obj);
						}
						else if (obj is int)
						{
							binaryWriter.Write(2);
							binaryWriter.Write((int)obj);
						}
						else if (obj is float)
						{
							binaryWriter.Write(3);
							binaryWriter.Write((float)obj);
						}
						else if (obj is bool)
						{
							binaryWriter.Write(4);
							binaryWriter.Write((bool)obj);
						}
						else
						{
							UnityEngine.Debug.Log("Entity '" + entityBase.gameObject.name + "' contain wrong data for serialization! Valid only null, string, int, float, bool.");
						}
					}
				}
				else
				{
					binaryWriter.Write(0);
				}
			}
		}
		return memoryStream.GetBuffer();
	}

	public IEnumerator SetEntityBuffer(byte[] buffer)
	{
		MemoryStream ms = new MemoryStream(buffer);
		BinaryReader br = new BinaryReader(ms);
		int count = br.ReadInt32();
		for (int i = 0; i < count; i++)
		{
			int typeEx = br.ReadInt32();
			EntityType type = (EntityType)Mathf.Abs(typeEx);
			bool haveData = typeEx < 0;
			float x = br.ReadSingle();
			float y = br.ReadSingle();
			float z = br.ReadSingle();
			Vector3 pos = new Vector3(x, y, z);
			float rx = br.ReadSingle();
			float ry = br.ReadSingle();
			float rz = br.ReadSingle();
			Quaternion rot = Quaternion.Euler(rx, ry, rz);
			if (haveData || type >= EntityType.RAIL)
			{
				int dataLength = (int)br.ReadByte();
				if (dataLength == 255)
				{
					dataLength = br.ReadInt32();
				}
				object[] data = null;
				if (dataLength > 0)
				{
					data = new object[dataLength];
					for (int j = 0; j < dataLength; j++)
					{
						byte objectType = br.ReadByte();
						if (objectType == 0)
						{
							data[j] = null;
						}
						else if (objectType == 1)
						{
							data[j] = br.ReadString();
						}
						else if (objectType == 2)
						{
							data[j] = br.ReadInt32();
						}
						else if (objectType == 3)
						{
							data[j] = br.ReadSingle();
						}
						else if (objectType == 4)
						{
							data[j] = br.ReadBoolean();
						}
					}
				}
				this.AddEntityNetwork(type, pos, rot, ProfileINI.nickname, null, data);
			}
			else if (type == EntityType.TABLICHKAF || type == EntityType.TABLICHKAW)
			{
				object[] lines = new object[]
				{
					br.ReadString(),
					br.ReadString(),
					br.ReadString(),
					br.ReadString()
				};
				this.AddEntityNetwork(type, pos, rot, ProfileINI.nickname, null, lines);
			}
			else
			{
				this.AddEntityNetwork(type, pos, rot, ProfileINI.nickname, null, null);
			}
			if (i % 10 == 0)
			{
				yield return 0;
			}
		}
		yield break;
	}

	public void FireNukeAt(IntVect hitPoint, Ray ray)
	{
		UnityEngine.Debug.Log(hitPoint + " - " + ray);
		float num = (float)hitPoint.X;
		float num2 = (float)hitPoint.Y;
		float num3 = (float)hitPoint.Z;
		for (int i = 0; i <= 10; i++)
		{
			num += ray.direction.x;
			num2 += ray.direction.y;
			num3 += ray.direction.z;
			for (int j = 0; j < 10; j++)
			{
				int x = (int)(UnityEngine.Random.insideUnitSphere.x * 3f + num);
				int y = (int)(UnityEngine.Random.insideUnitSphere.y * 3f + num2);
				int z = (int)(UnityEngine.Random.insideUnitSphere.z * 3f + num3);
				this._WorldData.SetBlockTypeWithRegeneration(x, y, z, BlockType.Air, BlockKind.Default);
			}
		}
	}

	public void SetCaching()
	{
		if (this.Audio == null)
		{
			this.Audio = WorldGameObjectX.Instance.MainPlayerEyes.GetComponent<AudioSource>();
		}
	}

	public void Dig2(IntVect targetLocation, Vector3 hitPoint, float damage = 1f, bool isArcadeMode = false)
	{
		if (App.Instance.Settings.isWatch)
		{
			return;
		}
		if (!Level.Instance.CanBuild)
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP", null), false);
			return;
		}
		BlockType blockType = this._WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z);
		if (blockType != BlockType.Leaves && blockType != BlockType.Water && hitPoint != Vector3.zero)
		{
			UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.Sparks, hitPoint, Quaternion.identity);
		}
		if (!App.Instance.Settings.destroyable)
		{
			WorldGameObjectX.Instance.SendSoundRpc((int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
			return;
		}
		if (TeamBattle.Instance != null && (blockType == BlockType.TeamBlue || blockType == BlockType.TeamRed))
		{
			WorldGameObjectX.Instance.SendSoundRpc((int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
			return;
		}
		if (blockType == BlockType.Water)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BangWater, this.Audio);
			WorldGameObjectX.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, new object[]
			{
				targetLocation.X,
				targetLocation.Y,
				targetLocation.Z,
				false,
				isArcadeMode
			});
		}
		else
		{
			CubeDamage2 cubeDamage2;
			if (!this._cubedamages.ContainsKey(targetLocation))
			{
				CubeDamage2 cubeDamage = new CubeDamage2();
				this._cubedamages[targetLocation] = cubeDamage;
				cubeDamage2 = cubeDamage;
				int num = (int)blockType;
				int life_index = WorldGameObjectX.Instance.BlockParametrs[num].life_index;
				cubeDamage2.life = WorldGameObjectX.Instance.BlockLifes[life_index].amount;
				this.m_DiggingLocation = targetLocation;
				this.bFirstDig = true;
			}
			else
			{
				cubeDamage2 = this._cubedamages[targetLocation];
				this.bFirstDig = false;
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.SUPER_KIRCKA) != 0)
			{
				cubeDamage2.life = 0;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.KIRCKA) != 0)
			{
				cubeDamage2.life -= (int)(50f * damage);
			}
			else
			{
				cubeDamage2.life -= (int)(25f * damage);
			}
			if (cubeDamage2.life <= 0)
			{
				this._cubedamages.Remove(targetLocation);
				this.DropEntityFrom(targetLocation, this._WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z));
				WorldGameObjectX.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, new object[]
				{
					targetLocation.X,
					targetLocation.Y,
					targetLocation.Z,
					false,
					isArcadeMode
				});
				if (this.bFirstDig)
				{
					SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
					WorldGameObjectX.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, new object[]
					{
						(int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect,
						hitPoint
					});
				}
				else
				{
					WorldGameObjectX.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, new object[]
					{
						(int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].DestroyEffect,
						hitPoint
					});
					SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].DestroyEffect, this.Audio);
				}
			}
			else
			{
				WorldGameObjectX.Instance.SendSoundRpc((int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, hitPoint);
				SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
			}
		}
	}

	public void Dig(IntVect targetLocation, Vector3 hitPoint, Vector3 normal, bool isArcadeMode = false)
	{
		if (!Level.Instance.CanBuild)
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP", null), false);
			return;
		}
		BlockType blockType = this._WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z);
		GameObject cubeDamage = WorldGameObjectX.Instance.GetCubeDamage();
		cubeDamage.transform.localScale = WorldGameObjectX.Instance.GetSelectelScale();
		cubeDamage.transform.position = WorldGameObjectX.Instance.GetSelectelPos();
		cubeDamage.transform.rotation = WorldGameObjectX.Instance.GetSelectelRot();
		if (blockType != BlockType.Leaves && blockType != BlockType.Water)
		{
			UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.Sparks, hitPoint, Quaternion.identity);
		}
		if (!App.Instance.Settings.destroyable)
		{
			WorldGameObjectX.Instance.SendSoundRpc((int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
			return;
		}
		if (TeamBattle.Instance != null && (blockType == BlockType.TeamBlue || blockType == BlockType.TeamRed))
		{
			WorldGameObjectX.Instance.SendSoundRpc((int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, hitPoint);
			SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
			return;
		}
		if (blockType == BlockType.Water)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BangWater, this.Audio);
			WorldGameObjectX.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, new object[]
			{
				targetLocation.X,
				targetLocation.Y,
				targetLocation.Z,
				false,
				isArcadeMode
			});
			this.m_CentSkyX = 0;
			cubeDamage.GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			if (targetLocation != this.m_DiggingLocation)
			{
				int num = (int)blockType;
				int life_index = WorldGameObjectX.Instance.BlockParametrs[num].life_index;
				this.m_CentSkyX = WorldGameObjectX.Instance.BlockLifes[life_index].amount;
				this.m_DiggingLocation = targetLocation;
				this.bFirstDig = true;
				cubeDamage.GetComponent<MeshRenderer>().enabled = true;
				cubeDamage.GetComponent<CubeDamage>().Reset();
			}
			else
			{
				this.bFirstDig = false;
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.SUPER_KIRCKA) != 0)
			{
				this.m_CentSkyX = 0;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.KIRCKA) != 0)
			{
				this.m_CentSkyX -= 50;
			}
			else
			{
				this.m_CentSkyX -= 25;
			}
			cubeDamage.GetComponent<CubeDamage>().SetFrame((100 - this.m_CentSkyX) / 10);
			if (blockType == BlockType.Reshetka1 || blockType == BlockType.Reshetka2 || blockType == BlockType.Reshetka3 || blockType == BlockType.Reshetka4 || blockType == BlockType.Reshetka4 || blockType == BlockType.Reshetka5)
			{
				cubeDamage.GetComponent<MeshRenderer>().enabled = false;
			}
			else
			{
				cubeDamage.GetComponent<MeshRenderer>().enabled = true;
			}
			if (this.m_CentSkyX <= 0)
			{
				this.DropEntityFrom(targetLocation, this._WorldData.GetBlockType(targetLocation.X, targetLocation.Y, targetLocation.Z));
				WorldGameObjectX.Instance.photonView.RPC("RemoveBlockAt", PhotonTargets.All, new object[]
				{
					targetLocation.X,
					targetLocation.Y,
					targetLocation.Z,
					false,
					isArcadeMode
				});
				this.m_CentSkyX = 100;
				cubeDamage.GetComponent<MeshRenderer>().enabled = false;
				if (this.bFirstDig)
				{
					WorldGameObjectX.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, new object[]
					{
						(int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect,
						hitPoint
					});
					SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
				}
				else
				{
					WorldGameObjectX.Instance.photonView.RPC("SoundRPC", PhotonTargets.Others, new object[]
					{
						(int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].DestroyEffect,
						hitPoint
					});
					SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].DestroyEffect, this.Audio);
				}
			}
			else
			{
				WorldGameObjectX.Instance.SendSoundRpc((int)WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, hitPoint);
				SoundManager.Instance.Play(WorldGameObjectX.Instance.BlockParametrs[(int)blockType].HitEffect, this.Audio);
			}
		}
	}

	public static void DestroyChunk(Chunk chunk)
	{
		if (chunk.LandChunk != null)
		{
			MeshFilter component = chunk.LandChunk.GetComponent<MeshFilter>();
			component.mesh.Clear();
			UnityEngine.Object.Destroy(component.mesh);
			component.mesh = null;
			UnityEngine.Object.Destroy(component);
			UnityEngine.Object.Destroy(chunk.LandChunk.gameObject);
			chunk.LandChunk = null;
		}
		if (chunk.WaterChunk != null)
		{
			MeshFilter component2 = chunk.WaterChunk.GetComponent<MeshFilter>();
			component2.mesh.Clear();
			UnityEngine.Object.Destroy(component2.mesh);
			component2.mesh = null;
			UnityEngine.Object.Destroy(component2);
			UnityEngine.Object.Destroy(chunk.WaterChunk.gameObject);
			chunk.WaterChunk = null;
		}
		if (chunk.GlassChunk != null)
		{
			MeshFilter component3 = chunk.GlassChunk.GetComponent<MeshFilter>();
			component3.mesh.Clear();
			UnityEngine.Object.Destroy(component3.mesh);
			component3.mesh = null;
			UnityEngine.Object.Destroy(component3);
			UnityEngine.Object.Destroy(chunk.GlassChunk.gameObject);
			chunk.GlassChunk = null;
		}
		chunk = null;
	}

	public static World Instance;

	public readonly ILightProcessor Lighting;

	public readonly IMeshGenerator MeshGen;

	public string CurMapId = string.Empty;

	public bool IsHomeWorld = true;

	public LevelDelta CurLevelDelta = new LevelDelta();

	public AudioSource Audio;

	private readonly ITerrainGenerator _TerrainGenerator;

	private readonly WorldData _WorldData;

	private readonly IWorldDecorator _WorldDecorator;

	private int _LastFrameDestruction;

	public World.ZipLevelThread zipedLevel = new World.ZipLevelThread();

	private float lastFrame;

	public int EntityCount;

	private SecuredValue<int> m_CentSkyX;

	private bool bFirstDig;

	private IntVect m_DiggingLocation;

	private DateTime m_LastDigTime;

	public readonly Queue<Vector3> Diggings = new Queue<Vector3>();

	private Dictionary<IntVect, CubeDamage2> _cubedamages = new Dictionary<IntVect, CubeDamage2>();

	public class ZipLevelThread
	{
		public void Run()
		{
			this.finished = false;
			this.ziped = Utils.ZipByte(this.level);
			this.finished = true;
			this.level = null;
		}

		public byte[] GetLevelPart(int part)
		{
			MemoryStream memoryStream = new MemoryStream(this.ziped);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = 0;
			for (int i = 0; i < this.ziped.Length; i += World.ZipLevelThread.chunk_size)
			{
				int num2 = World.ZipLevelThread.chunk_size;
				if (i + World.ZipLevelThread.chunk_size > this.ziped.Length)
				{
					num2 = this.ziped.Length - i;
				}
				if (part == num)
				{
					return binaryReader.ReadBytes(num2);
				}
				memoryStream.Seek((long)num2, SeekOrigin.Current);
				num++;
			}
			return null;
		}

		public List<byte[]> GetCurrentZipedLevel()
		{
			MemoryStream input = new MemoryStream(this.ziped);
			BinaryReader binaryReader = new BinaryReader(input);
			List<byte[]> list = new List<byte[]>();
			for (int i = 0; i < this.ziped.Length; i += World.ZipLevelThread.chunk_size)
			{
				int count = World.ZipLevelThread.chunk_size;
				if (i + World.ZipLevelThread.chunk_size > this.ziped.Length)
				{
					count = this.ziped.Length - i;
				}
				list.Add(binaryReader.ReadBytes(count));
			}
			return list;
		}

		public static int chunk_size = 250;

		public byte[] level;

		public byte[] ziped;

		public bool finished;
	}
}
