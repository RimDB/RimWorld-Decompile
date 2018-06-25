﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x020003FF RID: 1023
	public class GenStep_ScatterLumpsMineable : GenStep_Scatterer
	{
		// Token: 0x04000AAA RID: 2730
		public ThingDef forcedDefToScatter;

		// Token: 0x04000AAB RID: 2731
		public int forcedLumpSize;

		// Token: 0x04000AAC RID: 2732
		public float maxValue = float.MaxValue;

		// Token: 0x04000AAD RID: 2733
		[Unsaved]
		protected List<IntVec3> recentLumpCells = new List<IntVec3>();

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06001191 RID: 4497 RVA: 0x00098404 File Offset: 0x00096804
		public override int SeedPart
		{
			get
			{
				return 920906419;
			}
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00098420 File Offset: 0x00096820
		public override void Generate(Map map)
		{
			this.minSpacing = 5f;
			this.warnOnFail = false;
			int num = base.CalculateFinalCount(map);
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec;
				if (!this.TryFindScatterCell(map, out intVec))
				{
					return;
				}
				this.ScatterAt(intVec, map, 1);
				this.usedSpots.Add(intVec);
			}
			this.usedSpots.Clear();
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00098490 File Offset: 0x00096890
		protected ThingDef ChooseThingDef()
		{
			ThingDef result;
			if (this.forcedDefToScatter != null)
			{
				result = this.forcedDefToScatter;
			}
			else
			{
				result = DefDatabase<ThingDef>.AllDefs.RandomElementByWeightWithFallback(delegate(ThingDef d)
				{
					float result2;
					if (d.building == null)
					{
						result2 = 0f;
					}
					else if (d.building.mineableThing != null && d.building.mineableThing.BaseMarketValue > this.maxValue)
					{
						result2 = 0f;
					}
					else
					{
						result2 = d.building.mineableScatterCommonality;
					}
					return result2;
				}, null);
			}
			return result;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x000984D4 File Offset: 0x000968D4
		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			bool result;
			if (base.NearUsedSpot(c, this.minSpacing))
			{
				result = false;
			}
			else
			{
				Building edifice = c.GetEdifice(map);
				result = (edifice != null && edifice.def.building.isNaturalRock);
			}
			return result;
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00098530 File Offset: 0x00096930
		protected override void ScatterAt(IntVec3 c, Map map, int stackCount = 1)
		{
			ThingDef thingDef = this.ChooseThingDef();
			if (thingDef != null)
			{
				int numCells = (this.forcedLumpSize <= 0) ? thingDef.building.mineableScatterLumpSizeRange.RandomInRange : this.forcedLumpSize;
				this.recentLumpCells.Clear();
				foreach (IntVec3 intVec in GridShapeMaker.IrregularLump(c, map, numCells))
				{
					GenSpawn.Spawn(thingDef, intVec, map, WipeMode.Vanish);
					this.recentLumpCells.Add(intVec);
				}
			}
		}
	}
}
