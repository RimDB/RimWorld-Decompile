﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class IncidentWorker_DeepDrillInfestation : IncidentWorker
	{
		private static List<Thing> tmpDrills = new List<Thing>();

		private const float MinPointsFactor = 0.24f;

		private const float MaxPointsFactor = 0.48f;

		private const float MinPoints = 115f;

		private const float MaxPoints = 800f;

		public IncidentWorker_DeepDrillInfestation()
		{
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Map map = (Map)parms.target;
			IncidentWorker_DeepDrillInfestation.tmpDrills.Clear();
			DeepDrillInfestationIncidentUtility.GetUsableDeepDrills(map, IncidentWorker_DeepDrillInfestation.tmpDrills);
			return IncidentWorker_DeepDrillInfestation.tmpDrills.Any<Thing>();
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IncidentWorker_DeepDrillInfestation.tmpDrills.Clear();
			DeepDrillInfestationIncidentUtility.GetUsableDeepDrills(map, IncidentWorker_DeepDrillInfestation.tmpDrills);
			Thing deepDrill;
			if (!IncidentWorker_DeepDrillInfestation.tmpDrills.TryRandomElement(out deepDrill))
			{
				return false;
			}
			IntVec3 intVec = CellFinder.FindNoWipeSpawnLocNear(deepDrill.Position, map, ThingDefOf.TunnelHiveSpawner, Rot4.North, 2, (IntVec3 x) => x.Walkable(map) && x.GetFirstThing(map, deepDrill.def) == null && x.GetFirstThing(map) == null && x.GetFirstThing(map, ThingDefOf.Hive) == null && x.GetFirstThing(map, ThingDefOf.TunnelHiveSpawner) == null);
			if (intVec == deepDrill.Position)
			{
				return false;
			}
			TunnelHiveSpawner tunnelHiveSpawner = (TunnelHiveSpawner)ThingMaker.MakeThing(ThingDefOf.TunnelHiveSpawner, null);
			tunnelHiveSpawner.spawnHive = false;
			tunnelHiveSpawner.insectsPoints = Mathf.Clamp(parms.points * Rand.Range(0.24f, 0.48f), 115f, 800f);
			tunnelHiveSpawner.spawnedByInfestationThingComp = true;
			GenSpawn.Spawn(tunnelHiveSpawner, intVec, map, WipeMode.FullRefund);
			deepDrill.TryGetComp<CompCreatesInfestations>().Notify_CreatedInfestation();
			base.SendStandardLetter(new TargetInfo(tunnelHiveSpawner.Position, map, false), null, new string[0]);
			return true;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static IncidentWorker_DeepDrillInfestation()
		{
		}

		[CompilerGenerated]
		private sealed class <TryExecuteWorker>c__AnonStorey0
		{
			internal Map map;

			internal Thing deepDrill;

			public <TryExecuteWorker>c__AnonStorey0()
			{
			}

			internal bool <>m__0(IntVec3 x)
			{
				return x.Walkable(this.map) && x.GetFirstThing(this.map, this.deepDrill.def) == null && x.GetFirstThing(this.map) == null && x.GetFirstThing(this.map, ThingDefOf.Hive) == null && x.GetFirstThing(this.map, ThingDefOf.TunnelHiveSpawner) == null;
			}
		}
	}
}
