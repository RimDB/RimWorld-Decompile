﻿using System;
using Verse;

namespace RimWorld
{
	public class CompUseEffect_StartWick : CompUseEffect
	{
		public CompUseEffect_StartWick()
		{
		}

		public override void DoEffect(Pawn usedBy)
		{
			base.DoEffect(usedBy);
			this.parent.GetComp<CompExplosive>().StartWick(null);
		}
	}
}
