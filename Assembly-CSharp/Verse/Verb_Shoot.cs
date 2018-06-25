﻿using System;
using RimWorld;

namespace Verse
{
	// Token: 0x02000FD9 RID: 4057
	public class Verb_Shoot : Verb_LaunchProjectile
	{
		// Token: 0x17000FEB RID: 4075
		// (get) Token: 0x06006242 RID: 25154 RVA: 0x001E35BC File Offset: 0x001E19BC
		protected override int ShotsPerBurst
		{
			get
			{
				return this.verbProps.burstShotCount;
			}
		}

		// Token: 0x06006243 RID: 25155 RVA: 0x001E35DC File Offset: 0x001E19DC
		public override void WarmupComplete()
		{
			base.WarmupComplete();
			Pawn pawn = this.currentTarget.Thing as Pawn;
			if (pawn != null && !pawn.Downed && base.CasterIsPawn && base.CasterPawn.skills != null)
			{
				float num = (!pawn.HostileTo(this.caster)) ? 20f : 170f;
				float num2 = this.verbProps.AdjustedFullCycleTime(this, base.CasterPawn, this.ownerEquipment);
				base.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
			}
		}

		// Token: 0x06006244 RID: 25156 RVA: 0x001E3684 File Offset: 0x001E1A84
		protected override bool TryCastShot()
		{
			bool flag = base.TryCastShot();
			if (flag && base.CasterIsPawn)
			{
				base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
			}
			return flag;
		}
	}
}
