﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007E9 RID: 2025
	public class Designator_ZoneAddStockpile_Resources : Designator_ZoneAddStockpile
	{
		// Token: 0x06002D06 RID: 11526 RVA: 0x0017AC94 File Offset: 0x00179094
		public Designator_ZoneAddStockpile_Resources()
		{
			this.preset = StorageSettingsPreset.DefaultStockpile;
			this.defaultLabel = this.preset.PresetName();
			this.defaultDesc = "DesignatorZoneCreateStorageResourcesDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Stockpile", true);
			this.hotKey = KeyBindingDefOf.Misc1;
			this.tutorTag = "ZoneAddStockpile_Resources";
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x0017ACF7 File Offset: 0x001790F7
		protected override void FinalizeDesignationSucceeded()
		{
			base.FinalizeDesignationSucceeded();
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.StorageTab, OpportunityType.GoodToKnow);
		}
	}
}
