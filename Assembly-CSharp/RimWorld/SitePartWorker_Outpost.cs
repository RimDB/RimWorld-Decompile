﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld.Planet;
using Verse;

namespace RimWorld
{
	public class SitePartWorker_Outpost : SitePartWorker
	{
		[CompilerGenerated]
		private static Func<Pawn, bool> <>f__am$cache0;

		public SitePartWorker_Outpost()
		{
		}

		public override string GetArrivedLetterPart(Map map, out string preferredLabel, out LetterDef preferredLetterDef, out LookTargets lookTargets)
		{
			string arrivedLetterPart = base.GetArrivedLetterPart(map, out preferredLabel, out preferredLetterDef, out lookTargets);
			lookTargets = (from x in map.mapPawns.AllPawnsSpawned
			where x.RaceProps.Humanlike && x.HostileTo(Faction.OfPlayer)
			select x).FirstOrDefault<Pawn>();
			return arrivedLetterPart;
		}

		public override string GetPostProcessedDescriptionDialogue(Site site, SiteCoreOrPartBase siteCoreOrPart)
		{
			return string.Format(base.GetPostProcessedDescriptionDialogue(site, siteCoreOrPart), this.GetEnemiesCount(site, siteCoreOrPart.parms));
		}

		public override string GetPostProcessedThreatLabel(Site site, SiteCoreOrPartBase siteCoreOrPart)
		{
			return string.Concat(new object[]
			{
				base.GetPostProcessedThreatLabel(site, siteCoreOrPart),
				" (",
				this.GetEnemiesCount(site, siteCoreOrPart.parms),
				" ",
				"Enemies".Translate(),
				")"
			});
		}

		private int GetEnemiesCount(Site site, SiteCoreOrPartParams parms)
		{
			return PawnGroupMakerUtility.GeneratePawnKindsExample(new PawnGroupMakerParms
			{
				tile = site.Tile,
				faction = site.Faction,
				groupKind = PawnGroupKindDefOf.Settlement,
				points = parms.threatPoints,
				inhabitants = true,
				seed = new int?(OutpostSitePartUtility.GetPawnGroupMakerSeed(parms))
			}).Count<PawnKindDef>();
		}

		[CompilerGenerated]
		private static bool <GetArrivedLetterPart>m__0(Pawn x)
		{
			return x.RaceProps.Humanlike && x.HostileTo(Faction.OfPlayer);
		}
	}
}
