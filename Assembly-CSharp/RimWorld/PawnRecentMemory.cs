﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000509 RID: 1289
	public class PawnRecentMemory : IExposable
	{
		// Token: 0x04000DB9 RID: 3513
		private Pawn pawn;

		// Token: 0x04000DBA RID: 3514
		private int lastLightTick = 999999;

		// Token: 0x04000DBB RID: 3515
		private int lastOutdoorTick = 999999;

		// Token: 0x06001723 RID: 5923 RVA: 0x000CBDA4 File Offset: 0x000CA1A4
		public PawnRecentMemory(Pawn pawn)
		{
			this.pawn = pawn;
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x000CBDCC File Offset: 0x000CA1CC
		public int TicksSinceLastLight
		{
			get
			{
				return Find.TickManager.TicksGame - this.lastLightTick;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001725 RID: 5925 RVA: 0x000CBDF4 File Offset: 0x000CA1F4
		public int TicksSinceOutdoors
		{
			get
			{
				return Find.TickManager.TicksGame - this.lastOutdoorTick;
			}
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x000CBE1A File Offset: 0x000CA21A
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.lastLightTick, "lastLightTick", 999999, false);
			Scribe_Values.Look<int>(ref this.lastOutdoorTick, "lastOutdoorTick", 999999, false);
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x000CBE4C File Offset: 0x000CA24C
		public void RecentMemoryInterval()
		{
			if (this.pawn.Spawned)
			{
				if (this.pawn.Map.glowGrid.PsychGlowAt(this.pawn.Position) != PsychGlow.Dark)
				{
					this.lastLightTick = Find.TickManager.TicksGame;
				}
				if (this.Outdoors())
				{
					this.lastOutdoorTick = Find.TickManager.TicksGame;
				}
			}
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x000CBEC0 File Offset: 0x000CA2C0
		private bool Outdoors()
		{
			Room room = this.pawn.GetRoom(RegionType.Set_Passable);
			return room != null && room.PsychologicallyOutdoors;
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x000CBEF1 File Offset: 0x000CA2F1
		public void Notify_Spawned(bool respawningAfterLoad)
		{
			this.lastLightTick = Find.TickManager.TicksGame;
			if (!respawningAfterLoad && this.Outdoors())
			{
				this.lastOutdoorTick = Find.TickManager.TicksGame;
			}
		}
	}
}
