﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JoyGiver_GoForWalk : JoyGiver
	{
		public JoyGiver_GoForWalk()
		{
		}

		public override Job TryGiveJob(Pawn pawn)
		{
			if (!JoyUtility.EnjoyableOutsideNow(pawn, null))
			{
				return null;
			}
			if (PawnUtility.WillSoonHaveBasicNeed(pawn))
			{
				return null;
			}
			Predicate<IntVec3> cellValidator = (IntVec3 x) => !PawnUtility.KnownDangerAt(x, pawn.Map, pawn) && !x.GetTerrain(pawn.Map).avoidWander && x.Standable(pawn.Map) && !x.Roofed(pawn.Map);
			Predicate<Region> validator = delegate(Region x)
			{
				IntVec3 intVec;
				return x.Room.PsychologicallyOutdoors && !x.IsForbiddenEntirely(pawn) && x.TryFindRandomCellInRegionUnforbidden(pawn, cellValidator, out intVec);
			};
			Region reg;
			if (!CellFinder.TryFindClosestRegionWith(pawn.GetRegion(RegionType.Set_Passable), TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), validator, 100, out reg, RegionType.Set_Passable))
			{
				return null;
			}
			IntVec3 root;
			if (!reg.TryFindRandomCellInRegionUnforbidden(pawn, cellValidator, out root))
			{
				return null;
			}
			List<IntVec3> list;
			if (!WalkPathFinder.TryFindWalkPath(pawn, root, out list))
			{
				return null;
			}
			Job job = new Job(this.def.jobDef, list[0]);
			job.targetQueueA = new List<LocalTargetInfo>();
			for (int i = 1; i < list.Count; i++)
			{
				job.targetQueueA.Add(list[i]);
			}
			job.locomotionUrgency = LocomotionUrgency.Walk;
			return job;
		}

		[CompilerGenerated]
		private sealed class <TryGiveJob>c__AnonStorey0
		{
			internal Pawn pawn;

			internal Predicate<IntVec3> cellValidator;

			public <TryGiveJob>c__AnonStorey0()
			{
			}

			internal bool <>m__0(IntVec3 x)
			{
				return !PawnUtility.KnownDangerAt(x, this.pawn.Map, this.pawn) && !x.GetTerrain(this.pawn.Map).avoidWander && x.Standable(this.pawn.Map) && !x.Roofed(this.pawn.Map);
			}

			internal bool <>m__1(Region x)
			{
				IntVec3 intVec;
				return x.Room.PsychologicallyOutdoors && !x.IsForbiddenEntirely(this.pawn) && x.TryFindRandomCellInRegionUnforbidden(this.pawn, this.cellValidator, out intVec);
			}
		}
	}
}
