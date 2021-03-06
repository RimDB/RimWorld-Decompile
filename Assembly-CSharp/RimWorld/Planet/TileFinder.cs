﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Verse;

namespace RimWorld.Planet
{
	public static class TileFinder
	{
		private static List<Pair<int, int>> tmpTiles = new List<Pair<int, int>>();

		private static List<int> tmpPlayerTiles = new List<int>();

		[CompilerGenerated]
		private static Func<int, int> <>f__am$cache0;

		public static int RandomStartingTile()
		{
			return TileFinder.RandomSettlementTileFor(Faction.OfPlayer, true, null);
		}

		public static int RandomSettlementTileFor(Faction faction, bool mustBeAutoChoosable = false, Predicate<int> extraValidator = null)
		{
			for (int i = 0; i < 500; i++)
			{
				int num;
				if ((from _ in Enumerable.Range(0, 100)
				select Rand.Range(0, Find.WorldGrid.TilesCount)).TryRandomElementByWeight(delegate(int x)
				{
					Tile tile = Find.WorldGrid[x];
					if (!tile.biome.canBuildBase || !tile.biome.implemented || tile.hilliness == Hilliness.Impassable)
					{
						return 0f;
					}
					if (mustBeAutoChoosable && !tile.biome.canAutoChoose)
					{
						return 0f;
					}
					if (extraValidator != null && !extraValidator(x))
					{
						return 0f;
					}
					return tile.biome.settlementSelectionWeight;
				}, out num))
				{
					if (TileFinder.IsValidTileForNewSettlement(num, null))
					{
						return num;
					}
				}
			}
			Log.Error("Failed to find faction base tile for " + faction, false);
			return 0;
		}

		public static bool IsValidTileForNewSettlement(int tile, StringBuilder reason = null)
		{
			Tile tile2 = Find.WorldGrid[tile];
			if (!tile2.biome.canBuildBase)
			{
				if (reason != null)
				{
					reason.Append("CannotLandBiome".Translate(new object[]
					{
						tile2.biome.LabelCap
					}));
				}
				return false;
			}
			if (!tile2.biome.implemented)
			{
				if (reason != null)
				{
					reason.Append("BiomeNotImplemented".Translate() + ": " + tile2.biome.LabelCap);
				}
				return false;
			}
			if (tile2.hilliness == Hilliness.Impassable)
			{
				if (reason != null)
				{
					reason.Append("CannotLandImpassableMountains".Translate());
				}
				return false;
			}
			SettlementBase settlementBase = Find.WorldObjects.SettlementBaseAt(tile);
			if (settlementBase != null)
			{
				if (reason != null)
				{
					if (settlementBase.Faction == null)
					{
						reason.Append("TileOccupied".Translate());
					}
					else if (settlementBase.Faction == Faction.OfPlayer)
					{
						reason.Append("YourBaseAlreadyThere".Translate());
					}
					else
					{
						reason.Append("BaseAlreadyThere".Translate(new object[]
						{
							settlementBase.Faction.Name
						}));
					}
				}
				return false;
			}
			if (Find.WorldObjects.AnySettlementBaseAtOrAdjacent(tile))
			{
				if (reason != null)
				{
					reason.Append("FactionBaseAdjacent".Translate());
				}
				return false;
			}
			if (Find.WorldObjects.AnyMapParentAt(tile) || Current.Game.FindMap(tile) != null || Find.WorldObjects.AnyWorldObjectOfDefAt(WorldObjectDefOf.AbandonedSettlement, tile))
			{
				if (reason != null)
				{
					reason.Append("TileOccupied".Translate());
				}
				return false;
			}
			return true;
		}

