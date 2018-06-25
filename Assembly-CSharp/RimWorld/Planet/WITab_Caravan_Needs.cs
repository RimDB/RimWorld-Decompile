﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet
{
	public class WITab_Caravan_Needs : WITab
	{
		private Vector2 scrollPosition;

		private float scrollViewHeight;

		private Pawn specificNeedsTabForPawn;

		private Vector2 thoughtScrollPosition;

		private bool doNeeds;

		public WITab_Caravan_Needs()
		{
			this.labelKey = "TabCaravanNeeds";
		}

		private float SpecificNeedsTabWidth
		{
			get
			{
				float result;
				if (this.specificNeedsTabForPawn == null || this.specificNeedsTabForPawn.Destroyed)
				{
					result = 0f;
				}
				else
				{
					result = NeedsCardUtility.GetSize(this.specificNeedsTabForPawn).x;
				}
				return result;
			}
		}

		protected override void FillTab()
		{
			CaravanNeedsTabUtility.DoRows(this.size, base.SelCaravan.PawnsListForReading, base.SelCaravan, ref this.scrollPosition, ref this.scrollViewHeight, ref this.specificNeedsTabForPawn, this.doNeeds);
		}

		protected override void UpdateSize()
		{
			base.UpdateSize();
			this.size = CaravanNeedsTabUtility.GetSize(base.SelCaravan.PawnsListForReading, this.PaneTopY, true);
			if (this.size.x + this.SpecificNeedsTabWidth > (float)UI.screenWidth)
			{
				this.doNeeds = false;
				this.size = CaravanNeedsTabUtility.GetSize(base.SelCaravan.PawnsListForReading, this.PaneTopY, false);
			}
			else
			{
				this.doNeeds = true;
			}
			this.size.y = Mathf.Max(this.size.y, NeedsCardUtility.FullSize.y);
		}

		protected override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			Pawn localSpecificNeedsTabForPawn = this.specificNeedsTabForPawn;
			if (localSpecificNeedsTabForPawn != null)
			{
				Rect tabRect = base.TabRect;
				float specificNeedsTabWidth = this.SpecificNeedsTabWidth;
				Rect rect = new Rect(tabRect.xMax - 1f, tabRect.yMin, specificNeedsTabWidth, tabRect.height);
				Find.WindowStack.ImmediateWindow(1439870015, rect, WindowLayer.GameUI, delegate
				{
					if (!localSpecificNeedsTabForPawn.DestroyedOrNull())
					{
						NeedsCardUtility.DoNeedsMoodAndThoughts(rect.AtZero(), localSpecificNeedsTabForPawn, ref this.thoughtScrollPosition);
						if (Widgets.CloseButtonFor(rect.AtZero()))
						{
							this.specificNeedsTabForPawn = null;
							SoundDefOf.TabClose.PlayOneShotOnCamera(null);
						}
					}
				}, true, false, 1f);
			}
		}

		[CompilerGenerated]
		private sealed class <ExtraOnGUI>c__AnonStorey0
		{
			internal Pawn localSpecificNeedsTabForPawn;

			internal WITab_Caravan_Needs $this;

			public <ExtraOnGUI>c__AnonStorey0()
			{
			}
		}

		[CompilerGenerated]
		private sealed class <ExtraOnGUI>c__AnonStorey1
		{
			internal Rect rect;

			internal WITab_Caravan_Needs.<ExtraOnGUI>c__AnonStorey0 <>f__ref$0;

			public <ExtraOnGUI>c__AnonStorey1()
			{
			}

			internal void <>m__0()
			{
				if (!this.<>f__ref$0.localSpecificNeedsTabForPawn.DestroyedOrNull())
				{
					NeedsCardUtility.DoNeedsMoodAndThoughts(this.rect.AtZero(), this.<>f__ref$0.localSpecificNeedsTabForPawn, ref this.<>f__ref$0.$this.thoughtScrollPosition);
					if (Widgets.CloseButtonFor(this.rect.AtZero()))
					{
						this.<>f__ref$0.$this.specificNeedsTabForPawn = null;
						SoundDefOf.TabClose.PlayOneShotOnCamera(null);
					}
				}
			}
		}
	}
}
