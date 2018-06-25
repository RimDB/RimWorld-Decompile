﻿using System;

namespace Verse.AI
{
	// Token: 0x02000ADB RID: 2779
	public static class WanderUtility
	{
		// Token: 0x06003D99 RID: 15769 RVA: 0x002066C4 File Offset: 0x00204AC4
		public static IntVec3 BestCloseWanderRoot(IntVec3 trueWanderRoot, Pawn pawn)
		{
			for (int i = 0; i < 50; i++)
			{
				IntVec3 intVec;
				if (i < 8)
				{
					intVec = trueWanderRoot + GenRadial.RadialPattern[i];
				}
				else
				{
					intVec = trueWanderRoot + GenRadial.RadialPattern[i - 8 + 1] * 7;
				}
				if (intVec.InBounds(pawn.Map) && intVec.Walkable(pawn.Map) && pawn.CanReach(intVec, PathEndMode.OnCell, Danger.Some, false, TraverseMode.ByPawn))
				{
					return intVec;
				}
			}
			return IntVec3.Invalid;
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x00206778 File Offset: 0x00204B78
		public static bool InSameRoom(IntVec3 locA, IntVec3 locB, Map map)
		{
			Room room = locA.GetRoom(map, RegionType.Set_All);
			return room == null || room == locB.GetRoom(map, RegionType.Set_All);
		}
	}
}
