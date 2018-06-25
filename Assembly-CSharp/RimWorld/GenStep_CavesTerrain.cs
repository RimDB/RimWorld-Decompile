﻿using System;
using Verse;
using Verse.Noise;

namespace RimWorld
{
	// Token: 0x020003EB RID: 1003
	public class GenStep_CavesTerrain : GenStep
	{
		// Token: 0x04000A7A RID: 2682
		private const float WaterFrequency = 0.08f;

		// Token: 0x04000A7B RID: 2683
		private const float GravelFrequency = 0.16f;

		// Token: 0x04000A7C RID: 2684
		private const float WaterThreshold = 0.93f;

		// Token: 0x04000A7D RID: 2685
		private const float GravelThreshold = 0.55f;

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x00093E7C File Offset: 0x0009227C
		public override int SeedPart
		{
			get
			{
				return 1921024373;
			}
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x00093E98 File Offset: 0x00092298
		public override void Generate(Map map)
		{
			if (Find.World.HasCaves(map.Tile))
			{
				Perlin perlin = new Perlin(0.079999998211860657, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
				Perlin perlin2 = new Perlin(0.15999999642372131, 2.0, 0.5, 6, Rand.Int, QualityMode.Medium);
				MapGenFloatGrid caves = MapGenerator.Caves;
				foreach (IntVec3 c in map.AllCells)
				{
					if (caves[c] > 0f)
					{
						TerrainDef terrain = c.GetTerrain(map);
						if (!terrain.IsRiver)
						{
							float num = (float)perlin.GetValue((double)c.x, 0.0, (double)c.z);
							float num2 = (float)perlin2.GetValue((double)c.x, 0.0, (double)c.z);
							if (num > 0.93f)
							{
								map.terrainGrid.SetTerrain(c, TerrainDefOf.WaterShallow);
							}
							else if (num2 > 0.55f)
							{
								map.terrainGrid.SetTerrain(c, TerrainDefOf.Gravel);
							}
						}
					}
				}
			}
		}
	}
}
