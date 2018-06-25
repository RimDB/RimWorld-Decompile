﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020002DA RID: 730
	public class PawnCapacityOffset
	{
		// Token: 0x04000765 RID: 1893
		public PawnCapacityDef capacity;

		// Token: 0x04000766 RID: 1894
		public float scale = 1f;

		// Token: 0x04000767 RID: 1895
		public float max = 9999f;

		// Token: 0x06000C0E RID: 3086 RVA: 0x0006B07C File Offset: 0x0006947C
		public float GetOffset(float capacityEfficiency)
		{
			return (Mathf.Min(capacityEfficiency, this.max) - 1f) * this.scale;
		}
	}
}
