﻿using System;

namespace Verse.Sound
{
	// Token: 0x02000DC3 RID: 3523
	public static class SustainerAggregatorUtility
	{
		// Token: 0x04003463 RID: 13411
		private static float AggregateRadius = 12f;

		// Token: 0x06004EAF RID: 20143 RVA: 0x00291C2C File Offset: 0x0029002C
		public static Sustainer AggregateOrSpawnSustainerFor(ISizeReporter reporter, SoundDef def, SoundInfo info)
		{
			Sustainer sustainer = null;
			foreach (Sustainer sustainer2 in Find.SoundRoot.sustainerManager.AllSustainers)
			{
				if (sustainer2.def == def && sustainer2.info.Maker.Map == info.Maker.Map && sustainer2.info.Maker.Cell.InHorDistOf(info.Maker.Cell, SustainerAggregatorUtility.AggregateRadius))
				{
					sustainer = sustainer2;
					break;
				}
			}
			if (sustainer == null)
			{
				sustainer = def.TrySpawnSustainer(info);
			}
			else
			{
				sustainer.Maintain();
			}
			if (sustainer.externalParams.sizeAggregator == null)
			{
				sustainer.externalParams.sizeAggregator = new SoundSizeAggregator();
			}
			sustainer.externalParams.sizeAggregator.RegisterReporter(reporter);
			return sustainer;
		}
	}
}
