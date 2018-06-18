﻿using System;

namespace Verse.AI
{
	// Token: 0x02000A56 RID: 2646
	public class QueuedJob : IExposable
	{
		// Token: 0x06003AE1 RID: 15073 RVA: 0x001F3EC0 File Offset: 0x001F22C0
		public QueuedJob()
		{
		}

		// Token: 0x06003AE2 RID: 15074 RVA: 0x001F3EC9 File Offset: 0x001F22C9
		public QueuedJob(Job job, JobTag? tag)
		{
			this.job = job;
			this.tag = tag;
		}

		// Token: 0x06003AE3 RID: 15075 RVA: 0x001F3EE0 File Offset: 0x001F22E0
		public void ExposeData()
		{
			Scribe_Deep.Look<Job>(ref this.job, "job", new object[0]);
			Scribe_Values.Look<JobTag?>(ref this.tag, "tag", null, false);
		}

		// Token: 0x04002542 RID: 9538
		public Job job;

		// Token: 0x04002543 RID: 9539
		public JobTag? tag;
	}
}