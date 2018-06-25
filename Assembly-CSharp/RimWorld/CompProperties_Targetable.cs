﻿using System;

namespace RimWorld
{
	// Token: 0x02000256 RID: 598
	public class CompProperties_Targetable : CompProperties_UseEffect
	{
		// Token: 0x040004B1 RID: 1201
		public bool psychicSensitiveTargetsOnly;

		// Token: 0x040004B2 RID: 1202
		public bool fleshCorpsesOnly;

		// Token: 0x040004B3 RID: 1203
		public bool nonDessicatedCorpsesOnly;

		// Token: 0x06000A95 RID: 2709 RVA: 0x0005FF1C File Offset: 0x0005E31C
		public CompProperties_Targetable()
		{
			this.compClass = typeof(CompTargetable);
		}
	}
}
