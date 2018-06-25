﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000726 RID: 1830
	public class CompPlantHarmRadius : ThingComp
	{
		// Token: 0x0400160A RID: 5642
		private int plantHarmAge;

		// Token: 0x0400160B RID: 5643
		private int ticksToPlantHarm;

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06002856 RID: 10326 RVA: 0x001589B0 File Offset: 0x00156DB0
		protected CompProperties_PlantHarmRadius PropsPlantHarmRadius
		{
			get
			{
				return (CompProperties_PlantHarmRadius)this.props;
			}
		}

		// Token: 0x06002857 RID: 10327 RVA: 0x001589D0 File Offset: 0x00156DD0
		public override void PostExposeData()
		{
			Scribe_Values.Look<int>(ref this.plantHarmAge, "plantHarmAge", 0, false);
			Scribe_Values.Look<int>(ref this.ticksToPlantHarm, "ticksToPlantHarm", 0, false);
		}

		// Token: 0x06002858 RID: 10328 RVA: 0x001589F8 File Offset: 0x00156DF8
		public override void CompTick()
		{
			if (this.parent.Spawned)
			{
				this.plantHarmAge++;
				this.ticksToPlantHarm--;
				if (this.ticksToPlantHarm <= 0)
				{
					float x = (float)this.plantHarmAge / 60000f;
					float num = this.PropsPlantHarmRadius.radiusPerDayCurve.Evaluate(x);
					float num2 = 3.14159274f * num * num;
					float num3 = num2 * this.PropsPlantHarmRadius.harmFrequencyPerArea;
					float num4 = 60f / num3;
					int num5;
					if (num4 >= 1f)
					{
						this.ticksToPlantHarm = GenMath.RoundRandom(num4);
						num5 = 1;
					}
					else
					{
						this.ticksToPlantHarm = 1;
						num5 = GenMath.RoundRandom(1f / num4);
					}
					for (int i = 0; i < num5; i++)
					{
						this.HarmRandomPlantInRadius(num);
					}
				}
			}
		}

		// Token: 0x06002859 RID: 10329 RVA: 0x00158AE4 File Offset: 0x00156EE4
		private void HarmRandomPlantInRadius(float radius)
		{
			IntVec3 c = this.parent.Position + (Rand.InsideUnitCircleVec3 * radius).ToIntVec3();
			if (c.InBounds(this.parent.Map))
			{
				Plant plant = c.GetPlant(this.parent.Map);
				if (plant != null)
				{
					if (plant.LeaflessNow)
					{
						if (Rand.Value < 0.2f)
						{
							plant.Kill(null, null);
						}
					}
					else
					{
						plant.MakeLeafless(Plant.LeaflessCause.Poison);
					}
				}
			}
		}
	}
}
