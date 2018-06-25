﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x020006B8 RID: 1720
	public static class BlightUtility
	{
		// Token: 0x06002505 RID: 9477 RVA: 0x0013E154 File Offset: 0x0013C554
		public static Plant GetFirstBlightableNowPlant(IntVec3 c, Map map)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Plant plant = thingList[i] as Plant;
				if (plant != null && plant.BlightableNow)
				{
					return plant;
				}
			}
			return null;
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0013E1B0 File Offset: 0x0013C5B0
		public static Plant GetFirstBlightableEverPlant(IntVec3 c, Map map)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Plant plant = thingList[i] as Plant;
				if (plant != null && plant.def.plant.Blightable)
				{
					return plant;
				}
			}
			return null;
		}
	}
}
