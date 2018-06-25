﻿using System;

namespace Verse.AI
{
	public class MentalStateWorker_MurderousRage : MentalStateWorker
	{
		public MentalStateWorker_MurderousRage()
		{
		}

		public override bool StateCanOccur(Pawn pawn)
		{
			return base.StateCanOccur(pawn) && MurderousRageMentalStateUtility.FindPawnToKill(pawn) != null;
		}
	}
}
