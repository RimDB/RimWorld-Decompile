﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_PrisonerGetDressed : ThinkNode_JobGiver
	{
		public JobGiver_PrisonerGetDressed()
		{
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.guest.PrisonerIsSecure && pawn.apparel != null)
			{
				if (!pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.Legs))
				{
					Apparel apparel = this.FindGarmentCoveringPart(pawn, BodyPartGroupDefOf.Legs);
					if (apparel != null)
					{
						return new Job(JobDefOf.Wear, apparel)
						{
							ignoreForbidden = true
						};
					}
				}
				if (!pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.Torso))
				{
					Apparel apparel2 = this.FindGarmentCoveringPart(pawn, BodyPartGroupDefOf.Torso);
					if (apparel2 != null)
					{
						return new Job(JobDefOf.Wear, apparel2)
						{
							ignoreForbidden = true
						};
					}
				}
			}
			return null;
		}

		private Apparel FindGarmentCoveringPart(Pawn pawn, BodyPartGroupDef bodyPartGroupDef)
		{
			Room room = pawn.GetRoom(RegionType.Set_Passable);
			if (room.isPrisonCell)
			{
				foreach (IntVec3 c in room.Cells)
				{
					List<Thing> thingList = c.GetThingList(pawn.Map);
					for (int i = 0; i < thingList.Count; i++)
					{
						Apparel apparel = thingList[i] as Apparel;
						if (apparel != null && apparel.def.apparel.bodyPartGroups.Contains(bodyPartGroupDef) && pawn.CanReserve(apparel, 1, -1, null, false) && ApparelUtility.HasPartsToWear(pawn, apparel.def))
						{
							return apparel;
						}
					}
				}
			}
			return null;
		}
	}
}
