﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x0200023A RID: 570
	public static class ThingDefGenerator_Corpses
	{
		// Token: 0x040003F0 RID: 1008
		private const float DaysToStartRot = 2.5f;

		// Token: 0x040003F1 RID: 1009
		private const float DaysToDessicate = 5f;

		// Token: 0x040003F2 RID: 1010
		private const float RotDamagePerDay = 2f;

		// Token: 0x040003F3 RID: 1011
		private const float DessicatedDamagePerDay = 0.7f;

		// Token: 0x06000A48 RID: 2632 RVA: 0x0005C7F4 File Offset: 0x0005ABF4
		public static IEnumerable<ThingDef> ImpliedCorpseDefs()
		{
			foreach (ThingDef raceDef in DefDatabase<ThingDef>.AllDefs.ToList<ThingDef>())
			{
				if (raceDef.category == ThingCategory.Pawn)
				{
					ThingDef d = new ThingDef();
					d.category = ThingCategory.Item;
					d.thingClass = typeof(Corpse);
					d.selectable = true;
					d.tickerType = TickerType.Rare;
					d.altitudeLayer = AltitudeLayer.ItemImportant;
					d.scatterableOnMapGen = false;
					d.SetStatBaseValue(StatDefOf.Beauty, -50f);
					d.SetStatBaseValue(StatDefOf.DeteriorationRate, 2f);
					d.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, 0.05f);
					d.alwaysHaulable = true;
					d.soundDrop = SoundDefOf.Corpse_Drop;
					d.pathCost = 15;
					d.socialPropernessMatters = false;
					d.tradeability = Tradeability.None;
					d.inspectorTabs = new List<Type>();
					d.inspectorTabs.Add(typeof(ITab_Pawn_Health));
					d.inspectorTabs.Add(typeof(ITab_Pawn_Character));
					d.inspectorTabs.Add(typeof(ITab_Pawn_Gear));
					d.inspectorTabs.Add(typeof(ITab_Pawn_Social));
					d.inspectorTabs.Add(typeof(ITab_Pawn_Log));
					d.comps.Add(new CompProperties_Forbiddable());
					d.recipes = new List<RecipeDef>();
					if (!raceDef.race.IsMechanoid)
					{
						d.recipes.Add(RecipeDefOf.RemoveBodyPart);
					}
					d.defName = "Corpse_" + raceDef.defName;
					d.label = "CorpseLabel".Translate(new object[]
					{
						raceDef.label
					});
					d.description = "CorpseDesc".Translate(new object[]
					{
						raceDef.label
					});
					d.soundImpactDefault = raceDef.soundImpactDefault;
					d.SetStatBaseValue(StatDefOf.Flammability, raceDef.GetStatValueAbstract(StatDefOf.Flammability, null));
					d.SetStatBaseValue(StatDefOf.MaxHitPoints, (float)raceDef.BaseMaxHitPoints);
					d.SetStatBaseValue(StatDefOf.Mass, raceDef.statBases.GetStatOffsetFromList(StatDefOf.Mass));
					d.SetStatBaseValue(StatDefOf.Nutrition, 5.2f);
					d.modContentPack = raceDef.modContentPack;
					d.ingestible = new IngestibleProperties();
					d.ingestible.parent = d;
					IngestibleProperties ing = d.ingestible;
					ing.foodType = FoodTypeFlags.Corpse;
					ing.sourceDef = raceDef;
					ing.preferability = ((!raceDef.race.IsFlesh) ? FoodPreferability.NeverForNutrition : FoodPreferability.DesperateOnly);
					DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(ing, "tasteThought", ThoughtDefOf.AteCorpse.defName);
					ing.maxNumToIngestAtOnce = 1;
					ing.ingestEffect = EffecterDefOf.EatMeat;
					ing.ingestSound = SoundDefOf.RawMeat_Eat;
					ing.specialThoughtDirect = raceDef.race.FleshType.ateDirect;
					if (raceDef.race.IsFlesh)
					{
						CompProperties_Rottable compProperties_Rottable = new CompProperties_Rottable();
						compProperties_Rottable.daysToRotStart = 2.5f;
						compProperties_Rottable.daysToDessicated = 5f;
						compProperties_Rottable.rotDamagePerDay = 2f;
						compProperties_Rottable.dessicatedDamagePerDay = 0.7f;
						d.comps.Add(compProperties_Rottable);
						CompProperties_SpawnerFilth compProperties_SpawnerFilth = new CompProperties_SpawnerFilth();
						compProperties_SpawnerFilth.filthDef = ThingDefOf.Filth_CorpseBile;
						compProperties_SpawnerFilth.spawnCountOnSpawn = 0;
						compProperties_SpawnerFilth.spawnMtbHours = 0f;
						compProperties_SpawnerFilth.spawnRadius = 0.1f;
						compProperties_SpawnerFilth.spawnEveryDays = 1f;
						compProperties_SpawnerFilth.requiredRotStage = new RotStage?(RotStage.Rotting);
						d.comps.Add(compProperties_SpawnerFilth);
					}
					if (d.thingCategories == null)
					{
						d.thingCategories = new List<ThingCategoryDef>();
					}
					if (raceDef.race.Humanlike)
					{
						DirectXmlCrossRefLoader.RegisterListWantsCrossRef<ThingCategoryDef>(d.thingCategories, ThingCategoryDefOf.CorpsesHumanlike.defName, d);
					}
					else
					{
						DirectXmlCrossRefLoader.RegisterListWantsCrossRef<ThingCategoryDef>(d.thingCategories, raceDef.race.FleshType.corpseCategory.defName, d);
					}
					raceDef.race.corpseDef = d;
					yield return d;
				}
			}
			yield break;
		}
	}
}