		public static bool TryFindPassableTileWithTraversalDistance(int rootTile, int minDist, int maxDist, out int result, Predicate<int> validator = null, bool ignoreFirstTilePassability = false, bool preferCloserTiles = false)
		{
			TileFinder.tmpTiles.Clear();
			Find.WorldFloodFiller.FloodFill(rootTile, (int x) => !Find.World.Impassable(x) || (x == rootTile && ignoreFirstTilePassability), delegate(int tile, int traversalDistance)
			{
				if (traversalDistance > maxDist)
				{
					return true;
				}
				if (traversalDistance >= minDist && (validator == null || validator(tile)))
				{
					TileFinder.tmpTiles.Add(new Pair<int, int>(tile, traversalDistance));
				}
				return false;
			}, int.MaxValue, null);
			if (preferCloserTiles)
			{
				Pair<int, int> pair;
				if (TileFinder.tmpTiles.TryRandomElementByWeight((Pair<int, int> x) => 1f - (float)(x.Second - minDist) / ((float)(maxDist - minDist) + 0.01f), out pair))
				{
					result = pair.First;
					return true;
				}
				result = -1;
				return false;
			}
			else
			{
				Pair<int, int> pair;
				if (TileFinder.tmpTiles.TryRandomElement(out pair))
				{
					result = pair.First;
					return true;
				}
				result = -1;
				return false;
			}
		}

