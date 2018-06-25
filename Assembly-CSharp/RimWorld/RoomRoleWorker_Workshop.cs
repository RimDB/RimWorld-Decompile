﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200043C RID: 1084
	public class RoomRoleWorker_Workshop : RoomRoleWorker
	{
		// Token: 0x060012D9 RID: 4825 RVA: 0x000A2E7C File Offset: 0x000A127C
		public override float GetScore(Room room)
		{
			int num = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (int i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				if (containedAndAdjacentThings[i] is Building_WorkTable && containedAndAdjacentThings[i].def.designationCategory == DesignationCategoryDefOf.Production)
				{
					num++;
				}
			}
			return 13.5f * (float)num;
		}
	}
}
