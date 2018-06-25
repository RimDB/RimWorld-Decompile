﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000448 RID: 1096
	public class TemperatureMemory : IExposable
	{
		// Token: 0x04000B8E RID: 2958
		private Map map;

		// Token: 0x04000B8F RID: 2959
		private int growthSeasonUntilTick = -1;

		// Token: 0x04000B90 RID: 2960
		private int noSowUntilTick = -1;

		// Token: 0x04000B91 RID: 2961
		private const int TicksBuffer = 30000;

		// Token: 0x06001302 RID: 4866 RVA: 0x000A41DB File Offset: 0x000A25DB
		public TemperatureMemory(Map map)
		{
			this.map = map;
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06001303 RID: 4867 RVA: 0x000A41FC File Offset: 0x000A25FC
		public bool GrowthSeasonOutdoorsNow
		{
			get
			{
				return Find.TickManager.TicksGame < this.growthSeasonUntilTick;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06001304 RID: 4868 RVA: 0x000A4224 File Offset: 0x000A2624
		public bool GrowthSeasonOutdoorsNowForSowing
		{
			get
			{
				return (this.noSowUntilTick <= 0 || Find.TickManager.TicksGame >= this.noSowUntilTick) && this.GrowthSeasonOutdoorsNow;
			}
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x000A4268 File Offset: 0x000A2668
		public void GrowthSeasonMemoryTick()
		{
			if (this.map.mapTemperature.OutdoorTemp > 0f && this.map.mapTemperature.OutdoorTemp < 58f)
			{
				this.growthSeasonUntilTick = Find.TickManager.TicksGame + 30000;
			}
			else if (this.map.mapTemperature.OutdoorTemp < -2f)
			{
				this.growthSeasonUntilTick = -1;
				this.noSowUntilTick = Find.TickManager.TicksGame + 30000;
			}
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x000A4300 File Offset: 0x000A2700
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.growthSeasonUntilTick, "growthSeasonUntilTick", 0, true);
			Scribe_Values.Look<int>(ref this.noSowUntilTick, "noSowUntilTick", 0, true);
		}
	}
}
