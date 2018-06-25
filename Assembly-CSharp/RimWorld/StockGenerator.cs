﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000769 RID: 1897
	public abstract class StockGenerator
	{
		// Token: 0x040016A5 RID: 5797
		[Unsaved]
		public TraderKindDef trader;

		// Token: 0x040016A6 RID: 5798
		public IntRange countRange = IntRange.zero;

		// Token: 0x040016A7 RID: 5799
		public List<ThingDefCountRangeClass> customCountRanges;

		// Token: 0x040016A8 RID: 5800
		public FloatRange totalPriceRange = FloatRange.Zero;

		// Token: 0x040016A9 RID: 5801
		public TechLevel maxTechLevelGenerate = TechLevel.Archotech;

		// Token: 0x040016AA RID: 5802
		public TechLevel maxTechLevelBuy = TechLevel.Archotech;

		// Token: 0x040016AB RID: 5803
		public PriceType price = PriceType.Normal;

		// Token: 0x060029F6 RID: 10742 RVA: 0x001636AC File Offset: 0x00161AAC
		public virtual void ResolveReferences(TraderKindDef trader)
		{
			this.trader = trader;
		}

		// Token: 0x060029F7 RID: 10743 RVA: 0x001636B8 File Offset: 0x00161AB8
		public virtual IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			yield break;
		}

		// Token: 0x060029F8 RID: 10744
		public abstract IEnumerable<Thing> GenerateThings(int forTile);

		// Token: 0x060029F9 RID: 10745
		public abstract bool HandlesThingDef(ThingDef thingDef);

		// Token: 0x060029FA RID: 10746 RVA: 0x001636DC File Offset: 0x00161ADC
		public bool TryGetPriceType(ThingDef thingDef, TradeAction action, out PriceType priceType)
		{
			bool result;
			if (!this.HandlesThingDef(thingDef))
			{
				priceType = PriceType.Undefined;
				result = false;
			}
			else
			{
				priceType = this.price;
				result = true;
			}
			return result;
		}

		// Token: 0x060029FB RID: 10747 RVA: 0x00163714 File Offset: 0x00161B14
		protected int RandomCountOf(ThingDef def)
		{
			IntRange intRange = this.countRange;
			if (this.customCountRanges != null)
			{
				for (int i = 0; i < this.customCountRanges.Count; i++)
				{
					if (this.customCountRanges[i].thingDef == def)
					{
						intRange = this.customCountRanges[i].countRange;
						break;
					}
				}
			}
			int result;
			if (intRange.max <= 0 && this.totalPriceRange.max <= 0f)
			{
				result = 0;
			}
			else if (intRange.max > 0 && this.totalPriceRange.max <= 0f)
			{
				result = intRange.RandomInRange;
			}
			else if (intRange.max <= 0 && this.totalPriceRange.max > 0f)
			{
				result = Mathf.RoundToInt(this.totalPriceRange.RandomInRange / def.BaseMarketValue);
			}
			else
			{
				int num = 0;
				int randomInRange;
				do
				{
					randomInRange = intRange.RandomInRange;
					num++;
					if (num > 100)
					{
						break;
					}
				}
				while (!this.totalPriceRange.Includes((float)randomInRange * def.BaseMarketValue));
				result = randomInRange;
			}
			return result;
		}
	}
}
