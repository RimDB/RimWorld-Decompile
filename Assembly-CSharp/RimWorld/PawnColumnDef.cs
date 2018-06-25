﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020002B4 RID: 692
	public class PawnColumnDef : Def
	{
		// Token: 0x0400069B RID: 1691
		public Type workerClass = typeof(PawnColumnWorker);

		// Token: 0x0400069C RID: 1692
		public bool sortable;

		// Token: 0x0400069D RID: 1693
		public bool ignoreWhenCalculatingOptimalTableSize;

		// Token: 0x0400069E RID: 1694
		[NoTranslate]
		public string headerIcon;

		// Token: 0x0400069F RID: 1695
		public Vector2 headerIconSize;

		// Token: 0x040006A0 RID: 1696
		[MustTranslate]
		public string headerTip;

		// Token: 0x040006A1 RID: 1697
		public bool headerAlwaysInteractable;

		// Token: 0x040006A2 RID: 1698
		public bool paintable;

		// Token: 0x040006A3 RID: 1699
		public TrainableDef trainable;

		// Token: 0x040006A4 RID: 1700
		public int gap;

		// Token: 0x040006A5 RID: 1701
		public WorkTypeDef workType;

		// Token: 0x040006A6 RID: 1702
		public bool moveWorkTypeLabelDown;

		// Token: 0x040006A7 RID: 1703
		public int widthPriority;

		// Token: 0x040006A8 RID: 1704
		[Unsaved]
		private PawnColumnWorker workerInt;

		// Token: 0x040006A9 RID: 1705
		[Unsaved]
		private Texture2D headerIconTex;

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000B8B RID: 2955 RVA: 0x000684A4 File Offset: 0x000668A4
		public PawnColumnWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (PawnColumnWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000B8C RID: 2956 RVA: 0x000684F0 File Offset: 0x000668F0
		public Texture2D HeaderIcon
		{
			get
			{
				if (this.headerIconTex == null && !this.headerIcon.NullOrEmpty())
				{
					this.headerIconTex = ContentFinder<Texture2D>.Get(this.headerIcon, true);
				}
				return this.headerIconTex;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000B8D RID: 2957 RVA: 0x00068540 File Offset: 0x00066940
		public Vector2 HeaderIconSize
		{
			get
			{
				Vector2 result;
				if (this.headerIconSize != default(Vector2))
				{
					result = this.headerIconSize;
				}
				else
				{
					Texture2D texture2D = this.HeaderIcon;
					if (texture2D != null)
					{
						result = new Vector2((float)texture2D.width, (float)texture2D.height);
					}
					else
					{
						result = Vector2.zero;
					}
				}
				return result;
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x000685AC File Offset: 0x000669AC
		public bool HeaderInteractable
		{
			get
			{
				return this.sortable || !this.headerTip.NullOrEmpty() || this.headerAlwaysInteractable;
			}
		}
	}
}
