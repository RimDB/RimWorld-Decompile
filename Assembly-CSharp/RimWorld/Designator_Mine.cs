﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007D1 RID: 2001
	public class Designator_Mine : Designator
	{
		// Token: 0x06002C5A RID: 11354 RVA: 0x00176560 File Offset: 0x00174960
		public Designator_Mine()
		{
			this.defaultLabel = "DesignatorMine".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Mine", true);
			this.defaultDesc = "DesignatorMineDesc".Translate();
			this.useMouseIcon = true;
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.soundSucceeded = SoundDefOf.Designate_Mine;
			this.hotKey = KeyBindingDefOf.Misc10;
			this.tutorTag = "Mine";
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002C5B RID: 11355 RVA: 0x001765E4 File Offset: 0x001749E4
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06002C5C RID: 11356 RVA: 0x001765FC File Offset: 0x001749FC
		public override bool DragDrawMeasurements
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06002C5D RID: 11357 RVA: 0x00176614 File Offset: 0x00174A14
		protected override DesignationDef Designation
		{
			get
			{
				return DesignationDefOf.Mine;
			}
		}

		// Token: 0x06002C5E RID: 11358 RVA: 0x00176630 File Offset: 0x00174A30
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			AcceptanceReport result;
			if (!c.InBounds(base.Map))
			{
				result = false;
			}
			else if (base.Map.designationManager.DesignationAt(c, this.Designation) != null)
			{
				result = AcceptanceReport.WasRejected;
			}
			else if (c.Fogged(base.Map))
			{
				result = true;
			}
			else
			{
				Mineable firstMineable = c.GetFirstMineable(base.Map);
				if (firstMineable == null)
				{
					result = "MessageMustDesignateMineable".Translate();
				}
				else
				{
					AcceptanceReport acceptanceReport = this.CanDesignateThing(firstMineable);
					if (!acceptanceReport.Accepted)
					{
						result = acceptanceReport;
					}
					else
					{
						result = AcceptanceReport.WasAccepted;
					}
				}
			}
			return result;
		}

		// Token: 0x06002C5F RID: 11359 RVA: 0x001766F0 File Offset: 0x00174AF0
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			AcceptanceReport result;
			if (!t.def.mineable)
			{
				result = false;
			}
			else if (base.Map.designationManager.DesignationAt(t.Position, this.Designation) != null)
			{
				result = AcceptanceReport.WasRejected;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06002C60 RID: 11360 RVA: 0x00176754 File Offset: 0x00174B54
		public override void DesignateSingleCell(IntVec3 loc)
		{
			base.Map.designationManager.AddDesignation(new Designation(loc, this.Designation));
			base.Map.designationManager.TryRemoveDesignation(loc, DesignationDefOf.SmoothWall);
			if (DebugSettings.godMode)
			{
				Mineable firstMineable = loc.GetFirstMineable(base.Map);
				if (firstMineable != null)
				{
					firstMineable.DestroyMined(null);
				}
			}
		}

		// Token: 0x06002C61 RID: 11361 RVA: 0x001767BF File Offset: 0x00174BBF
		public override void DesignateThing(Thing t)
		{
			this.DesignateSingleCell(t.Position);
		}

		// Token: 0x06002C62 RID: 11362 RVA: 0x001767CE File Offset: 0x00174BCE
		protected override void FinalizeDesignationSucceeded()
		{
			base.FinalizeDesignationSucceeded();
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Mining, KnowledgeAmount.SpecificInteraction);
		}

		// Token: 0x06002C63 RID: 11363 RVA: 0x001767E2 File Offset: 0x00174BE2
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}

		// Token: 0x06002C64 RID: 11364 RVA: 0x001767EA File Offset: 0x00174BEA
		public override void RenderHighlight(List<IntVec3> dragCells)
		{
			DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
		}
	}
}
