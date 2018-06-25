﻿using System;

namespace Verse.AI
{
	// Token: 0x02000AAB RID: 2731
	public abstract class ThinkNode_ChancePerHour : ThinkNode_Priority
	{
		// Token: 0x06003D08 RID: 15624 RVA: 0x00204B04 File Offset: 0x00202F04
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			ThinkResult result;
			if (Find.TickManager.TicksGame < this.GetLastTryTick(pawn) + 2500)
			{
				result = ThinkResult.NoJob;
			}
			else
			{
				this.SetLastTryTick(pawn, Find.TickManager.TicksGame);
				float num = this.MtbHours(pawn);
				if (num <= 0f)
				{
					result = ThinkResult.NoJob;
				}
				else
				{
					Rand.PushState();
					int salt = Gen.HashCombineInt(base.UniqueSaveKey, 26504059);
					Rand.Seed = pawn.RandSeedForHour(salt);
					bool flag = Rand.MTBEventOccurs(num, 2500f, 2500f);
					Rand.PopState();
					if (flag)
					{
						result = base.TryIssueJobPackage(pawn, jobParams);
					}
					else
					{
						result = ThinkResult.NoJob;
					}
				}
			}
			return result;
		}

		// Token: 0x06003D09 RID: 15625
		protected abstract float MtbHours(Pawn pawn);

		// Token: 0x06003D0A RID: 15626 RVA: 0x00204BC0 File Offset: 0x00202FC0
		private int GetLastTryTick(Pawn pawn)
		{
			int num;
			int result;
			if (pawn.mindState.thinkData.TryGetValue(base.UniqueSaveKey, out num))
			{
				result = num;
			}
			else
			{
				result = -99999;
			}
			return result;
		}

		// Token: 0x06003D0B RID: 15627 RVA: 0x00204BFE File Offset: 0x00202FFE
		private void SetLastTryTick(Pawn pawn, int val)
		{
			pawn.mindState.thinkData[base.UniqueSaveKey] = val;
		}
	}
}
