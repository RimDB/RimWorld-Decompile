﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200046D RID: 1133
	public class Recipe_Surgery : RecipeWorker
	{
		// Token: 0x04000BF5 RID: 3061
		private const float CatastrophicFailChance = 0.5f;

		// Token: 0x04000BF6 RID: 3062
		private const float RidiculousFailChanceFromCatastrophic = 0.1f;

		// Token: 0x04000BF7 RID: 3063
		private const float InspiredSurgeryFailChanceFactor = 0.1f;

		// Token: 0x04000BF8 RID: 3064
		private static readonly SimpleCurve MedicineMedicalPotencyToSurgeryChanceFactor = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.7f),
				true
			},
			{
				new CurvePoint(1f, 1f),
				true
			},
			{
				new CurvePoint(2f, 1.3f),
				true
			}
		};

		// Token: 0x060013E3 RID: 5091 RVA: 0x000ABF84 File Offset: 0x000AA384
		protected bool CheckSurgeryFail(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
		{
			float num = 1f;
			if (!patient.RaceProps.IsMechanoid)
			{
				num *= surgeon.GetStatValue(StatDefOf.MedicalSurgerySuccessChance, true);
			}
			if (patient.InBed())
			{
				num *= patient.CurrentBed().GetStatValue(StatDefOf.SurgerySuccessChanceFactor, true);
			}
			num *= Recipe_Surgery.MedicineMedicalPotencyToSurgeryChanceFactor.Evaluate(this.GetAverageMedicalPotency(ingredients, bill));
			num *= this.recipe.surgerySuccessChanceFactor;
			if (surgeon.InspirationDef == InspirationDefOf.Inspired_Surgery && !patient.RaceProps.IsMechanoid)
			{
				if (num < 1f)
				{
					num = 1f - (1f - num) * 0.1f;
				}
				surgeon.mindState.inspirationHandler.EndInspiration(InspirationDefOf.Inspired_Surgery);
			}
			bool result;
			if (!Rand.Chance(num))
			{
				if (Rand.Chance(this.recipe.deathOnFailedSurgeryChance))
				{
					HealthUtility.GiveInjuriesOperationFailureCatastrophic(patient, part);
					if (!patient.Dead)
					{
						patient.Kill(null, null);
					}
					Messages.Message("MessageMedicalOperationFailureFatal".Translate(new object[]
					{
						surgeon.LabelShort,
						patient.LabelShort,
						this.recipe.LabelCap
					}), patient, MessageTypeDefOf.NegativeHealthEvent, true);
				}
				else if (Rand.Chance(0.5f))
				{
					if (Rand.Chance(0.1f))
					{
						Messages.Message("MessageMedicalOperationFailureRidiculous".Translate(new object[]
						{
							surgeon.LabelShort,
							patient.LabelShort
						}), patient, MessageTypeDefOf.NegativeHealthEvent, true);
						HealthUtility.GiveInjuriesOperationFailureRidiculous(patient);
					}
					else
					{
						Messages.Message("MessageMedicalOperationFailureCatastrophic".Translate(new object[]
						{
							surgeon.LabelShort,
							patient.LabelShort
						}), patient, MessageTypeDefOf.NegativeHealthEvent, true);
						HealthUtility.GiveInjuriesOperationFailureCatastrophic(patient, part);
					}
				}
				else
				{
					Messages.Message("MessageMedicalOperationFailureMinor".Translate(new object[]
					{
						surgeon.LabelShort,
						patient.LabelShort
					}), patient, MessageTypeDefOf.NegativeHealthEvent, true);
					HealthUtility.GiveInjuriesOperationFailureMinor(patient, part);
				}
				if (!patient.Dead)
				{
					this.TryGainBotchedSurgeryThought(patient, surgeon);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x000AC1DD File Offset: 0x000AA5DD
		private void TryGainBotchedSurgeryThought(Pawn patient, Pawn surgeon)
		{
			if (patient.RaceProps.Humanlike)
			{
				patient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.BotchedMySurgery, surgeon);
			}
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x000AC218 File Offset: 0x000AA618
		private float GetAverageMedicalPotency(List<Thing> ingredients, Bill bill)
		{
			Bill_Medical bill_Medical = bill as Bill_Medical;
			ThingDef thingDef;
			if (bill_Medical != null)
			{
				thingDef = bill_Medical.consumedInitialMedicineDef;
			}
			else
			{
				thingDef = null;
			}
			int num = 0;
			float num2 = 0f;
			if (thingDef != null)
			{
				num++;
				num2 += thingDef.GetStatValueAbstract(StatDefOf.MedicalPotency, null);
			}
			for (int i = 0; i < ingredients.Count; i++)
			{
				Medicine medicine = ingredients[i] as Medicine;
				if (medicine != null)
				{
					num += medicine.stackCount;
					num2 += medicine.GetStatValue(StatDefOf.MedicalPotency, true) * (float)medicine.stackCount;
				}
			}
			float result;
			if (num == 0)
			{
				result = 1f;
			}
			else
			{
				result = num2 / (float)num;
			}
			return result;
		}
	}
}
