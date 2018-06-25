﻿using System;
using RimWorld.Planet;

namespace RimWorld
{
	public class BiomeWorker_TropicalSwamp : BiomeWorker
	{
		public BiomeWorker_TropicalSwamp()
		{
		}

		public override float GetScore(Tile tile, int tileID)
		{
			float result;
			if (tile.WaterCovered)
			{
				result = -100f;
			}
			else if (tile.temperature < 15f)
			{
				result = 0f;
			}
			else if (tile.rainfall < 2000f)
			{
				result = 0f;
			}
			else if (tile.swampiness < 0.5f)
			{
				result = 0f;
			}
			else
			{
				result = 28f + (tile.temperature - 20f) * 1.5f + (tile.rainfall - 600f) / 165f + tile.swampiness * 3f;
			}
			return result;
		}
	}
}
