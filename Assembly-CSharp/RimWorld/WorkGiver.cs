﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public abstract class WorkGiver
	{
		public WorkGiverDef def;

		protected WorkGiver()
		{
		}

		public virtual bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return false;
		}

		public virtual Job NonScanJob(Pawn pawn)
		{
			return null;
		}

		public PawnCapacityDef MissingRequiredCapacity(Pawn pawn)
		{
			for (int i = 0; i < this.def.requiredCapacities.Count; i++)
			{
				if (!pawn.health.capacities.CapableOf(this.def.requiredCapacities[i]))
				{
					return this.def.requiredCapacities[i];
				}
			}
			return null;
		}
	}
}
