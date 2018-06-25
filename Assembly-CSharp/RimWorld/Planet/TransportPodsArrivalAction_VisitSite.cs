﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000617 RID: 1559
	public class TransportPodsArrivalAction_VisitSite : TransportPodsArrivalAction
	{
		// Token: 0x0400124C RID: 4684
		private Site site;

		// Token: 0x0400124D RID: 4685
		private PawnsArrivalModeDef arrivalMode;

		// Token: 0x06001F6A RID: 8042 RVA: 0x00110663 File Offset: 0x0010EA63
		public TransportPodsArrivalAction_VisitSite()
		{
		}

		// Token: 0x06001F6B RID: 8043 RVA: 0x0011066C File Offset: 0x0010EA6C
		public TransportPodsArrivalAction_VisitSite(Site site, PawnsArrivalModeDef arrivalMode)
		{
			this.site = site;
			this.arrivalMode = arrivalMode;
		}

		// Token: 0x06001F6C RID: 8044 RVA: 0x00110683 File Offset: 0x0010EA83
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Site>(ref this.site, "site", false);
			Scribe_Defs.Look<PawnsArrivalModeDef>(ref this.arrivalMode, "arrivalMode");
		}

		// Token: 0x06001F6D RID: 8045 RVA: 0x001106B0 File Offset: 0x0010EAB0
		public override FloatMenuAcceptanceReport StillValid(IEnumerable<IThingHolder> pods, int destinationTile)
		{
			FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(pods, destinationTile);
			FloatMenuAcceptanceReport result;
			if (!floatMenuAcceptanceReport)
			{
				result = floatMenuAcceptanceReport;
			}
			else if (this.site != null && this.site.Tile != destinationTile)
			{
				result = false;
			}
			else
			{
				result = TransportPodsArrivalAction_VisitSite.CanVisit(pods, this.site);
			}
			return result;
		}

		// Token: 0x06001F6E RID: 8046 RVA: 0x00110714 File Offset: 0x0010EB14
		public override bool ShouldUseLongEvent(List<ActiveDropPodInfo> pods, int tile)
		{
			return !this.site.HasMap;
		}

		// Token: 0x06001F6F RID: 8047 RVA: 0x00110738 File Offset: 0x0010EB38
		public override void Arrived(List<ActiveDropPodInfo> pods, int tile)
		{
			Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(pods);
			bool flag = !this.site.HasMap;
			Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(this.site.Tile, SiteCoreWorker.MapSize, null);
			if (flag)
			{
				Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
				PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(orGenerateMap.mapPawns.AllPawns, "LetterRelatedPawnsInMapWherePlayerLanded".Translate(new object[]
				{
					Faction.OfPlayer.def.pawnsPlural
				}), LetterDefOf.NeutralEvent, true, true);
			}
			Messages.Message("MessageTransportPodsArrived".Translate(), lookTarget, MessageTypeDefOf.TaskCompletion, true);
			this.arrivalMode.Worker.TravelingTransportPodsArrived(pods, orGenerateMap);
		}

		// Token: 0x06001F70 RID: 8048 RVA: 0x001107EC File Offset: 0x0010EBEC
		public static FloatMenuAcceptanceReport CanVisit(IEnumerable<IThingHolder> pods, Site site)
		{
			FloatMenuAcceptanceReport result;
			if (site == null || !site.Spawned || !site.core.transportPodsCanLandAndGenerateMap)
			{
				result = false;
			}
			else if (!TransportPodsArrivalActionUtility.AnyNonDownedColonist(pods))
			{
				result = false;
			}
			else if (site.EnterCooldownBlocksEntering())
			{
				result = FloatMenuAcceptanceReport.WithFailMessage("MessageEnterCooldownBlocksEntering".Translate(new object[]
				{
					site.EnterCooldownDaysLeft().ToString("0.#")
				}));
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x00110888 File Offset: 0x0010EC88
		public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(CompLaunchable representative, IEnumerable<IThingHolder> pods, Site site)
		{
			foreach (FloatMenuOption f in TransportPodsArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_VisitSite>(() => TransportPodsArrivalAction_VisitSite.CanVisit(pods, site), () => new TransportPodsArrivalAction_VisitSite(site, PawnsArrivalModeDefOf.EdgeDrop), "DropAtEdge".Translate(), representative, site.Tile))
			{
				yield return f;
			}
			foreach (FloatMenuOption f2 in TransportPodsArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_VisitSite>(() => TransportPodsArrivalAction_VisitSite.CanVisit(pods, site), () => new TransportPodsArrivalAction_VisitSite(site, PawnsArrivalModeDefOf.CenterDrop), "DropInCenter".Translate(), representative, site.Tile))
			{
				yield return f2;
			}
			yield break;
		}
	}
}
