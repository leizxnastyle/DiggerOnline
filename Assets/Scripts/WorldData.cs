using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldData
{
	public WorldData()
	{
		WorldData.Instance = this;
	}

	public IntVect ChunkBlockVolume
	{
		get
		{
			return new IntVect(this._ChunkBlockWidth, this._ChunkBlockHeight, this._ChunkBlockDepth);
		}
	}

	public void SetBlockKind(int x, int y, int z, BlockKind kind)
	{
		if (x < 0 || y < 0 || z < 0 || x >= this.WidthInBlocks || y >= this.HeightInBlocks || z >= this.DepthInBlocks)
		{
			return;
		}
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		int x2 = x % this.ChunkBlockWidth;
		int y2 = y % this.ChunkBlockHeight;
		int z2 = z % this.ChunkBlockDepth;
		this.Chunks[num, num2, num3].SetBlockKind(kind, x2, y2, z2);
	}

	public BlockKind GetBlockKind(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= this.WidthInBlocks || y >= this.HeightInBlocks || z >= this.DepthInBlocks)
		{
			return BlockKind.Default;
		}
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		int x2 = x % this.ChunkBlockWidth;
		int y2 = y % this.ChunkBlockHeight;
		int z2 = z % this.ChunkBlockDepth;
		return this.Chunks[num, num2, num3].GetBlockKind(x2, y2, z2);
	}

	public void GenerateUVCoordinatesForAllBlocks()
	{
		this.SetBlockUVCoordinates(BlockType.TopSoil, 0, 5, 1);
		this.SetBlockUVCoordinates(BlockType.Dirt, 1, 1, 1);
		this.SetBlockUVCoordinates(BlockType.Leaves, 4, 4, 4);
		this.SetBlockUVCoordinates(BlockType.Lava, 3, 3, 3);
		this.SetBlockUVCoordinates(BlockType.Stone, 6, 6, 6);
		this.SetBlockUVCoordinates(BlockType.Wood, 2, 7, 2);
		this.SetBlockUVCoordinates(BlockType.Brick, 9);
		this.SetBlockUVCoordinates(BlockType.WoodPlank, 10);
		this.SetBlockUVCoordinates(BlockType.Brick2, 11);
		this.SetBlockUVCoordinates(BlockType.WoodPlank2, 12);
		this.SetBlockUVCoordinates(BlockType.WoodPlank3, 13);
		this.SetBlockUVCoordinates(BlockType.WoodPlank4, 14);
		this.SetBlockUVCoordinates(BlockType.Brick3, 8);
		this.SetBlockUVCoordinates(BlockType.Brick4, 25);
		this.SetBlockUVCoordinates(BlockType.Brick5, 26);
		this.SetBlockUVCoordinates(BlockType.Brick6, 27);
		this.SetBlockUVCoordinates(BlockType.Brick7, 29);
		this.SetBlockUVCoordinates(BlockType.WoodPlank5, 28);
		this.SetBlockUVCoordinates(BlockType.Colored1, 16);
		this.SetBlockUVCoordinates(BlockType.Colored2, 17);
		this.SetBlockUVCoordinates(BlockType.Colored3, 18);
		this.SetBlockUVCoordinates(BlockType.Colored4, 19);
		this.SetBlockUVCoordinates(BlockType.Colored5, 20);
		this.SetBlockUVCoordinates(BlockType.Colored6, 21);
		this.SetBlockUVCoordinates(BlockType.Colored7, 22);
		this.SetBlockUVCoordinates(BlockType.Colored8, 23);
		this.SetBlockUVCoordinates(BlockType.Colored9, 24);
		this.SetBlockUVCoordinates(BlockType.Stone17, 30);
		this.SetBlockUVCoordinates(BlockType.Stone17_1, 31);
		this.SetBlockUVCoordinates(BlockType.Stone17_2, 32);
		this.SetBlockUVCoordinates(BlockType.Stone9, 33);
		this.SetBlockUVCoordinates(BlockType.Stone9_2, 34);
		this.SetBlockUVCoordinates(BlockType.Stone10, 35);
		this.SetBlockUVCoordinates(BlockType.Stone10_1, 36);
		this.SetBlockUVCoordinates(BlockType.Stone14, 37);
		this.SetBlockUVCoordinates(BlockType.Stone14_1, 38);
		this.SetBlockUVCoordinates(BlockType.Stone14_2, 39);
		this.SetBlockUVCoordinates(BlockType.Stone14_3, 40);
		this.SetBlockUVCoordinates(BlockType.Stone14_4, 41);
		this.SetBlockUVCoordinates(BlockType.Stone4_1, 42);
		this.SetBlockUVCoordinates(BlockType.Stone4_2, 43);
		this.SetBlockUVCoordinates(BlockType.Stone15, 44);
		this.SetBlockUVCoordinates(BlockType.Stone15_1, 45);
		this.SetBlockUVCoordinates(BlockType.Stone15_2, 46);
		this.SetBlockUVCoordinates(BlockType.Stone15_3, 47);
		this.SetBlockUVCoordinates(BlockType.Stone16, 48);
		this.SetBlockUVCoordinates(BlockType.Stone16_1, 49);
		this.SetBlockUVCoordinates(BlockType.Stone16_2, 50);
		this.SetBlockUVCoordinates(BlockType.Stone16_3, 51);
		this.SetBlockUVCoordinates(BlockType.Stone18, 52);
		this.SetBlockUVCoordinates(BlockType.Stone18_1, 53);
		this.SetBlockUVCoordinates(BlockType.Stone18_2, 54);
		this.SetBlockUVCoordinates(BlockType.Stone19, 55);
		this.SetBlockUVCoordinates(BlockType.Stone19_1, 56);
		this.SetBlockUVCoordinates(BlockType.Stone19_2, 57);
		this.SetBlockUVCoordinates(BlockType.Coal, 58);
		this.SetBlockUVCoordinates(BlockType.Iron, 59);
		this.SetBlockUVCoordinates(BlockType.Sand, 60);
		this.SetBlockUVCoordinates(BlockType.Gold, 61);
		this.SetBlockUVCoordinates(BlockType.Crates, 62);
		this.SetBlockUVCoordinates(BlockType.Wood8, 63);
		this.SetBlockUVCoordinates(BlockType.Wood9, 64);
		this.SetBlockUVCoordinates(BlockType.Wood10, 65);
		this.SetBlockUVCoordinates(BlockType.Wood11, 66);
		this.SetBlockUVCoordinates(BlockType.Glass1, 67);
		this.SetBlockUVCoordinates(BlockType.Glass2, 68);
		this.SetBlockUVCoordinates(BlockType.Glass3, 69);
		this.SetBlockUVCoordinates(BlockType.Glass4, 70);
		this.SetBlockUVCoordinates(BlockType.Glass5, 71);
		this.SetBlockUVCoordinates(BlockType.Glass6, 72);
		this.SetBlockUVCoordinates(BlockType.Glass7, 73);
		this.SetBlockUVCoordinates(BlockType.Glass8, 74);
		this.SetBlockUVCoordinates(BlockType.Glass9, 75);
		this.SetBlockUVCoordinates(BlockType.Reshetka1, 76);
		this.SetBlockUVCoordinates(BlockType.Reshetka2, 77);
		this.SetBlockUVCoordinates(BlockType.Reshetka3, 78);
		this.SetBlockUVCoordinates(BlockType.Reshetka4, 79);
		this.SetBlockUVCoordinates(BlockType.Reshetka5, 80);
		this.SetBlockUVCoordinates(BlockType.Water, 81);
		this.SetBlockUVCoordinates(BlockType.Snow, 82);
		this.SetBlockUVCoordinates(BlockType.SnowDirt, 82, 83, 1);
		this.SetBlockUVCoordinates(BlockType.Ice, 84);
		this.SetBlockUVCoordinates(BlockType.Gum, 85, 86, 86);
		this.SetBlockUVCoordinates(BlockType.TeamBlue, 87);
		this.SetBlockUVCoordinates(BlockType.TeamRed, 88);
		this.SetBlockUVCoordinates(BlockType.Tile1, 89);
		this.SetBlockUVCoordinates(BlockType.Tile2, 90);
		this.SetBlockUVCoordinates(BlockType.Tile3, 91);
		this.SetBlockUVCoordinates(BlockType.Tile4, 92);
		this.SetBlockUVCoordinates(BlockType.Tile5, 93);
		this.SetBlockUVCoordinates(BlockType.Castle01_Wall, 94);
		this.SetBlockUVCoordinates(BlockType.Castle02_Wall, 95);
		this.SetBlockUVCoordinates(BlockType.Castle03_Floor, 96);
		this.SetBlockUVCoordinates(BlockType.Castle04_Floor, 97);
		this.SetBlockUVCoordinates(BlockType.Castle05_Floor, 98);
		this.SetBlockUVCoordinates(BlockType.Castle06, 99);
		this.SetBlockUVCoordinates(BlockType.Castle07_Grate, 100);
		this.SetBlockUVCoordinates(BlockType.Castle08_Wall, 101);
		this.SetBlockUVCoordinates(BlockType.Castle09, 102);
		this.SetBlockUVCoordinates(BlockType.Dungeon01_Wall, 103);
		this.SetBlockUVCoordinates(BlockType.Dungeon02_Floor, 104);
		this.SetBlockUVCoordinates(BlockType.Dungeon03, 105);
		this.SetBlockUVCoordinates(BlockType.Dungeon04_Wall, 106);
		this.SetBlockUVCoordinates(BlockType.Dungeon05_FloorElement, 107);
		this.SetBlockUVCoordinates(BlockType.Dungeon06_WallBottom, 108);
		this.SetBlockUVCoordinates(BlockType.Dungeon07_WallElement, 109);
		this.SetBlockUVCoordinates(BlockType.Dungeon08_WoodBar, 110);
		this.SetBlockUVCoordinates(BlockType.Dungeon09_WoodBox, 111);
		this.SetBlockUVCoordinates(BlockType.Dungeon10_Grate, 112);
		this.SetBlockUVCoordinates(BlockType.Dungeon11_WallElement, 113);
		this.SetBlockUVCoordinates(BlockType.Dungeon12_Floor, 114);
		this.SetBlockUVCoordinates(BlockType.Dungeon13_Floor, 115);
		this.SetBlockUVCoordinates(BlockType.Fortress01_Floor, 116);
		this.SetBlockUVCoordinates(BlockType.Fortress02_Wall, 117);
		this.SetBlockUVCoordinates(BlockType.Fortress03_Floor, 118);
		this.SetBlockUVCoordinates(BlockType.Fortress04_Wall, 119);
		this.SetBlockUVCoordinates(BlockType.Fortress05_Wall, 120);
		this.SetBlockUVCoordinates(BlockType.Fortress06_Wall, 121);
		this.SetBlockUVCoordinates(BlockType.Fortress07_Floor, 122);
		this.SetBlockUVCoordinates(BlockType.Fortress08_Grate, 123);
		this.SetBlockUVCoordinates(BlockType.Fortress09_WoodBox, 124);
		this.SetBlockUVCoordinates(BlockType.Fortress10_WoodBar, 125);
		this.SetBlockUVCoordinates(BlockType.Library01_WallBottom, 126);
		this.SetBlockUVCoordinates(BlockType.Library02_Floor, 127);
		this.SetBlockUVCoordinates(BlockType.Library03_Floor, 128);
		this.SetBlockUVCoordinates(BlockType.Library04_Wall, 129);
		this.SetBlockUVCoordinates(BlockType.Library05_WallElement, 130);
		this.SetBlockUVCoordinates(BlockType.Library06_BookShelf, 131);
		this.SetBlockUVCoordinates(BlockType.Library07, 132);
		this.SetBlockUVCoordinates(BlockType.Tavern01_WallBottom, 133);
		this.SetBlockUVCoordinates(BlockType.Tavern02_Wall, 134);
		this.SetBlockUVCoordinates(BlockType.Tavern03_WallElement, 135);
		this.SetBlockUVCoordinates(BlockType.Tavern04_WoodBox, 136);
		this.SetBlockUVCoordinates(BlockType.Tavern05_Floor, 137);
		this.SetBlockUVCoordinates(BlockType.Tavern06_Wall, 138);
		this.SetBlockUVCoordinates(BlockType.Tavern07_Wall, 139);
		this.SetBlockUVCoordinates(BlockType.Tavern08_FloorElement, 140);
		this.SetBlockUVCoordinates(BlockType.Tavern09_WoodBar, 141);
		this.SetBlockUVCoordinates(BlockType.Tavern10_WallBottom, 142);
		this.SetBlockUVCoordinates(BlockType.Tavern11_WallBottom, 143);
		this.SetBlockUVCoordinates(BlockType.Tile7, 144);
		this.SetBlockUVCoordinates(BlockType.Tile9, 145);
		this.SetBlockUVCoordinates(BlockType.Military01, 146);
		this.SetBlockUVCoordinates(BlockType.Military02, 147);
		this.SetBlockUVCoordinates(BlockType.Military03, 148);
		this.SetBlockUVCoordinates(BlockType.Military04, 149);
		this.SetBlockUVCoordinates(BlockType.Military05, 150);
		this.SetBlockUVCoordinates(BlockType.Military06, 151);
		this.SetBlockUVCoordinates(BlockType.Military07, 152);
		this.SetBlockUVCoordinates(BlockType.Military08, 153);
		this.SetBlockUVCoordinates(BlockType.Military09, 154);
		this.SetBlockUVCoordinates(BlockType.Military10, 155);
		this.SetBlockUVCoordinates(BlockType.Military11, 156);
		this.SetBlockUVCoordinates(BlockType.Military12, 157);
		this.SetBlockUVCoordinates(BlockType.Industrial01, 158);
		this.SetBlockUVCoordinates(BlockType.Industrial02, 159);
		this.SetBlockUVCoordinates(BlockType.Industrial03, 160);
		this.SetBlockUVCoordinates(BlockType.Industrial04, 161);
		this.SetBlockUVCoordinates(BlockType.Industrial05, 162);
		this.SetBlockUVCoordinates(BlockType.Industrial06, 163);
		this.SetBlockUVCoordinates(BlockType.Industrial07, 164);
		this.SetBlockUVCoordinates(BlockType.Industrial08, 165);
		this.SetBlockUVCoordinates(BlockType.Industrial09, 166);
		this.SetBlockUVCoordinates(BlockType.Industrial10, 167);
		this.SetBlockUVCoordinates(BlockType.Industrial11, 168);
		this.SetBlockUVCoordinates(BlockType.Industrial12, 169);
		this.SetBlockUVCoordinates(BlockType.DarkCastle01, 170);
		this.SetBlockUVCoordinates(BlockType.DarkCastle02, 171);
		this.SetBlockUVCoordinates(BlockType.DarkCastle03, 172);
		this.SetBlockUVCoordinates(BlockType.DarkCastle04, 173);
		this.SetBlockUVCoordinates(BlockType.DarkCastle05, 174);
		this.SetBlockUVCoordinates(BlockType.DarkCastle06, 175);
		this.SetBlockUVCoordinates(BlockType.DarkCastle07, 176);
		this.SetBlockUVCoordinates(BlockType.DarkCastle08, 177);
		this.SetBlockUVCoordinates(BlockType.DarkCastle09, 178);
		this.SetBlockUVCoordinates(BlockType.DarkCastle10, 179);
		this.SetBlockUVCoordinates(BlockType.DarkCastle11, 180);
		this.SetBlockUVCoordinates(BlockType.DarkCastle12, 181);
		this.SetBlockUVCoordinates(BlockType.DarkCastle13, 182);
		this.SetBlockUVCoordinates(BlockType.DarkCastle14, 183);
		this.SetBlockUVCoordinates(BlockType.DarkCastle15, 184);
		this.SetBlockUVCoordinates(BlockType.DarkCastle16, 185);
		this.SetBlockUVCoordinates(BlockType.DarkCastle17, 186);
		this.SetBlockUVCoordinates(BlockType.DarkCastle18, 187);
		this.SetBlockUVCoordinates(BlockType.DarkCastle19, 188);
		this.SetBlockUVCoordinates(BlockType.DarkCastle20, 189);
		this.SetBlockUVCoordinates(BlockType.HideWhenStep, 158, 158, 158);
		this.SetBlockUVCoordinates(BlockType.RestoreWhenStep, 190);
		this.SetBlockUVCoordinates(BlockType.CityInterio01, 191);
		this.SetBlockUVCoordinates(BlockType.CityInterio02, 192);
		this.SetBlockUVCoordinates(BlockType.CityInterio03, 193);
		this.SetBlockUVCoordinates(BlockType.CityInterio04, 194);
		this.SetBlockUVCoordinates(BlockType.CityInterio05, 195);
		this.SetBlockUVCoordinates(BlockType.CityInterio06, 196);
		this.SetBlockUVCoordinates(BlockType.CityInterio07, 197);
		this.SetBlockUVCoordinates(BlockType.CityInterio08, 242, 198, 242);
		this.SetBlockUVCoordinates(BlockType.CityInterio09, 199);
		this.SetBlockUVCoordinates(BlockType.CityInterio10, 200);
		this.SetBlockUVCoordinates(BlockType.CityInterio11, 201);
		this.SetBlockUVCoordinates(BlockType.CityInterio12, 202);
		this.SetBlockUVCoordinates(BlockType.CityInterio13, 203);
		this.SetBlockUVCoordinates(BlockType.CityInterio14, 243, 204, 243);
		this.SetBlockUVCoordinates(BlockType.CityInterio15, 205);
		this.SetBlockUVCoordinates(BlockType.CityInterio16, 206);
		this.SetBlockUVCoordinates(BlockType.CityInterio17, 207);
		this.SetBlockUVCoordinates(BlockType.CityInterio18, 208);
		this.SetBlockUVCoordinates(BlockType.CityInterio19, 209);
		this.SetBlockUVCoordinates(BlockType.CityInterio20, 210);
		this.SetBlockUVCoordinates(BlockType.CityInterio21, 211);
		this.SetBlockUVCoordinates(BlockType.CityExterior01, 212);
		this.SetBlockUVCoordinates(BlockType.CityExterior02, 213);
		this.SetBlockUVCoordinates(BlockType.CityExterior03, 214);
		this.SetBlockUVCoordinates(BlockType.CityExterior04, 215);
		this.SetBlockUVCoordinates(BlockType.CityExterior05, 216);
		this.SetBlockUVCoordinates(BlockType.CityExterior06, 217);
		this.SetBlockUVCoordinates(BlockType.CityExterior07, 218);
		this.SetBlockUVCoordinates(BlockType.CityExterior08, 219);
		this.SetBlockUVCoordinates(BlockType.CityExterior09, 220);
		this.SetBlockUVCoordinates(BlockType.CityExterior10, 221);
		this.SetBlockUVCoordinates(BlockType.CityExterior11, 222);
		this.SetBlockUVCoordinates(BlockType.CityExterior12, 223);
		this.SetBlockUVCoordinates(BlockType.CityExterior13, 224);
		this.SetBlockUVCoordinates(BlockType.CityExterior14, 225);
		this.SetBlockUVCoordinates(BlockType.CityExterior15, 226);
		this.SetBlockUVCoordinates(BlockType.CityExterior16, 227);
		this.SetBlockUVCoordinates(BlockType.CityExterior17, 228);
		this.SetBlockUVCoordinates(BlockType.CityExterior18, 229);
		this.SetBlockUVCoordinates(BlockType.CityExterior19, 230);
		this.SetBlockUVCoordinates(BlockType.CityExterior20, 231);
		this.SetBlockUVCoordinates(BlockType.CityExterior21, 232);
		this.SetBlockUVCoordinates(BlockType.CityExterior22, 233);
		this.SetBlockUVCoordinates(BlockType.CityExterior23, 234);
		this.SetBlockUVCoordinates(BlockType.CityExterior24, 235);
		this.SetBlockUVCoordinates(BlockType.CityExterior25, 236);
		this.SetBlockUVCoordinates(BlockType.CityExterior26, 237);
		this.SetBlockUVCoordinates(BlockType.CityExterior27, 238);
		this.SetBlockUVCoordinates(BlockType.CityExterior28, 239);
		this.SetBlockUVCoordinates(BlockType.CityExterior29, 240);
		this.SetBlockUVCoordinates(BlockType.CityExterior30, 241);
		this.SetBlockUVCoordinates(BlockType.NbBeige, 244);
		this.SetBlockUVCoordinates(BlockType.NbBlue, 245);
		this.SetBlockUVCoordinates(BlockType.NbBrown, 246);
		this.SetBlockUVCoordinates(BlockType.NbDarkGrey, 247);
		this.SetBlockUVCoordinates(BlockType.NbGray, 248);
		this.SetBlockUVCoordinates(BlockType.NbKhaki, 249);
		this.SetBlockUVCoordinates(BlockType.Autumn1, 251, 250, 1);
		this.SetBlockUVCoordinates(BlockType.Autumn2, 254, 253, 254);
		this.SetBlockUVCoordinates(BlockType.Autumn3, 252, 252, 252);
	}

	private void SetBlockUVCoordinates(BlockType blockType, int topIndex, int sideIndex, int bottomIndex)
	{
		this.BlockUvCoordinates[blockType] = new BlockUVCoordinates(this.WorldTextureAtlasUvs[topIndex], this.WorldTextureAtlasUvs[sideIndex], this.WorldTextureAtlasUvs[bottomIndex]);
		this.BlockTextures[blockType] = WorldGameObjectX.Instance.World_Textures[sideIndex];
	}

	private void SetBlockUVCoordinates(BlockType blockType, int Index)
	{
		this.SetBlockUVCoordinates(blockType, Index, Index, Index);
	}

	public int GetMaxBlockX()
	{
		return this._ChunksWide * this._ChunkBlockWidth - 1;
	}

	public int GetMaxBlockY()
	{
		return this._ChunksHigh * this._ChunkBlockHeight - 1;
	}

	public int ChunksWide
	{
		get
		{
			return this._ChunksWide;
		}
		set
		{
			this._ChunksWide = value;
		}
	}

	public int ChunksHigh
	{
		get
		{
			return this._ChunksHigh;
		}
		set
		{
			this._ChunksHigh = value;
		}
	}

	public int ChunksDeep
	{
		get
		{
			return this._ChunksDeep;
		}
		set
		{
			this._ChunksDeep = value;
		}
	}

	public int ChunkBlockWidth
	{
		get
		{
			return this._ChunkBlockWidth;
		}
		set
		{
			this._ChunkBlockWidth = value;
		}
	}

	public int ChunkBlockHeight
	{
		get
		{
			return this._ChunkBlockHeight;
		}
		set
		{
			this._ChunkBlockHeight = value;
		}
	}

	public int ChunkBlockDepth
	{
		get
		{
			return this._ChunkBlockDepth;
		}
		set
		{
			this._ChunkBlockDepth = value;
		}
	}

	public int WidthInBlocks
	{
		get
		{
			return this._ChunksWide * this._ChunkBlockWidth;
		}
	}

	public int HeightInBlocks
	{
		get
		{
			return this._ChunksHigh * this._ChunkBlockHeight;
		}
	}

	public int DepthInBlocks
	{
		get
		{
			return this._ChunksDeep * this._ChunkBlockDepth;
		}
	}

	public int TotalChunks
	{
		get
		{
			return this._ChunksWide * this._ChunksHigh;
		}
	}

	public int CenterChunkX
	{
		get
		{
			return this._ChunksWide / 2;
		}
	}

	public int CenterChunkY
	{
		get
		{
			return this._ChunksHigh / 2;
		}
	}

	public PChunkList AllChunks
	{
		get
		{
			PChunkList pchunkList = PChunkList.Acquire();
			for (int i = 0; i < this._ChunksWide; i++)
			{
				for (int j = 0; j < this._ChunksHigh; j++)
				{
					for (int k = 0; k < this._ChunksDeep; k++)
					{
						pchunkList.Add(this.Chunks[i, j, k]);
					}
				}
			}
			return pchunkList;
		}
	}

	public void SetDimensions(int chunksWide, int chunksHigh, int chunksDeep, int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth)
	{
		this.ChunksWide = chunksWide;
		this.ChunksHigh = chunksHigh;
		this.ChunksDeep = chunksDeep;
		this.ChunkBlockWidth = chunkBlockWidth;
		this.ChunkBlockHeight = chunkBlockHeight;
		this.ChunkBlockDepth = chunkBlockDepth;
		this.m_ChunkBufferLength = this._ChunkBlockDepth * this._ChunkBlockHeight * this._ChunkBlockWidth;
	}

	public void InitializeGridChunks()
	{
		this.Chunks = new Chunk[this._ChunksWide, this._ChunksHigh, this._ChunksDeep];
		for (int i = 0; i < this._ChunksWide; i++)
		{
			for (int j = 0; j < this._ChunksHigh; j++)
			{
				for (int k = 0; k < this._ChunksDeep; k++)
				{
					this.Chunks[i, j, k] = new Chunk(i, j, k);
					this.Chunks[i, j, k].InitializeBlocks(this.ChunkBlockWidth, this.ChunkBlockHeight, this.ChunkBlockDepth);
				}
			}
		}
	}

	public float GlobalXOffset
	{
		get
		{
			return this.m_GlobalXOffset;
		}
		set
		{
			this.m_GlobalXOffset = value;
		}
	}

	public float GlobalZOffset
	{
		get
		{
			return this.m_GlobalZOffset;
		}
		set
		{
			this.m_GlobalZOffset = value;
		}
	}

	public float NoiseBlockXOffset
	{
		get
		{
			return this.m_NoiseBlockXOffset;
		}
		set
		{
			this.m_NoiseBlockXOffset = value;
		}
	}

	public Chunk GetChunk(int x, int y, int z)
	{
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		return this.Chunks[num, num2, num3];
	}

	public Chunk GetChunkSafe(int x, int y, int z)
	{
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		if (num < 0 || num > this.Chunks.GetUpperBound(0) || num2 < 0 || num2 > this.Chunks.GetUpperBound(1) || num3 < 0 || num3 > this.Chunks.GetUpperBound(2))
		{
			return null;
		}
		return this.Chunks[num, num2, num3];
	}

	public Vector3 GetChunkInd(int x, int y, int z)
	{
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		return new Vector3((float)num, (float)num2, (float)num3);
	}

	public void SetBlockTypeWithLightRegeneration(int x, int y, int z, BlockType blockType, BlockKind kind = BlockKind.Default)
	{
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		int num4 = x % this.ChunkBlockWidth;
		int num5 = y % this.ChunkBlockHeight;
		int num6 = z % this.ChunkBlockDepth;
		this.Chunks[num, num2, num3].SetBlockType(blockType, num4, num5, num6);
		this.Chunks[num, num2, num3].SetBlockKind(kind, num4, num5, num6);
		this.SetBlockTypeRegeneration(blockType, num, num2, num3, num4, num5, num6);
	}

	public void SetBlockTypeWithRegeneration(int x, int y, int z, BlockType blockType, BlockKind kind = BlockKind.Default)
	{
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		int num4 = x % this.ChunkBlockWidth;
		int num5 = y % this.ChunkBlockHeight;
		int num6 = z % this.ChunkBlockDepth;
		BlockType blockType2 = this.Chunks[num, num2, num3].GetBlockType(num4, num5, num6);
		this.Chunks[num, num2, num3].SetBlockType(blockType, num4, num5, num6);
		this.Chunks[num, num2, num3].SetBlockKind(kind, num4, num5, num6);
		this.SetBlockTypeRegeneration(blockType2, num, num2, num3, num4, num5, num6);
	}

	private void SetBlockTypeRegeneration(BlockType blockType, int chunkX, int chunkY, int chunkZ, int blockX, int blockY, int blockZ)
	{
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
		{
			this.Chunks[chunkX, chunkY, chunkZ].NeedsGlassRegen = true;
		}
		else if (blockType == BlockType.Water)
		{
			this.Chunks[chunkX, chunkY, chunkZ].NeedsWaterRegen = true;
		}
		else
		{
			this.Chunks[chunkX, chunkY, chunkZ].NeedsRegeneration = true;
		}
		if (blockX == 0 && chunkX > 0)
		{
			if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
			{
				this.Chunks[chunkX - 1, chunkY, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				this.Chunks[chunkX - 1, chunkY, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				this.Chunks[chunkX - 1, chunkY, chunkZ].NeedsRegeneration = true;
			}
		}
		else if (blockX == this.ChunkBlockWidth - 1 && chunkX < this.ChunksWide - 1)
		{
			if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
			{
				this.Chunks[chunkX + 1, chunkY, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				this.Chunks[chunkX + 1, chunkY, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				this.Chunks[chunkX + 1, chunkY, chunkZ].NeedsRegeneration = true;
			}
		}
		if (blockY == 0 && chunkY > 0)
		{
			if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
			{
				this.Chunks[chunkX, chunkY - 1, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				this.Chunks[chunkX, chunkY - 1, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				this.Chunks[chunkX, chunkY - 1, chunkZ].NeedsRegeneration = true;
			}
		}
		else if (blockY == this.ChunkBlockHeight - 1 && chunkY < this.ChunksHigh - 1)
		{
			if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
			{
				this.Chunks[chunkX, chunkY + 1, chunkZ].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				this.Chunks[chunkX, chunkY + 1, chunkZ].NeedsWaterRegen = true;
			}
			else
			{
				this.Chunks[chunkX, chunkY + 1, chunkZ].NeedsRegeneration = true;
			}
		}
		if (blockZ == 0 && chunkZ > 0)
		{
			if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
			{
				this.Chunks[chunkX, chunkY, chunkZ - 1].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				this.Chunks[chunkX, chunkY, chunkZ - 1].NeedsWaterRegen = true;
			}
			else
			{
				this.Chunks[chunkX, chunkY, chunkZ - 1].NeedsRegeneration = true;
			}
		}
		else if (blockZ == this.ChunkBlockDepth - 1 && chunkZ < this.ChunksDeep - 1)
		{
			if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
			{
				this.Chunks[chunkX, chunkY, chunkZ + 1].NeedsGlassRegen = true;
			}
			else if (blockType == BlockType.Water)
			{
				this.Chunks[chunkX, chunkY, chunkZ + 1].NeedsWaterRegen = true;
			}
			else
			{
				this.Chunks[chunkX, chunkY, chunkZ + 1].NeedsRegeneration = true;
			}
		}
	}

	public void SetBlockType(int x, int y, int z, BlockType blockType)
	{
		if (x < 0 || y < 0 || z < 0 || x >= this.WidthInBlocks || y >= this.HeightInBlocks || z >= this.DepthInBlocks)
		{
			return;
		}
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		int x2 = x % this.ChunkBlockWidth;
		int y2 = y % this.ChunkBlockHeight;
		int z2 = z % this.ChunkBlockDepth;
		this.Chunks[num, num2, num3].SetBlockType(blockType, x2, y2, z2);
	}

	public BlockType GetBlockType(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0 || x >= this.WidthInBlocks || y >= this.HeightInBlocks || z >= this.DepthInBlocks)
		{
			return BlockType.Air;
		}
		int num = x / this.ChunkBlockWidth;
		int num2 = y / this.ChunkBlockHeight;
		int num3 = z / this.ChunkBlockDepth;
		int x2 = x % this.ChunkBlockWidth;
		int y2 = y % this.ChunkBlockHeight;
		int z2 = z % this.ChunkBlockDepth;
		return this.Chunks[num, num2, num3].GetBlockType(x2, y2, z2);
	}

	public bool IsValidBlock(int x, int y, int z)
	{
		return x >= 0 && y >= 0 && z >= 0 && x < this.WidthInBlocks && y < this.HeightInBlocks && z < this.DepthInBlocks;
	}

	public void SetChunkWaterRegen(int x, int y, int z)
	{
		Chunk chunkSafe = this.GetChunkSafe(x, y, z);
		if (chunkSafe != null)
		{
			chunkSafe.NeedsWaterRegen = true;
		}
	}

	public PChunkList ChunksNeedingWaterRegeneration
	{
		get
		{
			PChunkList pchunkList = PChunkList.Acquire();
			pchunkList.Clear();
			for (int i = 0; i < this._ChunksWide; i++)
			{
				for (int j = 0; j < this._ChunksHigh; j++)
				{
					for (int k = 0; k < this._ChunksDeep; k++)
					{
						if (this.Chunks[i, j, k].NeedsWaterRegen)
						{
							pchunkList.Add(this.Chunks[i, j, k]);
						}
					}
				}
			}
			return pchunkList;
		}
	}

	public PChunkList ChunksNeedingGlassRegeneration
	{
		get
		{
			PChunkList pchunkList = PChunkList.Acquire();
			pchunkList.Clear();
			for (int i = 0; i < this._ChunksWide; i++)
			{
				for (int j = 0; j < this._ChunksHigh; j++)
				{
					for (int k = 0; k < this._ChunksDeep; k++)
					{
						if (this.Chunks[i, j, k].NeedsGlassRegen)
						{
							pchunkList.Add(this.Chunks[i, j, k]);
						}
					}
				}
			}
			return pchunkList;
		}
	}

	public PChunkList ChunksNeedingLandRegeneration
	{
		get
		{
			PChunkList pchunkList = PChunkList.Acquire();
			pchunkList.Clear();
			for (int i = 0; i < this._ChunksWide; i++)
			{
				for (int j = 0; j < this._ChunksHigh; j++)
				{
					for (int k = 0; k < this._ChunksDeep; k++)
					{
						if (this.Chunks[i, j, k].NeedsRegeneration)
						{
							pchunkList.Add(this.Chunks[i, j, k]);
						}
					}
				}
			}
			return pchunkList;
		}
	}

	public void AddFinishedLandChunk(Chunk chunk)
	{
		this.FinishedLandChunks.Enqueue(chunk);
	}

	public void AddFinishedWaterChunk(Chunk chunk)
	{
		this.FinishedWaterChunks.Enqueue(chunk);
	}

	public void AddFinishedGlassChunk(Chunk chunk)
	{
		this.FinishedGlassChunks.Enqueue(chunk);
	}

	public Chunk GetFinishedLandChunk()
	{
		if (this.FinishedLandChunks.Count == 0)
		{
			return null;
		}
		return this.FinishedLandChunks.Dequeue();
	}

	public Chunk GetFinishedWaterChunk()
	{
		if (this.FinishedWaterChunks.Count == 0)
		{
			return null;
		}
		return this.FinishedWaterChunks.Dequeue();
	}

	public Chunk GetFinishedGlassChunk()
	{
		if (this.FinishedGlassChunks.Count == 0)
		{
			return null;
		}
		return this.FinishedGlassChunks.Dequeue();
	}

	public Rect[] WorldTextureAtlasUvs { get; set; }

	public Dictionary<BlockType, BlockUVCoordinates> BlockUvCoordinates
	{
		get
		{
			return this.m_BlockUVCoordinates;
		}
	}

	public static WorldData Instance;

	private int _ChunksWide = 8;

	private int _ChunksHigh = 8;

	private int _ChunksDeep = 8;

	private int _ChunkBlockWidth = 16;

	private int _ChunkBlockHeight = 16;

	private int _ChunkBlockDepth = 16;

	public int m_ChunkBufferLength;

	private float m_NoiseBlockXOffset = (float)App.Instance.Rnd.Next(0, 10000);

	private readonly Dictionary<BlockType, BlockUVCoordinates> m_BlockUVCoordinates = new Dictionary<BlockType, BlockUVCoordinates>();

	public Dictionary<BlockType, Texture2D> BlockTextures = new Dictionary<BlockType, Texture2D>();

	private float m_GlobalXOffset;

	private float m_GlobalZOffset;

	public Chunk[,,] Chunks;

	public TQueue<Chunk> FinishedLandChunks = new TQueue<Chunk>();

	public TQueue<Chunk> FinishedWaterChunks = new TQueue<Chunk>();

	public TQueue<Chunk> FinishedGlassChunks = new TQueue<Chunk>();
}
