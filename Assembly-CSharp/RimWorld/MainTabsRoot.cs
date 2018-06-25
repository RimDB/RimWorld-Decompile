﻿using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x02000879 RID: 2169
	public class MainTabsRoot
	{
		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x0600317B RID: 12667 RVA: 0x001AD878 File Offset: 0x001ABC78
		public MainButtonDef OpenTab
		{
			get
			{
				MainTabWindow mainTabWindow = Find.WindowStack.WindowOfType<MainTabWindow>();
				MainButtonDef result;
				if (mainTabWindow == null)
				{
					result = null;
				}
				else
				{
					result = mainTabWindow.def;
				}
				return result;
			}
		}

		// Token: 0x0600317C RID: 12668 RVA: 0x001AD8AC File Offset: 0x001ABCAC
		public void HandleLowPriorityShortcuts()
		{
			if (this.OpenTab == MainButtonDefOf.Inspect && (Find.Selector.NumSelected == 0 || WorldRendererUtility.WorldRenderedNow))
			{
				this.EscapeCurrentTab(false);
			}
			if (Find.Selector.NumSelected == 0 && Event.current.type == EventType.MouseDown && Event.current.button == 1 && !WorldRendererUtility.WorldRenderedNow)
			{
				Event.current.Use();
				MainButtonDefOf.Architect.Worker.InterfaceTryActivate();
			}
			if (this.OpenTab != null && this.OpenTab != MainButtonDefOf.Inspect)
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button != 2)
				{
					this.EscapeCurrentTab(true);
					if (Event.current.button == 0)
					{
						Find.Selector.ClearSelection();
						Find.WorldSelector.ClearSelection();
					}
				}
			}
		}

		// Token: 0x0600317D RID: 12669 RVA: 0x001AD9A7 File Offset: 0x001ABDA7
		public void EscapeCurrentTab(bool playSound = true)
		{
			this.SetCurrentTab(null, playSound);
		}

		// Token: 0x0600317E RID: 12670 RVA: 0x001AD9B2 File Offset: 0x001ABDB2
		public void SetCurrentTab(MainButtonDef tab, bool playSound = true)
		{
			if (tab != this.OpenTab)
			{
				this.ToggleTab(tab, playSound);
			}
		}

		// Token: 0x0600317F RID: 12671 RVA: 0x001AD9D0 File Offset: 0x001ABDD0
		public void ToggleTab(MainButtonDef newTab, bool playSound = true)
		{
			if (this.OpenTab != null || newTab != null)
			{
				if (this.OpenTab == newTab)
				{
					Find.WindowStack.TryRemove(this.OpenTab.TabWindow, true);
					if (playSound)
					{
						SoundDefOf.TabClose.PlayOneShotOnCamera(null);
					}
				}
				else
				{
					if (this.OpenTab != null)
					{
						Find.WindowStack.TryRemove(this.OpenTab.TabWindow, true);
					}
					if (newTab != null)
					{
						Find.WindowStack.Add(newTab.TabWindow);
					}
					if (playSound)
					{
						if (newTab == null)
						{
							SoundDefOf.TabClose.PlayOneShotOnCamera(null);
						}
						else
						{
							SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
						}
					}
					if (TutorSystem.TutorialMode && newTab != null)
					{
						TutorSystem.Notify_Event("Open-MainTab-" + newTab.defName);
					}
				}
			}
		}
	}
}
