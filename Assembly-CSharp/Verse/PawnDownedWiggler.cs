﻿using System;
using RimWorld;

namespace Verse
{
	// Token: 0x02000CEE RID: 3310
	public class PawnDownedWiggler
	{
		// Token: 0x04003157 RID: 12631
		private Pawn pawn;

		// Token: 0x04003158 RID: 12632
		public float downedAngle = PawnDownedWiggler.RandomDownedAngle;

		// Token: 0x04003159 RID: 12633
		public int ticksToIncapIcon = 0;

		// Token: 0x0400315A RID: 12634
		private bool usingCustomRotation = false;

		// Token: 0x0400315B RID: 12635
		private const float DownedAngleWidth = 45f;

		// Token: 0x0400315C RID: 12636
		private const float DamageTakenDownedAngleShift = 10f;

		// Token: 0x0400315D RID: 12637
		private const int IncapWigglePeriod = 300;

		// Token: 0x0400315E RID: 12638
		private const int IncapWiggleLength = 90;

		// Token: 0x0400315F RID: 12639
		private const float IncapWiggleSpeed = 0.35f;

		// Token: 0x04003160 RID: 12640
		private const int TicksBetweenIncapIcons = 200;

		// Token: 0x060048E2 RID: 18658 RVA: 0x00263D6A File Offset: 0x0026216A
		public PawnDownedWiggler(Pawn pawn)
		{
			this.pawn = pawn;
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x060048E3 RID: 18659 RVA: 0x00263D94 File Offset: 0x00262194
		private static float RandomDownedAngle
		{
			get
			{
				float num = Rand.Range(45f, 135f);
				if (Rand.Value < 0.5f)
				{
					num += 180f;
				}
				return num;
			}
		}

		// Token: 0x060048E4 RID: 18660 RVA: 0x00263DD4 File Offset: 0x002621D4
		public void WigglerTick()
		{
			if (this.pawn.Downed && this.pawn.Spawned && !this.pawn.InBed())
			{
				this.ticksToIncapIcon--;
				if (this.ticksToIncapIcon <= 0)
				{
					MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOf.Mote_IncapIcon);
					this.ticksToIncapIcon = 200;
				}
				if (this.pawn.Awake())
				{
					int num = Find.TickManager.TicksGame % 300 * 2;
					if (num < 90)
					{
						this.downedAngle += 0.35f;
					}
					else if (num < 390 && num >= 300)
					{
						this.downedAngle -= 0.35f;
					}
				}
			}
		}

		// Token: 0x060048E5 RID: 18661 RVA: 0x00263EC8 File Offset: 0x002622C8
		public void SetToCustomRotation(float rot)
		{
			this.downedAngle = rot;
			this.usingCustomRotation = true;
		}

		// Token: 0x060048E6 RID: 18662 RVA: 0x00263EDC File Offset: 0x002622DC
		public void Notify_DamageApplied(DamageInfo dam)
		{
			if ((this.pawn.Downed || this.pawn.Dead) && dam.Def.hasForcefulImpact)
			{
				this.downedAngle += 10f * Rand.Range(-1f, 1f);
				if (!this.usingCustomRotation)
				{
					if (this.downedAngle > 315f)
					{
						this.downedAngle = 315f;
					}
					if (this.downedAngle < 45f)
					{
						this.downedAngle = 45f;
					}
					if (this.downedAngle > 135f && this.downedAngle < 225f)
					{
						if (this.downedAngle > 180f)
						{
							this.downedAngle = 225f;
						}
						else
						{
							this.downedAngle = 135f;
						}
					}
				}
				else
				{
					if (this.downedAngle >= 360f)
					{
						this.downedAngle -= 360f;
					}
					if (this.downedAngle < 0f)
					{
						this.downedAngle += 360f;
					}
				}
			}
		}
	}
}
