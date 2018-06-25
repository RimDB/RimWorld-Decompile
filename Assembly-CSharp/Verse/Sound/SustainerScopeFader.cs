﻿using System;

namespace Verse.Sound
{
	// Token: 0x02000DC5 RID: 3525
	public class SustainerScopeFader
	{
		// Token: 0x04003467 RID: 13415
		public bool inScope = true;

		// Token: 0x04003468 RID: 13416
		public float inScopePercent = 1f;

		// Token: 0x04003469 RID: 13417
		private const float ScopeMatchFallRate = 0.03f;

		// Token: 0x0400346A RID: 13418
		private const float ScopeMatchRiseRate = 0.05f;

		// Token: 0x06004EBC RID: 20156 RVA: 0x002921E0 File Offset: 0x002905E0
		public void SustainerScopeUpdate()
		{
			if (this.inScope)
			{
				float num = this.inScopePercent + 0.05f;
				this.inScopePercent = num;
				if (this.inScopePercent > 1f)
				{
					this.inScopePercent = 1f;
				}
			}
			else
			{
				this.inScopePercent -= 0.03f;
				if (this.inScopePercent <= 0.001f)
				{
					this.inScopePercent = 0f;
				}
			}
		}
	}
}
