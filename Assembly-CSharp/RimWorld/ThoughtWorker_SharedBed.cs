﻿using System;
using Verse;

namespace RimWorld
{
	public class ThoughtWorker_SharedBed : ThoughtWorker
	{
		public ThoughtWorker_SharedBed()
		{
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			return LovePartnerRelationUtility.GetMostDislikedNonPartnerBedOwner(p) != null;
		}
	}
}
