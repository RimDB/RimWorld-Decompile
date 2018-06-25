﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	public class RaidStrategyWorker_ImmediateAttack : RaidStrategyWorker
	{
		public RaidStrategyWorker_ImmediateAttack()
		{
		}

		protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
		{
			IntVec3 originCell = (!parms.spawnCenter.IsValid) ? pawns[0].PositionHeld : parms.spawnCenter;
			LordJob result;
			if (parms.faction.HostileTo(Faction.OfPlayer))
			{
				result = new LordJob_AssaultColony(parms.faction, true, true, false, false, true);
			}
			else
			{
				IntVec3 fallbackLocation;
				RCellFinder.TryFindRandomSpotJustOutsideColony(originCell, map, out fallbackLocation);
				result = new LordJob_AssistColony(parms.faction, fallbackLocation);
			}
			return result;
		}
	}
}
