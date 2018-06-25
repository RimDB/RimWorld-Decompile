﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000998 RID: 2456
	public class SpecialThingFilterWorker_Rotten : SpecialThingFilterWorker
	{
		// Token: 0x06003728 RID: 14120 RVA: 0x001D86C4 File Offset: 0x001D6AC4
		public override bool Matches(Thing t)
		{
			CompRottable compRottable = t.TryGetComp<CompRottable>();
			return compRottable != null && !compRottable.PropsRot.rotDestroys && compRottable.Stage != RotStage.Fresh;
		}

		// Token: 0x06003729 RID: 14121 RVA: 0x001D870C File Offset: 0x001D6B0C
		public override bool CanEverMatch(ThingDef def)
		{
			CompProperties_Rottable compProperties = def.GetCompProperties<CompProperties_Rottable>();
			return compProperties != null && !compProperties.rotDestroys;
		}
	}
}
