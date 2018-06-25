﻿using System;
using Verse;

namespace RimWorld
{
	public class GenStep_Animals : GenStep
	{
		public GenStep_Animals()
		{
		}

		public override int SeedPart
		{
			get
			{
				return 1298760307;
			}
		}

		public override void Generate(Map map)
		{
			int num = 0;
			while (!map.wildAnimalSpawner.AnimalEcosystemFull)
			{
				num++;
				if (num >= 10000)
				{
					Log.Error("Too many iterations.", false);
					break;
				}
				IntVec3 loc = RCellFinder.RandomAnimalSpawnCell_MapGen(map);
				if (!map.wildAnimalSpawner.SpawnRandomWildAnimalAt(loc))
				{
					break;
				}
			}
		}
	}
}
