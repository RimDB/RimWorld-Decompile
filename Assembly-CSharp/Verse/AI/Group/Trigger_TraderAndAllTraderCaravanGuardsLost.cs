﻿using System;
using RimWorld;

namespace Verse.AI.Group
{
	public class Trigger_TraderAndAllTraderCaravanGuardsLost : Trigger
	{
		public Trigger_TraderAndAllTraderCaravanGuardsLost()
		{
		}

		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			bool result;
			if (signal.type == TriggerSignalType.PawnLost)
			{
				for (int i = 0; i < lord.ownedPawns.Count; i++)
				{
					TraderCaravanRole traderCaravanRole = lord.ownedPawns[i].GetTraderCaravanRole();
					if (traderCaravanRole == TraderCaravanRole.Trader || traderCaravanRole == TraderCaravanRole.Guard)
					{
						return false;
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
