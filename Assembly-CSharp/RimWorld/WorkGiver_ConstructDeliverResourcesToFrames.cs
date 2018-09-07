﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_ConstructDeliverResourcesToFrames : WorkGiver_ConstructDeliverResources
	{
		public WorkGiver_ConstructDeliverResourcesToFrames()
		{
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.BuildingFrame);
			}
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t.Faction != pawn.Faction)
			{
				return null;
			}
			Frame frame = t as Frame;
			if (frame == null)
			{
				return null;
			}
			if (GenConstruct.FirstBlockingThing(frame, pawn) != null)
			{
				return GenConstruct.HandleBlockingThingJob(frame, pawn, forced);
			}
			bool checkConstructionSkill = this.def.workType == WorkTypeDefOf.Construction;
			if (!GenConstruct.CanConstruct(frame, pawn, checkConstructionSkill, forced))
			{
				return null;
			}
			return base.ResourceDeliverJobFor(pawn, frame, true);
		}
	}
}
