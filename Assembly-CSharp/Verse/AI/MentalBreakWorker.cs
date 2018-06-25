﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000A5A RID: 2650
	public class MentalBreakWorker
	{
		// Token: 0x04002546 RID: 9542
		public MentalBreakDef def;

		// Token: 0x06003AFA RID: 15098 RVA: 0x001F4D48 File Offset: 0x001F3148
		public virtual float CommonalityFor(Pawn pawn)
		{
			float num = this.def.baseCommonality;
			if (pawn.Faction == Faction.OfPlayer && this.def.commonalityFactorPerPopulationCurve != null)
			{
				num *= this.def.commonalityFactorPerPopulationCurve.Evaluate((float)PawnsFinder.AllMaps_FreeColonists.Count<Pawn>());
			}
			return num;
		}

		// Token: 0x06003AFB RID: 15099 RVA: 0x001F4DA8 File Offset: 0x001F31A8
		public virtual bool BreakCanOccur(Pawn pawn)
		{
			bool result;
			if (this.def.requiredTrait != null && (pawn.story == null || !pawn.story.traits.HasTrait(this.def.requiredTrait)))
			{
				result = false;
			}
			else if (this.def.mentalState != null && pawn.story != null && pawn.story.traits.allTraits.Any((Trait tr) => tr.CurrentData.disallowedMentalStates != null && tr.CurrentData.disallowedMentalStates.Contains(this.def.mentalState)))
			{
				result = false;
			}
			else if (this.def.mentalState != null && !this.def.mentalState.Worker.StateCanOccur(pawn))
			{
				result = false;
			}
			else
			{
				if (pawn.story != null)
				{
					IEnumerable<MentalBreakDef> theOnlyAllowedMentalBreaks = pawn.story.traits.TheOnlyAllowedMentalBreaks;
					if (!theOnlyAllowedMentalBreaks.Contains(this.def) && theOnlyAllowedMentalBreaks.Any((MentalBreakDef x) => x.intensity == this.def.intensity && x.Worker.BreakCanOccur(pawn)))
					{
						return false;
					}
				}
				result = (!TutorSystem.TutorialMode || pawn.Faction != Faction.OfPlayer);
			}
			return result;
		}

		// Token: 0x06003AFC RID: 15100 RVA: 0x001F4F28 File Offset: 0x001F3328
		public virtual bool TryStart(Pawn pawn, Thought reason, bool causedByMood)
		{
			string text = (reason == null) ? null : reason.LabelCap;
			MentalStateHandler mentalStateHandler = pawn.mindState.mentalStateHandler;
			MentalStateDef mentalState = this.def.mentalState;
			string reason2 = text;
			return mentalStateHandler.TryStartMentalState(mentalState, reason2, false, causedByMood, null, false);
		}

		// Token: 0x06003AFD RID: 15101 RVA: 0x001F4F78 File Offset: 0x001F3378
		protected bool TrySendLetter(Pawn pawn, string textKey, Thought reason)
		{
			bool result;
			if (!PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				result = false;
			}
			else
			{
				string label = "MentalBreakLetterLabel".Translate() + ": " + this.def.LabelCap;
				string text = textKey.Translate(new object[]
				{
					pawn.Label
				}).CapitalizeFirst();
				if (reason != null)
				{
					text = text + "\n\n" + "FinalStraw".Translate(new object[]
					{
						reason.LabelCap
					});
				}
				text = text.AdjustedFor(pawn, "PAWN");
				Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, pawn, null, null);
				result = true;
			}
			return result;
		}
	}
}
