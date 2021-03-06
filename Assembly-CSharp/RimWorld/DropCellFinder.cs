﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class DropCellFinder
	{
		[CompilerGenerated]
		private static Func<Building, bool> <>f__am$cache0;

		[CompilerGenerated]
		private static Func<Building, bool> <>f__am$cache1;

		[CompilerGenerated]
		private static Predicate<Building> <>f__am$cache2;

		public static IntVec3 RandomDropSpot(Map map)
		{
			return CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.Roofed(map) && !c.Fogged(map), map, 1000);
		}

		public static IntVec3 TradeDropSpot(Map map)
		{
			IEnumerable<Building> collection = from b in map.listerBuildings.allBuildingsColonist
			where b.def.IsCommsConsole
			select b;
			IEnumerable<Building> enumerable = from b in map.listerBuildings.allBuildingsColonist
			where b.def.IsOrbitalTradeBeacon
			select b;
			Building building = enumerable.FirstOrDefault((Building b) => !map.roofGrid.Roofed(b.Position) && DropCellFinder.AnyAdjacentGoodDropSpot(b.Position, map, false, false));
			IntVec3 position;
			if (building != null)
			{
				position = building.Position;
				IntVec3 result;
				if (!DropCellFinder.TryFindDropSpotNear(position, map, out result, false, false))
				{
					Log.Error("Could find no good TradeDropSpot near dropCenter " + position + ". Using a random standable unfogged cell.", false);
					result = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.Fogged(map), map, 1000);
				}
				return result;
			}
			List<Building> list = new List<Building>();
			list.AddRange(enumerable);
			list.AddRange(collection);
			list.RemoveAll(delegate(Building b)
			{
				CompPowerTrader compPowerTrader = b.TryGetComp<CompPowerTrader>();
				return compPowerTrader != null && !compPowerTrader.PowerOn;
			});
			Predicate<IntVec3> validator = (IntVec3 c) => DropCellFinder.IsGoodDropSpot(c, map, false, false);
			if (!list.Any<Building>())
			{
				list.AddRange(map.listerBuildings.allBuildingsColonist);
				list.Shuffle<Building>();
				if (!list.Any<Building>())
				{
					return CellFinderLoose.RandomCellWith(validator, map, 1000);
				}
			}
			int num = 8;
			for (;;)
			{
				for (int i = 0; i < list.Count; i++)
				{
					IntVec3 position2 = list[i].Position;
					if (CellFinder.TryFindRandomCellNear(position2, map, num, validator, out position, -1))
					{
						return position;
					}
				}
				num = Mathf.RoundToInt((float)num * 1.1f);
				if (num > map.Size.x)
				{
					goto Block_9;
				}
			}
			return position;
			Block_9:
			Log.Error("Failed to generate trade drop center. Giving random.", false);
			return CellFinderLoose.RandomCellWith(validator, map, 1000);
		}

		public static bool TryFindDropSpotNear(IntVec3 center, Map map, out IntVec3 result, bool allowFogged, bool canRoofPunch)
		{
			if (DebugViewSettings.drawDestSearch)
			{
				map.debugDrawer.FlashCell(center, 1f, "center", 50);
			}
			Predicate<IntVec3> validator = (IntVec3 c) => DropCellFinder.IsGoodDropSpot(c, map, allowFogged, canRoofPunch) && map.reachability.CanReach(center, c, PathEndMode.OnCell, TraverseMode.PassDoors, Danger.Deadly);
			int num = 5;
			while (!CellFinder.TryFindRandomCellNear(center, map, num, validator, out result, -1))
			{
				num += 3;
				if (num > 16)
				{
					result = center;
					return false;
				}
			}
			return true;
		}

		public static bool IsGoodDropSpot(IntVec3 c, Map map, bool allowFogged, bool canRoofPunch)
		{
			if (!c.InBounds(map) || !c.Standable(map))
			{
				return false;
			}
			if (!DropCellFinder.CanPhysicallyDropInto(c, map, canRoofPunch))
			{
				if (DebugViewSettings.drawDestSearch)
				{
					map.debugDrawer.FlashCell(c, 0f, "phys", 50);
				}
				return false;
			}
			if (Current.ProgramState == ProgramState.Playing && !allowFogged && c.Fogged(map))
			{
				return false;
			}
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (thing is IActiveDropPod || thing is Skyfaller)
				{
					return false;
				}
				if (thing.def.category != ThingCategory.Plant && GenSpawn.SpawningWipes(ThingDefOf.ActiveDropPod, thing.def))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AnyAdjacentGoodDropSpot(IntVec3 c, Map map, bool allowFogged, bool canRoofPunch)
		{
			return DropCellFinder.IsGoodDropSpot(c + IntVec3.North, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.East, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.South, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.West, map, allowFogged, canRoofPunch);
		}

		public static IntVec3 FindRaidDropCenterDistant(Map map)
		{
			Faction hostFaction = map.ParentFaction ?? Faction.OfPlayer;
			IEnumerable<Thing> enumerable = map.mapPawns.FreeHumanlikesSpawnedOfFaction(hostFaction).Cast<Thing>();
			if (hostFaction == Faction.OfPlayer)
			{
				enumerable = enumerable.Concat(map.listerBuildings.allBuildingsColonist.Cast<Thing>());
			}
			else
			{
				enumerable = enumerable.Concat(from x in map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial)
				where x.Faction == hostFaction
				select x);
			}
			int num = 0;
			float num2 = 65f;
			IntVec3 intVec;
			for (;;)
			{
				intVec = CellFinder.RandomCell(map);
				num++;
				if (DropCellFinder.CanPhysicallyDropInto(intVec, map, true))
				{
					if (num > 300)
					{
						break;
					}
					if (!intVec.Roofed(map))
					{
						num2 -= 0.2f;
						bool flag = false;
						foreach (Thing thing in enumerable)
						{
							if ((float)(intVec - thing.Position).LengthHorizontalSquared < num2 * num2)
							{
								flag = true;
								break;
							}
						}
						if (!flag && map.reachability.CanReachFactionBase(intVec, hostFaction))
						{
							return intVec;
						}
					}
				}
			}
			return intVec;
		}

		public static bool TryFindRaidDropCenterClose(out IntVec3 spot, Map map)
		{
			Faction faction = map.ParentFaction ?? Faction.OfPlayer;
			int num = 0;
			for (;;)
			{
				IntVec3 root = IntVec3.Invalid;
				if (map.mapPawns.FreeHumanlikesSpawnedOfFaction(faction).Count<Pawn>() > 0)
				{
					root = map.mapPawns.FreeHumanlikesSpawnedOfFaction(faction).RandomElement<Pawn>().Position;
				}
				else
				{
					if (faction == Faction.OfPlayer)
					{
						List<Building> allBuildingsColonist = map.listerBuildings.allBuildingsColonist;
						for (int i = 0; i < allBuildingsColonist.Count; i++)
						{
							if (DropCellFinder.TryFindDropSpotNear(allBuildingsColonist[i].Position, map, out root, true, true))
							{
								break;
							}
						}
					}
					else
					{
						List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j].Faction == faction && DropCellFinder.TryFindDropSpotNear(list[j].Position, map, out root, true, true))
							{
								break;
							}
						}
					}
					if (!root.IsValid)
					{
						root = DropCellFinder.RandomDropSpot(map);
					}
				}
				spot = CellFinder.RandomClosewalkCellNear(root, map, 10, null);
				if (DropCellFinder.CanPhysicallyDropInto(spot, map, true))
				{
					break;
				}
				num++;
				if (num > 300)
				{
					goto Block_9;
				}
			}
			return true;
			Block_9:
			spot = CellFinderLoose.RandomCellWith((IntVec3 c) => DropCellFinder.CanPhysicallyDropInto(c, map, true), map, 1000);
			return false;
		}

		private static bool CanPhysicallyDropInto(IntVec3 c, Map map, bool canRoofPunch)
		{
			if (!c.Walkable(map))
			{
				return false;
			}
			RoofDef roof = c.GetRoof(map);
			if (roof != null)
			{
				if (!canRoofPunch)
				{
					return false;
				}
				if (roof.isThickRoof)
				{
					return false;
				}
			}
			return true;
		}

		[CompilerGenerated]
		private static bool <TradeDropSpot>m__0(Building b)
		{
			return b.def.IsCommsConsole;
		}

		[CompilerGenerated]
		private static bool <TradeDropSpot>m__1(Building b)
		{
			return b.def.IsOrbitalTradeBeacon;
		}

		[CompilerGenerated]
		private static bool <TradeDropSpot>m__2(Building b)
		{
			CompPowerTrader compPowerTrader = b.TryGetComp<CompPowerTrader>();
			return compPowerTrader != null && !compPowerTrader.PowerOn;
		}

		[CompilerGenerated]
		private sealed class <RandomDropSpot>c__AnonStorey0
		{
			internal Map map;

			public <RandomDropSpot>c__AnonStorey0()
			{
			}

			internal bool <>m__0(IntVec3 c)
			{
				return c.Standable(this.map) && !c.Roofed(this.map) && !c.Fogged(this.map);
			}
		}

		[CompilerGenerated]
		private sealed class <TradeDropSpot>c__AnonStorey1
		{
			internal Map map;

			public <TradeDropSpot>c__AnonStorey1()
			{
			}

			internal bool <>m__0(Building b)
			{
				return !this.map.roofGrid.Roofed(b.Position) && DropCellFinder.AnyAdjacentGoodDropSpot(b.Position, this.map, false, false);
			}

			internal bool <>m__1(IntVec3 c)
			{
				return DropCellFinder.IsGoodDropSpot(c, this.map, false, false);
			}

			internal bool <>m__2(IntVec3 c)
			{
				return c.Standable(this.map) && !c.Fogged(this.map);
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindDropSpotNear>c__AnonStorey2
		{
			internal Map map;

			internal bool allowFogged;

			internal bool canRoofPunch;

			internal IntVec3 center;

			public <TryFindDropSpotNear>c__AnonStorey2()
			{
			}

			internal bool <>m__0(IntVec3 c)
			{
				return DropCellFinder.IsGoodDropSpot(c, this.map, this.allowFogged, this.canRoofPunch) && this.map.reachability.CanReach(this.center, c, PathEndMode.OnCell, TraverseMode.PassDoors, Danger.Deadly);
			}
		}

		[CompilerGenerated]
		private sealed class <FindRaidDropCenterDistant>c__AnonStorey3
		{
			internal Faction hostFaction;

			public <FindRaidDropCenterDistant>c__AnonStorey3()
			{
			}

			internal bool <>m__0(Thing x)
			{
				return x.Faction == this.hostFaction;
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindRaidDropCenterClose>c__AnonStorey4
		{
			internal Map map;

			public <TryFindRaidDropCenterClose>c__AnonStorey4()
			{
			}

			internal bool <>m__0(IntVec3 c)
			{
				return DropCellFinder.CanPhysicallyDropInto(c, this.map, true);
			}
		}
	}
}
