﻿using System;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005C9 RID: 1481
	public abstract class CaravanArrivalAction : IExposable
	{
		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06001CBE RID: 7358
		public abstract string Label { get; }

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06001CBF RID: 7359
		public abstract string ReportString { get; }

		// Token: 0x06001CC0 RID: 7360 RVA: 0x000F6DF4 File Offset: 0x000F51F4
		public virtual FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
		{
			return true;
		}

		// Token: 0x06001CC1 RID: 7361
		public abstract void Arrived(Caravan caravan);

		// Token: 0x06001CC2 RID: 7362 RVA: 0x000F6E0F File Offset: 0x000F520F
		public virtual void ExposeData()
		{
		}
	}
}
