﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse
{
	public class RecipeWorkerCounter_MakeStoneBlocks : RecipeWorkerCounter
	{
		public RecipeWorkerCounter_MakeStoneBlocks()
		{
		}

		public override bool CanCountProducts(Bill_Production bill)
		{
			return true;
		}

		public override int CountProducts(Bill_Production bill)
		{
			int num = 0;
			List<ThingDef> childThingDefs = ThingCategoryDefOf.StoneBlocks.childThingDefs;
			for (int i = 0; i < childThingDefs.Count; i++)
			{
				num += bill.Map.resourceCounter.GetCount(childThingDefs[i]);
			}
			return num;
		}

		public override string ProductsDescription(Bill_Production bill)
		{
			return ThingCategoryDefOf.StoneBlocks.label;
		}

		public override bool CanPossiblyStoreInStockpile(Bill_Production bill, Zone_Stockpile stockpile)
		{
			foreach (ThingDef thingDef in bill.ingredientFilter.AllowedThingDefs)
			{
				if (!thingDef.butcherProducts.NullOrEmpty<ThingDefCountClass>())
				{
					ThingDef thingDef2 = thingDef.butcherProducts[0].thingDef;
					if (!stockpile.GetStoreSettings().AllowedToAccept(thingDef2))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
