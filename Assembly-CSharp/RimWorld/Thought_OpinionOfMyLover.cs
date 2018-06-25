﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Thought_OpinionOfMyLover : Thought_Situational
	{
		public Thought_OpinionOfMyLover()
		{
		}

		public override string LabelCap
		{
			get
			{
				DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(this.pawn, false);
				return string.Format(base.CurStage.label, directPawnRelation.def.GetGenderSpecificLabel(directPawnRelation.otherPawn), directPawnRelation.otherPawn.LabelShort).CapitalizeFirst();
			}
		}

		protected override float BaseMoodOffset
		{
			get
			{
				float num = 0.1f * (float)this.pawn.relations.OpinionOf(LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(this.pawn, false).otherPawn);
				float result;
				if (num < 0f)
				{
					result = Mathf.Min(num, -1f);
				}
				else
				{
					result = Mathf.Max(num, 1f);
				}
				return result;
			}
		}
	}
}
