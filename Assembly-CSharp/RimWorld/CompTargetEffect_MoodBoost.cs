﻿using System;
using Verse;

namespace RimWorld
{
	public class CompTargetEffect_MoodBoost : CompTargetEffect
	{
		public CompTargetEffect_MoodBoost()
		{
		}

		public override void DoEffectOn(Pawn user, Thing target)
		{
			Pawn pawn = (Pawn)target;
			if (pawn.Dead || pawn.needs == null || pawn.needs.mood == null)
			{
				return;
			}
			pawn.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.ArtifactMoodBoost), null);
		}
	}
}
