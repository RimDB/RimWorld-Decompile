﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020002AE RID: 686
	public class MainButtonDef : Def
	{
		// Token: 0x04000674 RID: 1652
		public Type workerClass = typeof(MainButtonWorker_ToggleTab);

		// Token: 0x04000675 RID: 1653
		public Type tabWindowClass;

		// Token: 0x04000676 RID: 1654
		public bool buttonVisible = true;

		// Token: 0x04000677 RID: 1655
		public int order;

		// Token: 0x04000678 RID: 1656
		public KeyCode defaultHotKey = KeyCode.None;

		// Token: 0x04000679 RID: 1657
		public bool canBeTutorDenied = true;

		// Token: 0x0400067A RID: 1658
		public bool validWithoutMap;

		// Token: 0x0400067B RID: 1659
		[Unsaved]
		public KeyBindingDef hotKey;

		// Token: 0x0400067C RID: 1660
		[Unsaved]
		public string cachedTutorTag;

		// Token: 0x0400067D RID: 1661
		[Unsaved]
		public string cachedHighlightTagClosed;

		// Token: 0x0400067E RID: 1662
		[Unsaved]
		private MainButtonWorker workerInt;

		// Token: 0x0400067F RID: 1663
		[Unsaved]
		private MainTabWindow tabWindowInt;

		// Token: 0x04000680 RID: 1664
		[Unsaved]
		private string cachedShortenedLabelCap;

		// Token: 0x04000681 RID: 1665
		[Unsaved]
		private float cachedLabelCapWidth = -1f;

		// Token: 0x04000682 RID: 1666
		[Unsaved]
		private float cachedShortenedLabelCapWidth = -1f;

		// Token: 0x04000683 RID: 1667
		public const int ButtonHeight = 35;

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000B74 RID: 2932 RVA: 0x000673D4 File Offset: 0x000657D4
		public MainButtonWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (MainButtonWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000B75 RID: 2933 RVA: 0x00067420 File Offset: 0x00065820
		public MainTabWindow TabWindow
		{
			get
			{
				if (this.tabWindowInt == null && this.tabWindowClass != null)
				{
					this.tabWindowInt = (MainTabWindow)Activator.CreateInstance(this.tabWindowClass);
					this.tabWindowInt.def = this;
				}
				return this.tabWindowInt;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x00067478 File Offset: 0x00065878
		public string ShortenedLabelCap
		{
			get
			{
				if (this.cachedShortenedLabelCap == null)
				{
					this.cachedShortenedLabelCap = base.LabelCap.Shorten();
				}
				return this.cachedShortenedLabelCap;
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x000674B0 File Offset: 0x000658B0
		public float LabelCapWidth
		{
			get
			{
				if (this.cachedLabelCapWidth < 0f)
				{
					GameFont font = Text.Font;
					Text.Font = GameFont.Small;
					this.cachedLabelCapWidth = Text.CalcSize(base.LabelCap).x;
					Text.Font = font;
				}
				return this.cachedLabelCapWidth;
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00067508 File Offset: 0x00065908
		public float ShortenedLabelCapWidth
		{
			get
			{
				if (this.cachedShortenedLabelCapWidth < 0f)
				{
					GameFont font = Text.Font;
					Text.Font = GameFont.Small;
					this.cachedShortenedLabelCapWidth = Text.CalcSize(this.ShortenedLabelCap).x;
					Text.Font = font;
				}
				return this.cachedShortenedLabelCapWidth;
			}
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x00067560 File Offset: 0x00065960
		public override void PostLoad()
		{
			base.PostLoad();
			this.cachedHighlightTagClosed = "MainTab-" + this.defName + "-Closed";
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x00067584 File Offset: 0x00065984
		public void Notify_SwitchedMap()
		{
			if (this.tabWindowInt != null)
			{
				Find.WindowStack.TryRemove(this.tabWindowInt, true);
				this.tabWindowInt = null;
			}
		}
	}
}
