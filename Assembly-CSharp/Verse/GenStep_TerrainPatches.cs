﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000C5A RID: 3162
	public class GenStep_TerrainPatches : GenStep
	{
		// Token: 0x04002F83 RID: 12163
		public TerrainDef terrainDef;

		// Token: 0x04002F84 RID: 12164
		public FloatRange patchesPer10kCellsRange;

		// Token: 0x04002F85 RID: 12165
		public FloatRange patchSizeRange;

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x06004590 RID: 17808 RVA: 0x0024C520 File Offset: 0x0024A920
		public override int SeedPart
		{
			get
			{
				return 1370184742;
			}
		}

		// Token: 0x06004591 RID: 17809 RVA: 0x0024C53C File Offset: 0x0024A93C
		public override void Generate(Map map)
		{
			int num = Mathf.RoundToInt((float)map.Area / 10000f * this.patchesPer10kCellsRange.RandomInRange);
			for (int i = 0; i < num; i++)
			{
				float randomInRange = this.patchSizeRange.RandomInRange;
				IntVec3 a = CellFinder.RandomCell(map);
				foreach (IntVec3 b in GenRadial.RadialPatternInRadius(randomInRange / 2f))
				{
					IntVec3 c = a + b;
					if (c.InBounds(map))
					{
						map.terrainGrid.SetTerrain(c, this.terrainDef);
					}
				}
			}
		}
	}
}
