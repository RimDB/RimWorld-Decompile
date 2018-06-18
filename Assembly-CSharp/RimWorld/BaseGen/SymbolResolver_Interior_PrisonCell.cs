﻿using System;

namespace RimWorld.BaseGen
{
	// Token: 0x020003DA RID: 986
	public class SymbolResolver_Interior_PrisonCell : SymbolResolver
	{
		// Token: 0x060010F1 RID: 4337 RVA: 0x00090570 File Offset: 0x0008E970
		public override void Resolve(ResolveParams rp)
		{
			ThingSetMakerParams value = default(ThingSetMakerParams);
			value.techLevel = new TechLevel?((rp.faction == null) ? TechLevel.Spacer : rp.faction.def.techLevel);
			ResolveParams resolveParams = rp;
			resolveParams.thingSetMakerDef = ThingSetMakerDefOf.MapGen_PrisonCellStockpile;
			resolveParams.thingSetMakerParams = new ThingSetMakerParams?(value);
			resolveParams.innerStockpileSize = new int?(3);
			BaseGen.symbolStack.Push("innerStockpile", resolveParams);
			InteriorSymbolResolverUtility.PushBedroomHeatersCoolersAndLightSourcesSymbols(rp, false);
			BaseGen.symbolStack.Push("prisonerBed", rp);
		}

		// Token: 0x04000A4B RID: 2635
		private const int FoodStockpileSize = 3;
	}
}