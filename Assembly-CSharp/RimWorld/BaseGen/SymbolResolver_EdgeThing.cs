﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld.BaseGen
{
	public class SymbolResolver_EdgeThing : SymbolResolver
	{
		private List<int> randomRotations = new List<int>
		{
			0,
			1,
			2,
			3
		};

		private int MaxTriesToAvoidOtherEdgeThings = 4;

		public SymbolResolver_EdgeThing()
		{
		}

		public override bool CanResolve(ResolveParams rp)
		{
			if (!base.CanResolve(rp))
			{
				return false;
			}
			if (rp.singleThingDef != null)
			{
				bool? edgeThingAvoidOtherEdgeThings = rp.edgeThingAvoidOtherEdgeThings;
				bool avoidOtherEdgeThings = edgeThingAvoidOtherEdgeThings != null && edgeThingAvoidOtherEdgeThings.Value;
				if (rp.thingRot != null)
				{
					IntVec3 intVec;
					if (!this.TryFindSpawnCell(rp.rect, rp.singleThingDef, rp.thingRot.Value, avoidOtherEdgeThings, out intVec))
					{
						return false;
					}
				}
				else if (!rp.singleThingDef.rotatable)
				{
					IntVec3 intVec;
					if (!this.TryFindSpawnCell(rp.rect, rp.singleThingDef, Rot4.North, avoidOtherEdgeThings, out intVec))
					{
						return false;
					}
				}
				else
				{
					bool flag = false;
					for (int i = 0; i < 4; i++)
					{
						IntVec3 intVec;
						if (this.TryFindSpawnCell(rp.rect, rp.singleThingDef, new Rot4(i), avoidOtherEdgeThings, out intVec))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override void Resolve(ResolveParams rp)
		{
			ThingDef thingDef = rp.singleThingDef ?? (from x in DefDatabase<ThingDef>.AllDefsListForReading
			where (x.IsWeapon || x.IsMedicine || x.IsDrug) && x.graphicData != null && !x.destroyOnDrop && x.size.x <= rp.rect.Width && x.size.z <= rp.rect.Width && x.size.x <= rp.rect.Height && x.size.z <= rp.rect.Height
			select x).RandomElement<ThingDef>();
			IntVec3 invalid = IntVec3.Invalid;
			Rot4 value = Rot4.North;
			bool? edgeThingAvoidOtherEdgeThings = rp.edgeThingAvoidOtherEdgeThings;
			bool avoidOtherEdgeThings = edgeThingAvoidOtherEdgeThings != null && edgeThingAvoidOtherEdgeThings.Value;
			if (rp.thingRot != null)
			{
				if (!this.TryFindSpawnCell(rp.rect, thingDef, rp.thingRot.Value, avoidOtherEdgeThings, out invalid))
				{
					return;
				}
				value = rp.thingRot.Value;
			}
			else if (!thingDef.rotatable)
			{
				if (!this.TryFindSpawnCell(rp.rect, thingDef, Rot4.North, avoidOtherEdgeThings, out invalid))
				{
					return;
				}
				value = Rot4.North;
			}
			else
			{
				this.randomRotations.Shuffle<int>();
				bool flag = false;
				for (int i = 0; i < this.randomRotations.Count; i++)
				{
					if (this.TryFindSpawnCell(rp.rect, thingDef, new Rot4(this.randomRotations[i]), avoidOtherEdgeThings, out invalid))
					{
						value = new Rot4(this.randomRotations[i]);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			ResolveParams rp2 = rp;
			rp2.rect = CellRect.SingleCell(invalid);
			rp2.thingRot = new Rot4?(value);
			rp2.singleThingDef = thingDef;
			BaseGen.symbolStack.Push("thing", rp2);
		}

		private bool TryFindSpawnCell(CellRect rect, ThingDef thingDef, Rot4 rot, bool avoidOtherEdgeThings, out IntVec3 spawnCell)
		{
			if (avoidOtherEdgeThings)
			{
				spawnCell = IntVec3.Invalid;
				int num = -1;
				for (int i = 0; i < this.MaxTriesToAvoidOtherEdgeThings; i++)
				{
					IntVec3 intVec;
					if (this.TryFindSpawnCell(rect, thingDef, rot, out intVec))
					{
						int distanceSquaredToExistingEdgeThing = this.GetDistanceSquaredToExistingEdgeThing(intVec, rect, thingDef);
						if (!spawnCell.IsValid || distanceSquaredToExistingEdgeThing > num)
						{
							spawnCell = intVec;
							num = distanceSquaredToExistingEdgeThing;
							if (num == 2147483647)
							{
								break;
							}
						}
					}
				}
				return spawnCell.IsValid;
			}
			return this.TryFindSpawnCell(rect, thingDef, rot, out spawnCell);
		}

		private bool TryFindSpawnCell(CellRect rect, ThingDef thingDef, Rot4 rot, out IntVec3 spawnCell)
		{
			Map map = BaseGen.globalSettings.map;
			IntVec3 zero = IntVec3.Zero;
			IntVec2 size = thingDef.size;
			GenAdj.AdjustForRotation(ref zero, ref size, rot);
			CellRect empty = CellRect.Empty;
			Predicate<CellRect> basePredicate = delegate(CellRect x)
			{
				if (x.Cells.All((IntVec3 y) => y.Standable(map)))
				{
					if (!GenSpawn.WouldWipeAnythingWith(x, thingDef, map, (Thing z) => z.def.category == ThingCategory.Building))
					{
						return thingDef.category != ThingCategory.Item || x.CenterCell.GetFirstItem(map) == null;
					}
				}
				return false;
			};
			bool flag = false;
			if (thingDef.category == ThingCategory.Building)
			{
				flag = rect.TryFindRandomInnerRectTouchingEdge(size, out empty, (CellRect x) => basePredicate(x) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(x, map) && GenConstruct.TerrainCanSupport(x, map, thingDef));
				if (!flag)
				{
					flag = rect.TryFindRandomInnerRectTouchingEdge(size, out empty, (CellRect x) => basePredicate(x) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(x, map));
				}
			}
			if (!flag && !rect.TryFindRandomInnerRectTouchingEdge(size, out empty, basePredicate))
			{
				spawnCell = IntVec3.Invalid;
				return false;
			}
			CellRect.CellRectIterator iterator = empty.GetIterator();
			while (!iterator.Done())
			{
				if (GenAdj.OccupiedRect(iterator.Current, rot, thingDef.size) == empty)
				{
					spawnCell = iterator.Current;
					return true;
				}
				iterator.MoveNext();
			}
			Log.Error("We found a valid rect but we couldn't find the root position. This should never happen.", false);
			spawnCell = IntVec3.Invalid;
			return false;
		}

		private int GetDistanceSquaredToExistingEdgeThing(IntVec3 cell, CellRect rect, ThingDef thingDef)
		{
			Map map = BaseGen.globalSettings.map;
			int num = int.MaxValue;
			foreach (IntVec3 intVec in rect.EdgeCells)
			{
				List<Thing> thingList = intVec.GetThingList(map);
				bool flag = false;
				for (int i = 0; i < thingList.Count; i++)
				{
					if (thingList[i].def == thingDef)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					num = Mathf.Min(num, cell.DistanceToSquared(intVec));
				}
			}
			return num;
		}

		[CompilerGenerated]
		private sealed class <Resolve>c__AnonStorey0
		{
			internal ResolveParams rp;

			public <Resolve>c__AnonStorey0()
			{
			}

			internal bool <>m__0(ThingDef x)
			{
				return (x.IsWeapon || x.IsMedicine || x.IsDrug) && x.graphicData != null && !x.destroyOnDrop && x.size.x <= this.rp.rect.Width && x.size.z <= this.rp.rect.Width && x.size.x <= this.rp.rect.Height && x.size.z <= this.rp.rect.Height;
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindSpawnCell>c__AnonStorey1
		{
			internal Map map;

			internal ThingDef thingDef;

			internal Predicate<CellRect> basePredicate;

			private static Predicate<Thing> <>f__am$cache0;

			public <TryFindSpawnCell>c__AnonStorey1()
			{
			}

			internal bool <>m__0(CellRect x)
			{
				if (x.Cells.All((IntVec3 y) => y.Standable(this.map)))
				{
					if (!GenSpawn.WouldWipeAnythingWith(x, this.thingDef, this.map, (Thing z) => z.def.category == ThingCategory.Building))
					{
						return this.thingDef.category != ThingCategory.Item || x.CenterCell.GetFirstItem(this.map) == null;
					}
				}
				return false;
			}

			internal bool <>m__1(CellRect x)
			{
				return this.basePredicate(x) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(x, this.map) && GenConstruct.TerrainCanSupport(x, this.map, this.thingDef);
			}

			internal bool <>m__2(CellRect x)
			{
				return this.basePredicate(x) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(x, this.map);
			}

			internal bool <>m__3(IntVec3 y)
			{
				return y.Standable(this.map);
			}

			private static bool <>m__4(Thing z)
			{
				return z.def.category == ThingCategory.Building;
			}
		}
	}
}
