﻿using System;
using RimWorld.Planet;

namespace RimWorld
{
	// Token: 0x0200054F RID: 1359
	public class BiomeWorker_TemperateForest : BiomeWorker
	{
		// Token: 0x06001956 RID: 6486 RVA: 0x000DBF80 File Offset: 0x000DA380
		public override float GetScore(Tile tile, int tileID)
		{
			float result;
			if (tile.WaterCovered)
			{
				result = -100f;
			}
			else if (tile.temperature < -10f)
			{
				result = 0f;
			}
			else if (tile.rainfall < 600f)
			{
				result = 0f;
			}
			else
			{
				result = 15f + (tile.temperature - 7f) + (tile.rainfall - 600f) / 180f;
			}
			return result;
		}
	}
}
