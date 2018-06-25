﻿using System;
using Verse;

namespace RimWorld
{
	public class ThoughtWorker_Sick : ThoughtWorker
	{
		public ThoughtWorker_Sick()
		{
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.health.hediffSet.AnyHediffMakesSickThought;
		}
	}
}
