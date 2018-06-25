﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020004F7 RID: 1271
	public class Need_Comfort : Need_Seeker
	{
		// Token: 0x04000D61 RID: 3425
		public float lastComfortUsed;

		// Token: 0x04000D62 RID: 3426
		public int lastComfortUseTick;

		// Token: 0x04000D63 RID: 3427
		private const float MinNormal = 0.1f;

		// Token: 0x04000D64 RID: 3428
		private const float MinComfortable = 0.6f;

		// Token: 0x04000D65 RID: 3429
		private const float MinVeryComfortable = 0.7f;

		// Token: 0x04000D66 RID: 3430
		private const float MinExtremelyComfortablee = 0.8f;

		// Token: 0x04000D67 RID: 3431
		private const float MinLuxuriantlyComfortable = 0.9f;

		// Token: 0x04000D68 RID: 3432
		public const int ComfortUseInterval = 10;

		// Token: 0x060016DA RID: 5850 RVA: 0x000CA310 File Offset: 0x000C8710
		public Need_Comfort(Pawn pawn) : base(pawn)
		{
			this.threshPercents = new List<float>();
			this.threshPercents.Add(0.1f);
			this.threshPercents.Add(0.6f);
			this.threshPercents.Add(0.7f);
			this.threshPercents.Add(0.8f);
			this.threshPercents.Add(0.9f);
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060016DB RID: 5851 RVA: 0x000CA380 File Offset: 0x000C8780
		public override float CurInstantLevel
		{
			get
			{
				float result;
				if (!this.pawn.Spawned)
				{
					result = 0.5f;
				}
				else if (this.lastComfortUseTick > Find.TickManager.TicksGame - 10)
				{
					result = Mathf.Clamp01(this.lastComfortUsed);
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060016DC RID: 5852 RVA: 0x000CA3E0 File Offset: 0x000C87E0
		public ComfortCategory CurCategory
		{
			get
			{
				ComfortCategory result;
				if (this.CurLevel < 0.1f)
				{
					result = ComfortCategory.Uncomfortable;
				}
				else if (this.CurLevel < 0.6f)
				{
					result = ComfortCategory.Normal;
				}
				else if (this.CurLevel < 0.7f)
				{
					result = ComfortCategory.Comfortable;
				}
				else if (this.CurLevel < 0.8f)
				{
					result = ComfortCategory.VeryComfortable;
				}
				else if (this.CurLevel < 0.9f)
				{
					result = ComfortCategory.ExtremelyComfortable;
				}
				else
				{
					result = ComfortCategory.LuxuriantlyComfortable;
				}
				return result;
			}
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x000CA469 File Offset: 0x000C8869
		public void ComfortUsed(float comfort)
		{
			this.lastComfortUsed = comfort;
			this.lastComfortUseTick = Find.TickManager.TicksGame;
		}
	}
}
