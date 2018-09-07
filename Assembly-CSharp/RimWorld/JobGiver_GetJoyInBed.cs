﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_GetJoyInBed : JobGiver_GetJoy
	{
		private const float MaxJoyLevel = 0.5f;

		public JobGiver_GetJoyInBed()
		{
		}

		protected override bool CanDoDuringMedicalRest
		{
			get
			{
				return true;
			}
		}

		protected override bool JoyGiverAllowed(JoyGiverDef def)
		{
			return def.canDoWhileInBed;
		}

		protected override Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
		{
			return def.Worker.TryGiveJobWhileInBed(pawn);
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.CurJob == null || !pawn.InBed() || !pawn.Awake() || pawn.needs.joy == null)
			{
				return null;
			}
			float curLevel = pawn.needs.joy.CurLevel;
			if (curLevel > 0.5f)
			{
				return null;
			}
			return base.TryGiveJob(pawn);
		}
	}
}
