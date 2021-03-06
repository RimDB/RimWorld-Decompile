﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_EnterTransporter : ThinkNode_JobGiver
	{
		private static List<CompTransporter> tmpTransporters = new List<CompTransporter>();

		public JobGiver_EnterTransporter()
		{
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			int transportersGroup = pawn.mindState.duty.transportersGroup;
			List<Pawn> allPawnsSpawned = pawn.Map.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				if (allPawnsSpawned[i] != pawn && allPawnsSpawned[i].CurJobDef == JobDefOf.HaulToTransporter)
				{
					CompTransporter transporter = ((JobDriver_HaulToTransporter)allPawnsSpawned[i].jobs.curDriver).Transporter;
					if (transporter != null && transporter.groupID == transportersGroup)
					{
						return null;
					}
				}
			}
			TransporterUtility.GetTransportersInGroup(transportersGroup, pawn.Map, JobGiver_EnterTransporter.tmpTransporters);
			CompTransporter compTransporter = JobGiver_EnterTransporter.FindMyTransporter(JobGiver_EnterTransporter.tmpTransporters, pawn);
			JobGiver_EnterTransporter.tmpTransporters.Clear();
			if (compTransporter == null || !pawn.CanReach(compTransporter.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
			{
				return null;
			}
			return new Job(JobDefOf.EnterTransporter, compTransporter.parent);
		}

		public static CompTransporter FindMyTransporter(List<CompTransporter> transporters, Pawn me)
		{
			for (int i = 0; i < transporters.Count; i++)
			{
				List<TransferableOneWay> leftToLoad = transporters[i].leftToLoad;
				if (leftToLoad != null)
				{
					for (int j = 0; j < leftToLoad.Count; j++)
					{
						if (leftToLoad[j].AnyThing is Pawn)
						{
							List<Thing> things = leftToLoad[j].things;
							for (int k = 0; k < things.Count; k++)
							{
								if (things[k] == me)
								{
									return transporters[i];
								}
							}
						}
					}
				}
			}
			return null;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static JobGiver_EnterTransporter()
		{
		}
	}
}
