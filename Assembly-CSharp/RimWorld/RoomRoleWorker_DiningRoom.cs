﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000432 RID: 1074
	public class RoomRoleWorker_DiningRoom : RoomRoleWorker
	{
		// Token: 0x060012C5 RID: 4805 RVA: 0x000A28E4 File Offset: 0x000A0CE4
		public override float GetScore(Room room)
		{
			int num = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (int i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				Thing thing = containedAndAdjacentThings[i];
				if (thing.def.category == ThingCategory.Building && thing.def.surfaceType == SurfaceType.Eat)
				{
					num++;
				}
			}
			return (float)num * 8f;
		}
	}
}
