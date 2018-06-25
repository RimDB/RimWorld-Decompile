﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class IncidentWorker_ShipChunkDrop : IncidentWorker
	{
		private static readonly Pair<int, float>[] CountChance = new Pair<int, float>[]
		{
			new Pair<int, float>(1, 1f),
			new Pair<int, float>(2, 0.95f),
			new Pair<int, float>(3, 0.7f),
			new Pair<int, float>(4, 0.4f)
		};

		public IncidentWorker_ShipChunkDrop()
		{
		}

		private int RandomCountToDrop
		{
			get
			{
				float x2 = (float)Find.TickManager.TicksGame / 3600000f;
				float timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0f, 1.2f, 1f, 0.1f, x2), 0.1f, 1f);
				return IncidentWorker_ShipChunkDrop.CountChance.RandomElementByWeight(delegate(Pair<int, float> x)
				{
					float result;
					if (x.First == 1)
					{
						result = x.Second;
					}
					else
					{
						result = x.Second * timePassedFactor;
					}
					return result;
				}).First;
			}
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			bool result;
			if (!base.CanFireNowSub(parms))
			{
				result = false;
			}
			else
			{
				Map map = (Map)parms.target;
				IntVec3 intVec;
				result = this.TryFindShipChunkDropCell(map.Center, map, 999999, out intVec);
			}
			return result;
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IntVec3 intVec;
			bool result;
			if (!this.TryFindShipChunkDropCell(map.Center, map, 999999, out intVec))
			{
				result = false;
			}
			else
			{
				this.SpawnShipChunks(intVec, map, this.RandomCountToDrop);
				Messages.Message("MessageShipChunkDrop".Translate(), new TargetInfo(intVec, map, false), MessageTypeDefOf.NeutralEvent, true);
				result = true;
			}
			return result;
		}

		private void SpawnShipChunks(IntVec3 firstChunkPos, Map map, int count)
		{
			this.SpawnChunk(firstChunkPos, map);
			for (int i = 0; i < count - 1; i++)
			{
				IntVec3 pos;
				if (this.TryFindShipChunkDropCell(firstChunkPos, map, 5, out pos))
				{
					this.SpawnChunk(pos, map);
				}
			}
		}

		private void SpawnChunk(IntVec3 pos, Map map)
		{
			SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, ThingDefOf.ShipChunk, pos, map);
		}

		private bool TryFindShipChunkDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
		{
			ThingDef shipChunkIncoming = ThingDefOf.ShipChunkIncoming;
			ref IntVec3 cell = ref pos;
			return CellFinderLoose.TryFindSkyfallerCell(shipChunkIncoming, map, out cell, 10, nearLoc, maxDist, true, false, false, false, true, false, null);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static IncidentWorker_ShipChunkDrop()
		{
		}

		[CompilerGenerated]
		private sealed class <>c__AnonStorey0
		{
			internal float timePassedFactor;

			public <>c__AnonStorey0()
			{
			}

			internal float <>m__0(Pair<int, float> x)
			{
				float result;
				if (x.First == 1)
				{
					result = x.Second;
				}
				else
				{
					result = x.Second * this.timePassedFactor;
				}
				return result;
			}
		}
	}
}
