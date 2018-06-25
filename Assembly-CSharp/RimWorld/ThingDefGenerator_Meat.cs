﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x0200023B RID: 571
	public static class ThingDefGenerator_Meat
	{
		// Token: 0x06000A49 RID: 2633 RVA: 0x0005CEF0 File Offset: 0x0005B2F0
		public static IEnumerable<ThingDef> ImpliedMeatDefs()
		{
			foreach (ThingDef sourceDef in DefDatabase<ThingDef>.AllDefs.ToList<ThingDef>())
			{
				if (sourceDef.category == ThingCategory.Pawn)
				{
					if (sourceDef.race.useMeatFrom == null)
					{
						if (!sourceDef.race.IsFlesh)
						{
							DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(sourceDef.race, "meatDef", "Steel");
						}
						else
						{
							ThingDef d = new ThingDef();
							d.resourceReadoutPriority = ResourceCountPriority.Middle;
							d.category = ThingCategory.Item;
							d.thingClass = typeof(ThingWithComps);
							d.graphicData = new GraphicData();
							d.graphicData.graphicClass = typeof(Graphic_Single);
							d.useHitPoints = true;
							d.selectable = true;
							d.SetStatBaseValue(StatDefOf.MaxHitPoints, 100f);
							d.altitudeLayer = AltitudeLayer.Item;
							d.stackLimit = 75;
							d.comps.Add(new CompProperties_Forbiddable());
							CompProperties_Rottable rotProps = new CompProperties_Rottable();
							rotProps.daysToRotStart = 2f;
							rotProps.rotDestroys = true;
							d.comps.Add(rotProps);
							d.tickerType = TickerType.Rare;
							d.SetStatBaseValue(StatDefOf.Beauty, -4f);
							d.alwaysHaulable = true;
							d.rotatable = false;
							d.pathCost = 15;
							d.drawGUIOverlay = true;
							d.socialPropernessMatters = true;
							d.modContentPack = sourceDef.modContentPack;
							d.category = ThingCategory.Item;
							if (sourceDef.race.Humanlike)
							{
								d.description = "MeatHumanDesc".Translate(new object[]
								{
									sourceDef.label
								});
							}
							else if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
							{
								d.description = "MeatInsectDesc".Translate(new object[]
								{
									sourceDef.label
								});
							}
							else
							{
								d.description = "MeatDesc".Translate(new object[]
								{
									sourceDef.label
								});
							}
							d.useHitPoints = true;
							d.SetStatBaseValue(StatDefOf.MaxHitPoints, 60f);
							d.SetStatBaseValue(StatDefOf.DeteriorationRate, 6f);
							d.SetStatBaseValue(StatDefOf.Mass, 0.03f);
							d.SetStatBaseValue(StatDefOf.Flammability, 0.5f);
							d.SetStatBaseValue(StatDefOf.Nutrition, 0.05f);
							d.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, 0.02f);
							d.BaseMarketValue = ThingDefGenerator_Meat.GetMeatMarketValue(sourceDef);
							if (d.thingCategories == null)
							{
								d.thingCategories = new List<ThingCategoryDef>();
							}
							DirectXmlCrossRefLoader.RegisterListWantsCrossRef<ThingCategoryDef>(d.thingCategories, "MeatRaw", d);
							d.ingestible = new IngestibleProperties();
							d.ingestible.parent = d;
							d.ingestible.foodType = FoodTypeFlags.Meat;
							d.ingestible.preferability = FoodPreferability.RawBad;
							DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(d.ingestible, "tasteThought", ThoughtDefOf.AteRawFood.defName);
							d.ingestible.ingestEffect = EffecterDefOf.EatMeat;
							d.ingestible.ingestSound = SoundDefOf.RawMeat_Eat;
							d.ingestible.specialThoughtDirect = sourceDef.race.FleshType.ateDirect;
							d.ingestible.specialThoughtAsIngredient = sourceDef.race.FleshType.ateAsIngredient;
							if (sourceDef.race.Humanlike)
							{
								d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/MeatHuman";
							}
							else
							{
								if (sourceDef.race.baseBodySize < 0.7f)
								{
									d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/MeatSmall";
								}
								else
								{
									d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/MeatBig";
								}
								d.graphicData.color = sourceDef.race.meatColor;
							}
							d.defName = "Meat_" + sourceDef.defName;
							if (sourceDef.race.meatLabel.NullOrEmpty())
							{
								d.label = "MeatLabel".Translate(new object[]
								{
									sourceDef.label
								});
							}
							else
							{
								d.label = sourceDef.race.meatLabel;
							}
							d.ingestible.sourceDef = sourceDef;
							sourceDef.race.meatDef = d;
							yield return d;
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0005CF14 File Offset: 0x0005B314
		private static float GetMeatMarketValue(ThingDef sourceDef)
		{
			float result;
			if (sourceDef.race.Humanlike)
			{
				result = 0.8f;
			}
			else
			{
				result = 2f;
			}
			return result;
		}
	}
}
