﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x0200099C RID: 2460
	public class SpecialThingFilterWorker_NonSmeltableWeapons : SpecialThingFilterWorker
	{
		// Token: 0x06003737 RID: 14135 RVA: 0x001D8918 File Offset: 0x001D6D18
		public override bool Matches(Thing t)
		{
			return this.CanEverMatch(t.def) && !t.Smeltable;
		}

		// Token: 0x06003738 RID: 14136 RVA: 0x001D8950 File Offset: 0x001D6D50
		public override bool CanEverMatch(ThingDef def)
		{
			bool result;
			if (!def.IsWeapon)
			{
				result = false;
			}
			else
			{
				if (!def.thingCategories.NullOrEmpty<ThingCategoryDef>())
				{
					for (int i = 0; i < def.thingCategories.Count; i++)
					{
						for (ThingCategoryDef thingCategoryDef = def.thingCategories[i]; thingCategoryDef != null; thingCategoryDef = thingCategoryDef.parent)
						{
							if (thingCategoryDef == ThingCategoryDefOf.Weapons)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06003739 RID: 14137 RVA: 0x001D89DC File Offset: 0x001D6DDC
		public override bool AlwaysMatches(ThingDef def)
		{
			return this.CanEverMatch(def) && !def.smeltable && !def.MadeFromStuff;
		}
	}
}
