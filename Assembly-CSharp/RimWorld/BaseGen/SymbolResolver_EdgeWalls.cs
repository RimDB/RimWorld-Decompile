﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld.BaseGen
{
	public class SymbolResolver_EdgeWalls : SymbolResolver
	{
		public SymbolResolver_EdgeWalls()
		{
		}

		public override void Resolve(ResolveParams rp)
		{
			ThingDef wallStuff = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction, false);
			foreach (IntVec3 c in rp.rect.EdgeCells)
			{
				this.TrySpawnWall(c, rp, wallStuff);
			}
		}

		private Thing TrySpawnWall(IntVec3 c, ResolveParams rp, ThingDef wallStuff)
		{
			Map map = BaseGen.globalSettings.map;
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (!thingList[i].def.destroyable)
				{
					return null;
				}
				if (thingList[i] is Building_Door)
				{
					return null;
				}
			}
			for (int j = thingList.Count - 1; j >= 0; j--)
			{
				thingList[j].Destroy(DestroyMode.Vanish);
			}
			if (rp.chanceToSkipWallBlock != null && Rand.Chance(rp.chanceToSkipWallBlock.Value))
			{
				return null;
			}
			Thing thing = ThingMaker.MakeThing(ThingDefOf.Wall, wallStuff);
			thing.SetFaction(rp.faction, null);
			return GenSpawn.Spawn(thing, c, map, WipeMode.Vanish);
		}
	}
}
