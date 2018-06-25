﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x0200088A RID: 2186
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_Predator : PawnColumnWorker_Icon
	{
		// Token: 0x04001AD0 RID: 6864
		private static readonly Texture2D Icon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Predator", true);

		// Token: 0x060031E4 RID: 12772 RVA: 0x001AF058 File Offset: 0x001AD458
		protected override Texture2D GetIconFor(Pawn pawn)
		{
			Texture2D result;
			if (pawn.RaceProps.predator)
			{
				result = PawnColumnWorker_Predator.Icon;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060031E5 RID: 12773 RVA: 0x001AF08C File Offset: 0x001AD48C
		protected override string GetIconTip(Pawn pawn)
		{
			return "IsPredator".Translate();
		}
	}
}
