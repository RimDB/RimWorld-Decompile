﻿using System;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x0200060C RID: 1548
	public class SitePartWorker_Outpost : SitePartWorker
	{
		// Token: 0x06001F2D RID: 7981 RVA: 0x0010ED2C File Offset: 0x0010D12C
		public override string GetArrivedLetterPart(Map map, out string preferredLabel, out LetterDef preferredLetterDef, out LookTargets lookTargets)
		{
			string arrivedLetterPart = base.GetArrivedLetterPart(map, out preferredLabel, out preferredLetterDef, out lookTargets);
			lookTargets = (from x in map.mapPawns.AllPawnsSpawned
			where x.RaceProps.Humanlike && x.HostileTo(Faction.OfPlayer)
			select x).FirstOrDefault<Pawn>();
			return arrivedLetterPart;
		}
	}
}
