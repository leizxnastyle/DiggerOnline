using System;

public class Maps
{
	public static SecuredValue<Maps.StandartMap>[] StandartMaps = new SecuredValue<Maps.StandartMap>[]
	{
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Spaceport",
			Description = "Автор: Benns",
			SpriteName = "Spaceport",
			MapURL = "mavrin.org/Digger/Maps/TB_Spaceport.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Office",
			Description = "Автор: Benns",
			SpriteName = "Office",
			MapURL = "mavrin.org/Digger/Maps/TB_Office.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Zone69",
			Description = "Автор: Benns",
			SpriteName = "Zone69",
			MapURL = "mavrin.org/Digger/Maps/TB_Zone69.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Kitchen",
			Description = "Автор: Benns",
			SpriteName = "Kitchen",
			MapURL = "mavrin.org/Digger/Maps/TB_Kitchen.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Station",
			Description = "Автор: Benns",
			SpriteName = "Station",
			MapURL = "mavrin.org/Digger/Maps/TB_Station.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Factory",
			Description = "Автор: Benns",
			SpriteName = "Factory",
			MapURL = "mavrin.org/Digger/Maps/TB_Factory.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "ConstructionSite",
			Description = "Автор: Benns",
			SpriteName = "ConstructionSite",
			MapURL = "mavrin.org/Digger/Maps/Digger/TB_ConstructionSite.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Storage",
			Description = "Автор: Benns",
			SpriteName = "wall2015",
			MapURL = "mavrin.org/Digger/Maps/Digger/TB_Storage.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.TEAM_BATTLE,
			Destroyable = false,
			Name = "Butterfly_death",
			Description = string.Empty,
			SpriteName = "babochka",
			MapURL = "mavrin.org/Digger/Maps/TB_ButterflyDeath.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ZOMBIE_VIRUS,
			Destroyable = false,
			Name = "GasStation",
			Description = string.Empty,
			SpriteName = "sandoil",
			MapURL = "mavrin.org/Digger/Maps/ZV_GasStation.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.HUNGER_GAMES,
			Destroyable = false,
			Name = "Western",
			Description = "Автор: Фантазерка",
			SpriteName = "Western",
			MapURL = "mavrin.org/Digger/Maps/HG_Western2.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.HUNGER_GAMES,
			Destroyable = false,
			Name = "Rivendell",
			Description = "Автор: Фантазерка",
			SpriteName = "Rivendell",
			MapURL = "mavrin.org/Digger/Maps/HG_Rivendell.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.HUNGER_GAMES,
			Destroyable = false,
			Name = "Forest",
			Description = "Автор: Фантазерка",
			SpriteName = "Forest",
			MapURL = "mavrin.org/Digger/Maps/HG_Forest2.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.HUNGER_GAMES,
			Destroyable = false,
			Name = "Midory",
			Description = "Автор: КапитанБраво",
			SpriteName = "zorro3",
			MapURL = "mavrin.org/Digger/Maps/HG_Midory.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.HIDE_SEEK,
			Destroyable = true,
			Name = "TradeYard",
			Description = "Автор: КапитанБраво",
			SpriteName = "TradeYard",
			MapURL = "mavrin.org/Digger/Maps/HS_TradeYard.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.HIDE_SEEK,
			Destroyable = true,
			Name = "Greenwood",
			Description = "Автор: КапитанБраво",
			SpriteName = "Greenwood",
			MapURL = "mavrin.org/Digger/Maps/HS_Greenwood.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Spiral",
			Description = "Автор: QantivirusP",
			SpriteName = "Spiral",
			MapURL = "mavrin.org/Maps/RUN_Spiral.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "EyesOfStruggle",
			Description = "Автор: QantivirusP",
			SpriteName = "EyesOfStruggle",
			MapURL = "mavrin.org/Maps/RUN_EyesOfStruggle.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Balance",
			Description = "Автор: КапитанБраво",
			SpriteName = "Balance",
			MapURL = "mavrin.org/Maps/RUN_Balance.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Sun",
			Description = "Автор: КапитанБраво",
			SpriteName = "map_sun",
			MapURL = "mavrin.org/Maps/RUN_Sun.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Rainbow",
			Description = "Автор: КапитанБраво",
			SpriteName = "rainbow",
			MapURL = "mavrin.org/Maps/RUN_Rainbow.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Lines",
			Description = "Автор: Reich",
			SpriteName = "map505",
			MapURL = "mavrin.org/Maps/RUN_Lines.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Bricks",
			Description = "Автор: Atom_Neithon",
			SpriteName = "GotTmamHCg",
			MapURL = "mavrin.org/Maps/RUN_Bricks.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Zigzag",
			Description = "Автор: Benns",
			SpriteName = "ar_zigzag",
			MapURL = "mavrin.org/Maps/RUN_Zigzag.map"
		},
		new Maps.StandartMap
		{
			Game = GameINI.GameType.ARCADE,
			Destroyable = true,
			Name = "Round",
			Description = string.Empty,
			SpriteName = "round",
			MapURL = "mavrin.org/Maps/RUN_Rings.map"
		}
	};

	public class StandartMap
	{
		public SecuredValue<GameINI.GameType> Game;

		public SecuredValue<bool> Destroyable;

		public SecuredValue<string> Name;

		public SecuredValue<string> Description;

		public SecuredValue<string> SpriteName;

		public SecuredValue<string> MapURL;
	}
}