		public static bool TryFindRandomPlayerTile(out int tile, bool allowCaravans, Predicate<int> validator = null)
		{
			TileFinder.tmpPlayerTiles.Clear();
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].IsPlayerHome && maps[i].mapPawns.FreeColonistsSpawnedCount != 0 && (validator == null || validator(maps[i].Tile)))
				{
					TileFinder.tmpPlayerTiles.Add(maps[i].Tile);
				}
			}
			if (allowCaravans)
			{
				List<Caravan> caravans = Find.WorldObjects.Caravans;
				for (int j = 0; j < caravans.Count; j++)
				{
					if (caravans[j].IsPlayerControlled && (validator == null || validator(caravans[j].Tile)))
					{
						TileFinder.tmpPlayerTiles.Add(caravans[j].Tile);
					}
				}
			}
			if (TileFinder.tmpPlayerTiles.TryRandomElement(out tile))
			{
				return true;
			}
			Map map;
			if ((from x in Find.Maps
			where x.IsPlayerHome && (validator == null || validator(x.Tile))
			select x).TryRandomElement(out map))
			{
				tile = map.Tile;
				return true;
			}
			Map map2;
			if ((from x in Find.Maps
			where x.mapPawns.FreeColonistsSpawnedCount != 0 && (validator == null || validator(x.Tile))
			select x).TryRandomElement(out map2))
			{
				tile = map2.Tile;
				return true;
			}
			Caravan caravan;
			if (!allowCaravans && (from x in Find.WorldObjects.Caravans
			where x.IsPlayerControlled && (validator == null || validator(x.Tile))
			select x).TryRandomElement(out caravan))
			{
				tile = caravan.Tile;
				return true;
			}
			tile = -1;
			return false;
		}

		public static bool TryFindNewSiteTile(out int tile, int minDist = 7, int maxDist = 27, bool allowCaravans = false, bool preferCloserTiles = true, int nearThisTile = -1)
		{
			Func<int, int> findTile = delegate(int root)
			{
				int minDist2 = minDist;
				int maxDist2 = maxDist;
				int result2;
				ref int result = ref result2;
				Predicate<int> validator = (int x) => !Find.WorldObjects.AnyWorldObjectAt(x) && TileFinder.IsValidTileForNewSettlement(x, null);
				bool preferCloserTiles2 = preferCloserTiles;
				if (TileFinder.TryFindPassableTileWithTraversalDistance(root, minDist2, maxDist2, out result, validator, false, preferCloserTiles2))
				{
					return result2;
				}
				return -1;
			};
			int arg;
			if (nearThisTile != -1)
			{
				arg = nearThisTile;
			}
			else if (!TileFinder.TryFindRandomPlayerTile(out arg, allowCaravans, (int x) => findTile(x) != -1))
			{
				tile = -1;
				return false;
			}
			tile = findTile(arg);
			return tile != -1;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static TileFinder()
		{
		}

		[CompilerGenerated]
		private static int <RandomSettlementTileFor>m__0(int _)
		{
			return Rand.Range(0, Find.WorldGrid.TilesCount);
		}

		[CompilerGenerated]
		private sealed class <RandomSettlementTileFor>c__AnonStorey0
		{
			internal bool mustBeAutoChoosable;

			internal Predicate<int> extraValidator;

			public <RandomSettlementTileFor>c__AnonStorey0()
			{
			}

			internal float <>m__0(int x)
			{
				Tile tile = Find.WorldGrid[x];
				if (!tile.biome.canBuildBase || !tile.biome.implemented || tile.hilliness == Hilliness.Impassable)
				{
					return 0f;
				}
				if (this.mustBeAutoChoosable && !tile.biome.canAutoChoose)
				{
					return 0f;
				}
				if (this.extraValidator != null && !this.extraValidator(x))
				{
					return 0f;
				}
				return tile.biome.settlementSelectionWeight;
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindPassableTileWithTraversalDistance>c__AnonStorey1
		{
			internal int rootTile;

			internal bool ignoreFirstTilePassability;

			internal int maxDist;

			internal int minDist;

			internal Predicate<int> validator;

			public <TryFindPassableTileWithTraversalDistance>c__AnonStorey1()
			{
			}

			internal bool <>m__0(int x)
			{
				return !Find.World.Impassable(x) || (x == this.rootTile && this.ignoreFirstTilePassability);
			}

			internal bool <>m__1(int tile, int traversalDistance)
			{
				if (traversalDistance > this.maxDist)
				{
					return true;
				}
				if (traversalDistance >= this.minDist && (this.validator == null || this.validator(tile)))
				{
					TileFinder.tmpTiles.Add(new Pair<int, int>(tile, traversalDistance));
				}
				return false;
			}

			internal float <>m__2(Pair<int, int> x)
			{
				return 1f - (float)(x.Second - this.minDist) / ((float)(this.maxDist - this.minDist) + 0.01f);
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindRandomPlayerTile>c__AnonStorey2
		{
			internal Predicate<int> validator;

			public <TryFindRandomPlayerTile>c__AnonStorey2()
			{
			}

			internal bool <>m__0(Map x)
			{
				return x.IsPlayerHome && (this.validator == null || this.validator(x.Tile));
			}

			internal bool <>m__1(Map x)
			{
				return x.mapPawns.FreeColonistsSpawnedCount != 0 && (this.validator == null || this.validator(x.Tile));
			}

			internal bool <>m__2(Caravan x)
			{
				return x.IsPlayerControlled && (this.validator == null || this.validator(x.Tile));
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindNewSiteTile>c__AnonStorey3
		{
			internal int minDist;

			internal int maxDist;

			internal bool preferCloserTiles;

			internal Func<int, int> findTile;

			private static Predicate<int> <>f__am$cache0;

			public <TryFindNewSiteTile>c__AnonStorey3()
			{
			}

			internal int <>m__0(int root)
			{
				int num = this.minDist;
				int num2 = this.maxDist;
				int result2;
				ref int result = ref result2;
				Predicate<int> validator = (int x) => !Find.WorldObjects.AnyWorldObjectAt(x) && TileFinder.IsValidTileForNewSettlement(x, null);
				bool flag = this.preferCloserTiles;
				if (TileFinder.TryFindPassableTileWithTraversalDistance(root, num, num2, out result, validator, false, flag))
				{
					return result2;
				}
				return -1;
			}

			internal bool <>m__1(int x)
			{
				return this.findTile(x) != -1;
			}

			private static bool <>m__2(int x)
			{
				return !Find.WorldObjects.AnyWorldObjectAt(x) && TileFinder.IsValidTileForNewSettlement(x, null);
			}
		}
	}
}
