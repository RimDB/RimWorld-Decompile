﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_AITrashColonyClose : ThinkNode_JobGiver
	{
		private const int CloseSearchRadius = 5;

		public JobGiver_AITrashColonyClose()
		{
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			Job result;
			if (!pawn.HostileTo(Faction.OfPlayer))
			{
				result = null;
			}
			else
			{
				bool flag = pawn.natives.IgniteVerb != null && pawn.HostileTo(Faction.OfPlayer);
				CellRect cellRect = CellRect.CenteredOn(pawn.Position, 5);
				for (int i = 0; i < 35; i++)
				{
					IntVec3 randomCell = cellRect.RandomCell;
					if (randomCell.InBounds(pawn.Map))
					{
						Building edifice = randomCell.GetEdifice(pawn.Map);
						if (edifice != null && TrashUtility.ShouldTrashBuilding(pawn, edifice, false) && GenSight.LineOfSight(pawn.Position, randomCell, pawn.Map, false, null, 0, 0))
						{
							if (DebugViewSettings.drawDestSearch && Find.CurrentMap == pawn.Map)
							{
								Find.CurrentMap.debugDrawer.FlashCell(randomCell, 1f, "trash bld", 50);
							}
							return TrashUtility.TrashJob(pawn, edifice);
						}
						if (flag)
						{
							Plant plant = randomCell.GetPlant(pawn.Map);
							if (plant != null && TrashUtility.ShouldTrashPlant(pawn, plant) && GenSight.LineOfSight(pawn.Position, randomCell, pawn.Map, false, null, 0, 0))
							{
								if (DebugViewSettings.drawDestSearch && Find.CurrentMap == pawn.Map)
								{
									Find.CurrentMap.debugDrawer.FlashCell(randomCell, 0.5f, "trash plant", 50);
								}
								return TrashUtility.TrashJob(pawn, plant);
							}
						}
						if (DebugViewSettings.drawDestSearch && Find.CurrentMap == pawn.Map)
						{
							Find.CurrentMap.debugDrawer.FlashCell(randomCell, 0f, "trash no", 50);
						}
					}
				}
				result = null;
			}
			return result;
		}
	}
}
