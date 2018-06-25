﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000472 RID: 1138
	public class PawnCapacityWorker_Breathing : PawnCapacityWorker
	{
		// Token: 0x06001403 RID: 5123 RVA: 0x000AE9E4 File Offset: 0x000ACDE4
		public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
		{
			BodyPartTagDef tag = BodyPartTagDefOf.BreathingSource;
			float num = PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, float.MaxValue, default(FloatRange), impactors);
			tag = BodyPartTagDefOf.BreathingPathway;
			float maximum = 1f;
			float num2 = num * PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, maximum, default(FloatRange), impactors);
			tag = BodyPartTagDefOf.BreathingSourceCage;
			maximum = 1f;
			return num2 * PawnCapacityUtility.CalculateTagEfficiency(diffSet, tag, maximum, default(FloatRange), impactors);
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x000AEA68 File Offset: 0x000ACE68
		public override bool CanHaveCapacity(BodyDef body)
		{
			return body.HasPartWithTag(BodyPartTagDefOf.BreathingSource);
		}
	}
}
