﻿using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007CA RID: 1994
	public class Designator_Deconstruct : Designator
	{
		// Token: 0x06002C23 RID: 11299 RVA: 0x001754E4 File Offset: 0x001738E4
		public Designator_Deconstruct()
		{
			this.defaultLabel = "DesignatorDeconstruct".Translate();
			this.defaultDesc = "DesignatorDeconstructDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct", true);
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.Designate_Deconstruct;
			this.hotKey = KeyBindingDefOf.Designator_Deconstruct;
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06002C24 RID: 11300 RVA: 0x0017555C File Offset: 0x0017395C
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06002C25 RID: 11301 RVA: 0x00175574 File Offset: 0x00173974
		protected override DesignationDef Designation
		{
			get
			{
				return DesignationDefOf.Deconstruct;
			}
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x00175590 File Offset: 0x00173990
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			AcceptanceReport result;
			if (!c.InBounds(base.Map))
			{
				result = false;
			}
			else if (!DebugSettings.godMode && c.Fogged(base.Map))
			{
				result = false;
			}
			else if (this.TopDeconstructibleInCell(c) == null)
			{
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x00175609 File Offset: 0x00173A09
		public override void DesignateSingleCell(IntVec3 loc)
		{
			this.DesignateThing(this.TopDeconstructibleInCell(loc));
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x0017561C File Offset: 0x00173A1C
		private Thing TopDeconstructibleInCell(IntVec3 loc)
		{
			foreach (Thing thing in from t in base.Map.thingGrid.ThingsAt(loc)
			orderby t.def.altitudeLayer descending
			select t)
			{
				if (this.CanDesignateThing(thing).Accepted)
				{
					return thing;
				}
			}
			return null;
		}

		// Token: 0x06002C29 RID: 11305 RVA: 0x001756C4 File Offset: 0x00173AC4
		public override void DesignateThing(Thing t)
		{
			Thing innerIfMinified = t.GetInnerIfMinified();
			if (DebugSettings.godMode || innerIfMinified.GetStatValue(StatDefOf.WorkToBuild, true) == 0f || t.def.IsFrame)
			{
				t.Destroy(DestroyMode.Deconstruct);
			}
			else
			{
				base.Map.designationManager.AddDesignation(new Designation(t, this.Designation));
			}
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x0017573C File Offset: 0x00173B3C
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building building = t.GetInnerIfMinified() as Building;
			AcceptanceReport result;
			if (building == null)
			{
				result = false;
			}
			else if (building.def.category != ThingCategory.Building)
			{
				result = false;
			}
			else if (!building.DeconstructibleBy(Faction.OfPlayer))
			{
				result = false;
			}
			else if (base.Map.designationManager.DesignationOn(t, this.Designation) != null)
			{
				result = false;
			}
			else if (base.Map.designationManager.DesignationOn(t, DesignationDefOf.Uninstall) != null)
			{
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x001757FD File Offset: 0x00173BFD
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}
	}
}
