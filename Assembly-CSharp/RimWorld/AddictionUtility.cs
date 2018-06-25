﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class AddictionUtility
	{
		public static bool IsAddicted(Pawn pawn, Thing drug)
		{
			return AddictionUtility.FindAddictionHediff(pawn, drug) != null;
		}

		public static bool IsAddicted(Pawn pawn, ChemicalDef chemical)
		{
			return AddictionUtility.FindAddictionHediff(pawn, chemical) != null;
		}

		public static Hediff_Addiction FindAddictionHediff(Pawn pawn, Thing drug)
		{
			Hediff_Addiction result;
			if (!drug.def.IsDrug)
			{
				result = null;
			}
			else
			{
				CompDrug compDrug = drug.TryGetComp<CompDrug>();
				if (!compDrug.Props.Addictive)
				{
					result = null;
				}
				else
				{
					result = AddictionUtility.FindAddictionHediff(pawn, compDrug.Props.chemical);
				}
			}
			return result;
		}

		public static Hediff_Addiction FindAddictionHediff(Pawn pawn, ChemicalDef chemical)
		{
			return (Hediff_Addiction)pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == chemical.addictionHediff);
		}

		public static Hediff FindToleranceHediff(Pawn pawn, ChemicalDef chemical)
		{
			Hediff result;
			if (chemical.toleranceHediff == null)
			{
				result = null;
			}
			else
			{
				result = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == chemical.toleranceHediff);
			}
			return result;
		}

		public static void ModifyChemicalEffectForToleranceAndBodySize(Pawn pawn, ChemicalDef chemicalDef, ref float effect)
		{
			if (chemicalDef != null)
			{
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					hediffs[i].ModifyChemicalEffect(chemicalDef, ref effect);
				}
			}
			effect /= pawn.BodySize;
		}

		public static void CheckDrugAddictionTeachOpportunity(Pawn pawn)
		{
			if (pawn.RaceProps.IsFlesh && pawn.Spawned)
			{
				if (pawn.Faction == Faction.OfPlayer || pawn.HostFaction == Faction.OfPlayer)
				{
					if (AddictionUtility.AddictedToAnything(pawn))
					{
						LessonAutoActivator.TeachOpportunity(ConceptDefOf.DrugAddiction, pawn, OpportunityType.Important);
					}
				}
			}
		}

		public static bool AddictedToAnything(Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i] is Hediff_Addiction)
				{
					return true;
				}
			}
			return false;
		}

		public static bool CanBingeOnNow(Pawn pawn, ChemicalDef chemical, DrugCategory drugCategory)
		{
			bool result;
			if (!chemical.canBinge)
			{
				result = false;
			}
			else if (!pawn.Spawned)
			{
				result = false;
			}
			else
			{
				List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug);
				for (int i = 0; i < list.Count; i++)
				{
					if (!list[i].Position.Fogged(list[i].Map))
					{
						if (drugCategory == DrugCategory.Any || list[i].def.ingestible.drugCategory == drugCategory)
						{
							CompDrug compDrug = list[i].TryGetComp<CompDrug>();
							if (compDrug.Props.chemical == chemical)
							{
								if (list[i].Position.Roofed(list[i].Map) || list[i].Position.InHorDistOf(pawn.Position, 45f))
								{
									if (pawn.CanReach(list[i], PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
									{
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		[CompilerGenerated]
		private sealed class <FindAddictionHediff>c__AnonStorey0
		{
			internal ChemicalDef chemical;

			public <FindAddictionHediff>c__AnonStorey0()
			{
			}

			internal bool <>m__0(Hediff x)
			{
				return x.def == this.chemical.addictionHediff;
			}
		}

		[CompilerGenerated]
		private sealed class <FindToleranceHediff>c__AnonStorey1
		{
			internal ChemicalDef chemical;

			public <FindToleranceHediff>c__AnonStorey1()
			{
			}

			internal bool <>m__0(Hediff x)
			{
				return x.def == this.chemical.toleranceHediff;
			}
		}
	}
}
