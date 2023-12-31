using System;

public enum StorePurchase
{
	NONE = 5,
	SLOTS,
	CUBES1,
	CUBES2,
	CUBES3,
	FLOWER,
	STOOL,
	TABLE,
	BACKET,
	BASKET,
	BARREL,
	FENCE,
	CHEST,
	FLOWER2,
	FLOWER3,
	CUBES4,
	SIGN,
	BED,
	CUPBOARDS,
	CUPBOARDL,
	STAIRSW,
	STAIRSB,
	DOORW,
	DOORM,
	TABLICHKA,
	TABLICHKAW,
	TORCHF,
	TORCHW,
	CHANGENICK,
	KIRCKA,
	GLASSCUBES,
	RESHETKACUBES,
	DINAMIT,
	ATOMBOMB,
	STANDART_SKIN,
	WATER,
	LADDER,
	SPEED,
	FLY,
	PIRATE_SKIN_1,
	HALLOWEEN6,
	KNIGHT_SKIN_1,
	NPC7,
	NPC8,
	WSKIN_SWD_CARBON,
	WSKIN_SG553_SPACE,
	PICTURE3_2,
	KNIGHT_SKIN_2,
	MEDIUM_MAP,
	FLAT_MAP,
	SAND_MAP,
	OCEAN_MAP,
	ISLAND_MAP,
	SNOW_MAP,
	ICE,
	PIRATE_SKIN_2,
	LAVA,
	SPAWN_POINT,
	GOLD_KUBOK,
	VIKING_SKIN_1,
	FISH_1,
	FISH_2,
	FISH_3,
	ZOMBIE_SKIN,
	BOAT,
	EMOTIONS,
	RAIL,
	TROLLEY,
	TIME_OF_DAY,
	CAT,
	DOG,
	CHICKEN,
	DEATH_KNIGHT_SKIN,
	MEM_SMILES,
	SOLDIER,
	BOOK,
	BIG_TABLE,
	BED_STONE,
	CHAIR_STONE,
	CHEST_STONE,
	FENCE_STONE,
	TABLE_STONE,
	TORCH_STONE,
	TORCH_FLOOR_STONE,
	BENCH_WOOD,
	BENCH_STONE,
	TOMBSTONE_1,
	TOMBSTONE_2,
	WEAPON_GLOK,
	WEAPON_MP5,
	WEAPON_AK47,
	WEAPON_M16,
	WEAPON_RIFLE,
	WEAPON_SNIPER_RIFLE,
	WEAPON_M1014,
	WEAPON_GRENADE_LAUNCHER,
	BONUS_LIFE,
	BONUS_ARMOR,
	BONUS_WEAPONS,
	BOUNS_GRANADE,
	TEAM_SPAWN_POINT,
	SOVIET,
	SWAT,
	FOOD_CAKE,
	FOOD_WOOD_PLATE,
	FOOD_SPOON,
	FOOD_FORK,
	FOOD_MUG,
	FOOD_JUG,
	FOOD_METAL_PLATE,
	FOOD_BREAD,
	FOOD_CHEES,
	FOOD_CHIKEN,
	FOOD_SVININA,
	FOOD_FISH,
	FOOD_STEIK,
	FOOD_ORANGE,
	FOOD_PIZZA,
	FOOD_COLA,
	BURGER,
	FOOD_PIROG,
	CUBE_BATUTE,
	AMERICAN,
	GERMAN,
	TEAMCUBES,
	TEAMDOORS,
	FLAG,
	PAINTING_01,
	PAINTING_02,
	PAINTING_03,
	PAINTING_04,
	PAINTING_05,
	PAINTING_06,
	PAINTING_07,
	PAINTING_08,
	PAINTING_09,
	PAINTING_10,
	PAINTING_12,
	PAINTING_13,
	KNIGHT_SKIN_0,
	SKELETON_SKIN,
	DARK_KO_SKIN,
	IRON_KO_SKIN,
	BOAR,
	CRAB,
	LOCK_DOOR,
	LOCK_KEY,
	GIRL_SKIN_1,
	GIRL_SKIN_2,
	WEAPON_MP40,
	WEAPON_PPSH,
	WEAPON_THOMPSON,
	BLOCK_KIND_FENCE,
	BLOCK_KIND_HALF,
	BLOCK_KIND_DIAGONAL,
	BLOCK_KIND_STAIR,
	TILE_CUBES,
	CASTLE_CUBES,
	DUNGEON_CUBES,
	FORTRESS_CUBES,
	LIBRARY_CUBES,
	TAVERN_CUBES,
	MILITARY_CUBES,
	MILITARY_ARMORY,
	MILITARY_BARREL,
	MILITARY_BED,
	MILITARY_CAM,
	MILITARY_CASE_BIG,
	MILITARY_CASE_SMALL,
	MILITARY_CHAIR,
	MILITARY_CHEST,
	MILITARY_CONSOLE_01,
	MILITARY_CONSOLE_02,
	MILITARY_CONSOLE_03,
	MILITARY_DOOR,
	MILITARY_LAMP,
	MILITARY_LANTERN,
	MILITARY_MUG,
	MILITARY_PC,
	MILITARY_TABLE_BIG,
	MILITARY_TABLE_SMALL,
	MILITARY_FENCE,
	MILITARY_BENCH,
	MILITARY_BOX_01,
	MILITARY_BOX_02,
	MILITARY_BRIEFING_BOARD,
	ARCHER_SKIN,
	TERRORIST_SKIN,
	COOK_SKIN,
	DARK_STALKER_SKIN,
	MERCENARY_SKIN,
	SMELTER_SKIN,
	MORE_EXPERIENCE,
	BATTLE_GRENADES,
	BATTLE_HEALTH,
	BATTLE_SHIELD,
	WEAPON_SAWN_OFF,
	WEAPON_WINCHESTER,
	WEAPON_SG552,
	WEAPON_REVOLVER,
	WEAPON_GRENADE_LAUNCHER2,
	WEAPON_SG550,
	WEAPON_LUGER,
	WEAPON_AUG,
	WEAPON_COLT,
	WEAPON_SVD,
	BLOCK_KIND_QUARTER,
	INDUSTRIAL_CUBES,
	HG_ARENA_SPAWN_POINT,
	HG_APOS_SPAWN_POINT,
	HG_ARROW_MEN,
	HG_UNIOR_KNIGHT,
	DARK_CASTLE_CUBES,
	DARK_CASTLE_BED,
	DARK_CASTLE_BENCH,
	DARK_CASTLE_BOOK_CONSOLE,
	DARK_CASTLE_BOX_BIG,
	DARK_CASTLE_BOX_BIG_BOOKS,
	DARK_CASTLE_BOX_SMALL,
	DARK_CASTLE_CAGE,
	DARK_CASTLE_CHAIN,
	DARK_CASTLE_CHAIR,
	DARK_CASTLE_CHEST,
	DARK_CASTLE_COFFIN,
	DARK_CASTLE_DARK_PICTURE_01,
	DARK_CASTLE_DARK_PICTURE_02,
	DARK_CASTLE_DARK_PICTURE_03,
	DARK_CASTLE_DOOR,
	DARK_CASTLE_FENCE,
	DARK_CASTLE_GARGOYLEY,
	DARK_CASTLE_HERALDRY,
	DARK_CASTLE_SARCOPHAGUS,
	DARK_CASTLE_SKULL_MUG,
	DARK_CASTLE_STANDARD_01,
	DARK_CASTLE_STANDARD_02,
	DARK_CASTLE_TABLE_BIG,
	DARK_CASTLE_TABLE_SMALL,
	DARK_CASTLE_THRONE,
	DARK_CASTLE_TORCH_01,
	DARK_CASTLE_TORCH_02,
	DARK_CASTLE_TROLL,
	HG_ARCHER_BUILD_1,
	HG_ARCHER_BUILD_2,
	HG_ARCHER_BUILD_3,
	HG_ARCHER_BUILD_4,
	HG_ARCHER_BUILD_5,
	HG_KNIGHT_BUILD_1,
	HG_KNIGHT_BUILD_2,
	HG_KNIGHT_BUILD_3,
	HG_KNIGHT_BUILD_4,
	HG_KNIGHT_BUILD_5,
	CARPET_01,
	CARPET_02,
	CARPET_03,
	CARPET_04,
	CARPET_05,
	CARPET_06,
	CARPET_07,
	CARPET_08,
	CARPET_09,
	CARPET_10,
	CARPET_11TH,
	CARPET_11TV,
	BLOCK_KIND_CORNER,
	CUBE_RESTORE,
	BLOCK_KIND_STAIR_CORNER,
	CITY_INTERIOR,
	CITY_EXTERIOR,
	CE_ATTENTION_BOARD,
	CE_BIG_GARBAGE_BOX_CLOSE,
	CE_BIG_GARBAGE_BOX_OPEN,
	CE_BIG_STREET_LANTERN,
	CE_CONE,
	CE_FG,
	CE_GARBAGE_BOX,
	CE_MAIL_BOX,
	CE_METAL_DOOR,
	CE_PHONEBOX,
	CE_SMALL_SREET_LANTERN,
	CE_STREET_BENCH,
	CE_STREET_CLOCK,
	CE_STREET_LADDER,
	CE_WOOD_DOOR,
	CI_BATH,
	CI_BED,
	CI_BEDROOM_CASE,
	CI_BEDROOM_LAMP,
	CI_BIG_LAMP,
	CI_BOWL,
	CI_COFFEE_CUP,
	CI_DOOR,
	CI_KITCHEN_BOARD,
	CI_KITCHEN_CHAIR,
	CI_KITCHEN_PART,
	CI_KITCHEN_PART_CORNER,
	CI_KITCHEN_TABLE,
	CI_KITCHEN_WASHING,
	CI_WLAPTOP,
	CI_LOUDSPEAKERS,
	CI_MICROWAVE,
	CI_OFFICE_CHAIR,
	CI_OFFICE_LAMP,
	CI_OFFICE_TABLE,
	CI_OFFICE_CASE,
	CI_OVEN,
	CI_PLATE,
	CI_REFRIGERATOR,
	CI_SAUCER,
	CI_SHOWER,
	CI_SMALL_LAMP,
	CI_SMALL_TABLE,
	CI_SOAP,
	CI_SOFA,
	CI_SOFT_CHAIR,
	CI_TV,
	CI_WARDROBE,
	CI_WASHBASIN,
	CI_WWASHING_MACHINE,
	CI_WC,
	ALL_INCLUSIVE,
	NY_OFFER,
	NY_TREE,
	SANTA_SKIN,
	NBBLU,
	NBGRAY,
	NBDARKGREY,
	NBBROWN,
	NBBEIGE,
	NBKHAKI,
	PL_CACTUS,
	PL_WATERLILY,
	TINY_MAP_SIZE,
	PL_WATERLILY2,
	HOUSEPLANTS1,
	HOUSEPLANTS2,
	BIOM_AUTUMN,
	WSKIN_AK47_BLUE,
	WSKIN_AK47_VULK,
	WSKIN_GLOCK_GEAR,
	WSKIN_GLOCK_PIRATE,
	WSKIN_M16_AZIMOV,
	WSKIN_M16_KHAKI,
	WSKIN_M1014_DAYME,
	WSKIN_M1014_RED,
	WSKIN_SG553_FIOL,
	WSKIN_SG553_TREANGLE,
	MUSH_WHITE,
	MUSH_FOX,
	MUSH_AMANITA,
	MUSH_ORANGE_CAP,
	MUSH_TOADSTOOL,
	SUPER_KIRCKA,
	CAT_BLACK,
	CAT_STRIPED,
	NY_16_TREE,
	NY_16_BIG_TREE,
	NY_16_SNOWMAN,
	NY_16_GIFT,
	NY_16_GIFT_BAG,
	NY_16_SOCK,
	NY_16_WREATH,
	WSKIN_AUG_WAVES,
	WSKIN_GLAUNCHER_SAKURA,
	WSKIN_MP5_SPRINT,
	WSKIN_SWD_VORTEX,
	WSKIN_SWD_RADIATION,
	WSKIN_AK47_MAGMA,
	WSKIN_SWD_LEGACY,
	WSKIN_SWD_GEAR,
	WSKIN_SG553_CAMO,
	WSKIN_SG553_DRAGON,
	HALLOWEEN1,
	HALLOWEEN2,
	HALLOWEEN3,
	HALLOWEEN4,
	HALLOWEEN5,
	NPC1,
	NPC2,
	WSKIN_MP5_PALETTE,
	WSKIN_MP5_CYBER,
	WSKIN_MP5_LUXURY,
	NPC3,
	NPC4,
	NPC5,
	TELEPORT,
	NPC6,
	FIREWORK1,
	FIREWORK2,
	FIREWORK3,
	FIREWORK4,
	NPC9,
	NPC10,
	NPC11,
	FLAG1,
	FLAG2,
	FLAG3,
	FLAG4,
	FLAG5,
	FLAG6,
	FLAG7,
	FLAG8,
	FLAG9,
	FLAG10,
	HOUSEPLANTS3,
	HOUSEPLANTS4,
	HOUSEPLANTS5,
	WSKIN_AUG_TIMBER,
	WSKIN_AK47_WASTELAND,
	WSKIN_M16_ICEBERG,
	WSKIN_M16_RIDGIE,
	WSKIN_GLOCK_BLOOD,
	WSKIN_GLOCK_CRIME,
	NPC12,
	NPC13,
	NPC14,
	RUN_MAP,
	PLATFORM_MAP,
	WSKIN_SG550_GLORY,
	WSKIN_SAWN_OFF_ORIGAMI,
	BEACH1,
	BEACH2,
	BEACH3,
	BEACH4,
	BEACH5,
	BEACH6,
	WSKIN_SG550_PIXEL,
	WSKIN_SG550_FUTURE,
	WSKIN_SG550_NEON,
	SCHOOL1,
	SCHOOL2,
	SCHOOL3,
	SCHOOL4,
	SCHOOL5,
	SCHOOL6,
	NPC15,
	NPC16,
	NPC17,
	CAMPING1,
	CAMPING2,
	CAMPING3,
	CAMPING4,
	CAMPING5,
	CAMPING6,
	WESTERN1,
	WESTERN2,
	WESTERN3,
	WESTERN4,
	WESTERN5,
	WESTERN6,
	WESTERN7,
	NY9,
	NY10,
	NY11,
	NY12,
	EAST1,
	EAST2,
	EAST3,
	EAST4,
	EAST5,
	EAST6,
	NPC18,
	NPC19,
	FLAG11,
	FLAG12,
	PIRATES1,
	PIRATES2,
	PIRATES3,
	PIRATES4,
	PIRATES5,
	PIRATES6,
	POLICEMAN_GIRL
}
