using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Store
{
	static Store()
	{
		Dictionary<EntityType, Store.EntityInfo> dictionary = new Dictionary<EntityType, Store.EntityInfo>();
		dictionary.Add(EntityType.FISH_1, new Store.EntityInfo
		{
			Purchase = StorePurchase.FISH_1,
			SpriteName = "fish_1",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.FISH_2, new Store.EntityInfo
		{
			Purchase = StorePurchase.FISH_2,
			SpriteName = "fish_2",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.FISH_3, new Store.EntityInfo
		{
			Purchase = StorePurchase.FISH_3,
			SpriteName = "fish_3",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.CHICKEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CHICKEN,
			SpriteName = "chicken_pet",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.BOAR, new Store.EntityInfo
		{
			Purchase = StorePurchase.BOAR,
			SpriteName = "pet_01_icon",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.CRAB, new Store.EntityInfo
		{
			Purchase = StorePurchase.CRAB,
			SpriteName = "pet_02_icon",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.DOG, new Store.EntityInfo
		{
			Purchase = StorePurchase.DOG,
			SpriteName = "DogIcon",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.CAT, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAT,
			SpriteName = "CatIcon",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.CAT_BLACK, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAT_BLACK,
			SpriteName = "CatBlackIcon",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.CAT_STRIPED, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAT_STRIPED,
			SpriteName = "CatGrayIcon",
			Tab = Store.TabType.Pets
		});
		dictionary.Add(EntityType.FLOWER_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER,
			SpriteName = "flower_bell_blue",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.FLOWER_RED, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER,
			SpriteName = "flower_bell_red",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.FLOWER_WHITE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER,
			SpriteName = "flower_bell_white",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.FLOWER_YELLOW, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER,
			SpriteName = "flower_bell_yellow",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.FLOWER_PERPLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER2,
			SpriteName = "flower_purple",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.BLUEBERY, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER2,
			SpriteName = "blueberry",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.STROBERY, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER2,
			SpriteName = "strawberry",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.CAMMOLINE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER2,
			SpriteName = "cammoline",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.BELLSMALL, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER2,
			SpriteName = "flower_bell_small",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.FERN, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER3,
			SpriteName = "Fern",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.WEAT, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER3,
			SpriteName = "wheet",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.SHROOM1, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER3,
			SpriteName = "shrum1",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.SHROOM2, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER3,
			SpriteName = "shrum2",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.GRASS, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLOWER3,
			SpriteName = "grass",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.TNT, new Store.EntityInfo
		{
			Purchase = StorePurchase.DINAMIT,
			SpriteName = "TNT",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.ATOMBOMB, new Store.EntityInfo
		{
			Purchase = StorePurchase.ATOMBOMB,
			SpriteName = "AviaBomb",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BOAT, new Store.EntityInfo
		{
			Purchase = StorePurchase.BOAT,
			SpriteName = "boat",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TROLLEY, new Store.EntityInfo
		{
			Purchase = StorePurchase.TROLLEY,
			SpriteName = "lorry",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.RAIL, new Store.EntityInfo
		{
			Purchase = StorePurchase.RAIL,
			SpriteName = "rails",
			Tab = Store.TabType.Decorations
		});
		Dictionary<EntityType, Store.EntityInfo> dictionary2 = dictionary;
		EntityType key = EntityType.SPAWN_ARROW;
		Store.EntityInfo entityInfo = new Store.EntityInfo();
		entityInfo.Purchase = StorePurchase.SPAWN_POINT;
		entityInfo.SpriteName = "checkpoint_flag_icon_01";
		entityInfo.Tab = Store.TabType.Tools;
		entityInfo.Validator = (() => Level.Instance.IsAdmin(null));
		dictionary2.Add(key, entityInfo);
		Dictionary<EntityType, Store.EntityInfo> dictionary3 = dictionary;
		EntityType key2 = EntityType.TEAM_SPAWN_ARROW_BLUE;
		entityInfo = new Store.EntityInfo();
		entityInfo.Purchase = StorePurchase.TEAM_SPAWN_POINT;
		entityInfo.SpriteName = "spawn_arrow_blue";
		entityInfo.Tab = Store.TabType.Tools;
		entityInfo.Validator = (() => Level.Instance.IsAdmin(null));
		dictionary3.Add(key2, entityInfo);
		Dictionary<EntityType, Store.EntityInfo> dictionary4 = dictionary;
		EntityType key3 = EntityType.TEAM_SPAWN_ARROW_RED;
		entityInfo = new Store.EntityInfo();
		entityInfo.Purchase = StorePurchase.TEAM_SPAWN_POINT;
		entityInfo.SpriteName = "spawn_arrow_red";
		entityInfo.Tab = Store.TabType.Tools;
		entityInfo.Validator = (() => Level.Instance.IsAdmin(null));
		dictionary4.Add(key3, entityInfo);
		dictionary.Add(EntityType.GOLD_KUBOK, new Store.EntityInfo
		{
			Purchase = StorePurchase.GOLD_KUBOK,
			SpriteName = "gold_cubok",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CHAIR, new Store.EntityInfo
		{
			Purchase = StorePurchase.STOOL,
			SpriteName = "chair_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CHAIR_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CHAIR_STONE,
			SpriteName = "chair_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TABLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.TABLE,
			SpriteName = "table_02_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TABLE_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.TABLE_STONE,
			SpriteName = "table_02_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BIG_TABLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.BIG_TABLE,
			SpriteName = "table_01_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BUCKET, new Store.EntityInfo
		{
			Purchase = StorePurchase.BACKET,
			SpriteName = "bucket",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BASKET, new Store.EntityInfo
		{
			Purchase = StorePurchase.BASKET,
			SpriteName = "basket",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BARREL, new Store.EntityInfo
		{
			Purchase = StorePurchase.BARREL,
			SpriteName = "barrel",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_H, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE,
			SpriteName = "fence_(X)_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_I, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE,
			SpriteName = "fence_(tile)_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_L, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE,
			SpriteName = "fence_(corner)_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_T, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE,
			SpriteName = "fence_(T)_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_STONE_X, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE_STONE,
			SpriteName = "fence_(X)_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_STONE_I, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE_STONE,
			SpriteName = "fence_(tile)_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_STONE_L, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE_STONE,
			SpriteName = "fence_(corner)_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FENCE_STONE_T, new Store.EntityInfo
		{
			Purchase = StorePurchase.FENCE_STONE,
			SpriteName = "fence_(T)_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CHEST, new Store.EntityInfo
		{
			Purchase = StorePurchase.CHEST,
			SpriteName = "chest_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CHEST_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CHEST_STONE,
			SpriteName = "chest_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.SIGN, new Store.EntityInfo
		{
			Purchase = StorePurchase.SIGN,
			SpriteName = "sign",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BED, new Store.EntityInfo
		{
			Purchase = StorePurchase.BED,
			SpriteName = "bed_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BED_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.BED_STONE,
			SpriteName = "bed_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CUBOARDS, new Store.EntityInfo
		{
			Purchase = StorePurchase.CUPBOARDS,
			SpriteName = "case_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CUPBOARDL, new Store.EntityInfo
		{
			Purchase = StorePurchase.CUPBOARDL,
			SpriteName = "big_case_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.STAIRSW, new Store.EntityInfo
		{
			Purchase = StorePurchase.STAIRSW,
			SpriteName = "stairs_w",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.STAIRSC, new Store.EntityInfo
		{
			Purchase = StorePurchase.STAIRSB,
			SpriteName = "stairs_s",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DOORW, new Store.EntityInfo
		{
			Purchase = StorePurchase.DOORW,
			SpriteName = "door_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DOORM, new Store.EntityInfo
		{
			Purchase = StorePurchase.DOORM,
			SpriteName = "door_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TABLICHKAF, new Store.EntityInfo
		{
			Purchase = StorePurchase.TABLICHKA,
			SpriteName = "tablet_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TABLICHKAW, new Store.EntityInfo
		{
			Purchase = StorePurchase.TABLICHKAW,
			SpriteName = "tablet_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TORCH, new Store.EntityInfo
		{
			Purchase = StorePurchase.TORCHW,
			SpriteName = "torch_01_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TORCH_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.TORCH_STONE,
			SpriteName = "torch_02_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TORCH_FLOOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.TORCHF,
			SpriteName = "torch_02_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TORCH_FLOOR_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.TORCH_FLOOR_STONE,
			SpriteName = "torch_02_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LADDER, new Store.EntityInfo
		{
			Purchase = StorePurchase.LADDER,
			SpriteName = "ladder",
			Tab = Store.TabType.Decorations
		});
		Dictionary<EntityType, Store.EntityInfo> dictionary5 = dictionary;
		EntityType key4 = EntityType.PICTURE3_2;
		entityInfo = new Store.EntityInfo();
		entityInfo.Purchase = StorePurchase.PICTURE3_2;
		entityInfo.SpriteName = "frame_icon3_2";
		entityInfo.Tab = Store.TabType.Decorations;
		entityInfo.Validator = (() => Level.Instance.IsAdmin(null));
		dictionary5.Add(key4, entityInfo);
		dictionary.Add(EntityType.BOOK, new Store.EntityInfo
		{
			Purchase = StorePurchase.BOOK,
			SpriteName = "object_icon_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BENCH_WOOD, new Store.EntityInfo
		{
			Purchase = StorePurchase.BENCH_WOOD,
			SpriteName = "bench_wood",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BENCH_STONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.BENCH_STONE,
			SpriteName = "bench_stone",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TOMBSTONE_1, new Store.EntityInfo
		{
			Purchase = StorePurchase.TOMBSTONE_1,
			SpriteName = "tombstone_01 (1)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TOMBSTONE_2, new Store.EntityInfo
		{
			Purchase = StorePurchase.TOMBSTONE_2,
			SpriteName = "tombstone_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_CAKE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_CAKE,
			SpriteName = "cake",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_WOOD_PLATE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_WOOD_PLATE,
			SpriteName = "wood_plate",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_SPOON, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_SPOON,
			SpriteName = "spoon",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_FORK, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_FORK,
			SpriteName = "fork",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_MUG, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_MUG,
			SpriteName = "mug",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_JUG, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_JUG,
			SpriteName = "jug",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_METAL_PLATE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_METAL_PLATE,
			SpriteName = "metal_plate",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_BREAD, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_BREAD,
			SpriteName = "bread",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_CHEES, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_CHEES,
			SpriteName = "cheese",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_CHIKEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_CHIKEN,
			SpriteName = "chicken",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_SVININA, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_SVININA,
			SpriteName = "pork",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_FISH, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_FISH,
			SpriteName = "fish",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_STEIK, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_STEIK,
			SpriteName = "meat",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_ORANGE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_ORANGE,
			SpriteName = "orange",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_PIZZA, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_PIZZA,
			SpriteName = "pizza",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_COLA, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_COLA,
			SpriteName = "cola",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.BURGER, new Store.EntityInfo
		{
			Purchase = StorePurchase.BURGER,
			SpriteName = "burger",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FOOD_PIROG, new Store.EntityInfo
		{
			Purchase = StorePurchase.FOOD_PIROG,
			SpriteName = "cheesecake",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TEAM_DOOR_RED, new Store.EntityInfo
		{
			Purchase = StorePurchase.TEAMDOORS,
			SpriteName = "double-doors_team_01_icon_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.TEAM_DOOR_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.TEAMDOORS,
			SpriteName = "double-doors_team_02_icon_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FLAG_RED, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG,
			SpriteName = "flag_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.FLAG_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG,
			SpriteName = "flag_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_01,
			SpriteName = "painting_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_02,
			SpriteName = "painting_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_03, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_03,
			SpriteName = "painting_03",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_04, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_04,
			SpriteName = "painting_04",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_05, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_05,
			SpriteName = "painting_05",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_06, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_06,
			SpriteName = "painting_06",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_07, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_07,
			SpriteName = "painting_07",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_08, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_08,
			SpriteName = "painting_08",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_09, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_09,
			SpriteName = "painting_09",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_10, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_10,
			SpriteName = "painting_10",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_12, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_12,
			SpriteName = "painting_12",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.PAINTING_13, new Store.EntityInfo
		{
			Purchase = StorePurchase.PAINTING_13,
			SpriteName = "painting_13",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_DOOR_RED, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_DOOR,
			SpriteName = "door_red",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_DOOR_YELLOW, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_DOOR,
			SpriteName = "door_yellow",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_DOOR_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_DOOR,
			SpriteName = "door_blue",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_DOOR_GREEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_DOOR,
			SpriteName = "door_green",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_DOOR_WHITE, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_DOOR,
			SpriteName = "door_white",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_KEY_RED, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_KEY,
			SpriteName = "key_red",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_KEY_YELLOW, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_KEY,
			SpriteName = "key_yellow",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_KEY_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_KEY,
			SpriteName = "key_blue",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_KEY_GREEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_KEY,
			SpriteName = "key_green",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.LOCK_KEY_WHITE, new Store.EntityInfo
		{
			Purchase = StorePurchase.LOCK_KEY,
			SpriteName = "key_white",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.MILITARY_ARMORY, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_ARMORY,
			SpriteName = "m_armory",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_BARREL, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_BARREL,
			SpriteName = "m_barrel",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_BED, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_BED,
			SpriteName = "m_bed",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CAM, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CAM,
			SpriteName = "m_cam",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CASE_BIG, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CASE_BIG,
			SpriteName = "m_case_big",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CASE_SMALL, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CASE_SMALL,
			SpriteName = "m_case_small",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CHAIR, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CHAIR,
			SpriteName = "m_chair",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CHEST, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CHEST,
			SpriteName = "m_chest",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CONSOLE_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CONSOLE_01,
			SpriteName = "m_console_01",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CONSOLE_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CONSOLE_02,
			SpriteName = "m_console_02",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_CONSOLE_03, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_CONSOLE_03,
			SpriteName = "m_console_03",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_DOOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_DOOR,
			SpriteName = "m_door",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_LAMP, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_LAMP,
			SpriteName = "m_lamp",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_LANTERN, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_LANTERN,
			SpriteName = "m_lantern",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_MUG, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_MUG,
			SpriteName = "m_mug",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_PC, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_PC,
			SpriteName = "m_PC",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_TABLE_BIG, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_TABLE_BIG,
			SpriteName = "m_table_big",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_TABLE_SMALL, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_TABLE_SMALL,
			SpriteName = "m_table_small",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_FENCE_L, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_FENCE,
			SpriteName = "m_fence_(corner)",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_FENCE_END, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_FENCE,
			SpriteName = "m_fence_(end)",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_FENCE_T, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_FENCE,
			SpriteName = "m_fence_(T)",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_FENCE_I, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_FENCE,
			SpriteName = "m_fence_(tile)",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_FENCE_X, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_FENCE,
			SpriteName = "m_fence_(X)",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_BENCH, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_BENCH,
			SpriteName = "m_bench",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_BOX_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_BOX_01,
			SpriteName = "m_box_01",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_BOX_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_BOX_02,
			SpriteName = "m_box_02",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.MILITARY_BRIEFING_BOARD, new Store.EntityInfo
		{
			Purchase = StorePurchase.MILITARY_BRIEFING_BOARD,
			SpriteName = "m_briefing_board",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.HG_DROP_BAG, new Store.EntityInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = string.Empty,
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.HG_ARENA_SPAWN_POINT, new Store.EntityInfo
		{
			Purchase = StorePurchase.HG_ARENA_SPAWN_POINT,
			SpriteName = "arena_flag_icon_inv",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.HG_APOS_SPAWN_POINT, new Store.EntityInfo
		{
			Purchase = StorePurchase.HG_APOS_SPAWN_POINT,
			SpriteName = "basement_icon_inv",
			Tab = Store.TabType.Tools
		});
		dictionary.Add(EntityType.DARK_CASTLE_BED, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_BED,
			SpriteName = "dark_castle_bed",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_BENCH, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_BENCH,
			SpriteName = "dark_castle_bench",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_BOOK_CONSOLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_BOOK_CONSOLE,
			SpriteName = "dark_castle_book_console",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_BOX_BIG, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_BOX_BIG,
			SpriteName = "dark_castle_box_big",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_BOX_BIG_BOOKS, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_BOX_BIG_BOOKS,
			SpriteName = "dark_castle_box_big_books",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_BOX_SMALL, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_BOX_SMALL,
			SpriteName = "dark_castle_box_small",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_CAGE, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CAGE,
			SpriteName = "dark_castle_cage",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_CHAIN_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CHAIN,
			SpriteName = "dark_castle_chain_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_CHAIN_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CHAIN,
			SpriteName = "dark_castle_chain_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_CHAIN_03, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CHAIN,
			SpriteName = "dark_castle_chain_03",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_CHAIR, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CHAIR,
			SpriteName = "dark_castle_chair",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_CHEST, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CHEST,
			SpriteName = "dark_castle_chest",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_COFFIN_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_COFFIN,
			SpriteName = "dark_castle_coffin_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_COFFIN_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_COFFIN,
			SpriteName = "dark_castle_coffin_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_DARK_PICTURE_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_DARK_PICTURE_01,
			SpriteName = "dark_castle_dark_picture_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_DARK_PICTURE_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_DARK_PICTURE_02,
			SpriteName = "dark_castle_dark_picture_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_DARK_PICTURE_03, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_DARK_PICTURE_03,
			SpriteName = "dark_castle_dark_picture_03",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_DOOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_DOOR,
			SpriteName = "dark_castle_door",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_FENCE_L, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_FENCE,
			SpriteName = "dark_castle_fence_(corner)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_FENCE_END, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_FENCE,
			SpriteName = "dark_castle_fence_(end)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_FENCE_T, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_FENCE,
			SpriteName = "dark_castle_fence_(T)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_FENCE_I, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_FENCE,
			SpriteName = "dark_castle_fence_(tile)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_FENCE_X, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_FENCE,
			SpriteName = "dark_castle_fence_(X)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_GARGOYLEY, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_GARGOYLEY,
			SpriteName = "dark_castle_gargoyley",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_HERALDRY, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_HERALDRY,
			SpriteName = "dark_castle_heraldry",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_SARCOPHAGUS, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_SARCOPHAGUS,
			SpriteName = "dark_castle_sarcophagus",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_SKULL_MUG, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_SKULL_MUG,
			SpriteName = "dark_castle_skull-mug",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_STANDARD_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_STANDARD_01,
			SpriteName = "dark_castle_standard_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_STANDARD_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_STANDARD_02,
			SpriteName = "dark_castle_standard_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_TABLE_BIG, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_TABLE_BIG,
			SpriteName = "dark_castle_table_big",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_TABLE_SMALL, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_TABLE_SMALL,
			SpriteName = "dark_castle_table_small",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_THRONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_THRONE,
			SpriteName = "dark_castle_throne",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_TORCH_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_TORCH_01,
			SpriteName = "dark_castle_torch_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_TORCH_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_TORCH_02,
			SpriteName = "dark_castle_torch_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.DARK_CASTLE_TROLL, new Store.EntityInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_TROLL,
			SpriteName = "dark_castle_troll",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_01, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_01,
			SpriteName = "CARPET_01",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_02, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_02,
			SpriteName = "CARPET_02",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_03, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_03,
			SpriteName = "CARPET_03",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_04, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_04,
			SpriteName = "CARPET_04",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_05, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_05,
			SpriteName = "CARPET_05",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_06, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_06,
			SpriteName = "CARPET_06",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_07, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_07,
			SpriteName = "CARPET_07",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_08, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_08,
			SpriteName = "CARPET_08",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_09, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_09,
			SpriteName = "CARPET_09",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_10, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_10,
			SpriteName = "CARPET_10",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CARPET_11TH, new Store.EntityInfo
		{
			Purchase = StorePurchase.CARPET_11TH,
			SpriteName = "CARPET_11_(tile_horizontal)",
			Tab = Store.TabType.Decorations
		});
		dictionary.Add(EntityType.CE_ATTENTION_BOARD, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_ATTENTION_BOARD,
			SpriteName = "attention-board_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_BIG_GARBAGE_BOX_CLOSE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_BIG_GARBAGE_BOX_CLOSE,
			SpriteName = "big_garbage_box_close_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_BIG_GARBAGE_BOX_OPEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_BIG_GARBAGE_BOX_OPEN,
			SpriteName = "big_garbage_box_open_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_BIG_STREET_LANTERN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_BIG_STREET_LANTERN,
			SpriteName = "big_street_lantern_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_CONE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_CONE,
			SpriteName = "cone_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_FG, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_FG,
			SpriteName = "FG_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_GARBAGE_BOX, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_GARBAGE_BOX,
			SpriteName = "garbage_box_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_MAIL_BOX, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_MAIL_BOX,
			SpriteName = "mail-box_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_METAL_DOOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_METAL_DOOR,
			SpriteName = "metal_door_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_PHONEBOX, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_PHONEBOX,
			SpriteName = "phonebox_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_SMALL_SREET_LANTERN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_SMALL_SREET_LANTERN,
			SpriteName = "small_street_lantern_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_STREET_BENCH, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_STREET_BENCH,
			SpriteName = "street_bench_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_STREET_CLOCK, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_STREET_CLOCK,
			SpriteName = "street_clock_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_STREET_LADDER, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_STREET_LADDER,
			SpriteName = "street_ladder_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CE_WOOD_DOOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CE_WOOD_DOOR,
			SpriteName = "wood_door_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_BATH, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_BATH,
			SpriteName = "bath",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_BED, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_BED,
			SpriteName = "bed",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_BEDROOM_CASE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_BEDROOM_CASE,
			SpriteName = "bedroom_case",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_BEDROOM_LAMP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_BEDROOM_LAMP,
			SpriteName = "bedroom_lamp",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_BIG_LAMP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_BIG_LAMP,
			SpriteName = "big_lamp",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_BOWL, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_BOWL,
			SpriteName = "bowl",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_COFFEE_CUP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_COFFEE_CUP,
			SpriteName = "coffee-cup",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_DOOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_DOOR,
			SpriteName = "door",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_KITCHEN_BOARD, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_KITCHEN_BOARD,
			SpriteName = "kitchen_board",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_KITCHEN_CHAIR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_KITCHEN_CHAIR,
			SpriteName = "kitchen_chair",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_KITCHEN_PART, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_KITCHEN_PART,
			SpriteName = "kitchen_part",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_KITCHEN_PART_CORNER, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_KITCHEN_PART_CORNER,
			SpriteName = "kitchen_part_corner",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_KITCHEN_TABLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_KITCHEN_TABLE,
			SpriteName = "kitchen_table",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_KITCHEN_WASHING, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_KITCHEN_WASHING,
			SpriteName = "kitchen_washing",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_WLAPTOP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_WLAPTOP,
			SpriteName = "laptop",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_LOUDSPEAKERS, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_LOUDSPEAKERS,
			SpriteName = "loudspeakers",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_MICROWAVE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_MICROWAVE,
			SpriteName = "microwave",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_OFFICE_CHAIR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_OFFICE_CHAIR,
			SpriteName = "office-chair",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_OFFICE_LAMP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_OFFICE_LAMP,
			SpriteName = "office-lamp",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_OFFICE_TABLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_OFFICE_TABLE,
			SpriteName = "office-table",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_OFFICE_CASE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_OFFICE_CASE,
			SpriteName = "office_case",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_OVEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_OVEN,
			SpriteName = "oven",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_PLATE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_PLATE,
			SpriteName = "plate",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_REFRIGERATOR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_REFRIGERATOR,
			SpriteName = "refrigerator",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SAUCER, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SAUCER,
			SpriteName = "saucer",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SHOWER, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SHOWER,
			SpriteName = "shower",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SMALL_LAMP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SMALL_LAMP,
			SpriteName = "small_lamp",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SMALL_TABLE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SMALL_TABLE,
			SpriteName = "small_table",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SOAP, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SOAP,
			SpriteName = "soap",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SOFA, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SOFA,
			SpriteName = "sofa",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_SOFT_CHAIR, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_SOFT_CHAIR,
			SpriteName = "soft-chair",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_TV, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_TV,
			SpriteName = "TV",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_WARDROBE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_WARDROBE,
			SpriteName = "wardrobe",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_WASHBASIN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_WASHBASIN,
			SpriteName = "washbasin",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_WWASHING_MACHINE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_WWASHING_MACHINE,
			SpriteName = "washing_machine",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.CI_WC, new Store.EntityInfo
		{
			Purchase = StorePurchase.CI_WC,
			SpriteName = "WC",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NY_TREE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_TREE,
			SpriteName = "ny_tree_icon",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.PL_CACTUS, new Store.EntityInfo
		{
			Purchase = StorePurchase.PL_CACTUS,
			SpriteName = "cactus",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.PL_WATERLILY, new Store.EntityInfo
		{
			Purchase = StorePurchase.PL_WATERLILY,
			SpriteName = "Waterlily",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.PL_WATERLILY2, new Store.EntityInfo
		{
			Purchase = StorePurchase.PL_WATERLILY2,
			SpriteName = "waterlily2",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.HOUSEPLANTS1, new Store.EntityInfo
		{
			Purchase = StorePurchase.HOUSEPLANTS1,
			SpriteName = "Houseplants1",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.HOUSEPLANTS2, new Store.EntityInfo
		{
			Purchase = StorePurchase.HOUSEPLANTS2,
			SpriteName = "Houseplants2",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.HOUSEPLANTS3, new Store.EntityInfo
		{
			Purchase = StorePurchase.HOUSEPLANTS3,
			SpriteName = "Houseplants3",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.HOUSEPLANTS4, new Store.EntityInfo
		{
			Purchase = StorePurchase.HOUSEPLANTS4,
			SpriteName = "Houseplants4",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.HOUSEPLANTS5, new Store.EntityInfo
		{
			Purchase = StorePurchase.HOUSEPLANTS5,
			SpriteName = "Houseplants5",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.MUSH_WHITE, new Store.EntityInfo
		{
			Purchase = StorePurchase.MUSH_WHITE,
			SpriteName = "Mushrooms1",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.MUSH_FOX, new Store.EntityInfo
		{
			Purchase = StorePurchase.MUSH_FOX,
			SpriteName = "Mushrooms2",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.MUSH_AMANITA, new Store.EntityInfo
		{
			Purchase = StorePurchase.MUSH_AMANITA,
			SpriteName = "Mushrooms3",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.MUSH_ORANGE_CAP, new Store.EntityInfo
		{
			Purchase = StorePurchase.MUSH_ORANGE_CAP,
			SpriteName = "Mushrooms4",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.MUSH_TOADSTOOL, new Store.EntityInfo
		{
			Purchase = StorePurchase.MUSH_TOADSTOOL,
			SpriteName = "Mushrooms5",
			Tab = Store.TabType.Plants
		});
		dictionary.Add(EntityType.NPC1, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC1,
			SpriteName = "NPC1",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC2, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC2,
			SpriteName = "NPC2",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC3, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC3,
			SpriteName = "NPC3",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC4, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC4,
			SpriteName = "NPC4",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC5, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC5,
			SpriteName = "NPC5",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC6, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC6,
			SpriteName = "NPC6",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC9, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC9,
			SpriteName = "NPC9",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC10, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC10,
			SpriteName = "NPC10",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC11, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC11,
			SpriteName = "NPC11",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC12, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC12,
			SpriteName = "NPC12",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC13, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC13,
			SpriteName = "NPC13",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC14, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC14,
			SpriteName = "NPC14",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC15, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC15,
			SpriteName = "NPC15",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC16, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC16,
			SpriteName = "NPC16",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC17, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC17,
			SpriteName = "NPC17",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC18, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC18,
			SpriteName = "NPC18",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NPC19, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC19,
			SpriteName = "NPC19",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG1, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG1,
			SpriteName = "Flag1",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG2, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG2,
			SpriteName = "Flag2",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG3, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG3,
			SpriteName = "Flag3",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG4, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG4,
			SpriteName = "Flag4",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG5, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG5,
			SpriteName = "Flag5",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG6, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG6,
			SpriteName = "Flag6",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG7, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG7,
			SpriteName = "Flag7",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG8, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG8,
			SpriteName = "Flag8",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG9, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG9,
			SpriteName = "Flag9",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG10, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG10,
			SpriteName = "Flag10",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.FLAG12, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG12,
			SpriteName = "Flag12",
			Tab = Store.TabType.Decorations_New
		});
		dictionary.Add(EntityType.NY_16_TREE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_TREE,
			SpriteName = "TreeHome",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY_16_BIG_TREE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_BIG_TREE,
			SpriteName = "Tree",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY_16_GIFT, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_GIFT,
			SpriteName = "Gift",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY4_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_GIFT,
			SpriteName = "NY4_Blue",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY4_GREEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_GIFT,
			SpriteName = "NY4_Green",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY_16_GIFT_BAG, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_GIFT_BAG,
			SpriteName = "GiftBag",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY_16_SNOWMAN, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_SNOWMAN,
			SpriteName = "Showman",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY_16_SOCK, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_SOCK,
			SpriteName = "Sock",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY_16_WREATH, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY_16_WREATH,
			SpriteName = "Wreath",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY9, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY9,
			SpriteName = "NY9",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY10, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY10,
			SpriteName = "NY10",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY11, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY11,
			SpriteName = "NY11",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY12_DIGGER, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY12,
			SpriteName = "NY12_Digger",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY12_HOUSE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY12,
			SpriteName = "NY12_House",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY12_TREE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY12,
			SpriteName = "NY12_Tree",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.NY12_SNOWFLAKE, new Store.EntityInfo
		{
			Purchase = StorePurchase.NY12,
			SpriteName = "NY12_Snowflake",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.FIREWORK1, new Store.EntityInfo
		{
			Purchase = StorePurchase.FIREWORK1,
			SpriteName = "FireworkRed",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.FIREWORK2, new Store.EntityInfo
		{
			Purchase = StorePurchase.FIREWORK1,
			SpriteName = "FireworkYellow",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.FIREWORK3, new Store.EntityInfo
		{
			Purchase = StorePurchase.FIREWORK1,
			SpriteName = "FireworkBlue",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.FIREWORK4, new Store.EntityInfo
		{
			Purchase = StorePurchase.FIREWORK1,
			SpriteName = "FireworkGreen",
			Tab = Store.TabType.Decorations_Star,
			Category = "NY"
		});
		dictionary.Add(EntityType.BEACH1, new Store.EntityInfo
		{
			Purchase = StorePurchase.BEACH1,
			SpriteName = "Beach1",
			Tab = Store.TabType.Decorations_Star,
			Category = "Beach"
		});
		dictionary.Add(EntityType.BEACH2, new Store.EntityInfo
		{
			Purchase = StorePurchase.BEACH2,
			SpriteName = "Beach2",
			Tab = Store.TabType.Decorations_Star,
			Category = "Beach"
		});
		dictionary.Add(EntityType.BEACH3, new Store.EntityInfo
		{
			Purchase = StorePurchase.BEACH3,
			SpriteName = "Beach3",
			Tab = Store.TabType.Decorations_Star,
			Category = "Beach"
		});
		dictionary.Add(EntityType.BEACH4, new Store.EntityInfo
		{
			Purchase = StorePurchase.BEACH4,
			SpriteName = "Beach4",
			Tab = Store.TabType.Decorations_Star,
			Category = "Beach"
		});
		dictionary.Add(EntityType.BEACH5, new Store.EntityInfo
		{
			Purchase = StorePurchase.BEACH5,
			SpriteName = "Beach5",
			Tab = Store.TabType.Decorations_Star,
			Category = "Beach"
		});
		dictionary.Add(EntityType.BEACH6, new Store.EntityInfo
		{
			Purchase = StorePurchase.BEACH6,
			SpriteName = "Beach6",
			Tab = Store.TabType.Decorations_Star,
			Category = "Beach"
		});
		dictionary.Add(EntityType.SCHOOL1, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL1,
			SpriteName = "School1",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL2, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL2,
			SpriteName = "School2",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL3, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL3,
			SpriteName = "School3",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL4, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL4,
			SpriteName = "School4",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL5_YELLOW, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL5,
			SpriteName = "School5_Yellow",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL5_RED, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL5,
			SpriteName = "School5_Red",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL5_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL5,
			SpriteName = "School5_Blue",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.SCHOOL6, new Store.EntityInfo
		{
			Purchase = StorePurchase.SCHOOL6,
			SpriteName = "School6",
			Tab = Store.TabType.Decorations_Star,
			Category = "School"
		});
		dictionary.Add(EntityType.CAMPING1_BLUE, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING1,
			SpriteName = "Camping1_Blue",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING1_YELLOW, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING1,
			SpriteName = "Camping1_Yellow",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING1_GREEN, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING1,
			SpriteName = "Camping1_Green",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING2, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING2,
			SpriteName = "Camping2",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING3, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING3,
			SpriteName = "Camping3",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING4, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING4,
			SpriteName = "Camping4",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING5, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING5,
			SpriteName = "Camping5",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.CAMPING6, new Store.EntityInfo
		{
			Purchase = StorePurchase.CAMPING6,
			SpriteName = "Camping6",
			Tab = Store.TabType.Decorations_Star,
			Category = "Camping"
		});
		dictionary.Add(EntityType.HALLOWEEN1, new Store.EntityInfo
		{
			Purchase = StorePurchase.HALLOWEEN1,
			SpriteName = "Halloween1",
			Tab = Store.TabType.Decorations_Star,
			Category = "Halloween"
		});
		dictionary.Add(EntityType.HALLOWEEN2, new Store.EntityInfo
		{
			Purchase = StorePurchase.HALLOWEEN2,
			SpriteName = "Halloween2",
			Tab = Store.TabType.Decorations_Star,
			Category = "Halloween"
		});
		dictionary.Add(EntityType.HALLOWEEN3, new Store.EntityInfo
		{
			Purchase = StorePurchase.HALLOWEEN3,
			SpriteName = "Halloween3",
			Tab = Store.TabType.Decorations_Star,
			Category = "Halloween"
		});
		dictionary.Add(EntityType.HALLOWEEN4, new Store.EntityInfo
		{
			Purchase = StorePurchase.HALLOWEEN4,
			SpriteName = "Halloween4",
			Tab = Store.TabType.Decorations_Star,
			Category = "Halloween"
		});
		dictionary.Add(EntityType.HALLOWEEN5, new Store.EntityInfo
		{
			Purchase = StorePurchase.HALLOWEEN5,
			SpriteName = "Halloween5",
			Tab = Store.TabType.Decorations_Star,
			Category = "Halloween"
		});
		dictionary.Add(EntityType.HALLOWEEN6, new Store.EntityInfo
		{
			Purchase = StorePurchase.HALLOWEEN6,
			SpriteName = "Halloween6",
			Tab = Store.TabType.Decorations_Star,
			Category = "Halloween"
		});
		dictionary.Add(EntityType.WESTERN1, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN1,
			SpriteName = "Western1",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.WESTERN2, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN2,
			SpriteName = "Western2",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.WESTERN3, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN3,
			SpriteName = "Western3",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.WESTERN4, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN4,
			SpriteName = "Western4",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.WESTERN5, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN5,
			SpriteName = "Western5",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.WESTERN6, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN6,
			SpriteName = "Western6",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.WESTERN7, new Store.EntityInfo
		{
			Purchase = StorePurchase.WESTERN7,
			SpriteName = "Western7",
			Tab = Store.TabType.Decorations_Star,
			Category = "Western"
		});
		dictionary.Add(EntityType.EAST1, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST1,
			SpriteName = "East1",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.EAST2, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST2,
			SpriteName = "East2",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.EAST3, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST3,
			SpriteName = "East3",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.EAST4, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST4,
			SpriteName = "East4",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.EAST5, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST5,
			SpriteName = "East5",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.EAST6_WHITE, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST6,
			SpriteName = "East6_White",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.EAST6_BLACK, new Store.EntityInfo
		{
			Purchase = StorePurchase.EAST6,
			SpriteName = "East6_Black",
			Tab = Store.TabType.Decorations_Star,
			Category = "East"
		});
		dictionary.Add(EntityType.PIRATES1, new Store.EntityInfo
		{
			Purchase = StorePurchase.PIRATES1,
			SpriteName = "Pirates1",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.PIRATES2, new Store.EntityInfo
		{
			Purchase = StorePurchase.PIRATES2,
			SpriteName = "Pirates2",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.PIRATES3, new Store.EntityInfo
		{
			Purchase = StorePurchase.PIRATES3,
			SpriteName = "Pirates3",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.PIRATES4, new Store.EntityInfo
		{
			Purchase = StorePurchase.PIRATES4,
			SpriteName = "Pirates4",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.PIRATES5, new Store.EntityInfo
		{
			Purchase = StorePurchase.PIRATES5,
			SpriteName = "Pirates5",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.PIRATES6, new Store.EntityInfo
		{
			Purchase = StorePurchase.PIRATES6,
			SpriteName = "Pirates6",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.FLAG11, new Store.EntityInfo
		{
			Purchase = StorePurchase.FLAG11,
			SpriteName = "Flag11",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.NPC7, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC7,
			SpriteName = "NPC7",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		dictionary.Add(EntityType.NPC8, new Store.EntityInfo
		{
			Purchase = StorePurchase.NPC8,
			SpriteName = "NPC8",
			Tab = Store.TabType.Decorations_Star,
			Category = "Pirates"
		});
		Store.Entities = dictionary;
		Dictionary<BlockType, Store.BlockInfo> dictionary6 = new Dictionary<BlockType, Store.BlockInfo>();
		dictionary6.Add(BlockType.WoodPlank, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "WoodPlank1",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.WoodPlank2, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "WoodPlank2",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.WoodPlank3, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "woodPlank3",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.WoodPlank4, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "WoodPlank4",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.WoodPlank5, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "WoodPlank5",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.Crates, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES4,
			SpriteName = "crateц",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.Wood8, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES4,
			SpriteName = "wood8",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.Wood9, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES4,
			SpriteName = "wood9",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.Wood10, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES4,
			SpriteName = "wood10",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.Wood11, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES4,
			SpriteName = "wood11",
			Tab = Store.TabType.Blocks_Wood
		});
		dictionary6.Add(BlockType.Brick, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "Brick1",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Brick2, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "Brick2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Brick3, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "Brick3",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Brick4, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "Brick4",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Brick6, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "Brick6",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Brick7, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "Brick7",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone10, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone10",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone10_1, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone10_1",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone9_2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone9_2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone14, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone14",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone14_1, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone14_1",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone14_2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone14_2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone14_4, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone14_4",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone15, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone15",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone15_1, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES1,
			SpriteName = "stone15_1",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone15_2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone15_2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone16, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone16",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone16_2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone16_2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone16_3, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone16_3",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone18, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone18",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone18_1, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone18_1",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone18_2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone18_2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone17, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone17",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone17_1, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone17_1",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Stone17_2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES2,
			SpriteName = "stone17_2",
			Tab = Store.TabType.Blocks_Stone
		});
		dictionary6.Add(BlockType.Colored1, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored1",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored2, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored2",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored3, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored3",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored4, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored4",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored5, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored5",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored6, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored6",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored7, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored7",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored8, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored8",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Colored9, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBES3,
			SpriteName = "colored9",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.Wood, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "bark_new1",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.TopSoil, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "grass-dirt_new1",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Leaves, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "leaf_new",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Dirt, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "dirt2",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Stone, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "rock_new",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Sand, new Store.BlockInfo
		{
			Purchase = StorePurchase.NONE,
			SpriteName = "sand",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Water, new Store.BlockInfo
		{
			Purchase = StorePurchase.WATER,
			SpriteName = "water",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Lava, new Store.BlockInfo
		{
			Purchase = StorePurchase.LAVA,
			SpriteName = "Лава3",
			Tab = Store.TabType.Blocks_Nature
		});
		Dictionary<BlockType, Store.BlockInfo> dictionary7 = dictionary6;
		BlockType key5 = BlockType.SnowDirt;
		Store.BlockInfo blockInfo = new Store.BlockInfo();
		blockInfo.Purchase = StorePurchase.NONE;
		blockInfo.SpriteName = "snow_dirt";
		blockInfo.Tab = Store.TabType.Blocks_Nature;
		blockInfo.Validator = (() => App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND);
		dictionary7.Add(key5, blockInfo);
		Dictionary<BlockType, Store.BlockInfo> dictionary8 = dictionary6;
		BlockType key6 = BlockType.Snow;
		blockInfo = new Store.BlockInfo();
		blockInfo.Purchase = StorePurchase.NONE;
		blockInfo.SpriteName = "snow";
		blockInfo.Tab = Store.TabType.Blocks_Nature;
		blockInfo.Validator = (() => App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND);
		dictionary8.Add(key6, blockInfo);
		dictionary6.Add(BlockType.Ice, new Store.BlockInfo
		{
			Purchase = StorePurchase.ICE,
			SpriteName = "ice",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Gum, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBE_BATUTE,
			SpriteName = "gum2",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.TeamBlue, new Store.BlockInfo
		{
			Purchase = StorePurchase.TEAMCUBES,
			SpriteName = "brick_blue",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.TeamRed, new Store.BlockInfo
		{
			Purchase = StorePurchase.TEAMCUBES,
			SpriteName = "brick_red",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.Glass1, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "glass wood",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass2, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "glass metall",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass3, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass7",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass4, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass6",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass5, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass5",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass6, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass4",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass7, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass3",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass8, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass2",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Glass9, new Store.BlockInfo
		{
			Purchase = StorePurchase.GLASSCUBES,
			SpriteName = "colored_glass1",
			Tab = Store.TabType.Blocks_Glass
		});
		dictionary6.Add(BlockType.Reshetka1, new Store.BlockInfo
		{
			Purchase = StorePurchase.RESHETKACUBES,
			SpriteName = "cell1",
			Tab = Store.TabType.Blocks_Cell
		});
		dictionary6.Add(BlockType.Reshetka2, new Store.BlockInfo
		{
			Purchase = StorePurchase.RESHETKACUBES,
			SpriteName = "cell2",
			Tab = Store.TabType.Blocks_Cell
		});
		dictionary6.Add(BlockType.Reshetka3, new Store.BlockInfo
		{
			Purchase = StorePurchase.RESHETKACUBES,
			SpriteName = "cell3",
			Tab = Store.TabType.Blocks_Cell
		});
		dictionary6.Add(BlockType.Reshetka4, new Store.BlockInfo
		{
			Purchase = StorePurchase.RESHETKACUBES,
			SpriteName = "cell4",
			Tab = Store.TabType.Blocks_Cell
		});
		dictionary6.Add(BlockType.Reshetka5, new Store.BlockInfo
		{
			Purchase = StorePurchase.RESHETKACUBES,
			SpriteName = "cell5",
			Tab = Store.TabType.Blocks_Cell
		});
		dictionary6.Add(BlockType.Tile1, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_01",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Tile2, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_02",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Tile3, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_03",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Tile4, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_04",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Tile5, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_05",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Tile7, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_07",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Tile9, new Store.BlockInfo
		{
			Purchase = StorePurchase.TILE_CUBES,
			SpriteName = "tile_09",
			Tab = Store.TabType.Blocks_Tile
		});
		dictionary6.Add(BlockType.Castle01_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_01_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle02_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_02_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle03_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_03_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle04_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_04_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle05_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_05_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle06, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_06",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle07_Grate, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_07_grate",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle08_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_08_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Castle09, new Store.BlockInfo
		{
			Purchase = StorePurchase.CASTLE_CUBES,
			SpriteName = "castle_texture_09",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon01_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_01_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon02_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_02_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon03, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_03",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon04_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_04_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon05_FloorElement, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_05_floor_element",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon06_WallBottom, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_06_wall_bottom",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon07_WallElement, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_07_wall_element",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon08_WoodBar, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_08_wood_bar",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon09_WoodBox, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_09_wood_box",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon10_Grate, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_10_grate",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon11_WallElement, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_11_wall_element",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon12_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_12_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Dungeon13_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.DUNGEON_CUBES,
			SpriteName = "dungeon_texture_13_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DUNGEON_CUBES"
		});
		dictionary6.Add(BlockType.Fortress01_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_01_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress02_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_02_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress03_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_03_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress04_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_04_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress05_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_05_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress06_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_06_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress07_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_07_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress08_Grate, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_08_grate",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress09_WoodBox, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_09_wood_box",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Fortress10_WoodBar, new Store.BlockInfo
		{
			Purchase = StorePurchase.FORTRESS_CUBES,
			SpriteName = "fortress_texture_10_wood_bar",
			Tab = Store.TabType.Blocks_Sets,
			Category = "FORTRESS_CUBES"
		});
		dictionary6.Add(BlockType.Library01_WallBottom, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_01_wall_bottom",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Library02_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_02_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Library03_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_03_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Library04_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_04_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Library05_WallElement, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_05_wall_element",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Library06_BookShelf, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_06_book_shelf",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Library07, new Store.BlockInfo
		{
			Purchase = StorePurchase.LIBRARY_CUBES,
			SpriteName = "library_texture_07",
			Tab = Store.TabType.Blocks_Sets,
			Category = "LIBRARY_CUBES"
		});
		dictionary6.Add(BlockType.Tavern01_WallBottom, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_01_wall_bottom",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern02_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_02_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern03_WallElement, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_03_wall_element",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern04_WoodBox, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_04_wood_box",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern05_Floor, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_05_floor",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern06_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_06_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern07_Wall, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_07_wall",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern08_FloorElement, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_08_floor_element",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern09_WoodBar, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_09_wood_bar",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern10_WallBottom, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_10_wall_bottom",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Tavern11_WallBottom, new Store.BlockInfo
		{
			Purchase = StorePurchase.TAVERN_CUBES,
			SpriteName = "tavern_texture_11_wall_bottom",
			Tab = Store.TabType.Blocks_Sets,
			Category = "TAVERN_CUBES"
		});
		dictionary6.Add(BlockType.Military01, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_01",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military02, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_02",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military03, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_03",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military04, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_04",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military05, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_05",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military06, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_06",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military07, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_07",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military08, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_08",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military09, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_09",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military10, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_10",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military11, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_11",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Military12, new Store.BlockInfo
		{
			Purchase = StorePurchase.MILITARY_CUBES,
			SpriteName = "military_12",
			Tab = Store.TabType.Blocks_Sets,
			Category = "MILITARY_CUBES"
		});
		dictionary6.Add(BlockType.Industrial01, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_01",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial02, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_02",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial03, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_03",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial04, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_04",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial05, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_05",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial06, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_06",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial07, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_07",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial08, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_08",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial09, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_09",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial10, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_10",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial11, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_11",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.Industrial12, new Store.BlockInfo
		{
			Purchase = StorePurchase.INDUSTRIAL_CUBES,
			SpriteName = "industrial_12",
			Tab = Store.TabType.Blocks_Sets,
			Category = "INDUSTRIAL_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle01, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_01",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle02, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_02",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle03, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_03",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle04, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_04",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle05, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_05",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle06, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_06",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle07, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_07",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle08, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_08",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle09, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_09",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle10, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_10",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle11, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_11",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle12, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_12",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle13, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_13",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle14, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_14",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle15, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_15",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle16, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_16",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle17, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_17",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle18, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_18",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle19, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_19",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.DarkCastle20, new Store.BlockInfo
		{
			Purchase = StorePurchase.DARK_CASTLE_CUBES,
			SpriteName = "dark_castle_20",
			Tab = Store.TabType.Blocks_Sets,
			Category = "DARK_CASTLE_CUBES"
		});
		dictionary6.Add(BlockType.RestoreWhenStep, new Store.BlockInfo
		{
			Purchase = StorePurchase.CUBE_RESTORE,
			SpriteName = "restore_when_step",
			Tab = Store.TabType.Blocks_Nature
		});
		dictionary6.Add(BlockType.CityInterio01, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_01",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio02, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_02",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio03, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_03",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio04, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_04",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio05, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_05",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio06, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_06",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio07, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_07",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio08, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_08",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio09, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_09",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio10, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_10",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio11, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_11",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio12, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_12",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio13, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_13",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio14, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_14",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio15, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_15",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio16, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_16",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio17, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_17",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio18, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_18",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio19, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_19",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio20, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_20",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityInterio21, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_INTERIOR,
			SpriteName = "city_interior_21",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_INTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior01, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_01",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior02, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_02",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior03, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_03",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior04, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_04",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior05, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_05",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior06, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_06",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior07, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_07",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior08, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_08",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior09, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_09",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior10, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_10",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior11, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_11",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior12, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_12",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior13, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_13",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior14, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_14",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior15, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_15",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior16, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_16",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior17, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_17",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior18, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_18",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior19, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_19",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior20, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_20",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior21, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_21",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior22, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_22",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior23, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_23",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior24, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_24",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior25, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_25",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior26, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_26",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior27, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_27",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior28, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_28",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior29, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_29",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.CityExterior30, new Store.BlockInfo
		{
			Purchase = StorePurchase.CITY_EXTERIOR,
			SpriteName = "city_exterior_30",
			Tab = Store.TabType.Blocks_Sets,
			Category = "CITY_EXTERIOR"
		});
		dictionary6.Add(BlockType.NbBeige, new Store.BlockInfo
		{
			Purchase = StorePurchase.NBBEIGE,
			SpriteName = "beigeiconinv",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.NbBlue, new Store.BlockInfo
		{
			Purchase = StorePurchase.NBBLU,
			SpriteName = "blueiconinv",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.NbBrown, new Store.BlockInfo
		{
			Purchase = StorePurchase.NBBROWN,
			SpriteName = "browniconinv",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.NbDarkGrey, new Store.BlockInfo
		{
			Purchase = StorePurchase.NBDARKGREY,
			SpriteName = "darkgreyiconinv",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.NbGray, new Store.BlockInfo
		{
			Purchase = StorePurchase.NBGRAY,
			SpriteName = "greyiconinv",
			Tab = Store.TabType.Blocks_Colored
		});
		dictionary6.Add(BlockType.NbKhaki, new Store.BlockInfo
		{
			Purchase = StorePurchase.NBKHAKI,
			SpriteName = "khakiiconinv",
			Tab = Store.TabType.Blocks_Colored
		});
		Dictionary<BlockType, Store.BlockInfo> dictionary9 = dictionary6;
		BlockType key7 = BlockType.Autumn1;
		blockInfo = new Store.BlockInfo();
		blockInfo.Purchase = StorePurchase.NONE;
		blockInfo.SpriteName = "Autumn1";
		blockInfo.Tab = Store.TabType.Blocks_Nature;
		blockInfo.Validator = (() => App.Instance.Settings.mapType == GameINI.MapType.AUTUMN);
		dictionary9.Add(key7, blockInfo);
		Dictionary<BlockType, Store.BlockInfo> dictionary10 = dictionary6;
		BlockType key8 = BlockType.Autumn2;
		blockInfo = new Store.BlockInfo();
		blockInfo.Purchase = StorePurchase.NONE;
		blockInfo.SpriteName = "Autumn2";
		blockInfo.Tab = Store.TabType.Blocks_Nature;
		blockInfo.Validator = (() => App.Instance.Settings.mapType == GameINI.MapType.AUTUMN);
		dictionary10.Add(key8, blockInfo);
		Dictionary<BlockType, Store.BlockInfo> dictionary11 = dictionary6;
		BlockType key9 = BlockType.Autumn3;
		blockInfo = new Store.BlockInfo();
		blockInfo.Purchase = StorePurchase.NONE;
		blockInfo.SpriteName = "Autumn3";
		blockInfo.Tab = Store.TabType.Blocks_Nature;
		blockInfo.Validator = (() => App.Instance.Settings.mapType == GameINI.MapType.AUTUMN);
		dictionary11.Add(key9, blockInfo);
		Store.Blocks = dictionary6;
		Store.Purchases = new Dictionary<StorePurchase, Store.PurchaseInfo>
		{
			{
				StorePurchase.SLOTS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2400, false, 1),
					Name = "SLOTS",
					Tab = Store.TabType.Untagged,
					Icon = string.Empty
				}
			},
			{
				StorePurchase.FLOWER,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 50, 150, 300, 0),
					Name = "FLOWERS1",
					Tab = Store.TabType.Plants,
					Icon = "plants3"
				}
			},
			{
				StorePurchase.FLOWER2,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 70, 250, 500, 0),
					Name = "FLOWERS2",
					Tab = Store.TabType.Plants,
					Icon = "plants2"
				}
			},
			{
				StorePurchase.FLOWER3,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 100, 350, 700, 0),
					Name = "FLOWERS3",
					Tab = Store.TabType.Plants,
					Icon = "plants1"
				}
			},
			{
				StorePurchase.BONUS_LIFE,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 10, false, 1),
					Name = "LIFE",
					Tab = Store.TabType.Untagged,
					Icon = "ico_box_ammo_health"
				}
			},
			{
				StorePurchase.BONUS_ARMOR,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 10, false, 1),
					Name = "ARMOR",
					Tab = Store.TabType.Untagged,
					Icon = "ico_box_ammo_armor"
				}
			},
			{
				StorePurchase.BONUS_WEAPONS,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 10, false, 1),
					Name = "WEAPONS",
					Tab = Store.TabType.Untagged,
					Icon = "ico_box_ammo_rifle"
				}
			},
			{
				StorePurchase.CHANGENICK,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 1, false, 1),
					Name = "CHANGENICK",
					Tab = Store.TabType.Tools,
					Icon = "кирка+"
				}
			},
			{
				StorePurchase.ALL_INCLUSIVE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gems, 150, true, 1),
					Name = "ALL_INCLUSIVE",
					Tab = Store.TabType.Weapons,
					Icon = "premium_icon",
					LargeIcon = "premium_art_01",
					Category = "WEAPON_OTHER"
				}
			},
			{
				StorePurchase.NY_OFFER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gems, 150, true, 1),
					Name = "NY_OFFER",
					Tab = Store.TabType.Weapons,
					Icon = "premium_icon",
					LargeIcon = "premium_art_01",
					Category = "WEAPON_OTHER"
				}
			},
			{
				StorePurchase.NY_TREE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gems, 150, false, 1),
					Name = "NY_TREE",
					Tab = Store.TabType.Weapons,
					Icon = "premium_icon",
					LargeIcon = "premium_art_01",
					Category = "WEAPON_OTHER"
				}
			},
			{
				StorePurchase.WEAPON_LUGER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, true, 1),
					Name = "LUGER",
					Tab = Store.TabType.Weapons,
					Icon = "big_luger",
					Category = "WEAPON_PISTOLS",
					WeaponStats = new int[]
					{
						20,
						120,
						75,
						8
					}
				}
			},
			{
				StorePurchase.WEAPON_GLOK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, true, 1),
					Name = "GLOCK",
					Tab = Store.TabType.Weapons,
					Icon = "G18",
					Category = "WEAPON_PISTOLS",
					WeaponStats = new int[]
					{
						15,
						300,
						75,
						17
					},
					MinLevel = 6
				}
			},
			{
				StorePurchase.WEAPON_SAWN_OFF,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 400, true, 1),
					Name = "SAWN_OFF",
					Tab = Store.TabType.Weapons,
					Icon = "big_sawn_off",
					Category = "WEAPON_SHOTGUNS",
					WeaponStats = new int[]
					{
						100,
						60,
						8,
						2
					}
				}
			},
			{
				StorePurchase.SANTA_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "SANTA_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Digger_NY",
					Category = "SKIN_BASE",
					Skin = 29
				}
			},
			{
				StorePurchase.STANDART_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 120, true, 1),
					Name = "NORMAL_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Digger",
					Category = "SKIN_BASE",
					Skin = 0
				}
			},
			{
				StorePurchase.GIRL_SKIN_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 750, true, 1),
					Name = "GIRL_SKIN_1",
					Tab = Store.TabType.Skins,
					Icon = "Girl1",
					Category = "SKIN_BASE",
					Skin = 17
				}
			},
			{
				StorePurchase.GIRL_SKIN_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "GIRL_SKIN_2",
					Tab = Store.TabType.Skins,
					Icon = "Girl2",
					Category = "SKIN_BASE",
					Skin = 18,
					RequiredPurchase = StorePurchase.GIRL_SKIN_1
				}
			},
			{
				StorePurchase.POLICEMAN_GIRL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "POLICEMAN_GIRL",
					Tab = Store.TabType.Skins,
					Icon = "PolicemanGirl",
					Category = "SKIN_PRO",
					Skin = 30,
					New = 1
				}
			},
			{
				StorePurchase.SMELTER_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "SMELTER_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Smelter",
					Category = "SKIN_PRO",
					Skin = 24
				}
			},
			{
				StorePurchase.COOK_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "COOK_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Cook",
					Category = "SKIN_PRO",
					Skin = 21
				}
			},
			{
				StorePurchase.ARCHER_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1499, true, 1),
					Name = "ARCHER_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Archer",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 19,
					Sales = 25
				}
			},
			{
				StorePurchase.KNIGHT_SKIN_0,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "KNIGHT0",
					Tab = Store.TabType.Skins,
					Icon = "RookieKnight",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 13
				}
			},
			{
				StorePurchase.KNIGHT_SKIN_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2500, true, 1),
					Name = "KNIGHT",
					Tab = Store.TabType.Skins,
					Icon = "Knight",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 2,
					RequiredPurchase = StorePurchase.KNIGHT_SKIN_0
				}
			},
			{
				StorePurchase.KNIGHT_SKIN_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "EPIC_KNIGHT",
					Tab = Store.TabType.Skins,
					Icon = "EpicKnight",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 3,
					RequiredPurchase = StorePurchase.KNIGHT_SKIN_1
				}
			},
			{
				StorePurchase.VIKING_SKIN_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "VIKING",
					Tab = Store.TabType.Skins,
					Icon = "Viking",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 5,
					RequiredPurchase = StorePurchase.ARCHER_SKIN
				}
			},
			{
				StorePurchase.PIRATE_SKIN_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "PIRATE",
					Tab = Store.TabType.Skins,
					Icon = "Pirate",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 1,
					RequiredPurchase = StorePurchase.ARCHER_SKIN
				}
			},
			{
				StorePurchase.PIRATE_SKIN_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "CAPTAIN_PIRATE",
					Tab = Store.TabType.Skins,
					Icon = "CaptainPirate",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 4,
					RequiredPurchase = StorePurchase.PIRATE_SKIN_1
				}
			},
			{
				StorePurchase.ZOMBIE_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2499, true, 1),
					Name = "ZOMBIE",
					Tab = Store.TabType.Skins,
					Icon = "Zombie",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 6,
					Sales = 25
				}
			},
			{
				StorePurchase.SKELETON_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3500, true, 1),
					Name = "SKELETON",
					Tab = Store.TabType.Skins,
					Icon = "SkeletonWarrior",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 14
				}
			},
			{
				StorePurchase.DEATH_KNIGHT_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 5000, true, 1),
					Name = "DEATH_KNIGHT",
					Tab = Store.TabType.Skins,
					Icon = "DeathKnight",
					Category = "SKIN_MIDDLE_AGES",
					Skin = 7
				}
			},
			{
				StorePurchase.SOLDIER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2500, true, 1),
					Name = "STALKER",
					Tab = Store.TabType.Skins,
					Icon = "Stalker",
					Category = "SKIN_MODERNITY",
					Skin = 8
				}
			},
			{
				StorePurchase.DARK_STALKER_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "DARK_STALKER_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "DarkStalker",
					Category = "SKIN_MODERNITY",
					Skin = 22
				}
			},
			{
				StorePurchase.MERCENARY_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "MERCENARY_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Mercenary",
					Category = "SKIN_MODERNITY",
					Skin = 20
				}
			},
			{
				StorePurchase.TERRORIST_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3500, true, 1),
					Name = "TERRORIST_SKIN",
					Tab = Store.TabType.Skins,
					Icon = "Terrorist",
					Category = "SKIN_MODERNITY",
					Skin = 23
				}
			},
			{
				StorePurchase.SWAT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2499, true, 1),
					Name = "SWAT",
					Tab = Store.TabType.Skins,
					Icon = "SWAT",
					Category = "SKIN_MODERNITY",
					Skin = 10,
					Sales = 25
				}
			},
			{
				StorePurchase.SOVIET,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "SOVIET_SOLDIER",
					Tab = Store.TabType.Skins,
					Icon = "SovietSoldier",
					Category = "SKIN_WORLD_WAR_II",
					Skin = 9
				}
			},
			{
				StorePurchase.AMERICAN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "US_SOLDIER",
					Tab = Store.TabType.Skins,
					Icon = "AmericanSoldier",
					Category = "SKIN_WORLD_WAR_II",
					Skin = 11,
					RequiredPurchase = StorePurchase.SOVIET
				}
			},
			{
				StorePurchase.GERMAN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "GERMAN_OFICER",
					Tab = Store.TabType.Skins,
					Icon = "GermanOfficer",
					Category = "SKIN_WORLD_WAR_II",
					Skin = 12,
					RequiredPurchase = StorePurchase.AMERICAN
				}
			},
			{
				StorePurchase.IRON_KO_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3500, true, 1),
					Name = "IRON_KO",
					Tab = Store.TabType.Skins,
					Icon = "IronDigger",
					Category = "SKIN_SUPERHEROES",
					Skin = 15
				}
			},
			{
				StorePurchase.DARK_KO_SKIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 5000, true, 1),
					Name = "DARK_KO",
					Tab = Store.TabType.Skins,
					Icon = "DarkDigger",
					Category = "SKIN_SUPERHEROES",
					Skin = 16,
					RequiredPurchase = StorePurchase.IRON_KO_SKIN
				}
			},
			{
				StorePurchase.CUBES4,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 70, 250, 500, 0),
					Name = "WOODEN_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_wood",
					Category = "key2015"
				}
			},
			{
				StorePurchase.CUBES1,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 100, 350, 700, 0),
					Name = "STONE_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_stones_01",
					Category = "key2015"
				}
			},
			{
				StorePurchase.CUBES2,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 100, 350, 700, 0),
					Name = "STONE_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_stones_02",
					Category = "key2015"
				}
			},
			{
				StorePurchase.CUBES3,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 150, 500, 1000, 0),
					Name = "COLORED_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_colored",
					Category = "key2015"
				}
			},
			{
				StorePurchase.GLASSCUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 180, 600, 1200, 0),
					Name = "GLASS_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_glass",
					Category = "key2015"
				}
			},
			{
				StorePurchase.RESHETKACUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 180, 600, 1200, 0),
					Name = "GRATES_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_cell",
					Category = "key2015"
				}
			},
			{
				StorePurchase.TILE_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 180, 600, 1200, 0),
					Name = "TILE_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_tile",
					Category = "key2015"
				}
			},
			{
				StorePurchase.ICE,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 220, 800, 1600, 0),
					Name = "ICE_BLOCK",
					Tab = Store.TabType.Blocks,
					Icon = "block_ice",
					Category = "key2015"
				}
			},
			{
				StorePurchase.LAVA,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 220, 800, 1600, 0),
					Name = "LAVA_BLOCK",
					Tab = Store.TabType.Blocks,
					Icon = "block_lava",
					Category = "key2015"
				}
			},
			{
				StorePurchase.TEAMCUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 250, 1000, 2000, 0),
					Name = "TEAM_BLOCKS",
					Tab = Store.TabType.Blocks,
					Icon = "block_team",
					Category = "key2015"
				}
			},
			{
				StorePurchase.WATER,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 240, 900, 1300, 0),
					Name = "WATER_BLOCK",
					Tab = Store.TabType.Blocks,
					Icon = "block_water",
					Category = "key2015",
					Sales = 25
				}
			},
			{
				StorePurchase.CUBE_BATUTE,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 350, 1300, 3000, 0),
					Name = "TRAMPOLINE_BLOCK",
					Tab = Store.TabType.Blocks,
					Icon = "block_trampoline",
					Category = "key2015"
				}
			},
			{
				StorePurchase.CUBE_RESTORE,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 350, 1300, 3000, 0),
					Name = "RESTORE_BLOCK",
					Tab = Store.TabType.Blocks,
					Icon = "block_restore",
					Category = "key2015"
				}
			},
			{
				StorePurchase.TAVERN_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "TAVERN_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_tavern",
					Category = "key2016"
				}
			},
			{
				StorePurchase.LIBRARY_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "LIBRARY_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_library",
					Category = "key2016"
				}
			},
			{
				StorePurchase.DUNGEON_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "DUNGEON_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_dungeon",
					Category = "key2016"
				}
			},
			{
				StorePurchase.FORTRESS_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "FORTRESS_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_fortress",
					Category = "key2016"
				}
			},
			{
				StorePurchase.CASTLE_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "CASTLE_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_castle",
					Category = "key2016"
				}
			},
			{
				StorePurchase.MILITARY_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "MILITARY_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_military",
					Category = "key2016"
				}
			},
			{
				StorePurchase.INDUSTRIAL_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2500, true, 1),
					Name = "INDUSTRIAL_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_industrial",
					Category = "key2016"
				}
			},
			{
				StorePurchase.DARK_CASTLE_CUBES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2500, true, 1),
					Name = "DARK_CASTLE_CUBES",
					Tab = Store.TabType.Blocks,
					Icon = "block_set_dark_castle",
					Category = "key2016"
				}
			},
			{
				StorePurchase.CITY_INTERIOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "CITY_INTERIOR",
					Tab = Store.TabType.Blocks,
					Icon = "interior_set_icon_01",
					Category = "key2016"
				}
			},
			{
				StorePurchase.CITY_EXTERIOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "CITY_EXTERIOR",
					Tab = Store.TabType.Blocks,
					Icon = "exterior_set_icon_01",
					Category = "key2016"
				}
			},
			{
				StorePurchase.BLOCK_KIND_FENCE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_FENCE",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks1",
					Category = "key2017"
				}
			},
			{
				StorePurchase.BLOCK_KIND_HALF,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_HALF",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks2",
					Category = "key2017"
				}
			},
			{
				StorePurchase.BLOCK_KIND_QUARTER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_QUARTER",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks3",
					Category = "key2017"
				}
			},
			{
				StorePurchase.BLOCK_KIND_CORNER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_CORNER",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks4",
					Category = "key2017"
				}
			},
			{
				StorePurchase.BLOCK_KIND_DIAGONAL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_DIAGONAL",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks5",
					Category = "key2017"
				}
			},
			{
				StorePurchase.BLOCK_KIND_STAIR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_STAIR",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks6",
					Category = "key2017"
				}
			},
			{
				StorePurchase.BLOCK_KIND_STAIR_CORNER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1850, true, 1),
					Name = "BLOCK_KIND_STAIR_CORNER",
					Tab = Store.TabType.Blocks,
					Icon = "FormBlocks7",
					Category = "key2017"
				}
			},
			{
				StorePurchase.NBBEIGE,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 30, 120, 300, 0),
					Name = "NBBEIGE",
					Tab = Store.TabType.Blocks,
					Icon = "beigeicon",
					Category = "key2018"
				}
			},
			{
				StorePurchase.NBBLU,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 30, 120, 300, 0),
					Name = "NBBLU",
					Tab = Store.TabType.Blocks,
					Icon = "blueicon",
					Category = "key2018"
				}
			},
			{
				StorePurchase.NBBROWN,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 30, 120, 300, 0),
					Name = "NBBROWN",
					Tab = Store.TabType.Blocks,
					Icon = "brownicon",
					Category = "key2018"
				}
			},
			{
				StorePurchase.NBDARKGREY,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 30, 120, 300, 0),
					Name = "NBDARKGREY",
					Tab = Store.TabType.Blocks,
					Icon = "darkgreyicon",
					Category = "key2018"
				}
			},
			{
				StorePurchase.NBGRAY,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 30, 120, 300, 0),
					Name = "NBGRAY",
					Tab = Store.TabType.Blocks,
					Icon = "greyicon",
					Category = "key2018"
				}
			},
			{
				StorePurchase.NBKHAKI,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 30, 120, 300, 0),
					Name = "NBKHAKI",
					Tab = Store.TabType.Blocks,
					Icon = "khakiicon",
					Category = "key2018"
				}
			},
			{
				StorePurchase.BIOM_AUTUMN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1250, true, 1),
					Name = "AUTUMN",
					Tab = Store.TabType.Maps,
					Icon = "Biom_Autumn",
					Category = "SC_MAP_BIOM",
					Data = GameINI.MapType.AUTUMN,
					New = 1
				}
			},
			{
				StorePurchase.SNOW_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "WINTER",
					Tab = Store.TabType.Maps,
					Icon = "Biom_Winter",
					Category = "SC_MAP_BIOM",
					Data = GameINI.MapType.SNOWLAND
				}
			},
			{
				StorePurchase.SAND_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "DESERT",
					Tab = Store.TabType.Maps,
					Icon = "Biom_Desert",
					Category = "SC_MAP_BIOM",
					Data = GameINI.MapType.SAND
				}
			},
			{
				StorePurchase.OCEAN_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "OCEAN",
					Tab = Store.TabType.Maps,
					Icon = "Biom_Ocean",
					Category = "SC_MAP_BIOM",
					Data = GameINI.MapType.OCEAN
				}
			},
			{
				StorePurchase.ISLAND_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "ISLANDS",
					Tab = Store.TabType.Maps,
					Icon = "Biom_Islands",
					Category = "SC_MAP_BIOM",
					Data = GameINI.MapType.ISLAND
				}
			},
			{
				StorePurchase.TINY_MAP_SIZE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "TINY_MAP",
					Tab = Store.TabType.Maps,
					Icon = "MapSize64",
					Category = "SC_MAP_SIZE",
					Data = GameINI.MapSize.TINY
				}
			},
			{
				StorePurchase.MEDIUM_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "MIDDLE_MAP",
					Tab = Store.TabType.Maps,
					Icon = "MapSize192",
					Category = "SC_MAP_SIZE",
					Data = GameINI.MapSize.MEDIUM
				}
			},
			{
				StorePurchase.BOUNS_GRANADE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "LARGE_MAP",
					Tab = Store.TabType.Maps,
					Icon = "MapSize256",
					Category = "SC_MAP_SIZE",
					Data = GameINI.MapSize.LARGE,
					RequiredPurchase = StorePurchase.MEDIUM_MAP
				}
			},
			{
				StorePurchase.FLAT_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "FLAT_MAP",
					Tab = Store.TabType.Maps,
					Icon = "MapTypeFlat",
					Category = "SC_MAP_TYPE",
					Data = GameINI.MapType.FLAT
				}
			},
			{
				StorePurchase.PLATFORM_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "PLATFORM_MAP",
					Tab = Store.TabType.Maps,
					Icon = "MapTypePlatform",
					Category = "SC_MAP_TYPE",
					Data = GameINI.MapType.PLATFORM
				}
			},
			{
				StorePurchase.RUN_MAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 850, true, 1),
					Name = "RUN_MAP",
					Tab = Store.TabType.Maps,
					Icon = "MapTypeRun",
					Category = "SC_MAP_TYPE",
					Data = GameINI.MapType.LAVA
				}
			},
			{
				StorePurchase.FISH_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 200, false, 1),
					Name = "FISH1",
					Tab = Store.TabType.Pets,
					Icon = "fish_1",
					Category = "FISH_CATEGORY",
					Sales = 50
				}
			},
			{
				StorePurchase.FISH_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 200, false, 1),
					Name = "FISH2",
					Tab = Store.TabType.Pets,
					Icon = "fish_2",
					Category = "FISH_CATEGORY",
					Sales = 50
				}
			},
			{
				StorePurchase.FISH_3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 200, false, 1),
					Name = "FISH3",
					Tab = Store.TabType.Pets,
					Icon = "fish_3",
					Category = "FISH_CATEGORY",
					Sales = 50
				}
			},
			{
				StorePurchase.CAT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "CAT",
					Tab = Store.TabType.Pets,
					Icon = "CatIcon",
					Category = "CAT_CATEGORY"
				}
			},
			{
				StorePurchase.CAT_BLACK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "CAT_BLACK",
					Tab = Store.TabType.Pets,
					Icon = "CatBlackIcon",
					Category = "CAT_CATEGORY"
				}
			},
			{
				StorePurchase.CAT_STRIPED,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "CAT_STRIPED",
					Tab = Store.TabType.Pets,
					Icon = "CatGrayIcon",
					Category = "CAT_CATEGORY"
				}
			},
			{
				StorePurchase.CHICKEN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "CHIKEN",
					Tab = Store.TabType.Pets,
					Icon = "chicken_pet",
					Category = "key2006"
				}
			},
			{
				StorePurchase.CRAB,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "CRAB",
					Tab = Store.TabType.Pets,
					Icon = "pet_02_icon",
					Category = "key2006"
				}
			},
			{
				StorePurchase.BOAR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "BOAR",
					Tab = Store.TabType.Pets,
					Icon = "pet_01_icon",
					Category = "key2006"
				}
			},
			{
				StorePurchase.DOG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 349, false, 1),
					Name = "DOG",
					Tab = Store.TabType.Pets,
					Icon = "DogIcon",
					Category = "key2006"
				}
			},
			{
				StorePurchase.HOUSEPLANTS1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "HOUSEPLANTS1",
					Tab = Store.TabType.Plants,
					Icon = "Houseplants1",
					Category = "key2023"
				}
			},
			{
				StorePurchase.HOUSEPLANTS2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "HOUSEPLANTS2",
					Tab = Store.TabType.Plants,
					Icon = "Houseplants2",
					Category = "key2023"
				}
			},
			{
				StorePurchase.HOUSEPLANTS3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "HOUSEPLANTS3",
					Tab = Store.TabType.Plants,
					Icon = "Houseplants3",
					Category = "key2023"
				}
			},
			{
				StorePurchase.HOUSEPLANTS4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "HOUSEPLANTS4",
					Tab = Store.TabType.Plants,
					Icon = "Houseplants4",
					Category = "key2023"
				}
			},
			{
				StorePurchase.HOUSEPLANTS5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "HOUSEPLANTS5",
					Tab = Store.TabType.Plants,
					Icon = "Houseplants5",
					Category = "key2023"
				}
			},
			{
				StorePurchase.MUSH_WHITE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MUSH_WHITE",
					Tab = Store.TabType.Plants,
					Icon = "Mushrooms1",
					Category = "key2022"
				}
			},
			{
				StorePurchase.MUSH_FOX,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MUSH_FOX",
					Tab = Store.TabType.Plants,
					Icon = "Mushrooms2",
					Category = "key2022"
				}
			},
			{
				StorePurchase.MUSH_AMANITA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MUSH_AMANITA",
					Tab = Store.TabType.Plants,
					Icon = "Mushrooms3",
					Category = "key2022"
				}
			},
			{
				StorePurchase.MUSH_ORANGE_CAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MUSH_ORANGE_CAP",
					Tab = Store.TabType.Plants,
					Icon = "Mushrooms4",
					Category = "key2022"
				}
			},
			{
				StorePurchase.MUSH_TOADSTOOL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MUSH_TOADSTOOL",
					Tab = Store.TabType.Plants,
					Icon = "Mushrooms5",
					Category = "key2022"
				}
			},
			{
				StorePurchase.PL_CACTUS,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 45, 180, 450, 0),
					Name = "PL_CACTUS",
					Tab = Store.TabType.Plants,
					Icon = "cactus",
					Category = "key2006"
				}
			},
			{
				StorePurchase.PL_WATERLILY,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 45, 180, 450, 0),
					Name = "PL_WATERLILY",
					Tab = Store.TabType.Plants,
					Icon = "Waterlily",
					Category = "key2006"
				}
			},
			{
				StorePurchase.PL_WATERLILY2,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 45, 180, 450, 0),
					Name = "PL_WATERLILY2",
					Tab = Store.TabType.Plants,
					Icon = "waterlily2",
					Category = "key2006"
				}
			},
			{
				StorePurchase.WSKIN_SG553_FIOL,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_SG553_FIOL",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SG552_Dairy",
					Category = "WCAT_SG553",
					SkinType = WeaponSkinType.sg552,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_SG552
				}
			},
			{
				StorePurchase.WSKIN_SG553_TREANGLE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_SG553_TREANGLE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SG552_Triangle",
					Category = "WCAT_SG553",
					SkinType = WeaponSkinType.sg552,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_SG552,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_SG553_CAMO,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_SG553_CAMO",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SG552_Camo",
					Category = "WCAT_SG553",
					SkinType = WeaponSkinType.sg552,
					SkinId = 3,
					RequiredPurchase = StorePurchase.WEAPON_SG552,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_SG553_DRAGON,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1999, true, 1),
					Name = "WSKIN_SG553_DRAGON",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SG552_Dragon",
					Category = "WCAT_SG553",
					SkinType = WeaponSkinType.sg552,
					SkinId = 4,
					RequiredPurchase = StorePurchase.WEAPON_SG552,
					Sales = 50
				}
			},
			{
				StorePurchase.WSKIN_SG553_SPACE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2499, true, 1),
					Name = "WSKIN_SG553_SPACE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SG552_Space",
					Category = "WCAT_SG553",
					SkinType = WeaponSkinType.sg552,
					SkinId = 5,
					RequiredPurchase = StorePurchase.WEAPON_SG552,
					Sales = 50
				}
			},
			{
				StorePurchase.WSKIN_SWD_VORTEX,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_SWD_VORTEX",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SVD_Vortex",
					Category = "WCAT_SWD",
					SkinType = WeaponSkinType.swd,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_SVD
				}
			},
			{
				StorePurchase.WSKIN_SWD_RADIATION,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_SWD_RADIATION",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SVD_Radiation",
					Category = "WCAT_SWD",
					SkinType = WeaponSkinType.swd,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_SVD,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_SWD_LEGACY,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_SWD_LEGACY",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SVD_Legacy",
					Category = "WCAT_SWD",
					SkinType = WeaponSkinType.swd,
					SkinId = 3,
					RequiredPurchase = StorePurchase.WEAPON_SVD,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_SWD_GEAR,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_SWD_GEAR",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SVD_Gear",
					Category = "WCAT_SWD",
					SkinType = WeaponSkinType.swd,
					SkinId = 4,
					RequiredPurchase = StorePurchase.WEAPON_SVD,
					MinLevel = 4
				}
			},
			{
				StorePurchase.WSKIN_SWD_CARBON,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 5000, true, 1),
					Name = "WSKIN_SWD_CARBON",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "SVD_Carbon",
					Category = "WCAT_SWD",
					SkinType = WeaponSkinType.swd,
					SkinId = 5,
					RequiredPurchase = StorePurchase.WEAPON_SVD,
					MinLevel = 5,
					New = 1
				}
			},
			{
				StorePurchase.WSKIN_GLOCK_GEAR,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_GLOCK_GEAR",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Sunset",
					Category = "WCAT_GLOCK",
					SkinType = WeaponSkinType.glock,
					SkinId = 1
				}
			},
			{
				StorePurchase.WSKIN_GLOCK_PIRATE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_GLOCK_PIRATE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Pirate2",
					Category = "WCAT_GLOCK",
					SkinType = WeaponSkinType.glock,
					SkinId = 2,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_GLOCK_BLOOD,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_GLOCK_BLOOD",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Blood",
					Category = "WCAT_GLOCK",
					SkinType = WeaponSkinType.glock,
					SkinId = 3,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_GLOCK_CRIME,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_GLOCK_CRIME",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Crime",
					Category = "WCAT_GLOCK",
					SkinType = WeaponSkinType.glock,
					SkinId = 4,
					MinLevel = 4
				}
			},
			{
				StorePurchase.WSKIN_MP5_PALETTE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_MP5_PALETTE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Palette",
					Category = "WCAT_MP5",
					SkinType = WeaponSkinType.mp5,
					SkinId = 2
				}
			},
			{
				StorePurchase.WSKIN_MP5_SPRINT,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_MP5_SPRINT",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Sprint",
					Category = "WCAT_MP5",
					SkinType = WeaponSkinType.mp5,
					SkinId = 1,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_MP5_CYBER,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_MP5_CYBER",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Cyber",
					Category = "WCAT_MP5",
					SkinType = WeaponSkinType.mp5,
					SkinId = 3,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_MP5_LUXURY,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_MP5_LUXURY",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Luxury",
					Category = "WCAT_MP5",
					SkinType = WeaponSkinType.mp5,
					SkinId = 4,
					MinLevel = 4
				}
			},
			{
				StorePurchase.WSKIN_AK47_BLUE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_AK47_BLUE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Lazurit",
					Category = "WCAT_AK47",
					SkinType = WeaponSkinType.ak47,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_AK47
				}
			},
			{
				StorePurchase.WSKIN_AK47_WASTELAND,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_AK47_WASTELAND",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Wasteland",
					Category = "WCAT_AK47",
					SkinType = WeaponSkinType.ak47,
					SkinId = 4,
					RequiredPurchase = StorePurchase.WEAPON_AK47,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_AK47_MAGMA,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_AK47_MAGMA",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Magma",
					Category = "WCAT_AK47",
					SkinType = WeaponSkinType.ak47,
					SkinId = 3,
					RequiredPurchase = StorePurchase.WEAPON_AK47,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_AK47_VULK,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_AK47_VULK",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Breeze",
					Category = "WCAT_AK47",
					SkinType = WeaponSkinType.ak47,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_AK47,
					MinLevel = 4
				}
			},
			{
				StorePurchase.WSKIN_M16_KHAKI,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_M16_KHAKI",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Jungle",
					Category = "WCAT_M16",
					SkinType = WeaponSkinType.m16,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_M16
				}
			},
			{
				StorePurchase.WSKIN_M16_ICEBERG,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_M16_ICEBERG",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Iceberg",
					Category = "WCAT_M16",
					SkinType = WeaponSkinType.m16,
					SkinId = 3,
					RequiredPurchase = StorePurchase.WEAPON_M16,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_M16_AZIMOV,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_M16_AZIMOV",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Anisimov",
					Category = "WCAT_M16",
					SkinType = WeaponSkinType.m16,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_M16,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_M16_RIDGIE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_M16_RIDGIE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Ridgie",
					Category = "WCAT_M16",
					SkinType = WeaponSkinType.m16,
					SkinId = 4,
					RequiredPurchase = StorePurchase.WEAPON_M16,
					MinLevel = 4
				}
			},
			{
				StorePurchase.WSKIN_SG550_PIXEL,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_SG550_PIXEL",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Pixel",
					Category = "WCAT_SG550",
					SkinType = WeaponSkinType.sg550,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_SG550
				}
			},
			{
				StorePurchase.WSKIN_SG550_FUTURE,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_SG550_FUTURE",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Future",
					Category = "WCAT_SG550",
					SkinType = WeaponSkinType.sg550,
					SkinId = 3,
					RequiredPurchase = StorePurchase.WEAPON_SG550,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_SG550_GLORY,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "WSKIN_SG550_GLORY",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Glory",
					Category = "WCAT_SG550",
					SkinType = WeaponSkinType.sg550,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_SG550,
					MinLevel = 3
				}
			},
			{
				StorePurchase.WSKIN_SG550_NEON,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_SG550_NEON",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Neon",
					Category = "WCAT_SG550",
					SkinType = WeaponSkinType.sg550,
					SkinId = 4,
					RequiredPurchase = StorePurchase.WEAPON_SG550,
					MinLevel = 4
				}
			},
			{
				StorePurchase.WSKIN_M1014_DAYME,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_M1014_DAYME",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "big_m1014red",
					Category = "WCAT_M1014",
					SkinType = WeaponSkinType.m1014,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_M1014
				}
			},
			{
				StorePurchase.WSKIN_M1014_RED,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_M1014_RED",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "big_m1014dayme",
					Category = "WCAT_M1014",
					SkinType = WeaponSkinType.m1014,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_M1014,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_AUG_WAVES,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_AUG_WAVES",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Waves",
					Category = "WCAT_AUG",
					SkinType = WeaponSkinType.aug,
					SkinId = 1,
					RequiredPurchase = StorePurchase.WEAPON_AUG
				}
			},
			{
				StorePurchase.WSKIN_AUG_TIMBER,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "WSKIN_AUG_TIMBER",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Timber",
					Category = "WCAT_AUG",
					SkinType = WeaponSkinType.aug,
					SkinId = 2,
					RequiredPurchase = StorePurchase.WEAPON_AUG,
					MinLevel = 2
				}
			},
			{
				StorePurchase.WSKIN_SAWN_OFF_ORIGAMI,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "WSKIN_SAWN_OFF_ORIGAMI",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "Origami",
					Category = "WCAT_SAWN_OFF",
					SkinType = WeaponSkinType.sawn_off,
					SkinId = 1
				}
			},
			{
				StorePurchase.WSKIN_GLAUNCHER_SAKURA,
				new Store.PurchaseInfoWeaponSkin
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "WSKIN_GLAUNCHER_SAKURA",
					Tab = Store.TabType.Weapon_Skin,
					Icon = "big_glsakura",
					Category = "WCAT_GLAUNCHER",
					SkinType = WeaponSkinType.glauncher,
					SkinId = 1,
					MinLevel = 4,
					RequiredPurchase = StorePurchase.WEAPON_GRENADE_LAUNCHER
				}
			},
			{
				StorePurchase.STOOL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "WOODEN_CHAIR",
					Tab = Store.TabType.Decorations,
					Icon = "chair_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.CHAIR_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "STONE_CHAIR",
					Tab = Store.TabType.Decorations,
					Icon = "chair_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BED,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "SIMPLE_BED",
					Tab = Store.TabType.Decorations,
					Icon = "bed_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BED_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "STONE_BED",
					Tab = Store.TabType.Decorations,
					Icon = "bed_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.FENCE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FENCE",
					Tab = Store.TabType.Decorations,
					Icon = "fence_(tile)_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.FENCE_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "STONE_FENCE",
					Tab = Store.TabType.Decorations,
					Icon = "fence_(tile)_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BACKET,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "BUCKET",
					Tab = Store.TabType.Decorations,
					Icon = "bucket",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BASKET,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "BASKET",
					Tab = Store.TabType.Decorations,
					Icon = "basket",
					Category = "key2007"
				}
			},
			{
				StorePurchase.DOORW,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "DOOR",
					Tab = Store.TabType.Decorations,
					Icon = "door_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TABLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "WOODEN_TABLE",
					Tab = Store.TabType.Decorations,
					Icon = "table_02_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TABLE_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "STONE_TABLE",
					Tab = Store.TabType.Decorations,
					Icon = "table_02_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BIG_TABLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "BIG_TABLE",
					Tab = Store.TabType.Decorations,
					Icon = "table_01_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.CHEST,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CHEST",
					Tab = Store.TabType.Decorations,
					Icon = "chest_02",
					Category = "key2007"
				}
			},
			{
				StorePurchase.CHEST_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 90, false, 1),
					Name = "STONE_CHEST",
					Tab = Store.TabType.Decorations,
					Icon = "chest_01",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BARREL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "BARREL",
					Tab = Store.TabType.Decorations,
					Icon = "barrel",
					Category = "key2007"
				}
			},
			{
				StorePurchase.SIGN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "SIGNPOST",
					Tab = Store.TabType.Decorations,
					Icon = "sign",
					Category = "key2007"
				}
			},
			{
				StorePurchase.CUPBOARDS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "NIGHTSTAND",
					Tab = Store.TabType.Decorations,
					Icon = "case_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.CUPBOARDL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CUPBOARD",
					Tab = Store.TabType.Decorations,
					Icon = "big_case_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.STAIRSW,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 15, false, 1),
					Name = "STAIRS",
					Tab = Store.TabType.Decorations,
					Icon = "stairs_w",
					Category = "key2007"
				}
			},
			{
				StorePurchase.STAIRSB,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 30, false, 1),
					Name = "STAIRS",
					Tab = Store.TabType.Decorations,
					Icon = "stairs_s",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TORCHW,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "WALL_TORCH",
					Tab = Store.TabType.Decorations,
					Icon = "torch_01_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TORCH_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "STONE_WALL_TORCH",
					Tab = Store.TabType.Decorations,
					Icon = "torch_01_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TORCHF,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLOOR_TORCH",
					Tab = Store.TabType.Decorations,
					Icon = "torch_02_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TORCH_FLOOR_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "STONE_FLOOR_TORCH",
					Tab = Store.TabType.Decorations,
					Icon = "torch_02_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TABLICHKA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLOOR_PLATE",
					Tab = Store.TabType.Decorations,
					Icon = "tablet_01",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TABLICHKAW,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "WALL_PLATE",
					Tab = Store.TabType.Decorations,
					Icon = "tablet_02",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BOOK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 200, false, 1),
					Name = "BOOK",
					Tab = Store.TabType.Decorations,
					Icon = "book_console_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BENCH_WOOD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "WOODEN_BENCH",
					Tab = Store.TabType.Decorations,
					Icon = "bench_wood",
					Category = "key2007"
				}
			},
			{
				StorePurchase.BENCH_STONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 90, false, 1),
					Name = "STONE_BENCH",
					Tab = Store.TabType.Decorations,
					Icon = "bench_stone",
					Category = "key2007"
				}
			},
			{
				StorePurchase.LADDER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "LADDER",
					Tab = Store.TabType.Decorations,
					Icon = "ladder",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TOMBSTONE_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "TOMB",
					Tab = Store.TabType.Decorations,
					Icon = "tombstone_01 (1)",
					Category = "key2007"
				}
			},
			{
				StorePurchase.TOMBSTONE_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "TOMB",
					Tab = Store.TabType.Decorations,
					Icon = "tombstone_02",
					Category = "key2007"
				}
			},
			{
				StorePurchase.DARK_CASTLE_BED,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 60, false, 1),
					Name = "DARK_CASTLE_BED",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_bed",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_BENCH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 60, false, 1),
					Name = "DARK_CASTLE_BENCH",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_bench",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_BOOK_CONSOLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 230, false, 1),
					Name = "DARK_CASTLE_BOOK_CONSOLE",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_book_console",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_BOX_BIG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 90, false, 1),
					Name = "DARK_CASTLE_BOX_BIG",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_box_big",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_BOX_BIG_BOOKS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 90, false, 1),
					Name = "DARK_CASTLE_BOX_BIG_BOOKS",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_box_big_books",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_BOX_SMALL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 60, false, 1),
					Name = "DARK_CASTLE_BOX_SMALL",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_box_small",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_CAGE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 150, false, 1),
					Name = "DARK_CASTLE_CAGE",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_cage",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_CHAIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "DARK_CASTLE_CHAIN",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_chain_02",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_CHAIR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "DARK_CASTLE_CHAIR",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_chair",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_CHEST,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "DARK_CASTLE_CHEST",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_chest",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_COFFIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 150, false, 1),
					Name = "DARK_CASTLE_COFFIN",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_coffin_01",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_DARK_PICTURE_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "DARK_CASTLE_DARK_PICTURE_01",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_dark_picture_01",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_DARK_PICTURE_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 125, false, 1),
					Name = "DARK_CASTLE_DARK_PICTURE_02",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_dark_picture_02",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_DARK_PICTURE_03,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 150, false, 1),
					Name = "DARK_CASTLE_DARK_PICTURE_03",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_dark_picture_03",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_DOOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 70, false, 1),
					Name = "DARK_CASTLE_DOOR",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_door",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_FENCE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 70, false, 1),
					Name = "DARK_CASTLE_FENCE",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_fence_(X)",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_GARGOYLEY,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 200, false, 1),
					Name = "DARK_CASTLE_GARGOYLEY",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_gargoyley",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_HERALDRY,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "DARK_CASTLE_HERALDRY",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_heraldry",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_SARCOPHAGUS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 250, false, 1),
					Name = "DARK_CASTLE_SARCOPHAGUS",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_sarcophagus",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_SKULL_MUG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 50, false, 1),
					Name = "DARK_CASTLE_SKULL_MUG",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_skull-mug",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_STANDARD_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 50, false, 1),
					Name = "DARK_CASTLE_STANDARD_01",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_standard_01",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_STANDARD_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 50, false, 1),
					Name = "DARK_CASTLE_STANDARD_02",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_standard_02",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_TABLE_BIG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 70, false, 1),
					Name = "DARK_CASTLE_TABLE_BIG",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_table_big",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_TABLE_SMALL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 30, false, 1),
					Name = "DARK_CASTLE_TABLE_SMALL",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_table_small",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_THRONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 150, false, 1),
					Name = "DARK_CASTLE_THRONE",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_throne",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_TORCH_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "DARK_CASTLE_TORCH_01",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_torch_01",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_TORCH_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "DARK_CASTLE_TORCH_02",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_torch_02",
					Category = "key2008"
				}
			},
			{
				StorePurchase.DARK_CASTLE_TROLL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 200, false, 1),
					Name = "DARK_CASTLE_TROLL",
					Tab = Store.TabType.Decorations,
					Icon = "dark_castle_troll",
					Category = "key2008"
				}
			},
			{
				StorePurchase.BURGER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "BURGER",
					Tab = Store.TabType.Decorations,
					Icon = "burger",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_CAKE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CAKE",
					Tab = Store.TabType.Decorations,
					Icon = "cake",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_WOOD_PLATE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "WOODEN_PLATE",
					Tab = Store.TabType.Decorations,
					Icon = "wood_plate",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_SPOON,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "SPOON",
					Tab = Store.TabType.Decorations,
					Icon = "spoon",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_FORK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FORCK",
					Tab = Store.TabType.Decorations,
					Icon = "fork",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_MUG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MUG",
					Tab = Store.TabType.Decorations,
					Icon = "mug",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_JUG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "JUG",
					Tab = Store.TabType.Decorations,
					Icon = "jug",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_METAL_PLATE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "METAL_PLATE",
					Tab = Store.TabType.Decorations,
					Icon = "metal_plate",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_BREAD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "BREAD",
					Tab = Store.TabType.Decorations,
					Icon = "bread",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_CHEES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CHEESE",
					Tab = Store.TabType.Decorations,
					Icon = "cheese",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_CHIKEN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CHIKEN",
					Tab = Store.TabType.Decorations,
					Icon = "chicken",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_SVININA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PIG",
					Tab = Store.TabType.Decorations,
					Icon = "pork",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_FISH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FISH",
					Tab = Store.TabType.Decorations,
					Icon = "fish",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_STEIK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "STEAK",
					Tab = Store.TabType.Decorations,
					Icon = "meat",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_ORANGE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "ORANGE",
					Tab = Store.TabType.Decorations,
					Icon = "orange",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_PIZZA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PIZZA",
					Tab = Store.TabType.Decorations,
					Icon = "pizza",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_COLA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "COLA",
					Tab = Store.TabType.Decorations,
					Icon = "cola",
					Category = "key2009"
				}
			},
			{
				StorePurchase.FOOD_PIROG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PIE",
					Tab = Store.TabType.Decorations,
					Icon = "cheesecake",
					Category = "key2009"
				}
			},
			{
				StorePurchase.PAINTING_07,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_07",
					Tab = Store.TabType.Decorations,
					Icon = "painting_07",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_08,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_08",
					Tab = Store.TabType.Decorations,
					Icon = "painting_08",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_09,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_09",
					Tab = Store.TabType.Decorations,
					Icon = "painting_09",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_10,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_10",
					Tab = Store.TabType.Decorations,
					Icon = "painting_10",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_03,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_03",
					Tab = Store.TabType.Decorations,
					Icon = "painting_03",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_04,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_04",
					Tab = Store.TabType.Decorations,
					Icon = "painting_04",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_05,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "PAINTING_05",
					Tab = Store.TabType.Decorations,
					Icon = "painting_05",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_06,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "PAINTING_06",
					Tab = Store.TabType.Decorations,
					Icon = "painting_06",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_12,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "PAINTING_12",
					Tab = Store.TabType.Decorations,
					Icon = "painting_12",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_13,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 1),
					Name = "PAINTING_13",
					Tab = Store.TabType.Decorations,
					Icon = "painting_13",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 150, false, 1),
					Name = "PAINTING_01",
					Tab = Store.TabType.Decorations,
					Icon = "painting_01",
					Category = "key2010"
				}
			},
			{
				StorePurchase.PAINTING_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 150, false, 1),
					Name = "PAINTING_02",
					Tab = Store.TabType.Decorations,
					Icon = "painting_02",
					Category = "key2010"
				}
			},
			{
				StorePurchase.CARPET_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 199, false, 1),
					Name = "CARPET_01",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_01",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 199, false, 1),
					Name = "CARPET_02",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_02",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_03,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 199, false, 1),
					Name = "CARPET_03",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_03",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_04,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_04",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_04",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_05,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_05",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_05",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_06,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_06",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_06",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_07,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_07",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_07",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_08,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_08",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_08",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_09,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_09",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_09",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_10,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_10",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_10",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CARPET_11TH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CARPET_11TH",
					Tab = Store.TabType.Decorations,
					Icon = "CARPET_11_(tile_horizontal)",
					Category = "key2011"
				}
			},
			{
				StorePurchase.CE_ATTENTION_BOARD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_ATTENTION_BOARD",
					Tab = Store.TabType.Decorations_New,
					Icon = "attention-board_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_BIG_GARBAGE_BOX_CLOSE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_BIG_GARBAGE_BOX_CLOSE",
					Tab = Store.TabType.Decorations_New,
					Icon = "big_garbage_box_close_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_BIG_GARBAGE_BOX_OPEN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_BIG_GARBAGE_BOX_OPEN",
					Tab = Store.TabType.Decorations_New,
					Icon = "big_garbage_box_open_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_BIG_STREET_LANTERN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_BIG_STREET_LANTERN",
					Tab = Store.TabType.Decorations_New,
					Icon = "big_street_lantern_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_CONE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_CONE",
					Tab = Store.TabType.Decorations_New,
					Icon = "cone_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_FG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_FG",
					Tab = Store.TabType.Decorations_New,
					Icon = "FG_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_GARBAGE_BOX,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_GARBAGE_BOX",
					Tab = Store.TabType.Decorations_New,
					Icon = "garbage_box_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_MAIL_BOX,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_MAIL_BOX",
					Tab = Store.TabType.Decorations_New,
					Icon = "mail-box_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_METAL_DOOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CE_METAL_DOOR",
					Tab = Store.TabType.Decorations_New,
					Icon = "metal_door_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_PHONEBOX,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_PHONEBOX",
					Tab = Store.TabType.Decorations_New,
					Icon = "phonebox_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_SMALL_SREET_LANTERN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_SMALL_SREET_LANTERN",
					Tab = Store.TabType.Decorations_New,
					Icon = "small_street_lantern_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_STREET_BENCH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CE_STREET_BENCH",
					Tab = Store.TabType.Decorations_New,
					Icon = "street_bench_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_STREET_CLOCK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_STREET_CLOCK",
					Tab = Store.TabType.Decorations_New,
					Icon = "street_clock_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_STREET_LADDER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CE_STREET_LADDER",
					Tab = Store.TabType.Decorations_New,
					Icon = "street_ladder_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CE_WOOD_DOOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CE_WOOD_DOOR",
					Tab = Store.TabType.Decorations_New,
					Icon = "wood_door_icon",
					Category = "key2013"
				}
			},
			{
				StorePurchase.CI_BATH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_BATH",
					Tab = Store.TabType.Decorations_New,
					Icon = "bath",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_BED,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CI_BED",
					Tab = Store.TabType.Decorations_New,
					Icon = "bed",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_BEDROOM_CASE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_BEDROOM_CASE",
					Tab = Store.TabType.Decorations_New,
					Icon = "bedroom_case",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_BEDROOM_LAMP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_BEDROOM_LAMP",
					Tab = Store.TabType.Decorations_New,
					Icon = "bedroom_lamp",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_BIG_LAMP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_BIG_LAMP",
					Tab = Store.TabType.Decorations_New,
					Icon = "big_lamp",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_BOWL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_BOWL",
					Tab = Store.TabType.Decorations_New,
					Icon = "bowl",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_COFFEE_CUP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_COFFEE_CUP",
					Tab = Store.TabType.Decorations_New,
					Icon = "coffee-cup",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_DOOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CI_DOOR",
					Tab = Store.TabType.Decorations_New,
					Icon = "door",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_KITCHEN_BOARD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_KITCHEN_BOARD",
					Tab = Store.TabType.Decorations_New,
					Icon = "kitchen_board",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_KITCHEN_CHAIR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CI_KITCHEN_CHAIR",
					Tab = Store.TabType.Decorations_New,
					Icon = "kitchen_chair",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_KITCHEN_PART,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_KITCHEN_PART",
					Tab = Store.TabType.Decorations_New,
					Icon = "kitchen_part",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_KITCHEN_PART_CORNER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_KITCHEN_PART_CORNER",
					Tab = Store.TabType.Decorations_New,
					Icon = "kitchen_part_corner",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_KITCHEN_TABLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_KITCHEN_TABLE",
					Tab = Store.TabType.Decorations_New,
					Icon = "kitchen_table",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_KITCHEN_WASHING,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_KITCHEN_WASHING",
					Tab = Store.TabType.Decorations_New,
					Icon = "kitchen_washing",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_WLAPTOP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_WLAPTOP",
					Tab = Store.TabType.Decorations_New,
					Icon = "laptop",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_LOUDSPEAKERS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_LOUDSPEAKERS",
					Tab = Store.TabType.Decorations_New,
					Icon = "loudspeakers",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_MICROWAVE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_MICROWAVE",
					Tab = Store.TabType.Decorations_New,
					Icon = "microwave",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_OFFICE_CHAIR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_OFFICE_CHAIR",
					Tab = Store.TabType.Decorations_New,
					Icon = "office-chair",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_OFFICE_LAMP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_OFFICE_LAMP",
					Tab = Store.TabType.Decorations_New,
					Icon = "office-lamp",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_OFFICE_TABLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_OFFICE_TABLE",
					Tab = Store.TabType.Decorations_New,
					Icon = "office-table",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_OFFICE_CASE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_OFFICE_CASE",
					Tab = Store.TabType.Decorations_New,
					Icon = "office_case",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_OVEN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_OVEN",
					Tab = Store.TabType.Decorations_New,
					Icon = "oven",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_PLATE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_PLATE",
					Tab = Store.TabType.Decorations_New,
					Icon = "plate",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_REFRIGERATOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_REFRIGERATOR",
					Tab = Store.TabType.Decorations_New,
					Icon = "refrigerator",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SAUCER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SAUCER",
					Tab = Store.TabType.Decorations_New,
					Icon = "saucer",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SHOWER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SHOWER",
					Tab = Store.TabType.Decorations_New,
					Icon = "shower",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SMALL_LAMP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SMALL_LAMP",
					Tab = Store.TabType.Decorations_New,
					Icon = "small_lamp",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SMALL_TABLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SMALL_TABLE",
					Tab = Store.TabType.Decorations_New,
					Icon = "small_table",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SOAP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SOAP",
					Tab = Store.TabType.Decorations_New,
					Icon = "soap",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SOFA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SOFA",
					Tab = Store.TabType.Decorations_New,
					Icon = "sofa",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_SOFT_CHAIR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_SOFT_CHAIR",
					Tab = Store.TabType.Decorations_New,
					Icon = "soft-chair",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_TV,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_TV",
					Tab = Store.TabType.Decorations_New,
					Icon = "TV",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_WARDROBE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_WARDROBE",
					Tab = Store.TabType.Decorations_New,
					Icon = "wardrobe",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_WASHBASIN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_WASHBASIN",
					Tab = Store.TabType.Decorations_New,
					Icon = "washbasin",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_WWASHING_MACHINE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_WWASHING_MACHINE",
					Tab = Store.TabType.Decorations_New,
					Icon = "washing_machine",
					Category = "key2012"
				}
			},
			{
				StorePurchase.CI_WC,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "CI_WC",
					Tab = Store.TabType.Decorations_New,
					Icon = "WC",
					Category = "key2012"
				}
			},
			{
				StorePurchase.MILITARY_ARMORY,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_ARMORY",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_armory",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_BARREL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_BARREL",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_barrel",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_BED,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_BED",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_bed",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CAM,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CAM",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_cam",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CASE_BIG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CASE_BIG",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_case_big",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CASE_SMALL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CASE_SMALL",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_case_small",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CHAIR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CHAIR",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_chair",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CHEST,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CHEST",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_chest",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CONSOLE_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CONSOLE_01",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_console_01",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CONSOLE_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CONSOLE_02",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_console_02",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_CONSOLE_03,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_CONSOLE_03",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_console_03",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_DOOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_DOOR",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_door",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_LAMP,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_LAMP",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_lamp",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_LANTERN,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_LANTERN",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_lantern",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_MUG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_MUG",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_mug",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_PC,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_PC",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_PC",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_TABLE_BIG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_TABLE_BIG",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_table_big",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_TABLE_SMALL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_TABLE_SMALL",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_table_small",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_FENCE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_FENCE",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_fence_(X)",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_BENCH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_BENCH",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_bench",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_BOX_01,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_BOX_01",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_box_01",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_BOX_02,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_BOX_02",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_box_02",
					Category = "key2014"
				}
			},
			{
				StorePurchase.MILITARY_BRIEFING_BOARD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "MILITARY_BRIEFING_BOARD",
					Tab = Store.TabType.Decorations_New,
					Icon = "m_briefing_board",
					Category = "key2014"
				}
			},
			{
				StorePurchase.FLAG1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG1",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag1",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG2",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag2",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG3",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag3",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG4",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag4",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG5",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag5",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG6",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag6",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG7,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG7",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag7",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG8,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG8",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag8",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG9,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG9",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag9",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG10,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG10",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag10",
					Category = "key2021"
				}
			},
			{
				StorePurchase.FLAG12,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "FLAG12",
					Tab = Store.TabType.Decorations_New,
					Icon = "Flag12",
					Category = "key2021",
					New = 1
				}
			},
			{
				StorePurchase.PIRATES1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "PIRATES1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Pirates1",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.PIRATES2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "PIRATES2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Pirates2",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.PIRATES3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "PIRATES3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Pirates3",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.PIRATES4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "PIRATES4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Pirates4",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.PIRATES5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "PIRATES5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Pirates5",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.PIRATES6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "PIRATES6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Pirates6",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.FLAG11,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "FLAG11",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Flag11",
					Category = "Pirates",
					New = 1
				}
			},
			{
				StorePurchase.NPC7,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 499, false, 1),
					Name = "NPC7",
					Tab = Store.TabType.Decorations_Star,
					Icon = "NPC7",
					Category = "Pirates",
					Sales = 50
				}
			},
			{
				StorePurchase.NPC8,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 499, false, 1),
					Name = "NPC8",
					Tab = Store.TabType.Decorations_Star,
					Icon = "NPC8",
					Category = "Pirates",
					Sales = 50
				}
			},
			{
				StorePurchase.EAST1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "EAST1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "East1",
					Category = "East"
				}
			},
			{
				StorePurchase.EAST2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "EAST2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "East2",
					Category = "East"
				}
			},
			{
				StorePurchase.EAST3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "EAST3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "East3",
					Category = "East"
				}
			},
			{
				StorePurchase.EAST4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "EAST4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "East4",
					Category = "East"
				}
			},
			{
				StorePurchase.EAST5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "EAST5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "East5",
					Category = "East"
				}
			},
			{
				StorePurchase.EAST6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "EAST6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "East6_White",
					Category = "East"
				}
			},
			{
				StorePurchase.NY_16_TREE,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 199, false, 1),
					Name = "NY_16_TREE",
					Tab = Store.TabType.Decorations_Star,
					Icon = "TreeHome",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY_16_BIG_TREE,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 199, false, 1),
					Name = "NY_16_BIG_TREE",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Tree",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.FIREWORK1,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 199, false, 1),
					Name = "NY3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "FireworkRed",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY_16_SNOWMAN,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY_16_SNOWMAN",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Showman",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY_16_GIFT,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY_16_GIFT",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Gift",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY_16_GIFT_BAG,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY_16_GIFT_BAG",
					Tab = Store.TabType.Decorations_Star,
					Icon = "GiftBag",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY_16_SOCK,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY_16_SOCK",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Sock",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY_16_WREATH,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY_16_WREATH",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Wreath",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY9,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY9",
					Tab = Store.TabType.Decorations_Star,
					Icon = "NY9",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY10,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY10",
					Tab = Store.TabType.Decorations_Star,
					Icon = "NY10",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY11,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY11",
					Tab = Store.TabType.Decorations_Star,
					Icon = "NY11",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.NY12,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "NY12",
					Tab = Store.TabType.Decorations_Star,
					Icon = "NY12_Digger",
					Category = "NY",
					New = 1
				}
			},
			{
				StorePurchase.FIREWORK2,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "FIREWORK2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "FireworkYellow",
					Category = "NY"
				}
			},
			{
				StorePurchase.FIREWORK3,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "FIREWORK3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "FireworkBlue",
					Category = "NY"
				}
			},
			{
				StorePurchase.FIREWORK4,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "FIREWORK4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "FireworkGreen",
					Category = "NY"
				}
			},
			{
				StorePurchase.WESTERN1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western1",
					Category = "Western"
				}
			},
			{
				StorePurchase.WESTERN2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western2",
					Category = "Western"
				}
			},
			{
				StorePurchase.WESTERN3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western3",
					Category = "Western"
				}
			},
			{
				StorePurchase.WESTERN4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western4",
					Category = "Western"
				}
			},
			{
				StorePurchase.WESTERN5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western5",
					Category = "Western"
				}
			},
			{
				StorePurchase.WESTERN6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western6",
					Category = "Western"
				}
			},
			{
				StorePurchase.WESTERN7,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "WESTERN7",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Western7",
					Category = "Western"
				}
			},
			{
				StorePurchase.HALLOWEEN1,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "HALLOWEEN1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Halloween1",
					Category = "Halloween",
					New = 1
				}
			},
			{
				StorePurchase.HALLOWEEN2,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "HALLOWEEN2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Halloween2",
					Category = "Halloween",
					New = 1
				}
			},
			{
				StorePurchase.HALLOWEEN3,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "HALLOWEEN3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Halloween3",
					Category = "Halloween",
					New = 1
				}
			},
			{
				StorePurchase.HALLOWEEN4,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "HALLOWEEN4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Halloween4",
					Category = "Halloween",
					New = 1
				}
			},
			{
				StorePurchase.HALLOWEEN5,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "HALLOWEEN5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Halloween5",
					Category = "Halloween",
					New = 1
				}
			},
			{
				StorePurchase.HALLOWEEN6,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "HALLOWEEN6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Halloween6",
					Category = "Halloween",
					New = 1
				}
			},
			{
				StorePurchase.CAMPING1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CAMPING1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Camping1_Blue",
					Category = "Camping",
					New = 1
				}
			},
			{
				StorePurchase.CAMPING2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CAMPING2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Camping2",
					Category = "Camping"
				}
			},
			{
				StorePurchase.CAMPING3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CAMPING3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Camping3",
					Category = "Camping"
				}
			},
			{
				StorePurchase.CAMPING4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CAMPING4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Camping4",
					Category = "Camping"
				}
			},
			{
				StorePurchase.CAMPING5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CAMPING5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Camping5",
					Category = "Camping"
				}
			},
			{
				StorePurchase.CAMPING6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "CAMPING6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Camping6",
					Category = "Camping"
				}
			},
			{
				StorePurchase.SCHOOL3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "SCHOOL3",
					Tab = Store.TabType.Decorations_Star,
					Icon = "School3",
					Category = "School"
				}
			},
			{
				StorePurchase.SCHOOL4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "SCHOOL4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "School4",
					Category = "School"
				}
			},
			{
				StorePurchase.SCHOOL1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "SCHOOL1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "School1",
					Category = "School"
				}
			},
			{
				StorePurchase.SCHOOL2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "SCHOOL2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "School2",
					Category = "School"
				}
			},
			{
				StorePurchase.SCHOOL5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "SCHOOL5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "School5_Yellow",
					Category = "School"
				}
			},
			{
				StorePurchase.SCHOOL6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "SCHOOL6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "School6",
					Category = "School"
				}
			},
			{
				StorePurchase.BEACH1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "BEACH1",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Beach1",
					Category = "Beach"
				}
			},
			{
				StorePurchase.BEACH2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "BEACH2",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Beach2",
					Category = "Beach"
				}
			},
			{
				StorePurchase.BEACH4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "BEACH4",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Beach4",
					Category = "Beach"
				}
			},
			{
				StorePurchase.BEACH6,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "BEACH6",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Beach6",
					Category = "Beach"
				}
			},
			{
				StorePurchase.BEACH5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "BEACH5",
					Tab = Store.TabType.Decorations_Star,
					Icon = "Beach5",
					Category = "Beach"
				}
			},
			{
				StorePurchase.NPC1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 999, false, 1),
					Name = "NPC1",
					Tab = Store.TabType.Tools,
					Icon = "NPC1",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 999, false, 1),
					Name = "NPC2",
					Tab = Store.TabType.Tools,
					Icon = "NPC2",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 999, false, 1),
					Name = "NPC3",
					Tab = Store.TabType.Tools,
					Icon = "NPC3",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 999, false, 1),
					Name = "NPC4",
					Tab = Store.TabType.Tools,
					Icon = "NPC4",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 999, false, 1),
					Name = "NPC5",
					Tab = Store.TabType.Tools,
					Icon = "NPC5",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC6,
				new Store.PurchaseInfo
				{
					Disabled = true,
					Cost = new Store.OnePay(Currency.Gold, 2000, false, 1),
					Name = "NPC6",
					Tab = Store.TabType.Tools,
					Icon = "NPC6",
					Category = "key2001",
					New = 1
				}
			},
			{
				StorePurchase.NPC9,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, false, 1),
					Name = "NPC9",
					Tab = Store.TabType.Tools,
					Icon = "NPC9",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC10,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, false, 1),
					Name = "NPC10",
					Tab = Store.TabType.Tools,
					Icon = "NPC10",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC11,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, false, 1),
					Name = "NPC11",
					Tab = Store.TabType.Tools,
					Icon = "NPC11",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC12,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, false, 1),
					Name = "NPC12",
					Tab = Store.TabType.Tools,
					Icon = "NPC12",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC13,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, false, 1),
					Name = "NPC13",
					Tab = Store.TabType.Tools,
					Icon = "NPC13",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC14,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, false, 1),
					Name = "NPC14",
					Tab = Store.TabType.Tools,
					Icon = "NPC14",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC15,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, false, 1),
					Name = "NPC15",
					Tab = Store.TabType.Tools,
					Icon = "NPC15",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC16,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, false, 1),
					Name = "NPC16",
					Tab = Store.TabType.Tools,
					Icon = "NPC16",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC17,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, false, 1),
					Name = "NPC17",
					Tab = Store.TabType.Tools,
					Icon = "NPC17",
					Category = "key2001"
				}
			},
			{
				StorePurchase.NPC18,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1999, false, 1),
					Name = "NPC18",
					Tab = Store.TabType.Tools,
					Icon = "NPC18",
					Category = "key2001",
					New = 1
				}
			},
			{
				StorePurchase.NPC19,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1999, false, 1),
					Name = "NPC19",
					Tab = Store.TabType.Tools,
					Icon = "NPC19",
					Category = "key2001",
					New = 1
				}
			},
			{
				StorePurchase.SPEED,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "SPEED_POTION",
					Tab = Store.TabType.Tools,
					Icon = "PotionSpeed",
					Category = "key2002"
				}
			},
			{
				StorePurchase.FLY,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "FLIGHT_POTION",
					Tab = Store.TabType.Tools,
					Icon = "PotionFly",
					Category = "key2002"
				}
			},
			{
				StorePurchase.TELEPORT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 4000, true, 1),
					Name = "TELEPORT",
					Tab = Store.TabType.Tools,
					Icon = "PotionTeleport",
					Category = "key2002",
					MinLevel = 3
				}
			},
			{
				StorePurchase.KIRCKA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "IMPROVED_PICK",
					Tab = Store.TabType.Tools,
					Icon = "silver_picker",
					Category = "key2002"
				}
			},
			{
				StorePurchase.SUPER_KIRCKA,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3000, true, 1),
					Name = "SUPER_KIRCKA",
					Tab = Store.TabType.Tools,
					Icon = "golden_picker",
					Category = "key2002",
					RequiredPurchase = StorePurchase.KIRCKA
				}
			},
			{
				StorePurchase.BEACH3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 750, false, 1),
					Name = "BEACH3",
					Tab = Store.TabType.Tools,
					Icon = "Beach3",
					Category = "key2004"
				}
			},
			{
				StorePurchase.BOAT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 600, false, 1),
					Name = "BLOAT",
					Tab = Store.TabType.Tools,
					Icon = "Boat",
					Category = "key2004",
					Sales = 25
				}
			},
			{
				StorePurchase.TROLLEY,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 750, false, 1),
					Name = "TROLLEY",
					Tab = Store.TabType.Tools,
					Icon = "Trolley",
					Category = "key2004"
				}
			},
			{
				StorePurchase.RAIL,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 100, false, 12),
					Name = "RAILS",
					Tab = Store.TabType.Tools,
					Icon = "Rails",
					Category = "key2004"
				}
			},
			{
				StorePurchase.EMOTIONS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 750, true, 1),
					Name = "ADV_SMILES",
					Tab = Store.TabType.Tools,
					Icon = "ico7",
					Category = "key2005"
				}
			},
			{
				StorePurchase.MEM_SMILES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 750, true, 1),
					Name = "MEM_FACES",
					Tab = Store.TabType.Tools,
					Icon = "icon",
					Category = "key2005"
				}
			},
			{
				StorePurchase.DINAMIT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 99, false, 1),
					Name = "DYNAMITE",
					Tab = Store.TabType.Tools,
					Icon = "TNT",
					Category = "key2006"
				}
			},
			{
				StorePurchase.ATOMBOMB,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 240, false, 1),
					Name = "AIR_BOMB",
					Tab = Store.TabType.Tools,
					Icon = "AviaBomb",
					Category = "key2006"
				}
			},
			{
				StorePurchase.SPAWN_POINT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 500, false, 1),
					Name = "RESPAWN",
					Tab = Store.TabType.Tools,
					Icon = "checkpoint_flag_icon_02",
					Category = "key2006"
				}
			},
			{
				StorePurchase.TEAM_SPAWN_POINT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "TEAM_RESPAWN",
					Tab = Store.TabType.Tools,
					Icon = "comand_spawn",
					Category = "key2006"
				}
			},
			{
				StorePurchase.FLAG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 25, false, 250),
					Name = "FLAG",
					Tab = Store.TabType.Tools,
					Icon = "flags_01_icon_02",
					Category = "key2006"
				}
			},
			{
				StorePurchase.GOLD_KUBOK,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 300, false, 1),
					Name = "GOLD_CUP",
					Tab = Store.TabType.Tools,
					Icon = "icon_cup_shop_100x75_001",
					Category = "key2006"
				}
			},
			{
				StorePurchase.TEAMDOORS,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 400, false, 1),
					Name = "TEAM_DOORS",
					Tab = Store.TabType.Tools,
					Icon = "double-doors_team_02_icon_02",
					Category = "key2006"
				}
			},
			{
				StorePurchase.LOCK_DOOR,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 300, false, 1),
					Name = "LOCK_DOOR",
					Tab = Store.TabType.Tools,
					Icon = "icon_shop_doors",
					Category = "key2006"
				}
			},
			{
				StorePurchase.LOCK_KEY,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 50, false, 1),
					Name = "LOCK_KEY",
					Tab = Store.TabType.Tools,
					Icon = "icon_shop_keys",
					Category = "key2006"
				}
			},
			{
				StorePurchase.HG_APOS_SPAWN_POINT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 1),
					Name = "HG_APOS_SPAWN_POINT",
					Tab = Store.TabType.Tools,
					Icon = "basement_icon_shop",
					Category = "key2006"
				}
			},
			{
				StorePurchase.HG_ARENA_SPAWN_POINT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 250, false, 1),
					Name = "HG_ARENA_SPAWN_POINT",
					Tab = Store.TabType.Tools,
					Icon = "arena_flag_icon_shop",
					Category = "key2006"
				}
			},
			{
				StorePurchase.HG_ARCHER_BUILD_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 500, true, 1),
					Name = "HG_ARCHER1",
					Tab = Store.TabType.Hungry_Games,
					Icon = "archer_build_01",
					Category = "key2019"
				}
			},
			{
				StorePurchase.HG_ARCHER_BUILD_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "HG_ARCHER2",
					Tab = Store.TabType.Hungry_Games,
					Icon = "archer_build_02",
					Category = "key2019",
					MinLevel = 4,
					RequiredPurchase = StorePurchase.HG_ARCHER_BUILD_1
				}
			},
			{
				StorePurchase.HG_ARCHER_BUILD_3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "HG_ARCHER3",
					Tab = Store.TabType.Hungry_Games,
					Icon = "archer_build_03",
					Category = "key2019",
					MinLevel = 6,
					RequiredPurchase = StorePurchase.HG_ARCHER_BUILD_2
				}
			},
			{
				StorePurchase.HG_ARCHER_BUILD_4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "HG_ARCHER4",
					Tab = Store.TabType.Hungry_Games,
					Icon = "archer_build_04",
					Category = "key2019",
					MinLevel = 7,
					RequiredPurchase = StorePurchase.HG_ARCHER_BUILD_3
				}
			},
			{
				StorePurchase.HG_ARCHER_BUILD_5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2500, true, 1),
					Name = "HG_ARCHER5",
					Tab = Store.TabType.Hungry_Games,
					Icon = "archer_build_05",
					Category = "key2019",
					MinLevel = 8,
					RequiredPurchase = StorePurchase.HG_ARCHER_BUILD_4
				}
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_1,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 500, true, 1),
					Name = "HG_KNIGHT1",
					Tab = Store.TabType.Hungry_Games,
					Icon = "knight_build_01",
					Category = "key2020"
				}
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "HG_KNIGHT2",
					Tab = Store.TabType.Hungry_Games,
					Icon = "knight_build_02",
					Category = "key2020",
					MinLevel = 4,
					RequiredPurchase = StorePurchase.HG_KNIGHT_BUILD_1
				}
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_3,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "HG_KNIGHT3",
					Tab = Store.TabType.Hungry_Games,
					Icon = "knight_build_03",
					Category = "key2020",
					MinLevel = 6,
					RequiredPurchase = StorePurchase.HG_KNIGHT_BUILD_2
				}
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_4,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2000, true, 1),
					Name = "HG_KNIGHT4",
					Tab = Store.TabType.Hungry_Games,
					Icon = "knight_build_04",
					Category = "key2020",
					MinLevel = 7,
					RequiredPurchase = StorePurchase.HG_KNIGHT_BUILD_3
				}
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2500, true, 1),
					Name = "HG_KNIGHT5",
					Tab = Store.TabType.Hungry_Games,
					Icon = "knight_build_05",
					Category = "key2020",
					MinLevel = 8,
					RequiredPurchase = StorePurchase.HG_KNIGHT_BUILD_4
				}
			},
			{
				StorePurchase.MORE_EXPERIENCE,
				new Store.PurchaseInfo
				{
					Cost = new Store.TimedPay(Currency.Gold, 1000, 5000, 0, 12000),
					Name = "MORE_EXPERIENCE",
					Tab = Store.TabType.Weapons,
					Icon = "premium_icon",
					LargeIcon = "premium_art_01",
					Category = "WEAPON_OTHER"
				}
			},
			{
				StorePurchase.BATTLE_GRENADES,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 10),
					Name = "BATTLE_GREANDES",
					Tab = Store.TabType.Weapons,
					Icon = "icon_grenades",
					Category = "WEAPON_OTHER",
					Cooldown = 15f
				}
			},
			{
				StorePurchase.BATTLE_HEALTH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 10),
					Name = "BATTLE_HEALTH",
					Tab = Store.TabType.Weapons,
					Icon = "icon_health",
					Category = "WEAPON_OTHER",
					Cooldown = 30f
				}
			},
			{
				StorePurchase.BATTLE_SHIELD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, false, 5),
					Name = "BATTLE_SHIELD",
					Tab = Store.TabType.Weapons,
					Icon = "icon_shield",
					Category = "WEAPON_OTHER",
					Cooldown = 60f
				}
			},
			{
				StorePurchase.WEAPON_REVOLVER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 400, true, 1),
					Name = "REVOLVER",
					Tab = Store.TabType.Weapons,
					Icon = "big_revolver",
					Category = "WEAPON_PISTOLS",
					WeaponStats = new int[]
					{
						33,
						60,
						75,
						6
					},
					MinLevel = 3
				}
			},
			{
				StorePurchase.WEAPON_COLT,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 900, true, 1),
					Name = "COLT",
					Tab = Store.TabType.Weapons,
					Icon = "big_colt",
					Category = "WEAPON_PISTOLS",
					WeaponStats = new int[]
					{
						30,
						86,
						75,
						7
					},
					MinLevel = 6
				}
			},
			{
				StorePurchase.WEAPON_WINCHESTER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 900, true, 1),
					Name = "WINCHESTER",
					Tab = Store.TabType.Weapons,
					Icon = "big_winchester",
					Category = "WEAPON_SHOTGUNS",
					WeaponStats = new int[]
					{
						125,
						60,
						8,
						5
					}
				}
			},
			{
				StorePurchase.WEAPON_M1014,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1400, true, 1),
					Name = "SHOTGUN",
					Tab = Store.TabType.Weapons,
					Icon = "big_m1014",
					Category = "WEAPON_SHOTGUNS",
					WeaponStats = new int[]
					{
						150,
						60,
						8,
						7
					},
					MinLevel = 5
				}
			},
			{
				StorePurchase.WEAPON_RIFLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1400, true, 1),
					Name = "RIFLE",
					Tab = Store.TabType.Weapons,
					Icon = "big_rifle",
					Category = "WEAPON_SNIPER_RIFLES",
					WeaponStats = new int[]
					{
						50,
						65,
						95,
						1
					}
				}
			},
			{
				StorePurchase.WEAPON_SNIPER_RIFLE,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1900, true, 1),
					Name = "SNIPER_RIFLE",
					Tab = Store.TabType.Weapons,
					Icon = "big_sniper",
					Category = "WEAPON_SNIPER_RIFLES",
					WeaponStats = new int[]
					{
						60,
						65,
						95,
						1
					},
					MinLevel = 6
				}
			},
			{
				StorePurchase.WEAPON_SVD,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 2400, true, 1),
					Name = "SVD",
					Tab = Store.TabType.Weapons,
					Icon = "big_svd",
					Category = "WEAPON_SNIPER_RIFLES",
					WeaponStats = new int[]
					{
						70,
						60,
						95,
						10
					},
					MinLevel = 8
				}
			},
			{
				StorePurchase.WEAPON_PPSH,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 700, true, 1),
					Name = "PPSH",
					Tab = Store.TabType.Weapons,
					Icon = "big_ppsh",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						15,
						428,
						85,
						70
					}
				}
			},
			{
				StorePurchase.WEAPON_THOMPSON,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1400, true, 1),
					Name = "THOMPSON",
					Tab = Store.TabType.Weapons,
					Icon = "big_thompson",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						13,
						500,
						85,
						30
					}
				}
			},
			{
				StorePurchase.WEAPON_MP40,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 900, true, 1),
					Name = "MP40",
					Tab = Store.TabType.Weapons,
					Icon = "big_mp40",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						14,
						461,
						85,
						32
					}
				}
			},
			{
				StorePurchase.WEAPON_MP5,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 49, true, 1),
					Name = "MP5",
					Tab = Store.TabType.Weapons,
					Icon = "big_mp5",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						8,
						545,
						65,
						30
					},
					MinLevel = 6
				}
			},
			{
				StorePurchase.WEAPON_AK47,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 500, true, 1),
					Name = "AK47",
					Tab = Store.TabType.Weapons,
					Icon = "big_ak47",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						10,
						600,
						65,
						30
					},
					MinLevel = 3
				}
			},
			{
				StorePurchase.WEAPON_M16,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1300, true, 1),
					Name = "M4A",
					Tab = Store.TabType.Weapons,
					Icon = "big_m16",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						12,
						600,
						75,
						30
					},
					MinLevel = 6
				}
			},
			{
				StorePurchase.WEAPON_AUG,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1000, true, 1),
					Name = "AUG",
					Tab = Store.TabType.Weapons,
					Icon = "big_aug",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						10,
						600,
						75,
						30
					},
					MinLevel = 5
				}
			},
			{
				StorePurchase.WEAPON_SG550,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1500, true, 1),
					Name = "SG550",
					Tab = Store.TabType.Weapons,
					Icon = "big_sg550",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						32,
						200,
						70,
						8
					},
					MinLevel = 7,
					Sales = 25
				}
			},
			{
				StorePurchase.WEAPON_SG552,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 1499, true, 1),
					Name = "WEAPON_SG552",
					Tab = Store.TabType.Weapons,
					Icon = "big_sg552",
					Category = "WEAPON_RIFLES",
					WeaponStats = new int[]
					{
						34,
						214,
						70,
						8
					},
					MinLevel = 6,
					Sales = 50
				}
			},
			{
				StorePurchase.WEAPON_GRENADE_LAUNCHER2,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 3500, true, 1),
					Name = "GRENADE_LAUNCHER2",
					Tab = Store.TabType.Weapons,
					Icon = "big_grenade_launcher2",
					Category = "WEAPON_GRENADE_LAUNCHERS",
					WeaponStats = new int[]
					{
						90,
						60,
						75,
						1
					},
					MinLevel = 5
				}
			},
			{
				StorePurchase.WEAPON_GRENADE_LAUNCHER,
				new Store.PurchaseInfo
				{
					Cost = new Store.OnePay(Currency.Gold, 4500, true, 1),
					Name = "GRENADE_GUN",
					Tab = Store.TabType.Weapons,
					Icon = "big_grenade_launcher",
					Category = "WEAPON_GRENADE_LAUNCHERS",
					WeaponStats = new int[]
					{
						50,
						55,
						75,
						3
					},
					MinLevel = 10
				}
			}
		};
		if (!Application.isEditor)
		{
			return;
		}
		string path = "/Users/user/Desktop/vk.diggerworld.ru/Server/";
		if (Application.isEditor && Directory.Exists(path))
		{
			Store.GeneratePurchasesForServer(path);
			Store.GenerateDataForPhotonServer(path);
		}
	}

	private static void GeneratePurchasesForServer(string path)
	{
		StreamWriter streamWriter = new StreamWriter(path + "PurchaseList.php");
		streamWriter.WriteLine("<?php");
		streamWriter.WriteLine("$purchases = array(");
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair in Store.Purchases)
		{
			if (!keyValuePair.Value.Disabled)
			{
				Store.Pay cost = keyValuePair.Value.Cost;
				string text = "\t" + (int)keyValuePair.Key + " => array(";
				if (cost is Store.OnePay)
				{
					Store.OnePay onePay = (Store.OnePay)cost;
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"'one', '",
						onePay.Curr,
						"', ",
						onePay.Cost,
						", ",
						onePay.Once.ToString().ToLower(),
						", ",
						onePay.Count
					});
				}
				else
				{
					Store.TimedPay timedPay = (Store.TimedPay)cost;
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"'timed', '",
						timedPay.Curr,
						"', ",
						timedPay.DayCost,
						", ",
						timedPay.WeekCost,
						", ",
						timedPay.ForeverCost,
						", ",
						timedPay.MonthCost
					});
				}
				text = text + ", " + ProfileINI.ExpForLevel(keyValuePair.Value.MinLevel);
				text = text + "), // " + keyValuePair.Key;
				streamWriter.WriteLine(text);
			}
		}
		streamWriter.WriteLine(");");
		streamWriter.WriteLine("?>");
		streamWriter.Close();
	}

	private static void GenerateDataForPhotonServer(string path)
	{
		StreamWriter streamWriter = new StreamWriter(path + "Data.cs");
		streamWriter.WriteLine("using System;\nusing System.Collections.Generic;\n\nnamespace Qmax.Kopatel\n{");
		streamWriter.WriteLine("\tpublic class Data");
		streamWriter.WriteLine("\t{");
		streamWriter.WriteLine("\t\tstatic public string[] RpcList = new string[]");
		streamWriter.WriteLine("\t\t{");
		foreach (string str in PhotonNetwork.PhotonServerSettings.RpcList)
		{
			streamWriter.WriteLine("\t\t\t\"" + str + "\",");
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine("\t};");
		streamWriter.WriteLine();
		streamWriter.WriteLine("\tpublic class Purchases\n\t{");
		string[] names = Enum.GetNames(typeof(StorePurchase));
		Array values = Enum.GetValues(typeof(StorePurchase));
		for (int i = 0; i < names.Length; i++)
		{
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"\t\tstatic public int ",
				names[i],
				" = ",
				(int)values.GetValue(i),
				";"
			}));
		}
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, string> EntityNames = new Dictionary<int, string>()");
		streamWriter.WriteLine("\t\t{");
		string[] names2 = Enum.GetNames(typeof(EntityType));
		Array values2 = Enum.GetValues(typeof(EntityType));
		for (int j = 0; j < names2.Length; j++)
		{
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"\t\t\t{ ",
				(int)values2.GetValue(j),
				", \"",
				names2[j],
				"\" },"
			}));
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, string> BlockNames = new Dictionary<int, string>()");
		streamWriter.WriteLine("\t\t{");
		string[] names3 = Enum.GetNames(typeof(BlockType));
		Array values3 = Enum.GetValues(typeof(BlockType));
		for (int k = 0; k < names3.Length; k++)
		{
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"\t\t\t{ ",
				(int)((byte)values3.GetValue(k)),
				", \"",
				names3[k],
				"\" },"
			}));
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, string> PurchaseNames = new Dictionary<int, string>()");
		streamWriter.WriteLine("\t\t{");
		string[] names4 = Enum.GetNames(typeof(StorePurchase));
		Array values4 = Enum.GetValues(typeof(StorePurchase));
		for (int l = 0; l < names4.Length; l++)
		{
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"\t\t\t{ ",
				(int)values4.GetValue(l),
				", \"",
				names4[l],
				"\" },"
			}));
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, int> EntityToPurchase = new Dictionary<int, int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<EntityType, Store.EntityInfo> keyValuePair in Store.Entities)
		{
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"\t\t\t{ ",
				(int)keyValuePair.Key,
				", ",
				(int)keyValuePair.Value.Purchase.Value,
				" },"
			}));
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, int> BlockToPurchase = new Dictionary<int, int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<BlockType, Store.BlockInfo> keyValuePair2 in Store.Blocks)
		{
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"\t\t\t{ ",
				(int)keyValuePair2.Key,
				", ",
				(int)keyValuePair2.Value.Purchase.Value,
				" },"
			}));
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, int> BlockKindToPurchase = new Dictionary<int, int>()");
		streamWriter.WriteLine("\t\t{");
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			1,
			", ",
			158,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			2,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			3,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			4,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			5,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			6,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			7,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			8,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			9,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			10,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			11,
			", ",
			158,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			12,
			", ",
			158,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			13,
			", ",
			158,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			14,
			", ",
			158,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			16,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			17,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			18,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			19,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			20,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			21,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			22,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			25,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			26,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			27,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			28,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			29,
			", ",
			211,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			30,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			31,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			32,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			33,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			34,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			35,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			36,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			37,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			38,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			39,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			40,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			41,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			42,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			43,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			44,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			45,
			", ",
			159,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			62,
			", ",
			268,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			64,
			", ",
			268,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			65,
			", ",
			268,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			63,
			", ",
			268,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			46,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			47,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			48,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			49,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			50,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			51,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			52,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			53,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			54,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			55,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			56,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			57,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			58,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			59,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			60,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			61,
			", ",
			160,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			67,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			66,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			68,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			69,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			70,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			71,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			72,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			73,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			74,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			75,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			76,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			77,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			78,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			79,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			80,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			81,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			82,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			83,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			84,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			85,
			", ",
			157,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			86,
			", ",
			270,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			89,
			", ",
			270,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			88,
			", ",
			270,
			" },"
		}));
		streamWriter.WriteLine(string.Concat(new object[]
		{
			"\t\t\t{ ",
			87,
			", ",
			270,
			" },"
		}));
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public HashSet<int> OnePayPurchases = new HashSet<int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair3 in Store.Purchases)
		{
			if (keyValuePair3.Value.Cost is Store.OnePay)
			{
				streamWriter.WriteLine("\t\t\t" + (int)keyValuePair3.Key + ",");
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public HashSet<int> TimedPurchases = new HashSet<int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair4 in Store.Purchases)
		{
			if (keyValuePair4.Value.Cost is Store.TimedPay)
			{
				streamWriter.WriteLine("\t\t\t" + (int)keyValuePair4.Key + ",");
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public HashSet<int> OncePayPurchases = new HashSet<int>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair5 in Store.Purchases)
		{
			if (keyValuePair5.Value.Cost is Store.OnePay && ((Store.OnePay)keyValuePair5.Value.Cost).Once)
			{
				streamWriter.WriteLine("\t\t\t" + (int)keyValuePair5.Key + ",");
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine(string.Empty);
		streamWriter.WriteLine("\t\tstatic public Dictionary<int, float> PurchaseCooldowns = new Dictionary<int, float>()");
		streamWriter.WriteLine("\t\t{");
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair6 in Store.Purchases)
		{
			if (keyValuePair6.Value.Cooldown > 0f)
			{
				streamWriter.WriteLine(string.Concat(new object[]
				{
					"\t\t\t{ ",
					(int)keyValuePair6.Key,
					", ",
					(int)keyValuePair6.Value.Cooldown.Value,
					"f },"
				}));
			}
		}
		streamWriter.WriteLine("\t\t};");
		streamWriter.WriteLine("\t}\n}");
		streamWriter.Close();
	}

	public static string[] TabTypeButtonNames = new string[]
	{
		".icons.ibtn_blocks",
		".icons.ibtn_blocks_wood",
		".icons.ibtn_blocks_stone",
		".icons.ibtn_blocks_nature",
		".icons.ibtn_blocks_colored",
		".icons.ibtn_blocks_glass",
		".icons.ibtn_blocks_cell",
		".icons.ibtn_blocks_tile",
		".icons.ibtn_blocks_sets",
		".icons.ibtn_pets",
		".icons.ibtn_maps",
		".icons.ibtn_skins",
		".icons.ibtn_plants",
		".icons.ibtn_weapons",
		".icons.ibtn_decorations",
		".icons.ibtn_tools",
		".icons.ibtn_hungry",
		".icons.ibtn_decorations_new",
		".icons.ibtn_weapons_skin",
		".icons.ibtn_decorations_star"
	};

	public static Dictionary<EntityType, Store.EntityInfo> Entities;

	public static Dictionary<BlockType, Store.BlockInfo> Blocks;

	public static Dictionary<StorePurchase, Store.PurchaseInfo> Purchases;

	public enum TabType
	{
		Blocks,
		Blocks_Wood,
		Blocks_Stone,
		Blocks_Nature,
		Blocks_Colored,
		Blocks_Glass,
		Blocks_Cell,
		Blocks_Tile,
		Blocks_Sets,
		Pets,
		Maps,
		Skins,
		Plants,
		Weapons,
		Decorations,
		Tools,
		Hungry_Games,
		Decorations_New,
		Weapon_Skin,
		Decorations_Star,
		Untagged
	}

	public class EntityInfo
	{
		public SecuredValue<StorePurchase> Purchase;

		public SecuredValue<string> SpriteName;

		public SecuredValue<Store.TabType> Tab;

		public SecuredValue<string> Category = string.Empty;

		public Func<bool> Validator;

		public SecuredValue<int> count = 0;
	}

	public class BlockInfo
	{
		public SecuredValue<StorePurchase> Purchase;

		public SecuredValue<string> SpriteName;

		public SecuredValue<Store.TabType> Tab;

		public SecuredValue<string> Category = string.Empty;

		public Func<bool> Validator;
	}

	public class PurchaseInfo
	{
		public bool Disabled;

		public Store.Pay Cost;

		public SecuredValue<string> Name;

		public SecuredValue<string> Icon;

		public SecuredValue<string> LargeIcon;

		public SecuredValue<Store.TabType> Tab;

		public SecuredValue<string> Category = string.Empty;

		public SecuredValue<int> Skin;

		public SecuredValue<object> Data;

		public SecuredValue<StorePurchase> RequiredPurchase = StorePurchase.NONE;

		public SecuredValue<float> Cooldown = 0f;

		public SecuredValue<int[]> WeaponStats;

		public SecuredValue<int> MinLevel = 0;

		public SecuredValue<int> Sales = 0;

		public SecuredValue<int> New = 0;
	}

	public class PurchaseInfoWeaponSkin : Store.PurchaseInfo
	{
		public WeaponSkinType SkinType;

		public int SkinId;
	}

	public class Pay
	{
		public SecuredValue<Currency> Curr;
	}

	public class OnePay : Store.Pay
	{
		public OnePay(Currency currency, int cost, bool once, int count = 1)
		{
			this.Curr = currency;
			this.Cost = cost;
			this.Once = once;
			this.Count = count;
			if (cost < 0 || count <= 0 || (once && count != 1))
			{
				UnityEngine.Debug.LogError("Invalid pay data");
			}
		}

		public SecuredValue<int> Cost;

		public SecuredValue<bool> Once;

		public SecuredValue<int> Count;
	}

	public class TimedPay : Store.Pay
	{
		public TimedPay(Currency currency, int dayCost, int weekCost, int foreverCost, int monthCost = 0)
		{
			this.Curr = currency;
			this.DayCost = dayCost;
			this.WeekCost = weekCost;
			this.ForeverCost = foreverCost;
			this.MonthCost = monthCost;
			if (dayCost < 0 || weekCost < 0 || foreverCost < 0 || monthCost < 0 || (monthCost != 0 && foreverCost != 0))
			{
				UnityEngine.Debug.LogError("Invalid pay data");
			}
		}

		public SecuredValue<int> DayCost;

		public SecuredValue<int> WeekCost;

		public SecuredValue<int> MonthCost;

		public SecuredValue<int> ForeverCost;
	}
}
