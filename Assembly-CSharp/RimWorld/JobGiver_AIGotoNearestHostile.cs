﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_AIGotoNearestHostile : ThinkNode_JobGiver
	{
		public JobGiver_AIGotoNearestHostile()
		{
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			float num = float.MaxValue;
			Thing thing = null;
			List<IAttackTarget> potentialTargetsFor = pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
			for (int i = 0; i < potentialTargetsFor.Count; i++)
			{
				IAttackTarget attackTarget = potentialTargetsFor[i];
				if (!attackTarget.ThreatDisabled(pawn))
				{
					Thing thing2 = (Thing)attackTarget;
					int num2 = thing2.Position.DistanceToSquared(pawn.Position);
					if ((float)num2 < num && pawn.CanReach(thing2, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						num = (float)num2;
						thing = thing2;
					}
				}
			}
			Job result;
			if (thing != null)
			{
				result = new Job(JobDefOf.Goto, thing)
				{
					checkOverrideOnExpire = true,
					expiryInterval = 500,
					collideWithPawns = true
				};
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
