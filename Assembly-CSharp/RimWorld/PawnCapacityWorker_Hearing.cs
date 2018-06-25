﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000475 RID: 1141
	public class PawnCapacityWorker_Hearing : PawnCapacityWorker
	{
		// Token: 0x0600140C RID: 5132 RVA: 0x000AEC50 File Offset: 0x000AD050
		public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
		{
			BodyPartTagDef hearingSource = BodyPartTagDefOf.HearingSource;
			return PawnCapacityUtility.CalculateTagEfficiency(diffSet, hearingSource, float.MaxValue, default(FloatRange), impactors);
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x000AEC88 File Offset: 0x000AD088
		public override bool CanHaveCapacity(BodyDef body)
		{
			return body.HasPartWithTag(BodyPartTagDefOf.HearingSource);
		}
	}
}
